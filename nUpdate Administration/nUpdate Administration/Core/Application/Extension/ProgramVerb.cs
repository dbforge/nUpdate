// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
namespace nUpdate.Administration.Core.Application.Extension
{
    /// <summary>
    ///     Provides representation of verb that is used to determine mode file is being opened in. Contained within ProgID.
    /// </summary>
    internal class ProgramVerb
    {
        private readonly string command;
        private readonly string name;

        /// <summary>
        /// </summary>
        /// <param name="name">Name of verb</param>
        /// <param name="command">Command-line path to program and arguments of associated program</param>
        public ProgramVerb(string name, string command)
        {
            this.name = name;
            this.command = command;
        }

        /// <summary>
        ///     Gets a value of the command-line path to the program that is to be called when this command verb is used.
        /// </summary>
        public string Command
        {
            get { return command; }
        }

        /// <summary>
        ///     Gets the name of the verb representing this command.
        /// </summary>
        /// <example>
        ///     "open"
        ///     "print"
        /// </example>
        public string Name
        {
            get { return name; }
        }
    }
}