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
        private static readonly SkillResponseFactory ResponseFactory = new SkillResponseFactory();

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

            // Validate we can handle this request
            var requestType = GetRequestType(input, context.Logger);
            if (requestType == null)
                throw new InvalidOperationException("Unsupported request");

            // Hand off to game to do magic
            var session = new AlexaSession(input.Session);
            var logger = new LambdaLogger(context.Logger);
            var arguments = GetArguments(input, context.Logger);
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

        /// <summary>
        /// From the request type / intent, work out what kind of request we're making of the game
        /// </summary>
        /// <param name="request">Alexa request</param>
        /// <param name="log">Logger to use</param>
        /// <returns>Requested type, or NULL if not applicable</returns>
        private static RequestType? GetRequestType(SkillRequest request, ILambdaLogger log)
        {
            log.LogLine($"Received '{request.Request.Type}' request");

            var requestType = request.GetRequestType();

            // Primary requests
            if (requestType == typeof(ILaunchRequest))
                return RequestType.LaunchGame;

            if (requestType == typeof(ISessionEndedRequest))
                return null;

            // Map specific responses to game request types
            if (requestType == typeof(IIntentRequest))
            {
                var intent = request.Request.Intent;
                log.LogLine($"Handling '{intent.Name}' intent with ID '{request.Request.RequestId}', Session ID '{request.Session.SessionId}'");

                switch (intent.Name)
                {
                    case "AnswerIntent":
                    case "AnswerOnlyIntent":
                        return RequestType.AnswerGeneric;

                    case "DontKnowIntent":
                        return RequestType.AnswerDontKnow;

                    case "AMAZON.YesIntent":
                        return RequestType.AnswerYes;

                    case "AMAZON.NoIntent":
                        return RequestType.AnswerNo;

                    case "AMAZON.StartOverIntent":
                        return RequestType.LaunchGame;

                    case "AMAZON.RepeatIntent":
                        return RequestType.AnswerRepeat;

                    case "AMAZON.HelpIntent":
                        return RequestType.AnswerHelp;

                    case "AMAZON.StopIntent":
                    case "AMAZON.CancelIntent":
                        return RequestType.EndGame;
                }
            }

            // Unknown request
            log.LogLine($"Unknown request: '{request.Request.Type}', '{request.Request?.Intent?.Name}' intent with ID '{request.Request.RequestId}', Session ID '{request.Session.SessionId}'");
            return null;
        }

        /// <summary>
        /// Get arguments from request
        /// </summary>
        /// <param name="request">Alexa request</param>
        /// <param name="logger">Logger to use</param>
        /// <returns>ReindeerGame arguments</returns>
        private static Argument[] GetArguments(SkillRequest request, ILambdaLogger logger)
        {
            if (request?.Request?.Intent?.Slots == null)
            {
                logger.LogLine($"No arguments for session '{request?.Session?.SessionId}'");
                return new Argument[0];
            }

            return request.Request.Intent.Slots
                .Select(slot => new Argument(slot.Key, slot.Value.Value))
                .ToArray();
        }
    }
}
