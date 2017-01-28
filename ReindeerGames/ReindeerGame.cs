using System;
using System.Collections.Generic;
using System.Text;
using Amazon.Lambda.Core;
using Slight.Alexa.Framework.Models.Requests;
using Slight.Alexa.Framework.Models.Requests.RequestTypes;
using Slight.Alexa.Framework.Models.Responses;

namespace ReindeerGames
{
    public interface IReindeerGame
    {
        /// <summary>
        /// Execute the request
        /// </summary>
        /// <returns>Response to user / Alexa</returns>
        SkillResponse Execute();
    }

    /// <summary>
    /// Reindeer Game for AI Assistants - currently pretty hard coded to Alexa...
    /// </summary>
    /// <remarks>Heavy use of arrays and direct logic for speed / memory usage keeping Function cost down</remarks>
    public sealed class ReindeerGame : IReindeerGame
    {
        // Settings
        private const int GameLength = 5;
        private const int AnswerCount = 4;
        private const string CardTitle = "Reindeer Games";

        // Slots
        private const string SlotAnswer = "Answer";

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
        private readonly ISkillResponseFactory _responseFactory;
        private readonly SkillRequest _request;
        private readonly ILambdaLogger _log;
        private readonly Random _rand = new Random();

        // State
        private readonly ReindeerGameSession _gameSession;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="input">Request from Alexa</param>
        /// <param name="context">AWS Lambda context</param>
        /// <param name="responseFactory">Factory to generate an Alexa response</param>
        public ReindeerGame(SkillRequest input, ILambdaContext context, ISkillResponseFactory responseFactory)
        {
            _request = input;
            _responseFactory = responseFactory;
            _log = context.Logger;
            _gameSession = new ReindeerGameSession(input.Session, _log);
        }

        /// <summary>
        /// Execute the request
        /// </summary>
        /// <returns>Response to user / Alexa</returns>
        public SkillResponse Execute()
        {
            if (_request.Session.New)
                SessionStarted(_request.Request.RequestId, _request.Session);

            _log.LogLine($"Received '{_request.Request.Type}' request");

            var requestType = _request.GetRequestType();

            if (requestType == typeof(ILaunchRequest))
                return GetWelcomeResponse();

            if (requestType == typeof(IIntentRequest))
                return HandleIntent(_request.Request, _request.Session);

            if (requestType == typeof(ISessionEndedRequest))
            {
                SessionEnded(_request.Request.RequestId, _request.Session);
                return null;
            }

            _log.LogLine("Unexpected request type: " + _request.Request.Type);
            throw new InvalidOperationException("Unexpected launch type: " + _request.Request.Type);
        }

        private void SessionStarted(string requestId, Session session)
        {
            _log.LogLine($"Session started ID '{requestId}', Session ID '{session.SessionId}'");
        }

        private void SessionEnded(string requestId, Session session)
        {
            _log.LogLine($"Session ended ID '{requestId}', Session ID '{session.SessionId}'");
        }

        /// <summary>
        /// Finish the game, close off the session
        /// </summary>
        /// <param name="session">Current session</param>
        /// <returns>Response to user</returns>
        private SkillResponse FinishGame(Session session)
        {
            _log.LogLine($"Finishing game with session '{session.SessionId}'");

            return _responseFactory.CreateSkillResponse(_responseFactory.CreateCoreResponseNoCard("Good bye!", string.Empty, true));
        }

        /// <summary>
        /// Trigger the correct code depending on what the user asked for
        /// </summary>
        /// <param name="request">User request</param>
        /// <param name="session">Current session</param>
        /// <returns>Response to user</returns>
        private SkillResponse HandleIntent(IIntentRequest request, Session session)
        {
            _log.LogLine($"Handling '{request.Intent.Name}' intent with ID '{request.RequestId}', Session ID '{session.SessionId}'");

            // We'll hardcode the string values here since they're in on place
            // If they need to be used elsewhere this code will need refactoring
            switch (request.Intent.Name)
            {
                case "AnswerIntent":
                case "AnswerOnlyIntent":
                case "DontKnowIntent":
                case "AMAZON.YesIntent":
                case "AMAZON.NoIntent":
                    return ProcessAnswer(request, session);

                case "AMAZON.StartOverIntent":
                    return GetWelcomeResponse();

                case "AMAZON.RepeatIntent":
                    return RepeatLastInstructions(request.RequestId, session);

                case "AMAZON.HelpIntent":
                    return GetHelp(request.RequestId, session);

                case "AMAZON.StopIntent":
                case "AMAZON.CancelIntent":
                    return FinishGame(session);

                default:
                    _log.LogLine("Unexpected intent: " + request.Intent.Name);
                    throw new InvalidOperationException("Unexpected intent: " + request.Intent.Name);
            }
        }

