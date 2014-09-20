// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11

using System;

namespace nUpdate.Administration.Core.Update.Operations
{
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
                case "CreateRegistryEntry":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Registry, OperationMethods.Create);
                case "DeleteRegistryEntry":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Registry, OperationMethods.Delete);
                case "SetRegistryKeyValue":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Registry, OperationMethods.SetValue);
                case "StartProcess":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Processes, OperationMethods.Start);
                case "Terminaterocess":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Processes, OperationMethods.Stop);
                case "StartService":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Services, OperationMethods.Start);
                case "StopService":
                    return new Tuple<OperationArea, OperationMethods>(OperationArea.Services, OperationMethods.Stop);
            }
            return null;
        }
    }
}