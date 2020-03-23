using System;
using System.Threading.Tasks;
using nUpdate.Administration.PluginBase.BusinessLogic;

namespace nUpdate.Administration.BusinessLogic
{
    public class SshServerUpdateProvider : IUpdateProvider
    {
        public Task UploadPackage()
        {
            throw new NotImplementedException();
        }
    }
}
