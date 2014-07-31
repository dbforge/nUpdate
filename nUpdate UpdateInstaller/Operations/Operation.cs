using System;

namespace nUpdate.UpdateInstaller.Operations
{
    public class Operation
    {
        internal string UpdateOperation { get; set; }

        public Operation(string operation)
        {
            this.UpdateOperation = operation;
        }

        /// <summary>
        /// Executes the set operation.
        /// </summary>
        public void ExecuteOperation()
        {
            //#F-#R-FullName-NewName // File-Operation, here: Rename a file.
            //#S-#S-ServiceName      // Service-Operation, here: Start a service.
            //#R-#C-Entry            // Registry-Operation, here: Create an entry.
            //#P-#S-ProcessName      // Process-Operation, here: Start a process.

            string[] operationParts = this.UpdateOperation.Split(new char[] { '-' });
            if (operationParts[0] == "#F")
            {
                var fileManager = new FileManager();
                switch (operationParts[1])
                {
                    case "#R":
                        if (operationParts.Length != 4)
                        {
                            fileManager.FileExceptions.Add(new Exception(String.Format("Renaming file {0} failed: Not enough arguments.", operationParts[1])));
                            return;
                        }
                        fileManager.RenameFile(operationParts[1], operationParts[2]);
                        break;

                    case "#C":
                        if (operationParts.Length != 3)
                        {
                            fileManager.FileExceptions.Add(new Exception(String.Format("Creating file {0} failed: Not enough arguments.", operationParts[1])));
                            return;
                        }
                        fileManager.CreateFile(operationParts[1]);
                        break;

                    case "#D":
                        if (operationParts.Length != 3)
                        {
                            fileManager.FileExceptions.Add(new Exception(String.Format("Deleting file {0} failed: Not enough arguments.", operationParts[1])));
                            return;
                        }
                        fileManager.DeleteFile(operationParts[1]);
                        break;
                    case "#M":
                        if (operationParts.Length != 4)
                        {
                            fileManager.FileExceptions.Add(new Exception(String.Format("Moving file {0} failed: Not enough arguments.", operationParts[1])));
                            return;
                        }
                        fileManager.MoveFile(operationParts[1], operationParts[2]);
                        break;
                }
            }
            else if (operationParts[0] == "#R")
            {
                var registryManager = new RegistryManager();
                switch (operationParts[1])
                {
                    case "#C":
                        if (operationParts.Length != 3)
                        {
                            registryManager.RegistryExceptions.Add(new Exception(String.Format("Creating file {0} failed: Not enough arguments.", operationParts[1])));
                            return;
                        }
                        registryManager.Create(operationParts[2]);
                        break;
                }
            }
        }
    }
}
