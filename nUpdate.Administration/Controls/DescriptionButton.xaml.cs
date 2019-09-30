using System.Windows;
using System.Windows.Media;

namespace nUpdate.Administration.Controls
{
    public partial class DescriptionButton
    {
        public DescriptionButton()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(DescriptionButton), new PropertyMetadata(null, OnIconChanged));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DescriptionButton), new PropertyMetadata(null, OnTextChanged));
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(DescriptionButton), new PropertyMetadata(null, OnDescriptionChanged));

        private static void OnDescriptionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var b = (DescriptionButton)obj;
            var value = (string)args.NewValue;
            if (value != null)
                b.DescriptionTextBlock.Text = value;
        }

        private static void OnIconChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var b = (DescriptionButton)obj;
            var value = (ImageSource)args.NewValue;
            if (value != null)
                b.Icon.Source = value;
        }

        private static void OnTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var b = (DescriptionButton)obj;
            var value = (string)args.NewValue;
            if (value != null)
                b.MainTextBlock.Text = value;
        }

        public ImageSource IconSource { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }
}
