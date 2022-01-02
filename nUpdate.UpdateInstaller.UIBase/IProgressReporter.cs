// IProgressReporter.cs, 01.08.2018
// Copyright (C) Dominic Beger 17.06.2019

using System;

namespace nUpdate.UpdateInstaller.UIBase
{
    public interface IProgressReporter
    {
        /// <summary>
        ///     Initializes the user interface and loads the necessary resources.
        /// </summary>
        void Initialize();

        /// <summary>
        ///     Reports the progress of the updating process.
        /// </summary>
        /// <param name="progress">The current progress percentage.</param>
        /// <param name="currentFile">The current file that is being copied to the destination.</param>
        void ReportUnpackingProgress(float progress, string currentFile);

        /// <summary>
        ///     Reports the progress of the updating process when operations are executed.
        /// </summary>
        /// <param name="progress">The current progress percentage.</param>
        /// <param name="currentOperation">The current operation that is being executed.</param>
        void ReportOperationProgress(float progress, string currentOperation);

        /// <summary>
        ///     Reports exceptions that occur during the updating process.
        /// </summary>
        /// <param name="ex">The current exception that occured.</param>
        void Fail(Exception ex);

        /// <summary>
        ///     Reports exceptions that occur during the data initialization process before any actions are performed.
        /// </summary>
        /// <param name="ex">The current exception that occured.</param>
        void InitializingFail(Exception ex);

        /// <summary>
        ///     Terminates the updating process.
        /// </summary>
        void Terminate();
    }
}