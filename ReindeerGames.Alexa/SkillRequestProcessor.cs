using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slight.Alexa.Framework.Models.Requests;
using Slight.Alexa.Framework.Models.Requests.RequestTypes;

namespace ReindeerGames.Alexa
{
    /// <summary>
    /// Get request information from the SkillRequest passed in
    /// </summary>
    public interface ISkillRequestProcessor
    {
        /// <summary>
        /// Validate the request is OK to be processed. Throws if not.
        /// </summary>
        /// <param name="request">Request to validate</param>
        void ValidateRequest(SkillRequest request);

        /// <summary>
        /// From the request type / intent, work out what kind of request we're making of the game
        /// </summary>
        /// <param name="request">Alexa request</param>
        /// <param name="log">Logger to use</param>
        /// <returns>Requested type, or NULL if not applicable</returns>
        RequestType? GetRequestType(SkillRequest request, ILogger log);

        /// <summary>
        /// Get arguments from request
        /// </summary>
        /// <param name="request">Alexa request</param>
        /// <param name="logger">Logger to use</param>
        /// <returns>ReindeerGame arguments</returns>
        Argument[] GetArguments(SkillRequest request, ILogger logger);
    }

    /// <summary>
    /// Get request information from the SkillRequest passed in
    /// </summary>
    public class SkillRequestProcessor : ISkillRequestProcessor
    {
        /// <summary>
        /// Application ID from Alexa Skill
        /// </summary>
        private readonly string _applicationId;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="applicationId">Application ID from Alexa Skill</param>
        public SkillRequestProcessor(string applicationId)
        {
            _applicationId = applicationId;
        }

        /// <summary>
        /// Validate the request is OK to be processed. Throws if not.
        /// </summary>
        /// <param name="request">Request to validate</param>
        public void ValidateRequest(SkillRequest request)
        {
            if (string.IsNullOrEmpty(_applicationId))
                return;

            if (request.Session.Application.ApplicationId != _applicationId)
                throw new InvalidOperationException("Incorrect Application ID");
        }

        /// <summary>
        /// From the request type / intent, work out what kind of request we're making of the game
        /// </summary>
        /// <param name="request">Alexa request</param>
        /// <param name="log">Logger to use</param>
        /// <returns>Requested type, or NULL if not applicable</returns>
        public RequestType? GetRequestType(SkillRequest request, ILogger log)
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
        public Argument[] GetArguments(SkillRequest request, ILogger logger)
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
