using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReindeerGames
{
    public class Response
    {
        /// <summary>
        /// Text to be spoken to the user
        /// </summary>
        public string SpokenResponse { get; }

        /// <summary>
        /// Text to be spoken to the user if the user doesn't respond promptly
        /// </summary>
        public string SpokenReprompt { get; }

        /// <summary>
        /// Title of card displayed on mobile
        /// </summary>
        public string CardTitle { get; }

        /// <summary>
        /// Text of card displayed on mobile
        /// </summary>
        public string CardText { get; }

        /// <summary>
        /// Whether this response should end the session 
        /// </summary>
        public bool EndSession { get; }

        /// <summary>
        /// Values to store in the session
        /// </summary>
        public Dictionary<string, object> SessionValues { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spokenResponse">Text to be spoken to the user</param>
        /// <param name="spokenReprompt">Text to be spoken to the user if the user doesn't respond promptly</param>
        /// <param name="cardTitle">Title of card displayed on mobile</param>
        /// <param name="cardText">Text of card displayed on mobile</param>
        /// <param name="endSession">Whether this response should end the session </param>
        /// <param name="sessionValues">Values to store in the session</param>
        public Response(string spokenResponse, string spokenReprompt, string cardTitle, string cardText, Dictionary<string, object> sessionValues = null,  bool endSession = false)
        {
            SpokenResponse = spokenResponse;
            SpokenReprompt = spokenReprompt;
            CardTitle = cardTitle;
            CardText = cardText;
            EndSession = endSession;
            SessionValues = sessionValues ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spokenResponse">Text to be spoken to the user</param>
        /// <param name="spokenReprompt">Text to be spoken to the user if the user doesn't respond promptly</param>
        /// <param name="endSession">Whether this response should end the session </param>
        /// <param name="sessionValues">Values to store in the session</param>
        public Response(string spokenResponse, string spokenReprompt, Dictionary<string, object> sessionValues = null, bool endSession = false)
        {
            SpokenResponse = spokenResponse;
            SpokenReprompt = spokenReprompt;
            CardTitle = null;
            CardText = null;
            EndSession = endSession;
            SessionValues = sessionValues ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spokenResponse">Text to be spoken to the user</param>
        /// <param name="endSession">Whether this response should end the session </param>
        /// <param name="sessionValues">Values to store in the session</param>
        public Response(string spokenResponse, Dictionary<string, object> sessionValues = null, bool endSession = false)
        {
            SpokenResponse = spokenResponse;
            SpokenReprompt = string.Empty;
            CardTitle = null;
            CardText = null;
            EndSession = endSession;
            SessionValues = sessionValues ?? new Dictionary<string, object>();
        }
    }
}
