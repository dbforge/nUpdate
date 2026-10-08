using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace nUpdate.Tests.Integration.Scenarios.Support;

/// <summary>
///     Drives the headless windows the way a user would: clicks land as pointer events at the control's position,
///     text arrives as key input. A control that is hidden, disabled or covered cannot be used, exactly as on screen.
/// </summary>
public static class User
{
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);

    /// <summary>Lets the dispatcher run everything that is queued, including bindings and layout.</summary>
    public static void Pump()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    /// <summary>Waits until a condition holds, pumping the dispatcher in between so the application can make progress.</summary>
    public static async Task WaitUntil(Func<bool> condition, string what)
    {
        var started = DateTime.UtcNow;
        while (true)
        {
            Pump();
            if (condition())
                return;
            if (DateTime.UtcNow - started > Timeout)
                throw new TimeoutException($"Waited {Timeout.TotalSeconds:0}s for {what}.");
            await Task.Delay(25);
        }
    }

    /// <summary>Finds a named control anywhere in a window, including inside user controls and templates.</summary>
    public static T Find<T>(Visual root, string name) where T : Control
    {
        Pump();
        return root.GetVisualDescendants().OfType<T>().FirstOrDefault(c => c.Name == name)
               ?? throw new InvalidOperationException($"There is no {typeof(T).Name} named \"{name}\" in {root.GetType().Name}.");
    }

    /// <summary>Finds a button by the text on it.</summary>
    public static Button ButtonWithText(Visual root, string text)
    {
        Pump();
        return root.GetVisualDescendants().OfType<Button>().FirstOrDefault(b => Equals(b.Content, text))
               ?? throw new InvalidOperationException($"There is no button \"{text}\" in {root.GetType().Name}.");
    }

    /// <summary>The buttons bound to an item of a list, e.g. the arrows next to an operation or the + of a palette entry.</summary>
    public static Button ButtonFor(Visual root, object dataContext, string text)
    {
        Pump();
        return root.GetVisualDescendants().OfType<Button>()
                   .FirstOrDefault(b => (Equals(b.Content, text) || Equals(ToolTip.GetTip(b), text)) && ReferenceEquals(b.DataContext, dataContext))
               ?? throw new InvalidOperationException($"There is no button \"{text}\" for {dataContext} in {root.GetType().Name}.");
    }

    /// <summary>The text box bound to an item of a list, e.g. the changelog of a language.</summary>
    public static TextBox TextBoxFor(Visual root, object dataContext)
    {
        Pump();
        return root.GetVisualDescendants().OfType<TextBox>().FirstOrDefault(b => ReferenceEquals(b.DataContext, dataContext))
               ?? throw new InvalidOperationException($"There is no text box for {dataContext} in {root.GetType().Name}.");
    }

    /// <summary>Clicks a control with the mouse. Fails when the user could not click it either.</summary>
    public static void Click(Control control)
    {
        Pump();
        var window = TopLevel.GetTopLevel(control) ?? throw new InvalidOperationException($"{Describe(control)} is not in a window.");
        window.UpdateLayout();
        if (!control.IsEffectivelyVisible)
            throw new InvalidOperationException($"{Describe(control)} is not visible.");
        if (!control.IsEffectivelyEnabled)
            throw new InvalidOperationException($"{Describe(control)} is disabled.");
        control.BringIntoView();
        window.UpdateLayout();
        Pump();
        window.UpdateLayout();
        Pump();
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)
                    ?? throw new InvalidOperationException($"{Describe(control)} has no position in its window.");
        var hit = window.InputHitTest(point) as Visual;
        if (hit is null || !(hit == control || hit.GetVisualAncestors().Contains(control)))
            throw new InvalidOperationException($"{Describe(control)} is covered by {hit?.GetType().Name ?? "nothing"} at {point}.");
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Pump();
    }

    /// <summary>Clicks a check box until it shows the wanted state.</summary>
    public static void Check(ToggleButton box, bool value)
    {
        Pump();
        if (box.IsChecked != value)
            Click(box);
        if (box.IsChecked != value)
            throw new InvalidOperationException($"{Describe(box)} did not change to {value}.");
    }

    /// <summary>Replaces the text of a box by typing, as a user who selected everything first.</summary>
    public static void Type(TextBox box, string text)
    {
        Pump();
        var window = TopLevel.GetTopLevel(box) ?? throw new InvalidOperationException($"{Describe(box)} is not in a window.");
        if (!box.IsEffectivelyVisible)
            throw new InvalidOperationException($"{Describe(box)} is not visible.");
        if (!box.IsEffectivelyEnabled)
            throw new InvalidOperationException($"{Describe(box)} is disabled.");
        box.Focus();
        box.SelectAll();
        if (text.Length == 0)
            window.KeyPress(Key.Back, RawInputModifiers.None, PhysicalKey.Backspace, null);
        else
            window.KeyTextInput(text);
        Pump();
        if (box.Text != text)
            throw new InvalidOperationException($"{Describe(box)} shows \"{box.Text}\" after typing \"{text}\".");
    }

    /// <summary>Chooses an entry of a combo box.</summary>
    public static void Select(SelectingItemsControl box, object item)
    {
        Pump();
        if (!box.IsEffectivelyEnabled)
            throw new InvalidOperationException($"{Describe(box)} is disabled.");
        box.SelectedItem = item;
        Pump();
    }

    /// <summary>Selects a row of a list.</summary>
    public static void SelectRow(SelectingItemsControl list, int index)
    {
        Pump();
        list.SelectedIndex = index;
        Pump();
    }

    /// <summary>Selects a row of a data grid.</summary>
    public static void SelectRow(DataGrid grid, int index)
    {
        Pump();
        grid.SelectedIndex = index;
        Pump();
    }

    /// <summary>Clicks the entry of a side navigation that shows the text.</summary>
    public static void SelectPage(ListBox navigation, string text)
    {
        Pump();
        var item = navigation.GetVisualDescendants().OfType<ListBoxItem>()
                       .FirstOrDefault(i => i.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == text))
                   ?? throw new InvalidOperationException($"There is no page \"{text}\".");
        Click(item);
    }

    /// <summary>The texts of every row of a list, in order, as the user reads them.</summary>
    public static IReadOnlyList<string> RowTexts(ItemsControl list)
    {
        Pump();
        return list.Items.Cast<object>().Select(i => i?.ToString() ?? string.Empty).ToList();
    }

    private static string Describe(Control control) => $"{control.GetType().Name} \"{control.Name ?? (control as ContentControl)?.Content?.ToString() ?? "?"}\"";
}
