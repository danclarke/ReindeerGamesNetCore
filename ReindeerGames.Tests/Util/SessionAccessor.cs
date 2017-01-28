using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReindeerGames;

namespace ReindeeGames.Tests.Util
{
    /// <summary>
    /// Quick accessors for game session response
    /// </summary>
    public static class SessionAccessor
    {
        public static SelectedQuestion GetQuestion(this IDictionary<string, object> dict)
        {
            try
            {
                return dict["CurrentQuestion"] as SelectedQuestion;
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public static int GetScore(this IDictionary<string, object> dict)
        {
            try
            {
                return (int)dict["Score"];
            }
            catch (KeyNotFoundException)
            {
                return -1;
            }
        }

        public static int[] GetQuestionIndices(this IDictionary<string, object> dict)
        {
            try
            {
                return (int[])dict["QuestionIndices"];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }
    }
}
