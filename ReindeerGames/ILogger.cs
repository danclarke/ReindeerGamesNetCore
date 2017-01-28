using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ReindeerGames
{
    public interface ILogger
    {
        void LogLine(string logText);
        void LogLine(string logTextFormat, params object[] args);
    }
}
