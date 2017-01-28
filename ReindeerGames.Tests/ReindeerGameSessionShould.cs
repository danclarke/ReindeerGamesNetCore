using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using ReindeerGames;
using Xunit;

namespace ReindeeGames.Tests
{
    public sealed class ReindeerGameSessionShould
    {
        private static readonly SelectedQuestion SampleQuestion = new SelectedQuestion
        {
            QuestionIndex = 3,
            QuestionNum = 5,
            AnswerShuffleIndices = new[] {3, 2, 1, 4},
            CorrectAnswerIndex = 2
        };

        private static readonly int[] QuestionIndices = {1, 2, 3, 4, 5};

        [Fact]
        public void ContainSessionDetails()
        {
            var gameSession = new ReindeerGameSession(GetTestSession(), Substitute.For<ILogger>());

            gameSession.CurrentQuestion.Should().BeSameAs(SampleQuestion);
            gameSession.Score.Should().Be(3);
            gameSession.QuestionIndices.ShouldAllBeEquivalentTo(QuestionIndices);
        }

        [Fact]
        public void FlagIfGameIsInProgress()
        {
            // Game in progress
            var gameSession = new ReindeerGameSession(GetTestSession(), Substitute.For<ILogger>());
            gameSession.IsGameInProgress().Should().BeTrue();

            // No game in progress
            var noGameSession = new ReindeerGameSession(Substitute.For<ISession>(), Substitute.For<ILogger>());
            noGameSession.IsGameInProgress().Should().BeFalse();
        }

        [Fact]
        public void ReturnValidSessionDictionary()
        {
            var gameSession = new ReindeerGameSession(GetTestSession(), Substitute.For<ILogger>());
            var dictionary = gameSession.CreateSessionDictionary();

            dictionary["Score"].Should().Be(3);
            ((int[])dictionary["QuestionIndices"]).ShouldAllBeEquivalentTo(QuestionIndices);
            dictionary["CurrentQuestion"].Should().BeSameAs(SampleQuestion);
        }

        private static ISession GetTestSession()
        {
            var session = Substitute.For<ISession>();
            session.GetObject<int>("Score").Returns(3);
            session.GetObject<int[]>("QuestionIndices").Returns(QuestionIndices);
            session.GetObject<SelectedQuestion>("CurrentQuestion").Returns(SampleQuestion);

            return session;
        }
    }
}
