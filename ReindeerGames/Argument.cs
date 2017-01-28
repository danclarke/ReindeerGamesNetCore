using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReindeerGames
{
    /// <summary>
    /// Argument received from user
    /// </summary>
    public class Argument
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of argument</param>
        /// <param name="value">Value of argument</param>
        public Argument(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Name of argument
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Value of argument
        /// </summary>
        public string Value { get; }
    }
}
