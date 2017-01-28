using System;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Slight.Alexa.Framework.Models.Requests;

namespace ReindeerGames.Alexa
{
    // TODO: Revisit all of this conversion madness when json.net is out of beta for the version required by the Alexa lib

    /// <summary>
    /// ReindeerGames session handler for Alexa
    /// </summary>
    public sealed class AlexaSession : ISession
    {
        private readonly Session _session;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">Session from Alexa</param>
        public AlexaSession(Session session)
        {
            _session = session;
        }

        public string Id => _session.SessionId;

        public T GetObject<T>(string key)
        {
            // Verify this is even possible
            if (!_session?.Attributes?.ContainsKey(key) ?? false)
                return default(T);

            var type = typeof(T);

            // Json.net can be a bit finicky with Alexa session values
            // So we'll need to be a little careful with how we process the data
            if (type.IsArray)
                return GetArray<T>(key);

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsValueType)
                return GetValueType<T>(key);
            else
                return GetPoco<T>(key);
        }

        /// <summary>
        /// Get simple POCO from session
        /// </summary>
        /// <typeparam name="T">Type of POCO</typeparam>
        /// <param name="key">Key for where POCO is in session</param>
        /// <returns>POCO</returns>
        private T GetPoco<T>(string key)
        {
            return ((JObject)_session.Attributes[key]).ToObject<T>();
        }

        /// <summary>
        /// Get an array from session
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="key">Key for where array is in session</param>
        /// <returns>Array</returns>
        private T GetArray<T>(string key)
        {
            return ((JArray)_session.Attributes[key]).ToObject<T>();
        }

        /// <summary>
        /// Get simple value type from session
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="key">Key for where object is in session</param>
        /// <returns>Value</returns>
        private T GetValueType<T>(string key)
        {
            // If a JValue try to just change the type
            var jValue = _session.Attributes[key] as JValue;
            if (jValue != null)
            {
                try
                {
                    return (T)Convert.ChangeType(jValue.Value, typeof(T));
                }
                catch (InvalidCastException) { }
            }

            // Sometimes Int32 comes back as Int64, so try to handle that
            // This is specific to Alexa, and doesn't appear locally
            if (typeof(T) == typeof(Int32))
            {
                try
                {
                    // Bit crazy but neccesary because of the generics
                    return (T)(object)(Int32)(Int64)_session.Attributes[key];
                }
                catch (InvalidCastException)
                {
                }
            }

            // Hope that json.net handles it OK automatically
            return (T)_session.Attributes[key];
        }
    }
}
