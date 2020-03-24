// FirstSetupData.cs, 23.03.2020
// Copyright (C) Dominic Beger 24.03.2020

namespace nUpdate.Administration.Models
{
    public class FirstSetupData
    {
        public string ApplicationDataLocation { get; set; }
        public string DefaultProjectDirectory { get; set; }
        public bool EncryptKeyDatabase { get; set; }
        public string MasterPassword { get; set; }
    }
}