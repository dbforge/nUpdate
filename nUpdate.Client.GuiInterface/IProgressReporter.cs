using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
