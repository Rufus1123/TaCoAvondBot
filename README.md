## Exercises: ##

(1) A Luis app was already created for you. \
  [a] Go to eu.luis.ai and you should be able to find the luis app. Take note of 
  the app id and the api key/authoring key.

  [b] Locate the appsettings file and fill out the Luis app details. \
	For the hostname you should use "westeurope.api.cognitive.microsoft.com".

(2) We will be creating a dialog that allows you to order pizza as well. \
  [a] Create "PizzaDialog.cs" in the Dialogs folder. Modify "MainDialog.cs" to 
  start the pizza dialog if a users says "I want pizza". You can use 
  "TacoDialog.cs" as a reference.

  Next, we will prompt the user to 
  - select a type of pizza (to be choosen from available pizza's)
  - input the size (diagonal in cm)
  - if they want pineapple on their pizza or not

  [b] Add a waterfallDialog, and in the waterfall dialog add a step that ends
  the dialog. Also create a private variable of type `PizzaDetails` in the  
  dialog. We will prompt the user for each of the properties in the following
  steps. Let the dialog return the string: 
  ```
     $"pizza {_pizzaDetails.Type} ({_pizzaDetails.Size} cm) {(_pizzaDetails.Pineapple ? "with" : "without")} pineapple"
  ```

  [c] Add a step to prompt the user to input the size of the pizza. We expect 
  them to return the pizza diameter in cm as an integer. Hence, use a 
  `NumberPrompt<int>`. Save the result in a private PizzaDetails variable. 

  HINT: \
  Example [here](https://github.com/microsoft/BotBuilder-Samples/blob/master/samples/csharp_dotnetcore/05.multi-turn-prompt/Dialogs/UserProfileDialog.cs#L86)

  [d] Add a step to the waterfall that asks the user if they want pineapple on 
  their pizza. Use a `SuggestedAction` for this. Again, save the result in 
  PizzaDetails.

  HINT: \
  Example [here](https://stackoverflow.com/a/54773668/9658454)

  [e] Add a waterfallstep to ask for the type of pizza, i.e. Salami, Tonno, etc. \
  **The easy way** 

  - Use a text prompt to store whatever the user types here.
	
  [Stretch :: Custom validation]
  In exercise [c] we saw that number prompts have automatic validation, i.e. 
  if the user inputs something that cannot be casted to a number, the bot asks 
  again. Textprompts accept any kind of input, so we need to write our own.
	
  - Validate the users input against list of pizza types. Change the pizza 
  type in PizzaDetails.cs to Enums.PizzaType. To validate user input, add a 
  PromptValidator to your text prompt. Make sure to add a `RetryPrompt` to the
  prompt.

  HINT: \
  Example [here](https://github.com/microsoft/BotBuilder-Samples/blob/master/samples/csharp_dotnetcore/05.multi-turn-prompt/Dialogs/UserProfileDialog.cs#L136)

  **The not so easy way** 

  - Use an adaptive card with images you can click on to select your pizza of
  choice. \
  You can use the card in Cards > PizzaOrderCard.json. Feel free to edit it, or 
  design your own if you're feeling creative. You can find samples 
  [here](https://adaptivecards.io/samples). Click the "Try it Yourself"-button 
  to use the online editor. Refer to the `CreateAdaptiveCardAttachment()` 
  function in `DialogAndWelcomeBot.cs` for how to import the json card.

  - Change the pizza type in PizzaDetails.cs to Enums.PizzaType and parse the 
  input into the PizzaDetails object.

  HINT: \
  How to generate a prompt with an adaptive card:
  You can use prompts, but instead of using `MessageFactory.Text` to add an 
  activity to the prompt, use `MessageFactory.Card`. You will need to explicitly
  cast it to an activity however. 
  ```
	var card = CreateAdaptiveCardAttachment();
	// Cast the IMessageActivity to an Activity before adding it to the prompt.
    var prompt = (Activity)MessageFactory.Attachment(card);

    var promptOptions = new PromptOptions { Prompt = prompt };
  ```

  HINT: \
  How to add the prompt to the dialog:
  Unfortunatly, there is no out-of-the-box support for prompting with adaptive 
  cards yet. We will be using a `TextPrompt` since there is no validation hidden
  in the framework. 
  ```
	private const string cardPromptId = "adaptiveCardPrompt";

	...

    AddDialog(new TextPrompt(cardPromptId));

	...

	return await stepContext.PromptAsync(cardPromptId, promptOptions, cancellationToken);
  ```

  **The hard way** 

  - Use an adaptive card with a dropdown selection or bullet choices for the 
  pizza type. The card should have images and a submit button. Upon submitting,
  the data filled out in this form is available in the 
  `stepContext.Context.Activity.Value` as a JObject. \
  You can use the ugly but functional card in Cards > PizzaOrderForm.json. Feel
  free to edit it, or design your own if you're feeling creative. You can find
  samples [here](https://adaptivecards.io/samples). Click the "Try it Yourself"-
  button to use the online editor. Refer to the `CreateAdaptiveCardAttachment()` 
  function in `DialogAndWelcomeBot.cs` for how to import the json card.

  - Change the pizza type in PizzaDetails.cs to Enums.PizzaType and parse the 
  input into the PizzaDetails object.

  HINT: \
  How to generate a prompt with an adaptive card:
  You can use prompts, but instead of using `MessageFactory.Text` to add an 
  activity to the prompt, use `MessageFactory.Card`. You will need to explicitly
  cast it to an activity however. 
  ```
	var card = CreateAdaptiveCardAttachment();
	// Cast the IMessageActivity to an Activity before adding it to the prompt.
    var prompt = (Activity)MessageFactory.Attachment(card);

    var promptOptions = new PromptOptions { Prompt = prompt };
  ```
	
  HINT: \
  How to add the prompt to the dialog:
  Unfortunatly, there is no out-of-the-box support for prompting with adaptive 
  cards yet. We will be using a `TextPrompt` since there is no validation hidden
  in the framework. 
  ```
	private const string cardPromptId = "adaptiveCardPrompt";

	...

    AddDialog(new TextPrompt(cardPromptId));

	...

	return await stepContext.PromptAsync(cardPromptId, promptOptions, cancellationToken);
  ```
	
  HINT: \
  How to process the input form: 
  ```
    var result = stepContext.Context.Activity.Value as JObject;
    var pizzaType = Enum.Parse<PizzaType>(result["pizzaType"].ToString());
  ```
