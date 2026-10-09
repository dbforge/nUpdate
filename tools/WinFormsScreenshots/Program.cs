using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Net;
using System.Security.Cryptography;
using nUpdate.Platform;
using nUpdate.UI.WindowsForms.Dialogs;
using nUpdate.Updating;

namespace WinFormsScreenshots;

/// <summary>
///     Renders the update dialog of nUpdate.UI.WindowsForms for the README: the real dialog for the sample application
///     Aurora, captured from the screen and put into the Windows 11 window frame the other screenshots use. Windows only;
///     the Windows job of CI runs it and uploads the PNG.
/// </summary>
/// <remarks>Usage: <c>dotnet run --project tools/WinFormsScreenshots -- &lt;output folder&gt;</c></remarks>
internal static class Program
{
    private const string Feed = """
        {
          "format": 1,
          "packages": [
            {
              "version": "2.2.0",
              "changelog": { "en": "- A new importer for CSV and Excel files\n- Dark mode follows the system\n- Starts twice as fast" },
              "files": [
                { "platform": "any", "path": "packages/2.2.0/any.zip", "size": 18400000, "sha512": "x", "signature": { "value": "s" }, "touches": ["processes"] }
              ]
            }
          ]
        }
        """;

    /// <summary>The title font of Windows 11, then of Windows 10.</summary>
    private static readonly string[] TitleFonts = ["Segoe UI Variable Text", "Segoe UI"];

    [STAThread]
    private static int Main(string[] args)
    {
        var output = args.Length == 1 ? args[0] : throw new ArgumentException("Pass the output folder.");
        // Unscaled pixels, like the screenshots WindowScreenshots renders.
        Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        using var rsa = RSA.Create(2048);
        var services = new UpdateManagerServices { HttpClient = new HttpClient(new FeedHandler()), ApplicationInfo = new Aurora() };
        using var manager = new UpdateManager(new Uri("https://updates.example.com/aurora/nupdate.json"), rsa.ExportSubjectPublicKeyInfoPem(),
            currentVersion: new UpdateVersion("2.1.0"), services: services);
        if (!manager.CheckForUpdatesAsync().GetAwaiter().GetResult())
        {
            Console.Error.WriteLine("The sample feed offers no update.");
            return 1;
        }

        using var dialog = new NewUpdateDialog(manager) { StartPosition = FormStartPosition.Manual, Location = new Point(48, 48), TopMost = true };
        dialog.Show();
        for (var i = 0; i < 40; i++)
        {
            Application.DoEvents();
            Thread.Sleep(25);
        }

        // From the screen rather than DrawToBitmap, which leaves a RichTextBox such as the changelog empty.
        var client = dialog.RectangleToScreen(dialog.ClientRectangle);
        using var content = new Bitmap(client.Width, client.Height);
        using (var graphics = Graphics.FromImage(content))
            graphics.CopyFromScreen(client.Location, Point.Empty, client.Size);
        using var framed = Windows11Frame(content, dialog.Text, dialog.Icon);
        Directory.CreateDirectory(output);
        var path = Path.Combine(output, "winforms-update-dialog.png");
        framed.Save(path, ImageFormat.Png);
        Console.WriteLine("Wrote " + path);
        dialog.Close();
        return 0;
    }

    /// <summary>
    ///     The client area in a window frame as Windows 11 draws it, with the measures of WindowScreenshots: a white
    ///     32-pixel title bar with the icon and the title, a 46-pixel Close button (the dialog cannot be resized), rounded
    ///     corners, an outline and a shadow.
    /// </summary>
    private static Bitmap Windows11Frame(Bitmap content, string title, Icon? icon)
    {
        const int margin = 28, titleBar = 32, radius = 8;
        var window = new Rectangle(margin, margin, content.Width + 2, content.Height + titleBar + 2);
        var bitmap = new Bitmap(window.Width + 2 * margin, window.Height + 2 * margin, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        // Stacked translucent shapes, larger and fainter outwards, give the soft shadow below the window.
        for (var spread = 20; spread > 0; spread--)
        {
            var shadow = Rectangle.Inflate(window, spread, spread);
            shadow.Offset(0, 6);
            using var shadowPath = RoundedRectangle(shadow, radius + spread);
            using var shadowBrush = new SolidBrush(Color.FromArgb(3, 0, 0, 0));
            graphics.FillPath(shadowBrush, shadowPath);
        }

        using var outline = RoundedRectangle(window, radius);
        graphics.FillPath(Brushes.White, outline);
        graphics.SetClip(outline);
        var ink = Color.FromArgb(0x1B, 0x1B, 0x1B);
        var textLeft = window.X + 1 + 12;
        if (icon is not null)
        {
            using var small = new Icon(icon, 16, 16);
            graphics.DrawIcon(small, new Rectangle(textLeft, window.Y + 1 + (titleBar - 16) / 2, 16, 16));
            textLeft += 16 + 10;
        }

        using (var font = new Font(TitleFonts.FirstOrDefault(IsInstalled) ?? FontFamily.GenericSansSerif.Name, 9f))
        using (var brush = new SolidBrush(ink))
        using (var format = new StringFormat { LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
            graphics.DrawString(title, font, brush, new RectangleF(textLeft, window.Y + 1, window.Right - 1 - 46 - textLeft, titleBar), format);

        using (var pen = new Pen(ink, 1f))
        {
            var centre = new PointF(window.Right - 1 - 23, window.Y + 1 + titleBar / 2f);
            graphics.DrawLine(pen, centre.X - 4.25f, centre.Y - 4.25f, centre.X + 4.25f, centre.Y + 4.25f);
            graphics.DrawLine(pen, centre.X + 4.25f, centre.Y - 4.25f, centre.X - 4.25f, centre.Y + 4.25f);
        }

        graphics.DrawImage(content, new Rectangle(window.X + 1, window.Y + 1 + titleBar, content.Width, content.Height));
        graphics.ResetClip();
        using (var pen = new Pen(Color.FromArgb(0xC9, 0xCC, 0xD1), 1f))
            graphics.DrawPath(pen, outline);
        return bitmap;
    }

    private static bool IsInstalled(string family)
    {
        using var fonts = new InstalledFontCollection();
        return fonts.Families.Any(f => string.Equals(f.Name, family, StringComparison.OrdinalIgnoreCase));
    }

    private static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
    {
        var diameter = 2 * radius;
        var path = new GraphicsPath();
        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    /// <summary>Answers every request with the sample feed.</summary>
    private sealed class FeedHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(Feed) });
    }

    /// <summary>The sample application of the screenshots.</summary>
    private sealed class Aurora : IApplicationInfo
    {
        public string ProductName => "Aurora";

        public string? ExecutablePath => Application.ExecutablePath;

        public string? DeclaredVersion => "2.1.0";

        public string UserAgentProduct => "Aurora/2.1.0";

        public int CurrentProcessId => Environment.ProcessId;
    }
}
