// Copyright © Dominic Beger 2017

namespace nUpdate.Updating
{
    /// <summary>
    ///     Represents an argument that is handled over to the application after the installation of an update.
    /// </summary>
    public class UpdateArgument
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateArgument" /> class.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="executionOptions">The execution options that should be used for the <see cref="UpdateArgument" />.</param>
        public UpdateArgument(string argument, UpdateArgumentExecutionOptions executionOptions)
        {
            Argument = argument;
            ExecutionOptions = executionOptions;
        }

        /// <summary>
        ///     Gets or sets the argument of the current <see cref="UpdateArgument" />.
        /// </summary>
        public string Argument { get; set; }

        /// <summary>
        ///     Gets or sets the execution options that should be used for the current <see cref="UpdateArgument" />.
        /// </summary>
        public UpdateArgumentExecutionOptions ExecutionOptions { get; set; }
    }
}