using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ReindeeGames.Tests.Util;
using ReindeerGames;
using Xunit;

namespace ReindeeGames.Tests
{
    public sealed class QuestionFactoryShould
    {
        private readonly IQuestionFactory _factory = new QuestionFactory();

        [Fact]
        public void ReturnQuestionOnDemand()
        {
            var question = _factory.GetQuestion(5);

            question.QuestionText.Should().Be("What of the following is not true?");
            question.Answers[1].Should().Be("Both reindeer and Caribou are the same species");
        }

        [Fact]
        public void ProvideAccurateAnswerCount()
        {
            for (int i = 0; i < _factory.NumQuestions; ++i)
            {
                var question = _factory.GetQuestion(i);
                question.Answers.Length.Should().Be(_factory.AnswerCount, "All questions must have the same number of answers");
            }
        }

        [Theory]
        [Repeat(10)]
        public void ReturnUniqueRandomQuestions()
        {
            var randomIndices = _factory.GetRandomQuestionIndices(10);

            // Magic code to check for duplicates
            // https://stackoverflow.com/a/18547390
            var duplicates = randomIndices.GroupBy(x => x)
                .Where(g => g.Count() > 1);

            duplicates.Any().Should().BeFalse("Should not have duplicate question indices");
        }

        [Theory]
        [Repeat(10)]
        public void ReturnRandomQuestionsInRange()
        {
            var randomIndices = _factory.GetRandomQuestionIndices(10);

            // Check bounds
            randomIndices.Min().Should().BeGreaterOrEqualTo(0);
            randomIndices.Max().Should().BeLessThan(_factory.NumQuestions);
        }

        [Theory]
        [Repeat(10)]
        public void ReturnUniqueRandomisedAnswerIndices()
        {
            var question = _factory.GetQuestionSelection(0, 1);

            var duplicates = question.AnswerShuffleIndices.GroupBy(x => x)
                .Where(g => g.Count() > 1);

            duplicates.Any().Should().BeFalse("Should not have duplicate answer indices");
        }

        [Theory]
        [Repeat(10)]
        public void ReturnRandomAnswersOfAnswerCount()
        {
            var question = _factory.GetQuestionSelection(0, 1);

            question.AnswerShuffleIndices.Length.Should().Be(_factory.AnswerCount);

            for (int i = 0; i < _factory.AnswerCount; ++i)
                question.AnswerShuffleIndices.Should().Contain(i, "Must contain ALL answers in return");
        }

        [Fact]
        public void ReturnQuestionSelectionWithCorrectQuestion()
        {
            var question = _factory.GetQuestionSelection(0, 1);

            question.QuestionIndex.Should().Be(0);
            question.QuestionNum.Should().Be(1);
        }

        [Theory]
        [Repeat(10)]
        public void PopulateCorrectAnswerIndexCorrectly()
        {
            var question = _factory.GetQuestionSelection(0, 1);

            question.AnswerShuffleIndices[0].Should().Be(question.CorrectAnswerIndex, 
                "Question 0 goes to a new index, that index should be the same as 'CorrectAnswerIndex'");
        }
    }
}
