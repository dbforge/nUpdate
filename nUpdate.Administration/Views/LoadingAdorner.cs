using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace nUpdate.Administration.Views
{
    public class LoadingAdorner : Adorner
    {
        public LoadingAdorner(UIElement adornedElement) 
            : base(adornedElement)
        { }

        public string Text { get; set; } = string.Empty;

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.LightGray, 1),
                new Rect(new Point(0, 0), DesiredSize));

            var typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, 
                FontWeights.Normal, FontStretches.Normal);
            var formattedText = new FormattedText(Text, CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, typeface, 12, Brushes.Black);
            var centerPoint = new Point(Width / 2, Height / 2);
            dc.DrawText(formattedText, new Point(centerPoint.X - formattedText.Width / 2, 
                centerPoint.Y - formattedText.Height / 2));

            base.OnRender(dc);
        }
    }
}
