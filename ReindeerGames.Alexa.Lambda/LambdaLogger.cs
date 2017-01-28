using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace ReindeerGames.Alexa.Lambda
{
    /// <summary>
    /// ReindeerGames logger that uses a LambdaLogger as the underlying provider
    /// </summary>
    public class LambdaLogger : ILogger
    {
        private readonly ILambdaLogger _log;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="log">Lambda logger</param>
        public LambdaLogger(ILambdaLogger log)
        {
            _log = log;
        }

        public void LogLine(string logText)
        {
            _log.LogLine(logText);
        }

        public void LogLine(string logTextFormat, params object[] args)
        {
            var message = string.Format(logTextFormat, args);
            _log.LogLine(message);
        }
    }
}
