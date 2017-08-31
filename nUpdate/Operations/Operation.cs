// Copyright © Dominic Beger 2017

namespace nUpdate.Operations
{
    /// <summary>
    ///     An update operation that is performed as soon as the updates are being installed.
    /// </summary>
    public class Operation
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Operation" /> class.
        /// </summary>
        public Operation(OperationArea area, OperationMethod method, string value, object value2 = null)
        {
            Area = area;
            Method = method;
            Value = value;
            Value2 = value2;
        }

        /// <summary>
        ///     Gets or sets <see cref="OperationArea" /> of the current operation.
        /// </summary>
        public OperationArea Area { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="OperationMethod" /> of the current operation.
        /// </summary>
        public OperationMethod Method { get; set; }

        /// <summary>
        ///     Gets or sets the value of the current operation.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     Gets or sets the second (optional) value of the current operation.
        /// </summary>
        public object Value2 { get; set; }

        /// <summary>
        ///     Gets the operation tag from a given operation.
        /// </summary>
        /// <param name="operation">The operation to get the tag from.</param>
        /// <returns>
        ///     Returns the tag as a <see cref="string" />.
        /// </returns>
        public static string GetOperationTag(Operation operation)
        {
            switch (operation.Area)
            {
                case OperationArea.Files:
                    switch (operation.Method)
                    {
                        case OperationMethod.Delete:
                            return "DeleteFile";
                        case OperationMethod.Rename:
                            return "RenameFile";
                    }
                    break;
                case OperationArea.Registry:
                    switch (operation.Method)
                    {
                        case OperationMethod.Create:
                            return "CreateRegistrySubKey";
                        case OperationMethod.Delete:
                            return "DeleteRegistrySubKey";
                        case OperationMethod.SetValue:
                            return "SetRegistryValue";
                    }
                    break;
                case OperationArea.Processes:
                    switch (operation.Method)
                    {
                        case OperationMethod.Start:
                            return "StartProcess";
                        case OperationMethod.Stop:
                            return "TerminateProcess";
                    }
                    break;
                case OperationArea.Services:
                    switch (operation.Method)
                    {
                        case OperationMethod.Start:
                            return "StartService";
                        case OperationMethod.Stop:
                            return "StopService";
                    }
                    break;
                case OperationArea.Scripts:
                    switch (operation.Method)
                    {
                        case OperationMethod.Execute:
                            return "ExecuteScript";
                    }
                    break;
            }
            return null;
        }
    }
}