using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NuGet.Packaging.Core;
using ReindeeGames.Tests.Util;
using ReindeerGames;
using Xunit;

namespace ReindeeGames.Tests
{
    public sealed class ReindeerGameShould
    {
        private static readonly IQuestionFactory QuestionFactory = new QuestionFactory();

        private static readonly SelectedQuestion SampleQuestion = new SelectedQuestion
        {
            QuestionIndex = 3,
            QuestionNum = 4,
            AnswerShuffleIndices = new[] { 2, 1, 0, 3 },
            CorrectAnswerIndex = 2
        };

        private static readonly SelectedQuestion FinalQuestion = new SelectedQuestion
        {
            QuestionIndex = 4,
            QuestionNum = 5,
            AnswerShuffleIndices = new[] { 2, 1, 0, 3 },
            CorrectAnswerIndex = 2
        };

        private static readonly int[] QuestionIndices = { 1, 2, 3, 4, 5 };

        [Fact]
        public void LaunchNewGameOnRequest()
        {
            var game = new ReindeerGame(Substitute.For<ISession>(), Substitute.For<ILogger>(), QuestionFactory);
            var response = game.Execute(RequestType.LaunchGame, new Argument[0]);

            response.SpokenResponse.Should().Contain("Let's begin");
            response.EndSession.Should().BeFalse();

            var questionInfo = response.SessionValues.GetQuestion();
            questionInfo.Should().NotBeNull();
            questionInfo.QuestionNum.Should().Be(1);
            questionInfo.AnswerShuffleIndices.Length.Should().Be(QuestionFactory.AnswerCount);

            response.SessionValues.GetScore().Should().Be(0);
        }

        [Fact]
        public void FinishGameOnRequest()
        {
            var game = new ReindeerGame(Substitute.For<ISession>(), Substitute.For<ILogger>(), QuestionFactory);
            var response = game.Execute(RequestType.EndGame, new Argument[0]);

            response.SpokenResponse.Should().Contain("Good bye");
            response.EndSession.Should().BeTrue();
        }

        [Fact]
        public void CheckGameInProgress()
        {
            var game = new ReindeerGame(Substitute.For<ISession>(), Substitute.For<ILogger>(), QuestionFactory);
            var response = game.Execute(RequestType.AnswerGeneric, new[] { new Argument("Answer", "One"), });

            response.SpokenResponse.Should().Contain("Let's begin");
            response.EndSession.Should().BeFalse();

            var questionInfo = response.SessionValues.GetQuestion();
            questionInfo.Should().NotBeNull();
            questionInfo.QuestionNum.Should().Be(1);
            questionInfo.AnswerShuffleIndices.Length.Should().Be(QuestionFactory.AnswerCount);

            response.SessionValues.GetScore().Should().Be(0);
        }

        [Fact]
        public void ValidateOutOfRangeAnswer()
        {
            var game = new ReindeerGame(GetInProgressSession(), Substitute.For<ILogger>(), QuestionFactory);
            var response = game.Execute(RequestType.AnswerGeneric, new[] { new Argument("Answer", "Five"), });

            response.SpokenResponse.Should().Contain("Your answer must be a number between");
            response.EndSession.Should().BeFalse();
        }

        [Fact]
        public void ValidateCorrectAnswer()
        {
            var game = new ReindeerGame(GetInProgressSession(), Substitute.For<ILogger>(), QuestionFactory);
            var response = game.Execute(RequestType.AnswerGeneric, new[] { new Argument("Answer", "Three"), });

            response.SpokenResponse.Should().Contain("Correct");
            response.SessionValues.GetQuestion().QuestionIndex.Should().Be(5);
            response.SessionValues.GetScore().Should().Be(4);
        }

        [Fact]
        public void ValidateIncorrectAnswer()
        {
            var game = new ReindeerGame(GetInProgressSession(), Substitute.For<ILogger>(), QuestionFactory);
            var response = game.Execute(RequestType.AnswerGeneric, new[] { new Argument("Answer", "One"), });

            response.SpokenResponse.Should().Contain("Incorrect");
            response.SessionValues.GetQuestion().QuestionIndex.Should().Be(5);
            response.SessionValues.GetScore().Should().Be(3);
        }

        [Fact]
        public void FinishGameOnCompletion()
        {
            var game = new ReindeerGame(GetInProgressSession(FinalQuestion), Substitute.For<ILogger>(), QuestionFactory);
            var response = game.Execute(RequestType.AnswerGeneric, new[] { new Argument("Answer", "One"), });

            response.SpokenResponse.Should().Contain("Thank you for playing!");
            response.EndSession.Should().BeTrue();
        }

        [Fact]
        public void ProvideHelpWithoutLosingSession()
        {
            var game = new ReindeerGame(GetInProgressSession(), Substitute.For<ILogger>(), QuestionFactory);
            var response = game.Execute(RequestType.AnswerHelp, new Argument[0]);

            response.SpokenResponse.Should().Contain("I will ask you");
            response.EndSession.Should().BeFalse();
            response.SessionValues.GetQuestion().Should().BeSameAs(SampleQuestion);
            response.SessionValues.GetScore().Should().Be(3);
        }

        private static ISession GetInProgressSession(SelectedQuestion question = null)
        {
            if (question == null)
                question = SampleQuestion;

            var session = Substitute.For<ISession>();
            session.GetObject<int>("Score").Returns(3);
            session.GetObject<int[]>("QuestionIndices").Returns(QuestionIndices);
            session.GetObject<SelectedQuestion>("CurrentQuestion").Returns(question);

            return session;
        }
    }
}
