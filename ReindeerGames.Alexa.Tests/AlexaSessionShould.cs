using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Slight.Alexa.Framework.Models.Requests;
using Xunit;

namespace ReindeerGames.Alexa.Tests
{
    // Tests are a bit implementation-specific here, but they're real-world tests so I'm going to sneak them in

    public sealed class AlexaSessionShould
    {
        private readonly JObject _fullObject;
        private readonly Session _testSession;

        public AlexaSessionShould()
        {
            _fullObject = JObject.Parse(SampleJson);
            _testSession = new Session
            {
                Attributes = GetSessionTokens()
            };
        }

        [Fact]
        public void GetObjectReturnsSelectedQuestion()
        {
            var session = new AlexaSession(_testSession);
            var question = session.GetObject<SelectedQuestion>("CurrentQuestion");

            question.ShouldBeEquivalentTo(SampleQuestion);
        }

        [Fact]
        public void GetObjectReturnsScore()
        {
            var session = new AlexaSession(_testSession);

            session.GetObject<int>("Score").Should().Be(1);
        }

        [Fact]
        public void GetObjectReturnsQuestionIndices()
        {
            var session = new AlexaSession(_testSession);

            session.GetObject<int[]>("QuestionIndices").ShouldAllBeEquivalentTo(SampleQuestionIndices);
        }

        private Dictionary<string, object> GetSessionTokens()
        {
            return new Dictionary<string, object>
            {
                {"Score", GetJToken("Score")},
                {"QuestionIndices", GetJToken("QuestionIndices")},
                {"CurrentQuestion", GetJToken("CurrentQuestion")},
            };
        }

        private JToken GetJToken(string key)
        {
            return _fullObject[key];
        }

        private static readonly SelectedQuestion SampleQuestion = new SelectedQuestion
        {
            QuestionIndex = 20,
            AnswerShuffleIndices = new[] {0, 3, 1, 2},
            CorrectAnswerIndex = 0,
            QuestionNum = 1
        };

        private static readonly int[] SampleQuestionIndices = {20, 1, 29, 12, 21};

        private const string SampleJson = @"{
    ""Score"": 1,
    ""QuestionIndices"": [
    20,
    1,
    29,
    12,
    21
    ],
    ""CurrentQuestion"": {
    ""questionIndex"": 20,
    ""answerShuffleIndices"": [
        0,
        3,
        1,
        2
    ],
    ""correctAnswerIndex"": 0,
    ""questionNum"": 1
    }
}";
    }
}
