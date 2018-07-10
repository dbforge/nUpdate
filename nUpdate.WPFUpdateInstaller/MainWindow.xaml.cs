using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using nUpdate.Internal.Core;
using nUpdate.UpdateInstaller.Client.GuiInterface;

namespace nUpdate.WPFUpdateInstaller
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IProgressReporter
    {

        
        public MainWindow()
        {
            InitializeComponent();
           }

        public void Initialize()
        {
            ShowDialog();
            using(MemoryStream memory = new MemoryStream())
            {
                var bitmap = IconHelper.ExtractAssociatedIcon(Program.ApplicationExecutablePath).ToBitmap();
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                Icon = bitmapImage;
            }
            Title = Program.AppName;
            copyingLabel.Text = Program.ExtractFilesText;
        }
        

        public void ReportUnpackingProgress(float progress, string currentFile)
        {
            Dispatcher.Invoke(() =>
            {
                extractProgressBar.Value = (int) progress;
                copyingLabel.Text = string.Format(Program.CopyingText, currentFile);
            });
        }

        public void ReportOperationProgress(float progress, string currentOperation)
        {
            Dispatcher.Invoke(() =>
            {
                extractProgressBar.Value = (int) progress;
                copyingLabel.Text = $"{currentOperation}";
            });
        }

        public void Fail(Exception ex)
        {
            Dispatcher.Invoke(new Action(() => MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error)));
        }

        public void InitializingFail(Exception ex)
        {
            Dispatcher.Invoke(new Action(() => MessageBox.Show(ex.Message,"Initialisation Fail",MessageBoxButton.OK,MessageBoxImage.Error)));
        }

        public void Terminate()
        {
            Dispatcher.Invoke(Close);
        }
    }
}
