using System;

namespace nUpdate.Administration.Core
{
    [Serializable]
    public class HelpEntry
    {
        /// <summary>
        ///     Gets or sets the question of the help entry.
        /// </summary>
        /// <value>
        /// The question.
        /// </value>
        public string Question { get; set; }

        /// <summary>
        ///     Gets or sets the answer of the help entry.
        /// </summary>
        /// <value>
        /// The answer.
        /// </value>
        public string Answer { get; set; }
    }
}
