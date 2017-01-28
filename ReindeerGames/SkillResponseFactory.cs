using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slight.Alexa.Framework.Models.Responses;

namespace ReindeerGames
{
    public interface ISkillResponseFactory
    {
        /// <summary>
        /// Create the basic response to the user
        /// </summary>
        /// <param name="title">Title of the card displayed on the mobile device</param>
        /// <param name="outputText">Spoken text to the user</param>
        /// <param name="repromptText">Spoken text to the user if they don't respond promptly</param>
        /// <param name="shouldEndSession">Whether this response finishes the session / game</param>
        /// <returns>Core response</returns>
        Response CreateCoreResponse(
            string title,
            string outputText,
            string repromptText,
            bool shouldEndSession = false);

        /// <summary>
        /// Create the basic response to the user, without generating a card that will be displayed on the user's mobile device
        /// </summary>
        /// <param name="outputText">Spoken text to the user</param>
        /// <param name="repromptText">Spoken text to the user if they don't respond promptly</param>
        /// <param name="shouldEndSession">Whether this response finishes the session / game</param>
        /// <returns>Core response</returns>
        Response CreateCoreResponseNoCard(
            string outputText,
            string repromptText,
            bool shouldEndSession = false);

        /// <summary>
        /// Create the SkillResponse
        /// </summary>
        /// <param name="response">Response to user</param>
        /// <param name="sessionAttributes">Attributes for the session</param>
        /// <returns>Skill Response</returns>
        SkillResponse CreateSkillResponse(Response response, Dictionary<string, object> sessionAttributes = null);
    }

    public class SkillResponseFactory : ISkillResponseFactory
    {
        /// <summary>
        /// Create the basic response to the user
        /// </summary>
        /// <param name="title">Title of the card displayed on the mobile device</param>
        /// <param name="outputText">Spoken text to the user</param>
        /// <param name="repromptText">Spoken text to the user if they don't respond promptly</param>
        /// <param name="shouldEndSession">Whether this response finishes the session / game</param>
        /// <returns>Core response</returns>
        public Response CreateCoreResponse(
            string title,
            string outputText,
            string repromptText,
            bool shouldEndSession = false)
        {
            var card = new SimpleCard
            {
                Title = title,
                Content = outputText
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
        public Response CreateCoreResponseNoCard(
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
        public SkillResponse CreateSkillResponse(Response response, Dictionary<string, object> sessionAttributes = null)
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
