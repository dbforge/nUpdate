using System.Windows;
using System.Windows.Media;

namespace nUpdate.Administration.Controls
{
    public partial class FeatureButton
    {
        public FeatureButton()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(FeatureButton), new PropertyMetadata(null, OnIconChanged));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(FeatureButton), new PropertyMetadata(null, OnTextChanged));

        private static void OnIconChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var b = (FeatureButton)obj;
            var value = (ImageSource)args.NewValue;
            if (value != null)
                b.Icon.Source = value;
        }

        private static void OnTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var b = (FeatureButton)obj;
            var value = (string)args.NewValue;
            if (value != null)
                b.MainTextBlock.Text = value;
        }

        public ImageSource IconSource { get; set; }
        public string Text { get; set; }
    }
}
