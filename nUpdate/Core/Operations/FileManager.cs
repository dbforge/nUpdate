using System;
using System.Collections.Generic;
using System.IO;

namespace nUpdate.Core.Operations
{
    internal class FileManager
    {
        public FileManager(Operation[] operations)
        {
        }

        /// <summary>
        ///     Returns an array of exceptions that appeared while performing different file operations.
        /// </summary>
        public Exception[] FileExceptions { get; set; }

        private void MoveFile(string filePath, string newPath)
        {
            try
            {
                File.Move(filePath, newPath);
            }
            catch (Exception ex)
            {
                var exceptions = new List<Exception>();
                foreach (Exception exception in FileExceptions)
                {
                    exceptions.Add(exception);
                }

                exceptions.Add(ex);
                FileExceptions = exceptions.ToArray();
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
                var exceptions = new List<Exception>();
                foreach (Exception exception in FileExceptions)
                {
                    exceptions.Add(exception);
                }

                exceptions.Add(ex);
                FileExceptions = exceptions.ToArray();
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
                var exceptions = new List<Exception>();
                foreach (Exception exception in FileExceptions)
                {
                    exceptions.Add(exception);
                }

                exceptions.Add(ex);
                FileExceptions = exceptions.ToArray();
            }
        }
    }
}