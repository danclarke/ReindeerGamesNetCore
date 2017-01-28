using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ReindeerGames
{
    /// <summary>
    /// Logger to use for logging status messages
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log a simple line of text
        /// </summary>
        /// <param name="logText">Text to log</param>
        void LogLine(string logText);

        /// <summary>
        /// Log a formatted line of text, like string.Format
        /// </summary>
        /// <param name="logTextFormat">Text format</param>
        /// <param name="args">Arguments for text format</param>
        void LogLine(string logTextFormat, params object[] args);
    }
}
