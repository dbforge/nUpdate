// Copyright © Dominic Beger 2018

using System.Collections.Generic;

namespace nUpdate.Administration.Core.Application.Extension
{
    /// <summary>
    ///     Provides more streamlined interface for associating a single or multiple extensions with a single program.
    /// </summary>
    public class AssociationManager
    {
        /// <summary>
        ///     Associates a single executable with a list of extensions.
        /// </summary>
        /// <param name="progId">Name of program id</param>
        /// <param name="executablePath">Path to executable to start including arguments.</param>
        /// <param name="extensions">String array of extensions to associate with program id.</param>
        /// <example>
        ///     progId = "MyTextFile"
        ///     executablePath = "notepad.exe %1"
        ///     extensions = ".txt", ".text"
        /// </example>
        public void Associate(string progId, string executablePath, params string[] extensions)
        {
            foreach (var s in extensions)
            {
                var fai = new FileAssociationInfo(s);

                if (!fai.Exists)
                    fai.Create(progId);

                fai.ProgId = progId;
            }

            var pai = new ProgramAssociationInfo(progId);

            if (!pai.Exists)
                pai.Create();

            pai.AddVerb(new ProgramVerb("open", executablePath));
        }

        /// <summary>
        ///     Associates an already existing program id with a list of extensions.
        /// </summary>
        /// <param name="progId">The program id to associate extensions with.</param>
        /// <param name="extensions">String array of extensions to associate with program id.</param>
        public void Associate(string progId, params string[] extensions)
        {
            foreach (var s in extensions)
            {
                var fai = new FileAssociationInfo(s);

                if (!fai.Exists)
                    fai.Create(progId);

                fai.ProgId = progId;
            }
        }

        /// <summary>
        ///     Determines of the list of extensions are associated with the specified program id.
        /// </summary>
        /// <param name="progId">Program id to check against.</param>
        /// <param name="extensions">String array of extensions to check against the program id.</param>
        /// <returns>String array of extensions that were not associated with the program id.</returns>
        public string[] CheckAssociation(string progId, params string[] extensions)
        {
            var notAssociated = new List<string>();

            foreach (var s in extensions)
            {
                var fai = new FileAssociationInfo(s);

                if (!fai.Exists || fai.ProgId != progId)
                    notAssociated.Add(s);
            }

            return notAssociated.ToArray();
        }
    }
}