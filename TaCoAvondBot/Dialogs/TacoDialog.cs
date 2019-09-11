// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.5.0

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaCoAvondBot.Enums;

namespace TaCoAvondBot.Dialogs
{
    public class TacoDialog : ComponentDialog
    {
        private const string cardPromptId = "cardPrompt";

        public TacoDialog()
            : base(nameof(TacoDialog))
        {
            
            AddDialog(new TextPrompt(cardPromptId));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ShowCardStep,
                ChosenTacoResponseStep
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ShowCardStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var welcomeCard = CreateAdaptiveCardAttachment();
            var response = (Activity)MessageFactory.Attachment(welcomeCard);

            var promptOptions = new PromptOptions { Prompt = response };

            return await stepContext.PromptAsync(cardPromptId, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ChosenTacoResponseStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var score = (double)stepContext.Options;
            var result = stepContext.Context.Activity.Value as JObject;
            var pizzaType = Enum.Parse(typeof(PizzaType), result["pizzaType"].ToString());

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"I am {score * 100}% certain you like taco's"));

            return await stepContext.EndDialogAsync("taco");
        }

        // Load attachment from embedded resource.
        private Attachment CreateAdaptiveCardAttachment()
        {
            using (var reader = new StreamReader("Cards\\PizzaOrderForm.json"))
            {
                var adaptiveCard = reader.ReadToEnd();
                return new Attachment()
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(adaptiveCard),
                };
            }
        }
    }
}
