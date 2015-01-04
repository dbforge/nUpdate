using System;

namespace nUpdate.Administration.Core.Update.Operations
{
    [Serializable]
    public class Operation
    {
        public Operation(OperationArea area, OperationMethods method, object value, object value2 = null)
        {
            Area = area;
            Method = method;
            Value = value;
            Value2 = value2;
        }

        /// <summary>
        ///     The area of the current operation.
        /// </summary>
        public OperationArea Area { get; set; }

        /// <summary>
        ///     The method of the current oepration.
        /// </summary>
        public OperationMethods Method { get; set; }

        /// <summary>
        ///     The value of the current operation.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        ///     The second value of the current operation if it needs more than one argument.
        /// </summary>
        public object Value2 { get; set; }

        /// <summary>
        ///     Gets the operation area and method from a given tag.
        /// </summary>
        /// <param name="areaTag">The tag to check.</param>
        /// <returns>Returns a new Tuple with the area and method for the given tag.</returns>
        public static Tuple<OperationArea, OperationMethods> GetOperation(object areaTag)
        {
            string areaTagString = areaTag.ToString();
            switch (areaTagString)
            {
                case "DeleteFile":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Files, OperationMethods.Delete);
                case "RenameFile":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Files, OperationMethods.Rename);
                case "CreateRegistrySubKey":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Registry, OperationMethods.Create);
                case "DeleteRegistrySubKey":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Registry, OperationMethods.Delete);
                case "SetRegistryValue":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Registry, OperationMethods.SetValue);
                case "DeleteRegistryValue":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Registry,
                        OperationMethods.DeleteValue);
                case "StartProcess":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Processes, OperationMethods.Start);
                case "TerminateProcess":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Processes, OperationMethods.Stop);
                case "StartService":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Services, OperationMethods.Start);
                case "StopService":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Services, OperationMethods.Stop);
            }
            return null;
        }

        /// <summary>
        ///     Gets the operation tag from a given operation.
        /// </summary>
        /// <param name="operation">The operation to get the tag from.</param>
        /// <returns>
        ///     Returns the tag as a string.
        /// </returns>
        public static string GetOperationTag(Operation operation)
        {
            switch (operation.Area)
            {
                case OperationArea.Files:
                    switch (operation.Method)
                    {
                        case OperationMethods.Delete:
                            return "DeleteFile";
                        case OperationMethods.Rename:
                            return "RenameFile";
                    }
                    break;
                case OperationArea.Registry:
                    switch (operation.Method)
                    {
                        case OperationMethods.Create:
                            return "CreateRegistrySubKey";
                        case OperationMethods.Delete:
                            return "DeleteRegistrySubKey";
                        case OperationMethods.SetValue:
                            return "SetRegistryValue";
                    }
                    break;
                case OperationArea.Processes:
                    switch (operation.Method)
                    {
                        case OperationMethods.Start:
                            return "StartProcess";
                        case OperationMethods.Stop:
                            return "TerminateProcess";
                    }
                    break;
                case OperationArea.Services:
                    switch (operation.Method)
                    {
                        case OperationMethods.Start:
                            return "StartService";
                        case OperationMethods.Stop:
                            return "StopService";
                    }
                    break;
            }
            return null;
        }
    }
}