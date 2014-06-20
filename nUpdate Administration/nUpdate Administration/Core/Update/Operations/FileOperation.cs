
namespace nUpdate.Administration.Core.Update.Operations
{
    internal class FileOperation
    {
        /// <summary>
        /// Returns the name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Returns the operation to execute.
        /// </summary>
        public FileOperations Operation { get; set; }

        /// <summary>
        /// Returns the new name of the file if the operation is "Rename".
        /// </summary>
        public string NewFileName { get; set; }
    }
}
