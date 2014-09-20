namespace nUpdate.Client.GuiInterface
{
    public interface IProgressReporter
    {
        void Initialize();
        void ReportProgress(int progress, string currentFile);
        void Fail(string infoMessage, string errorMessage);
        void Terminate();
    }
}