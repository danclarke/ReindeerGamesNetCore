using System.Collections.Generic;
using System.Linq;
using Slight.Alexa.Framework.Models.Responses;

namespace ReindeerGames.Alexa.Lambda
{
    using Response = Slight.Alexa.Framework.Models.Responses.Response;

    public interface ISkillResponseFactory
    {
        /// <summary>
        /// Create the SkillResponse
        /// </summary>
        /// <param name="response">Response to user</param>
        /// <returns>Skill Response</returns>
        SkillResponse CreateSkillResponse(ReindeerGames.Response response);
    }

    public class SkillResponseFactory : ISkillResponseFactory
    {
        /// <summary>
        /// Create the SkillResponse
        /// </summary>
        /// <param name="response">Response to user</param>
        /// <param name="sessionAttributes">Attributes for the session</param>
        /// <returns>Skill Response</returns>
        public SkillResponse CreateSkillResponse(ReindeerGames.Response response)
        {
            // Get the response applicable for card / no card
            var coreResponse = !string.IsNullOrWhiteSpace(response.CardTitle) 
                ? CreateCoreResponse(response.CardTitle, response.CardText, response.SpokenResponse, response.SpokenReprompt, response.EndSession) 
                : CreateCoreResponseNoCard(response.SpokenResponse, response.SpokenReprompt, response.EndSession);

            // If we don't want to send any session stuff, should return NULL
            var sessionValues = response.SessionValues.Any()
                ? response.SessionValues
                : null;

            // Create the actual response
            return CreateSkillResponse(coreResponse, sessionValues);
        }

        /// <summary>
        /// Create the basic response to the user
        /// </summary>
        /// <param name="title">Title of the card displayed on the mobile device</param>
        /// <param name="cardText">Text for the card</param>
        /// <param name="outputText">Spoken text to the user</param>
        /// <param name="repromptText">Spoken text to the user if they don't respond promptly</param>
        /// <param name="shouldEndSession">Whether this response finishes the session / game</param>
        /// <returns>Core response</returns>
        private static Response CreateCoreResponse(
            string title,
            string cardText,
            string outputText,
            string repromptText,
            bool shouldEndSession = false)
        {
            var card = new SimpleCard
            {
                Title = title,
                Content = cardText
            };

            return new Response()
            {
                Card = card,
                OutputSpeech = new PlainTextOutputSpeech()
                {
                    Text = outputText
                },
                Reprompt = new Reprompt()
                {
                    OutputSpeech = new PlainTextOutputSpeech
                    {
                        Text = repromptText
                    }
                },
                ShouldEndSession = shouldEndSession
            };
        }

        /// <summary>
        /// Create the basic response to the user, without generating a card that will be displayed on the user's mobile device
        /// </summary>
        /// <param name="outputText">Spoken text to the user</param>
        /// <param name="repromptText">Spoken text to the user if they don't respond promptly</param>
        /// <param name="shouldEndSession">Whether this response finishes the session / game</param>
        /// <returns>Core response</returns>
        private static Response CreateCoreResponseNoCard(
            string outputText,
            string repromptText,
            bool shouldEndSession = false)
        {
            return new Response()
            {
                OutputSpeech = new PlainTextOutputSpeech()
                {
                    Text = outputText
                },
                Reprompt = new Reprompt()
                {
                    OutputSpeech = new PlainTextOutputSpeech
                    {
                        Text = repromptText
                    }
                },
                ShouldEndSession = shouldEndSession
            };
        }

        /// <summary>
        /// Create the SkillResponse
        /// </summary>
        /// <param name="response">Response to user</param>
        /// <param name="sessionAttributes">Attributes for the session</param>
        /// <returns>Skill Response</returns>
        private static SkillResponse CreateSkillResponse(Response response, Dictionary<string, object> sessionAttributes = null)
        {
            return new SkillResponse
            {
                Response = response,
                SessionAttributes = sessionAttributes,
                Version = "1.0"
            };
        }
    }
}
