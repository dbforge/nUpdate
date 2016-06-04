// Author: Dominic Beger (Trade/ProgTrade)
namespace nUpdate.Administration
{
    internal enum StatisticsEntryMethod
    {
        /// <summary>
        ///     Statistics entries are made over the MySQL.Data framework using external access to the database.
        /// </summary>
        External,

        /// <summary>
        ///     Statistics entries are made over PHP using internal access to the database.
        /// </summary>
        Internal,
    }
}