        private SkillResponse RepeatLastInstructions(string requestId, Session session)
        {
            _log.LogLine($"Repeating last instructions with ID '{requestId}', Session ID '{session.SessionId}'");

            // See if we've already got a question good to go, if so repeat i
            if (_gameSession.IsGameInProgress())
            {
                var questionText = GetQuestionPromptText(_gameSession.CurrentQuestion);
                return _responseFactory.CreateSkillResponse(_responseFactory.CreateCoreResponseNoCard(questionText, questionText), session.Attributes);
            }

            // No question, start anew
            return GetWelcomeResponse();
        }

        /// <summary>
        /// Process a mid-game answer from the user
        /// </summary>
        /// <param name="request">User's answer</param>
        /// <param name="session">Current session</param>
        /// <returns>Response to user</returns>
        private SkillResponse ProcessAnswer(IIntentRequest request, Session session)
        {
            _log.LogLine($"Processing answer from user with ID '{request.RequestId}', Session ID '{session.SessionId}'");

            // If no game in progress, can't really process the answer....
            if (!_gameSession.IsGameInProgress())
            {
                _log.LogLine("Game not in progress, starting a new one");
                return GetWelcomeResponse();
            }

            // Validate answer from user
            var answer = GetAnswerNum(request.Intent);
            if (answer == null)
            {
                _log.LogLine("Answer not valid, prompting user to be more intelligent");

                var repeatQuestionText = GetQuestionPromptText(_gameSession.CurrentQuestion);
                var message = $"Your answer must be a number between 1 and {AnswerCount}. " + repeatQuestionText;
                return _responseFactory.CreateSkillResponse(_responseFactory.CreateCoreResponse(CardTitle, message, repeatQuestionText), session.Attributes);
            }

            // Process the answer and get the next question
            var response = GetAnswerResponse(answer.Value);
            var question = GetNextQuestion();

            // End of game!
            if (question == null)
            {
                _log.LogLine("No more questions, game end");
                response.AppendFormat("You got {0} out of {1} questions correct. Thank you for playing!", _gameSession.Score, GameLength);
                return _responseFactory.CreateSkillResponse(_responseFactory.CreateCoreResponse(CardTitle, response.ToString(), string.Empty, true));
            }

            // Move on to the next question.
            _log.LogLine("Continuing to next question");
            response.AppendFormat("Your score is {0}. ", _gameSession.Score);
            _gameSession.CurrentQuestion = question;
            var questionText = GetQuestionPromptText(_gameSession.CurrentQuestion);
            response.Append(questionText);

            return _responseFactory.CreateSkillResponse(
                _responseFactory.CreateCoreResponse(CardTitle, response.ToString(), questionText),
                _gameSession.CreateSessionDictionary());
        }

