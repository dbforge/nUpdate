using nUpdate.Administration.Application;

namespace nUpdate.Administration.Sql
{
    // ReSharper disable once InconsistentNaming
    internal class SQLManager
    {
        public SQLManager(UpdateProject project)
        {
            Data = project.SqlData;
        }

        public SqlData Data { get; set; }
    }
}