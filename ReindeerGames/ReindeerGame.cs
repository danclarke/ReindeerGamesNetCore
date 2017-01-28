using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace ReindeerGames
{
    public interface IReindeerGame
    {
        /// <summary>
        /// Execute the request
        /// </summary>
        /// <param name="request">Type of request from user</param>
        /// <param name="arguments">Arguments received from user, if any</param>
        /// <returns>Response to user / Alexa</returns>
        Response Execute(RequestType request, Argument[] arguments);
    }

    /// <summary>
    /// Reindeer Game for AI Assistants - currently pretty hard coded to Alexa...
    /// </summary>
    /// <remarks>Heavy use of arrays and direct logic for speed / memory usage keeping Function cost down</remarks>
    public sealed class ReindeerGame : IReindeerGame
    {
        // Settings
        private const int GameLength = 5;
        private const string CardTitle = "Reindeer Games";

        // Slots
        private const string SlotAnswer = "ANSWER";

        /// <summary>
        /// String answers. Key = Answer from user, Value = Numeric value
        /// </summary>
        private static readonly Dictionary<string, int> AnswerLookup = new Dictionary<string, int>
        {
            {"ONE", 1},
            {"TWO", 2},
            {"THREE", 3},
            {"FOUR", 4},
        };

        // Dependencies
        private readonly ISession _session;
        private readonly ILogger _log;
        private readonly IQuestionFactory _questionFactory;

        // State
        private readonly ReindeerGameSession _gameSession;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">Session from voice service</param>
        /// <param name="log">Logger to use for logging</param>
        /// <param name="questionFactory">Question Factory to use for questions</param>
        public ReindeerGame(ISession session, ILogger log, IQuestionFactory questionFactory)
        {
            _log = log;
            _questionFactory = questionFactory;
            _session = session;
            _gameSession = new ReindeerGameSession(session, _log);
        }

        /// <summary>
        /// Execute the request
        /// </summary>
        /// <param name="request">Type of request from user</param>
        /// <param name="arguments">Arguments received from user, if any</param>
        /// <returns>Response to user / Alexa</returns>
        public Response Execute(RequestType request, Argument[] arguments)
        {
            switch (request)
            {
                case RequestType.LaunchGame:
                    return GetWelcomeResponse();

                case RequestType.EndGame:
                    return FinishGame();

                case RequestType.AnswerYes:
                case RequestType.AnswerNo:
                case RequestType.AnswerDontKnow:
                case RequestType.AnswerGeneric:
                    return ProcessAnswer(arguments);

                case RequestType.AnswerRepeat:
                    return RepeatLastInstructions();

                case RequestType.AnswerHelp:
                    return GetHelp();

                default:
                    _log.LogLine("Unexpected request type: " + request);
                    throw new InvalidOperationException("Unexpected launch type: " + request);
            }
        }

        /// <summary>
        /// Finish the game, close off the session
        /// </summary>
        /// <returns>Response to user</returns>
        private Response FinishGame()
        {
            _log.LogLine($"Finishing game with session '{_session.Id}'");

            return new Response("Good bye!", endSession: true);
        }

        private Response RepeatLastInstructions()
        {
            // See if we've already got a question good to go, if so repeat i
            if (_gameSession.IsGameInProgress())
            {
                var questionText = GetQuestionPromptText(_gameSession.CurrentQuestion);
                return new Response(questionText, questionText, _gameSession.CreateSessionDictionary());
            }

            // No question, start anew
            return GetWelcomeResponse();
        }

        /// <summary>
        /// Process a mid-game answer from the user
        /// </summary>
        /// <param name="arguments">User provided data</param>
        /// <returns>Response to user</returns>
        private Response ProcessAnswer(IEnumerable<Argument> arguments)
        {
            _log.LogLine($"Processing answer from user with Session ID '{_session.Id}'");

            // If no game in progress, can't really process the answer....
            if (!_gameSession.IsGameInProgress())
            {
                _log.LogLine("Game not in progress, starting a new one");
                return GetWelcomeResponse();
            }

            // Validate answer from user
            var answer = GetAnswerNum(arguments);
            if (answer == null)
            {
                _log.LogLine("Answer not valid, prompting user to be more intelligent");

                var repeatQuestionText = GetQuestionPromptText(_gameSession.CurrentQuestion);
                var message = $"Your answer must be a number between 1 and {_questionFactory.AnswerCount}. " + repeatQuestionText;
                return new Response(message, repeatQuestionText, CardTitle, message, _gameSession.CreateSessionDictionary());
            }

            // Process the answer and get the next question
            var response = GetAnswerResponse(answer.Value);
            var question = GetNextQuestion();

            // End of game!
            if (question == null)
            {
                _log.LogLine("No more questions, game end");
                response.AppendFormat("You got {0} out of {1} questions correct. Thank you for playing!", _gameSession.Score, GameLength);
                var message = response.ToString();
                return new Response(message, null, CardTitle, message, endSession: true);
            }

            // Move on to the next question.
            _log.LogLine("Continuing to next question");
            response.AppendFormat("Your score is {0}. ", _gameSession.Score);
            _gameSession.CurrentQuestion = question;
            var questionText = GetQuestionPromptText(_gameSession.CurrentQuestion);
            response.Append(questionText);
            var responseText = response.ToString();

            return new Response(responseText, questionText, CardTitle, responseText, _gameSession.CreateSessionDictionary());
        }

        /// <summary>
        /// Initialise a brand new game
        /// </summary>
        /// <returns>Response to user</returns>
        private Response GetWelcomeResponse()
        {
            _log.LogLine("Starting a new game");

            var speechOutput = new StringBuilder(
                $"I will ask you {GameLength} questions, try to get as many right as you can. Just say the number of the answer. Let's begin. ");

            var sessionQuestions = _questionFactory.GetRandomQuestionIndices(GameLength);
            var firstQuestion = _questionFactory.GetQuestionSelection(sessionQuestions[0], 1);
            var questionText = GetQuestionPromptText(firstQuestion);

            // Ask question straight away
            speechOutput.Append(questionText);

            // Update session
            _gameSession.CurrentQuestion = firstQuestion;
            _gameSession.Score = 0;
            _gameSession.QuestionIndices = sessionQuestions;

            // Create the response and return
            var speechText = speechOutput.ToString();
            return new Response(speechText, questionText, CardTitle, speechText, _gameSession.CreateSessionDictionary());
        }

        /// <summary>
        /// Get the response for the specified answer, and update the score accordingly
        /// </summary>
        /// <param name="answer">Answer user gave</param>
        /// <returns>Response to user</returns>
        private StringBuilder GetAnswerResponse(int answer)
        {
            // Handle tally
            var output = new StringBuilder();
            var correctNumber = _gameSession.CurrentQuestion.CorrectAnswerIndex + 1;
            if (answer == correctNumber)
            {
                ++_gameSession.Score;
                output.Append("Correct. ");
            }
            else
            {
                var answerText =
                    _questionFactory.GetQuestion(_gameSession.CurrentQuestion.QuestionIndex)
                        .Answers[_gameSession.CurrentQuestion.AnswerShuffleIndices[
                            _gameSession.CurrentQuestion.CorrectAnswerIndex]];
                output.AppendFormat("Incorrect. The correct answer was {0}. {1}. ", _gameSession.CurrentQuestion.CorrectAnswerIndex + 1, answerText);
            }

            return output;
        }

        /// <summary>
        /// Output help to the user
        /// </summary>
        /// <returns>Help message</returns>
        private Response GetHelp()
        {
            _log.LogLine($"Speaking help with Session ID '{_session.Id}'");

            var message = $"I will ask you {GameLength} multiple choice questions. Respond with the number of the answer. For example, say one, two, three, or four. To start a new game at any time, say, start game. To repeat the last question, say, repeat. Would you like to keep playing?";

            const string repromptMessage = "To give an answer to a question, respond with the number of the answer. Would you like to keep playing?";

            return new Response(message, repromptMessage, _gameSession.CreateSessionDictionary());
        }

        /// <summary>
        /// Get the spoken text for a specifc question
        /// </summary>
        /// <param name="questionSelection">The question that's currently selected</param>
        /// <returns>Spoken text for question</returns>
        private string GetQuestionPromptText(SelectedQuestion questionSelection)
        {
            var builder = new StringBuilder();
            var question = _questionFactory.GetQuestion(questionSelection.QuestionIndex);

            // Question text
            builder.AppendFormat("Question {0}. ", questionSelection.QuestionNum);
            builder.Append(question.QuestionText);
            builder.Append(" ");

            // Question answers
            for (int i = 0; i < _questionFactory.AnswerCount; ++i)
            {
                var answer = question.Answers[questionSelection.AnswerShuffleIndices[i]];
                var answerNum = i + 1;
                builder.AppendFormat("{0}. {1}. ", answerNum, answer);
            }

            // Return text
            return builder.ToString();
        }

        /// <summary>
        /// Get the next question, or NULL if no more!
        /// </summary>
        /// <returns>Next question, or NULL for end of questions</returns>
        private SelectedQuestion GetNextQuestion()
        {
            var current = _gameSession.CurrentQuestion;
            var nextIndex = current.QuestionNum;

            if (nextIndex >= _gameSession.QuestionIndices.Length)
                return null;

            var questionIndex = _gameSession.QuestionIndices[nextIndex];
            return _questionFactory.GetQuestionSelection(questionIndex, current.QuestionNum + 1);
        }

        /// <summary>
        /// Get the answer from the user, or NULL if not valid
        /// </summary>
        /// <param name="arguments">Values from the user</param>
        /// <returns>Answer or NULL if not valid</returns>
        private int? GetAnswerNum(IEnumerable<Argument> arguments)
        {
            var argument = arguments.FirstOrDefault(a => a.Name.ToUpperInvariant() == SlotAnswer);

            // No value!
            if (string.IsNullOrWhiteSpace(argument?.Value))
                return null;

            _log.LogLine("Processing answer: " + argument.Value);

            // Check against string values
            int answer;
            var upperValue = argument.Value.ToUpperInvariant();
            if (AnswerLookup.TryGetValue(upperValue, out answer))
                return answer;

            // Try and convert to int
            if (!int.TryParse(argument.Value, out answer))
                return null;

            // Validate correct range
            if (answer > 0 && answer <= _questionFactory.AnswerCount)
                return answer;

            // Couldn't get anything worthwhile
            return null;
        }
    }
}
