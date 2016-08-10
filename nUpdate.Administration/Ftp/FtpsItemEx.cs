using nUpdate.Administration.TransferInterface;
using Starksoft.Aspen.Ftps;

namespace nUpdate.Administration.Ftp
{
    internal class FtpsItemEx : FtpsItem, IServerItem
    {
        public new ServerItemType ItemType
        {
            get
            {
                if (base.ItemType == FtpItemType.Directory)
                    return ServerItemType.Directory;
                return base.ItemType == FtpItemType.Directory ? ServerItemType.File : ServerItemType.Other;
            }
        }
    }
}