using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization;
using Slight.Alexa.Framework.Models.Requests;
using Slight.Alexa.Framework.Models.Requests.RequestTypes;
using Slight.Alexa.Framework.Models.Responses;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace ReindeerGames.Alexa.Lambda
{
    public sealed class Function
    {
        /// <summary>
        /// ID of the Alexa skill Application ID, to ensure we don't give out free skillz
        /// </summary>
        private const string ApplicationId = "amzn1.ask.skill.75a8b73f-80e5-4bca-b4a0-b5c49bc2334d";

        // Global dependencies
        private static readonly IQuestionFactory QuestionFactory = new QuestionFactory();
        private static readonly ISkillResponseFactory ResponseFactory = new SkillResponseFactory();
        private static readonly ISkillRequestProcessor RequestProcessor = new SkillRequestProcessor(ApplicationId);

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            var logger = new LambdaLogger(context.Logger);

            // Quick security check
            RequestProcessor.ValidateRequest(input);

            // Validate we can handle this request
            var requestType = RequestProcessor.GetRequestType(input, logger);
            if (requestType == null)
                throw new InvalidOperationException("Unsupported request");

            // Hand off to game to do magic
            var session = new AlexaSession(input.Session);
            var arguments = RequestProcessor.GetArguments(input, logger);
            var game = new ReindeerGame(session, logger, QuestionFactory);

            try
            {
                var response = game.Execute(requestType.Value, arguments);
                return ResponseFactory.CreateSkillResponse(response);
            }
            catch (Exception e)
            {
                context.Logger.LogLine("Exception processing game: " + e);
                throw;
            }
        }
    }
}
