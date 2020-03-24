// DescriptionButton.xaml.cs, 14.11.2019
// Copyright (C) Dominic Beger 24.03.2020

using System.Windows;
using System.Windows.Media;

namespace nUpdate.Administration.Controls
{
    public partial class DescriptionButton
    {
        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register("IconSource",
            typeof(ImageSource), typeof(DescriptionButton), new PropertyMetadata(null, OnIconChanged));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string),
            typeof(DescriptionButton), new PropertyMetadata(null, OnTextChanged));

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description",
            typeof(string), typeof(DescriptionButton), new PropertyMetadata(null, OnDescriptionChanged));

        public DescriptionButton()
        {
            InitializeComponent();
        }

        public string Description { get; set; }

        public ImageSource IconSource { get; set; }
        public string Text { get; set; }

        private static void OnDescriptionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var b = (DescriptionButton) obj;
            var value = (string) args.NewValue;
            if (value != null)
                b.DescriptionTextBlock.Text = value;
        }

        private static void OnIconChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var b = (DescriptionButton) obj;
            var value = (ImageSource) args.NewValue;
            if (value != null)
                b.Icon.Source = value;
        }

        private static void OnTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var b = (DescriptionButton) obj;
            var value = (string) args.NewValue;
            if (value != null)
                b.MainTextBlock.Text = value;
        }
    }
}