        /// <summary>
        /// Initialise a brand new game
        /// </summary>
        /// <returns>Response to user</returns>
        private SkillResponse GetWelcomeResponse()
        {
            _log.LogLine("Starting a new game");

            var speechOutput = new StringBuilder(
                $"I will ask you {GameLength} questions, try to get as many right as you can. Just say the number of the answer. Let's begin. ");

            var sessionQuestions = GetGameQuestionIndices();
            var firstQuestion = GetQuestion(sessionQuestions[0], 1);
            var questionText = GetQuestionPromptText(firstQuestion);

            // Ask question straight away
            speechOutput.Append(questionText);

            // Update session
            _gameSession.CurrentQuestion = firstQuestion;
            _gameSession.Score = 0;
            _gameSession.QuestionIndices = sessionQuestions;

            // Create the response and return
            return _responseFactory.CreateSkillResponse(
                _responseFactory.CreateCoreResponse(CardTitle, speechOutput.ToString(), questionText),
                _gameSession.CreateSessionDictionary());
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
                    Questions.QuestionList[_gameSession.CurrentQuestion.QuestionIndex]
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
        private SkillResponse GetHelp(string requestId, Session session)
        {
            _log.LogLine($"Speaking help with ID '{requestId}', Session ID '{session.SessionId}'");

            var message = $"I will ask you {GameLength} multiple choice questions. Respond with the number of the answer. For example, say one, two, three, or four. To start a new game at any time, say, start game. To repeat the last question, say, repeat. Would you like to keep playing?";

            const string repromptMessage = "To give an answer to a question, respond with the number of the answer. Would you like to keep playing?";

            return _responseFactory.CreateSkillResponse(
                _responseFactory.CreateCoreResponseNoCard(message, repromptMessage), 
                session.Attributes);
        }

        /// <summary>
        /// Get the spoken text for a specifc question
        /// </summary>
        /// <param name="questionSelection">The question that's currently selected</param>
        /// <returns>Spoken text for question</returns>
        private static string GetQuestionPromptText(SelectedQuestion questionSelection)
        {
            var builder = new StringBuilder();
            var question = Questions.QuestionList[questionSelection.QuestionIndex];

            // Question text
            builder.AppendFormat("Question {0}. ", questionSelection.QuestionNum);
            builder.Append(question.QuestionText);
            builder.Append(" ");

            // Question answers
            for (int i = 0; i < AnswerCount; ++i)
            {
                var answer = question.Answers[questionSelection.AnswerShuffleIndices[i]];
                var answerNum = i + 1;
                builder.AppendFormat("{0}. {1}. ", answerNum, answer);
            }

            // Return text
            return builder.ToString();
        }

        /// <summary>
        /// Get the question specified, with randomised info ready for serialisation into the session
        /// </summary>
        /// <param name="questionIndex">Index of question</param>
        /// <param name="questionNum">The number of this question to the user, starting from 1</param>
        /// <returns>Question detail</returns>
        private SelectedQuestion GetQuestion(int questionIndex, int questionNum)
        {
            var validIndices = new List<int>(AnswerCount);
            var shuffleIndices = new int[AnswerCount];
            int correctIndex;
            
            // Initially all AnswerCount locations are valid
            for (int i = 0; i < AnswerCount; ++i)
                validIndices.Add(i);

            // Select the new position of the correct (first) answer
            var index = _rand.Next(0, validIndices.Count);
            correctIndex = validIndices[index];
            shuffleIndices[0] = correctIndex;
            validIndices.Remove(index);

            // Select positions for the reamining answers
            for (int i = 1; i < AnswerCount; ++i)
            {
                index = _rand.Next(0, validIndices.Count);
                shuffleIndices[i] = validIndices[index];
                validIndices.Remove(index);
            }

            // Return the selection info
            return new SelectedQuestion
            {
                QuestionIndex = questionIndex,
                AnswerShuffleIndices = shuffleIndices,
                CorrectAnswerIndex = correctIndex,
                QuestionNum = questionNum
            };
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
            return GetQuestion(questionIndex, current.QuestionNum + 1);
        }

        /// <summary>
        /// Get indices for all of the questions that should be asked in the current session
        /// </summary>
        /// <returns>Question indices</returns>
        private int[] GetGameQuestionIndices()
        {
            var indices = new int[GameLength];

            // Reset array
            for (int i = 0; i < GameLength; ++i)
                indices[i] = -1;

            // Populate array, no duplicates
            for (int i = 0; i < GameLength; ++i)
            {
                while (true)
                {
                    var index = _rand.Next(0, Questions.QuestionList.Length);

                    // Check for duplicates
                    bool duplicate = false;
                    for (int j = 0; j < i; ++j)
                    {
                        if (indices[j] == index)
                        {
                            duplicate = true;
                            break;
                        }
                    }

                    if (duplicate)
                        continue;

                    // Not a duplicate, add to list
                    indices[i] = index;
                    break;
                }
            }

            // All done!
            return indices;
        }

        /// <summary>
        /// Get the answer from the user, or NULL if not valid
        /// </summary>
        /// <param name="intent">Intent from user</param>
        /// <returns>Answer or NULL if not valid</returns>
        private int? GetAnswerNum(Intent intent)
        {
            Slot slot = null;
            if (intent?.Slots.TryGetValue(SlotAnswer, out slot) ?? false)
            {
                // No value!
                if (string.IsNullOrWhiteSpace(slot?.Value))
                    return null;

                _log.LogLine("Processing answer: " + slot.Value);

                // Check against string values
                int answer;
                var upperValue = slot.Value.ToUpperInvariant();
                if (AnswerLookup.TryGetValue(upperValue, out answer))
                    return answer;

                // Try and convert to int
                if (!int.TryParse(slot.Value, out answer))
                    return null;

                // Validate correct range
                if (answer > 0 && answer <= AnswerCount)
                    return answer;
            }

            // Couldn't get anything worthwhile
            return null;
        }
    }
}
