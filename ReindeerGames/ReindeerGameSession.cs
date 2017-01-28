using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReindeerGames
{
    public sealed class ReindeerGameSession
    {
        // Keys for session
        private const string KeyCurrentQuestion = "CurrentQuestion";
        private const string KeyQuestionIndices = "QuestionIndices";
        private const string KeyScore = "Score";

        /// <summary>
        /// The current question, NULL if no game available
        /// </summary>
        public SelectedQuestion CurrentQuestion { get; set; }

        /// <summary>
        /// The questions to output
        /// </summary>
        public int[] QuestionIndices { get; set; }

        /// <summary>
        /// The current score
        /// </summary>
        public int Score { get; set; }

        public ReindeerGameSession(ISession session, ILogger logger)
        {
            CurrentQuestion = session.GetObject<SelectedQuestion>(KeyCurrentQuestion);

            // Load in rest of session data
            if (CurrentQuestion != null)
            {
                logger.LogLine("Restoring session...");

                Score = session.GetObject<int>(KeyScore);
                QuestionIndices = session.GetObject<int[]>(KeyQuestionIndices);
            }
        }

        /// <summary>
        /// Check whether a game is actually in progress
        /// </summary>
        /// <returns>Game is in progress</returns>
        public bool IsGameInProgress()
        {
            return CurrentQuestion != null;
        }

        /// <summary>
        /// Get the session dictionary for future access
        /// </summary>
        /// <returns>Dictionary to use for session</returns>
        public Dictionary<string, object> CreateSessionDictionary()
        {
            if (CurrentQuestion == null)
                throw new InvalidOperationException("CurrentQuestion must be populated to create a new game session!");

            return new Dictionary<string, object>
            {
                { KeyCurrentQuestion, CurrentQuestion },
                { KeyQuestionIndices, QuestionIndices },
                { KeyScore, Score }
            };
        }
    }
}
