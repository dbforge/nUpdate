// IProgressReporter.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System;

namespace nUpdate.UpdateInstaller.UserInterface
{
    public interface IProgressReporter
    {
        /// <summary>
        ///     Reports exceptions that occur during the updating process.
        /// </summary>
        /// <param name="ex">The current exception that occured.</param>
        void Fail(Exception ex);

        /// <summary>
        ///     Initializes the user interface and loads the necessary resources.
        /// </summary>
        void Initialize(string appExecutablePath, string appName);

        /// <summary>
        ///     Reports exceptions that occur during the data initialization process before any actions are performed.
        /// </summary>
        /// <param name="ex">The current exception that occured.</param>
        void InitializingFail(Exception ex);

        /// <summary>
        ///     Reports the progress of the updating process when operations are executed.
        /// </summary>
        /// <param name="progress">The current progress percentage.</param>
        /// <param name="currentOperation">The current operation that is being executed.</param>
        void ReportOperationProgress(float progress, string currentOperation);

        /// <summary>
        ///     Reports the progress of the updating process.
        /// </summary>
        /// <param name="progress">The current progress percentage.</param>
        /// <param name="currentFile">The current file that is being copied to the destination.</param>
        void ReportUnpackingProgress(float progress, string currentFile);

        /// <summary>
        ///     Terminates the updating process.
        /// </summary>
        void Terminate();
    }
}