using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Slight.Alexa.Framework.Models.Requests;
using Slight.Alexa.Framework.Models.Requests.RequestTypes;
using Xunit;

namespace ReindeerGames.Alexa.Tests
{
    public sealed class SkillRequestProcessorShould
    {
        [Theory]
        [InlineData("LocalId", "LocalId", true)]
        [InlineData("LocalId", "RequestId", false)]
        public void ValidatesApplicationId(string applicationId, string requestApplicationId, bool valid)
        {
            var request = new SkillRequest
            {
                Session = new Session
                {
                    Application = new Application
                    {
                        ApplicationId = requestApplicationId
                    }
                }
            };

            var processor = new SkillRequestProcessor(applicationId);

            if (!valid)
                Assert.Throws<InvalidOperationException>(() => processor.ValidateRequest(request));
            else
                processor.ValidateRequest(request);
        }

        [Theory]
        [InlineData("LaunchRequest", RequestType.LaunchGame)]
        [InlineData("SessionEndedRequest", null)]
        public void ReturnTheCorrectRequestTypeFromRawRequest(string requestName, RequestType? expectedRequestType)
        {
            var request = new SkillRequest
            {
                Request = new RequestBundle
                {
                    RequestId = "Test",
                    Type = requestName
                },
                Session = new Session
                {
                    SessionId = "Test"
                }
            };

            var processor = new SkillRequestProcessor("");
            var requestType = processor.GetRequestType(request, Substitute.For<ILogger>());

            requestType.Should().Be(expectedRequestType);
        }

        [Theory]
        [InlineData("AnswerIntent", RequestType.AnswerGeneric)]
        [InlineData("AnswerOnlyIntent", RequestType.AnswerGeneric)]
        [InlineData("DontKnowIntent", RequestType.AnswerDontKnow)]
        [InlineData("AMAZON.YesIntent", RequestType.AnswerYes)]
        [InlineData("AMAZON.NoIntent", RequestType.AnswerNo)]
        [InlineData("AMAZON.StartOverIntent", RequestType.LaunchGame)]
        [InlineData("AMAZON.RepeatIntent", RequestType.AnswerRepeat)]
        [InlineData("AMAZON.HelpIntent", RequestType.AnswerHelp)]
        [InlineData("AMAZON.StopIntent", RequestType.EndGame)]
        [InlineData("AMAZON.CancelIntent", RequestType.EndGame)]
        [InlineData("AMAZON.Unexpected", null)]
        public void ReturnTheCorrectRequestTypeFromIntents(string intentName, RequestType? expectedRequestType)
        {
            var request = new SkillRequest
            {
                Request = new RequestBundle
                {
                    RequestId = "Test",
                    Type = "IntentRequest",
                    Intent = new Intent
                    {
                        Name = intentName
                    }
                },
                Session = new Session
                {
                    SessionId = "Test"
                }
            };

            var processor = new SkillRequestProcessor("");
            var requestType = processor.GetRequestType(request, Substitute.For<ILogger>());

            requestType.Should().Be(expectedRequestType);
        }

        [Fact]
        public void GetsArgumentsFromSlots()
        {
            var request = new SkillRequest
            {
                Request = new RequestBundle
                {
                    Intent = new Intent
                    {
                        Name = "Test",
                        Slots = new Dictionary<string, Slot>
                        {
                            {"Answer", new Slot {Name = "Answer", Value = "Five"}}
                        }
                    }
                }
            };

            var expected = new[] {new Argument("Answer", "Five")};

            var processor = new SkillRequestProcessor("");
            var args = processor.GetArguments(request, Substitute.For<ILogger>());
            
            args.ShouldAllBeEquivalentTo(expected);
        }

        [Fact]
        public void HandlesEmptyArgumentsWithoutCrashing()
        {
            var request = new SkillRequest
            {
                Request = new RequestBundle
                {
                    Intent = new Intent()
                }
            };

            var processor = new SkillRequestProcessor("");
            var args = processor.GetArguments(request, Substitute.For<ILogger>());

            args.Length.Should().Be(0);
        }
    }
}
