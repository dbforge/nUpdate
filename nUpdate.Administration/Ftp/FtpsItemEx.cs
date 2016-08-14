using System;
using nUpdate.Administration.TransferInterface;
using Starksoft.Aspen.Ftps;

namespace nUpdate.Administration.Ftp
{
    internal class FtpsItemEx : IServerItem
    {
        private readonly FtpsItem _ftpsItem;

        public FtpsItemEx(FtpsItem ftpsItem)
        {
            _ftpsItem = ftpsItem;
        }

        public ServerItemType ItemType
        {
            get
            {
                if (_ftpsItem.ItemType == FtpItemType.Directory)
                    return ServerItemType.Directory;
                return _ftpsItem.ItemType == FtpItemType.Directory ? ServerItemType.File : ServerItemType.Other;
            }
        }

        public DateTime? Modified => _ftpsItem.Modified;

        public string Name => _ftpsItem.Name;

        public long Size => _ftpsItem.Size;
    }
}