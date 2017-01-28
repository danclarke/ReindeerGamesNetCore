using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;

namespace ReindeerGames.Alexa.AzureService
{
    /// <summary>
    /// Logger that logs to Application Insights
    /// </summary>
    public class InsightsLogger : ILogger
    {
        private readonly TelemetryClient _client;

        public InsightsLogger()
        {
            _client = new TelemetryClient();
        }

        public void LogLine(string logText)
        {
            _client.TrackTrace(logText);
        }

        public void LogLine(string logTextFormat, params object[] args)
        {
            LogLine(string.Format(logTextFormat, args));
        }
    }
}
