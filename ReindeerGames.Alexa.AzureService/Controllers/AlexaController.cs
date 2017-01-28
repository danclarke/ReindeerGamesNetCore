using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Slight.Alexa.Framework.Models.Requests;
using Slight.Alexa.Framework.Models.Responses;

namespace ReindeerGames.Alexa.AzureService.Controllers
{
    [Route("api/[controller]")]
    public class AlexaController : Controller
    {
        // Dependencies
        private readonly IQuestionFactory _questionFactory;
        private readonly ISkillResponseFactory _responseFactory;
        private readonly ISkillRequestProcessor _processor;
        private readonly ILogger _logger;

        public AlexaController(
            IQuestionFactory questionFactory,
            ISkillResponseFactory responseFactory,
            ISkillRequestProcessor processor,
            ILogger logger)
        {
            _questionFactory = questionFactory;
            _responseFactory = responseFactory;
            _processor = processor;
            _logger = logger;
        }

        [Route("Skill")]
        [HttpPost]
        public SkillResponse HandleSkillRequest([FromBody]SkillRequest request)
        {
            // Quick security check
            _processor.ValidateRequest(request);

            // Validate we can handle this request
            var requestType = _processor.GetRequestType(request, _logger);
            if (requestType == null)
                throw new InvalidOperationException("Unsupported request");

            // Hand off to game to do magic
            var session = new AlexaSession(request.Session);
            var arguments = _processor.GetArguments(request, _logger);
            var game = new ReindeerGame(session, _logger, _questionFactory);

            try
            {
                var response = game.Execute(requestType.Value, arguments);
                return _responseFactory.CreateSkillResponse(response);
            }
            catch (Exception e)
            {
                _logger.LogLine("Exception processing game: " + e);
                throw;
            }
        }
    }
}
