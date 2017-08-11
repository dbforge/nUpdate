// Author: Dominic Beger (Trade/ProgTrade) 2017

namespace nUpdate.Administration
{
    public class FirstSetupData
    {
        public string ApplicationDataLocation { get; set; }
        public string DefaultProjectDirectory { get; set; }
        public bool EncryptKeyDatabase { get; set; }
        public string MasterPassword { get; set; }
    }
}