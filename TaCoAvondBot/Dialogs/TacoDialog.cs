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
        public TacoDialog()
            : base(nameof(TacoDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ChosenTacoResponseStep
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ChosenTacoResponseStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var score = (double)stepContext.Options;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"I am {score * 100}% certain you like taco's"));

            return await stepContext.EndDialogAsync("taco");
        }
    }
}
