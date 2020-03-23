using System;
using FluentFTP;
using nUpdate.Administration.PluginBase.Models;

namespace nUpdate.Administration.Models.Ftp
{
    public class FtpServerItem : IServerItem
    {
        private readonly FtpListItem _ftpsItem;

        public FtpServerItem(FtpListItem ftpsItem)
        {
            _ftpsItem = ftpsItem;
        }

        public ServerItemType ItemType
        {
            get
            {
                if (_ftpsItem.Type == FtpFileSystemObjectType.Directory)
                    return ServerItemType.Directory;
                return _ftpsItem.Type == FtpFileSystemObjectType.File ? ServerItemType.File : ServerItemType.Other;
            }
        }

        public DateTime? Modified => _ftpsItem.Modified;

        public string Name => _ftpsItem.Name;

        public long Size => _ftpsItem.Size;
    }
}