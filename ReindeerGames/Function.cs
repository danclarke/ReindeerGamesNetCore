using System;
using Amazon.Lambda.Core;
using Slight.Alexa.Framework.Models.Requests;
using Slight.Alexa.Framework.Models.Responses;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ReindeerGames
{
    public class Function
    {
        /// <summary>
        /// ID of the Alexa skill Application ID, to ensure we don't give out free skillz
        /// </summary>
        private const string ApplicationId = "amzn1.ask.skill.75a8b73f-80e5-4bca-b4a0-b5c49bc2334d";

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            // Quick security check
            ValidateRequest(input);

            // Hand off to game to do magic
            var responseFactory = new SkillResponseFactory();
            var game = new ReindeerGame(input, context, responseFactory);

            try
            {
                return game.Execute();
            }
            catch (Exception e)
            {
                context.Logger.LogLine("Exception processing game: " + e);
                throw;
            }
        }

        /// <summary>
        /// Validate the request is OK to be processed. Throws if not.
        /// </summary>
        /// <param name="request">Request to validate</param>
        private static void ValidateRequest(SkillRequest request)
        {
            if (string.IsNullOrEmpty(ApplicationId))
                return;

            if (request.Session.Application.ApplicationId != ApplicationId)
                throw new InvalidOperationException("Incorrect Application ID");
        }
    }
}
