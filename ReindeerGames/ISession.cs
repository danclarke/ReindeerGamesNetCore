using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReindeerGames
{
    /// <summary>
    /// Session from external AI provider
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// ID of the session
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Get a specific object from the session
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="key">Key where object should be</param>
        /// <returns>Object</returns>
        T GetObject<T>(string key);
    }
}
