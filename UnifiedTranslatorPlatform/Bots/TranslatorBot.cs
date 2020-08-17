// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.9.2

using System;
using System.Collections.Generic;
using System.IO;
using AdaptiveCards;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using UnifiedTranslatorPlatform.Helper;
using System.Linq;

namespace UnifiedTranslatorPlatform.Bots
{
    public class TranslatorBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            if (!string.IsNullOrEmpty(turnContext.Activity.Text))
            {
                switch (turnContext.Activity.Text.Trim().ToLowerInvariant())
                {
                    case "translate text":
                        await turnContext.SendActivityAsync(MessageFactory.Attachment(TranslationTextInputCardAttachment()));
                        break;
                    case "translate document":
                        await turnContext.SendActivityAsync(MessageFactory.Text("Hello! Brother"), cancellationToken);
                        break;
                    default:
                        await turnContext.SendActivityAsync(MessageFactory.Text(turnContext.Activity.Text), cancellationToken);
                        break;
                }
            }
            else if (!string.IsNullOrEmpty(turnContext.Activity.Value.ToString()))
            {
                IList<Attachment> responseCarousel = new List<Attachment>();
                //TranslationInput inputData = JsonConvert.DeserializeObject<TranslationInput>(turnContext.Activity.Value.ToString());

                var targetLang = string.Empty;
                var textInput = string.Empty;
                var userInput = JObject.Parse(turnContext.Activity.Value.ToString());
                var userInputProperties = userInput.Properties();
                foreach (var property in userInputProperties)
                {
                    if (property.Name.ToString() == "TextInput")
                        textInput = property.Value.ToString();                   
                    else
                        targetLang += $",{property.Value.ToString()}";
                }
                targetLang = targetLang.Replace(",", "&to=");
                var response = await TranslationFunction.InvokeTranslationFunction(textInput, targetLang);
                TranslationOutput[] translationOutput = JsonConvert.DeserializeObject<TranslationOutput[]>(response);
                foreach (var o in translationOutput)
                {
                    responseCarousel.Add(TranslationResultCardAttachment(o.toLang, o.translatedText, o.confidenceScore));
                }
                await turnContext.SendActivityAsync(MessageFactory.Carousel(responseCarousel));
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            Attachment welcomeCard = WelcomeHeroCard();
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Attachment(welcomeCard));
                }
            }
        }

        private static Attachment CreateWelcomeCardAttachment()
        {
            var paths = new[] { ".", "Resources", "WelcomeCard.json" };
            var adaptiveCardJson = File.ReadAllText(Path.Combine(paths));

            var welcomeCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return welcomeCardAttachment;
        }

        private static Attachment WelcomeHeroCard()
        {
            var welcomeCard = new HeroCard
            {
                Title = "Unified Translation Platform",
                Subtitle = "Omni Translator Bot",
                Text = "Hello! I can help you translate texts and documents in 60 different languages. Select your desired option.",

                Images = new List<CardImage>
                {
                    new CardImage("https://ciklopea.com/wp-content/uploads/2017/07/translator.jpg"),
                },
                Buttons = new List<CardAction>
                {
                    //CardAction is used to process events in rich card                    
                    new CardAction(ActionTypes.ImBack, title: "Translate Text", value: "Translate Text"),
                    new CardAction(ActionTypes.PostBack, title: "Translate Document", value: "Translate Document")
                },

            };

            return welcomeCard.ToAttachment();
        }

        private static Attachment TranslationTextInputCardAttachment()
        {
            var paths = new[] { ".", "Resources", "TranslationTextInputCard.json" };
            var adaptiveCardJson = File.ReadAllText(Path.Combine(paths));

            var textInputCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return textInputCardAttachment;
        }

        private static Attachment TranslationResultCardAttachment(string targetLang, string translatedText, string confidenceScore)
        {
            var paths = new[] { ".", "Resources", "TranslationResultCard.json" };
            var jsonString = File.ReadAllText(Path.Combine(paths));
            var cardJson = JObject.Parse(jsonString);
            //Below lines of code read the json file and modify its content then present it to the user as a card
            JArray body = (JArray)cardJson["body"];
            JArray targetLangContainer = (JArray)(body[0]["items"][0]["columns"][1]["items"]);
            ((JObject)targetLangContainer[0])["text"] = targetLang;

            JArray translatedTextContainer = (JArray)(body[1]["items"]);
            ((JObject)translatedTextContainer[0])["text"] = translatedText;

            JArray translationPropContainer = (JArray)(body[2]["items"][0]["columns"][1]["items"]);
            ((JObject)translationPropContainer[0])["text"] = confidenceScore;

            var translationResultCard = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson.ToString()),
            };
            return translationResultCard;
        }
       
    }

}
