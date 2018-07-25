using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using nUpdate.Updating;

namespace nUpdate.WPFUserInterface.ViewModel
{
    class UpdateUiBaseVm : ViewModelBase
    {
        internal UpdateManager UpdateManager { get; set; }

        public BitmapImage WindowIcon { get; set;}


        internal BitmapImage GetIcon(Icon icon)
        {
            using(MemoryStream memory = new MemoryStream())
            {
                var bitmap = icon.ToBitmap();
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return  bitmapImage;
            }
        }

    }
}
