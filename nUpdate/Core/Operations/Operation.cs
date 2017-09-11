// Copyright © Dominic Beger 2017

using System;

namespace nUpdate.Internal.Core.Operations
{
    public class Operation
    {
        public Operation(OperationArea area, OperationMethod method, string value, object value2 = null)
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
        public OperationMethod Method { get; set; }

        /// <summary>
        ///     The value of the current operation.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     The second value of the current operation if it needs more than one argument.
        /// </summary>
        public object Value2 { get; set; }

        /// <summary>
        ///     Gets the operation area and method from a given tag.
        /// </summary>
        /// <param name="areaTag">The tag to check.</param>
        /// <returns>Returns a new Tuple with the area and method for the given tag.</returns>
        public static Tuple<OperationArea, OperationMethod> GetOperation(object areaTag)
        {
            var areaTagString = areaTag.ToString();
            switch (areaTagString)
            {
                case "DeleteFile":
                    return new Tuple<OperationArea, OperationMethod>(OperationArea.Files, OperationMethod.Delete);
                case "RenameFile":
                    return new Tuple<OperationArea, OperationMethod>(OperationArea.Files, OperationMethod.Rename);
                case "CreateRegistrySubKey":
                    return new Tuple<OperationArea, OperationMethod>(OperationArea.Registry, OperationMethod.Create);
                case "DeleteRegistrySubKey":
                    return new Tuple<OperationArea, OperationMethod>(OperationArea.Registry, OperationMethod.Delete);
                case "SetRegistryValue":
                    return new Tuple<OperationArea, OperationMethod>(OperationArea.Registry, OperationMethod.SetValue);
                case "DeleteRegistryValue":
                    return new Tuple<OperationArea, OperationMethod>(OperationArea.Registry,
                        OperationMethod.DeleteValue);
                case "StartProcess":
                    return new Tuple<OperationArea, OperationMethod>(OperationArea.Processes, OperationMethod.Start);
                case "TerminateProcess":
                    return new Tuple<OperationArea, OperationMethod>(OperationArea.Processes, OperationMethod.Stop);
                case "StartService":
                    return new Tuple<OperationArea, OperationMethod>(OperationArea.Services, OperationMethod.Start);
                case "StopService":
                    return new Tuple<OperationArea, OperationMethod>(OperationArea.Services, OperationMethod.Stop);
                case "ExecuteScript":
                    return new Tuple<OperationArea, OperationMethod>(OperationArea.Scripts, OperationMethod.Execute);
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
                        case OperationMethod.DeleteValue:
                            return "DeleteRegistryValue";
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