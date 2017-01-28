using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Newtonsoft.Json.Linq;
using Slight.Alexa.Framework.Models.Requests;

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

        public ReindeerGameSession(Session session, ILambdaLogger logger)
        {
            // Load in session data
            object questionObj = null;
            if (session?.Attributes?.TryGetValue(KeyCurrentQuestion, out questionObj) ?? false)
            {
                logger.LogLine("Restoring session...");

                CurrentQuestion = ((JObject)questionObj).ToObject<SelectedQuestion>();
                Score = (int)(Int64)session.Attributes[KeyScore]; // Comes back as 64bit, don't know why...
                QuestionIndices = ((JArray)session.Attributes[KeyQuestionIndices]).ToObject<int[]>();
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
