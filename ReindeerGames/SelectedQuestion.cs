using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReindeerGames
{
    /// <summary>
    /// The current question the user is meant to answer
    /// </summary>
    public class SelectedQuestion
    {
        /// <summary>
        /// Index of the question in the Questions.QuestionList array
        /// </summary>
        public int QuestionIndex { get; set; }

        /// <summary>
        /// Used to randomise the answers. Loop through this array in order, and display the answer of the array value.
        /// For example if AnswerShuffleIndices[0] is 2. Then the first answer to display is the third answer in the question's answers array.
        /// </summary>
        public int[] AnswerShuffleIndices { get; set; }

        /// <summary>
        /// The correct index after shuffling.
        /// </summary>
        public int CorrectAnswerIndex { get; set; }

        /// <summary>
        /// The question number in the series the user has been asked starting from 1.
        /// </summary>
        public int QuestionNum { get; set; }
    }
}
