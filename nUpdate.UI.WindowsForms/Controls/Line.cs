namespace nUpdate.UI.WindowsForms.Controls;

/// <summary>A thin separator line.</summary>
public class Line : Control
{
    public enum Alignment
    {
        Horizontal,
        Vertical,
    }

    public Alignment LineAlignment { get; set; }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (e is null)
            return;
        e.Graphics.DrawLine(Pens.LightGray, new Point(5, 5),
            LineAlignment == Alignment.Horizontal ? new Point(500, 5) : new Point(5, 500));
    }
}
