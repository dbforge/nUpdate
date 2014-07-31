using System;
using System.Collections.Generic;
using System.IO;

namespace nUpdate.UpdateInstaller.Operations
{
    internal class FileManager
    {
        /// <summary>
        ///     Returns an array of exceptions that appeared while performing different file operations.
        /// </summary>
        public List<Exception> FileExceptions { get; set; }

        public void CreateFile(string filePath)
        {
            try
            {
                using (FileStream fs = File.Create(filePath))
                {
                }
            }
            catch (Exception ex)
            {
                FileExceptions.Add(ex);
            }
        }

        public void MoveFile(string filePath, string newPath)
        {
            try
            {
                File.Move(filePath, newPath);
            }
            catch (Exception ex)
            {
                FileExceptions.Add(ex);
            }
        }

        public void RenameFile(string filePath, string newName)
        {
            try
            {
                File.Move(filePath, Path.Combine(Directory.GetParent(filePath).FullName, newName));
            }
            catch (Exception ex)
            {
                FileExceptions.Add(ex);
            }
        }

        public void DeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                FileExceptions.Add(ex);
            }
        }
    }
}