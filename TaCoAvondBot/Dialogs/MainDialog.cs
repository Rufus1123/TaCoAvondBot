// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.5.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using TaCoAvondBot.Luis;

namespace TaCoAvondBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly Recognizer _luisRecognizer;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(Recognizer luisRecognizer, TacoDialog bookingDialog)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new TacoDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ChooseFoodStepAsync,
                EnjoyWishesStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ChooseFoodStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                // LUIS is not configured, we just run the BookingDialog path with an empty BookingDetailsInstance.
                throw new Exception("Luis is not configured.");
            }

            // Call LUIS and gather any potential booking details. (Note the TurnContext has the response to the prompt.)
            var luisResult = await _luisRecognizer.RecognizeAsync<LuisModel>(stepContext.Context, cancellationToken);

            switch (luisResult.TopIntent().intent)
            {
                case LuisModel.Intent.WantFood:
                    if (luisResult.Entities._instance.Taco != null)
                    {
                        return await stepContext.BeginDialogAsync(nameof(TacoDialog), luisResult.TopIntent().score, cancellationToken);
                    }
                    else
                    {
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text("We don't have that. Please try \"I want pizza\" or \"I want taco's\"."));
                        return await stepContext.EndDialogAsync();
                    }
                default:
                    // Catch all for unhandled intents
                    var didntUnderstandMessageText = $"Sorry, I didn't get that. Please try asking in a different way (intent was {luisResult.TopIntent().intent})";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }
        private async Task<DialogTurnResult> EnjoyWishesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var chosenFood = (string)stepContext.Result;

            var response = stepContext.Context.Activity.CreateReply($"Your order has been placed and will be delivered shortly. Enjoy your {chosenFood}.");
            response.Type = ActivityTypes.EndOfConversation;

            await stepContext.Context.SendActivityAsync(response);

            return await stepContext.NextAsync();
        }
    }
}
