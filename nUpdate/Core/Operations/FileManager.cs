using System;
using System.Collections.Generic;
using System.IO;

namespace nUpdate.Core.Operations
{
    internal class FileManager
    {
        /// <summary>
        /// Returns an array of exceptions that appeared while performing different file operations.
        /// </summary>
        public Exception[] FileExceptions { get; set; }

        public FileManager(Operation[] operations)
        {
        }

        private void MoveFile(string filePath, string newPath)
        {
            try
            {
                File.Move(filePath, newPath);
            }
            catch (Exception ex)
            {
                List<Exception> exceptions = new List<Exception>();
                foreach (Exception exception in this.FileExceptions)
                {
                    exceptions.Add(exception);
                }

                exceptions.Add(ex);
                this.FileExceptions = exceptions.ToArray();
            }
        }

        private void RenameFile(string filePath, string newName)
        {
            try
            {
                File.Move(filePath, Path.Combine(Directory.GetParent(filePath).FullName, newName));
            }
            catch (Exception ex)
            {
                List<Exception> exceptions = new List<Exception>();
                foreach (Exception exception in this.FileExceptions)
                {
                    exceptions.Add(exception);
                }

                exceptions.Add(ex);
                this.FileExceptions = exceptions.ToArray();
            }
        }

        private void DeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                List<Exception> exceptions = new List<Exception>();
                foreach (Exception exception in this.FileExceptions)
                {
                    exceptions.Add(exception);
                }

                exceptions.Add(ex);
                this.FileExceptions = exceptions.ToArray();
            }
        }
    }
}
