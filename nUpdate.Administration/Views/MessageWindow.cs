using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using nUpdate.Administration.Views.Controls;

namespace nUpdate.Administration.Views;

/// <summary>A message box: a tinted icon, the title, the message and the button bar.</summary>
[ExcludeFromCodeCoverage] // Pure UI.
public sealed class MessageWindow : Window
{
    public enum Kind
    {
        Info,
        Error,
        Question,
    }

    internal MessageWindow(string title, string message, Kind kind, string confirmText, string? cancelText)
    {
        Title = title;
        Message = message;
        Width = 480;
        SizeToContent = SizeToContent.Height;
        CanResize = false;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        var buttons = new StackPanel
        { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 8 };
        if (cancelText is not null)
        {
            var cancel = new Button { Content = cancelText, IsCancel = true, MinWidth = 90 };
            cancel.Click += (_, _) => Close(false);
            buttons.Children.Add(cancel);
        }

        var confirm = new Button { Content = confirmText, IsDefault = true, MinWidth = 90 };
        confirm.Classes.Add("primary");
        confirm.Click += (_, _) => Close(true);
        buttons.Children.Add(confirm);

        var bar = new Border { Child = buttons };
        bar.Classes.Add("bottombar");
        DockPanel.SetDock(bar, Dock.Bottom);

        var (iconKey, tint) = kind switch
        {
            Kind.Error => ("IconError", "danger"),
            Kind.Question => ("IconQuestion", null),
            _ => ("IconInfo", null),
        };
        var tile = new Border
        {
            Width = 40,
            Height = 40,
            CornerRadius = new CornerRadius(10),
            VerticalAlignment = VerticalAlignment.Top,
            Child = new Icon { Width = 18, Height = 18, Data = (Geometry?)Application.Current!.FindResource(iconKey) },
        };
        tile.Classes.Add("tile");
        if (tint is not null)
            tile.Classes.Add(tint);
        var heading = new TextBlock { Text = title, TextWrapping = TextWrapping.Wrap };
        heading.Classes.Add("h2");
        var text = new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap, LineHeight = 20 };
        text.Classes.Add("secondary");
        var body = new StackPanel { Spacing = 6, Children = { heading, text } };
        var content = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*"),
            ColumnSpacing = 14,
            Margin = new Thickness(24, 22, 24, 22)
        };
        Grid.SetColumn(body, 1);
        content.Children.Add(tile);
        content.Children.Add(body);

        Content = new DockPanel { Children = { bar, content } };
    }

    /// <summary>The text shown to the user.</summary>
    public string Message { get; }

    public static async Task ShowAsync(Window? owner, string title, string message, Kind kind)
    {
        var window = new MessageWindow(title, message, kind, kind == Kind.Error ? "Close" : "OK", null);
        if (owner is null)
            window.Show();
        else
            await window.ShowDialog(owner);
    }

    public static async Task<bool> ConfirmAsync(Window? owner, string title, string message, string confirmText,
        string cancelText)
    {
        var window = new MessageWindow(title, message, Kind.Question, confirmText, cancelText);
        if (owner is null)
            window.Show();
        return owner is null ? false : await window.ShowDialog<bool?>(owner) ?? false;
    }
}
