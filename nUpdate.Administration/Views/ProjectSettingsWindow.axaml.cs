using Avalonia;
using Avalonia.Controls;

namespace nUpdate.Administration.Views;

public partial class ProjectSettingsWindow : Window
{
    public ProjectSettingsWindow()
    {
        InitializeComponent();
        SectionList.SelectionChanged += (_, _) => ScrollToSection();
    }

    /// <summary>The settings are one page; the rail scrolls to the section the user picks.</summary>
    private void ScrollToSection()
    {
        Control[] sections = [GeneralSection, TransferSection, StatisticsSection, SecuritySection, MigrationSection];
        if (SectionList.SelectedIndex < 0 || sections[SectionList.SelectedIndex].TranslatePoint(default, Sections) is not { } top)
            return;
        Scroller.Offset = new Vector(0, top.Y);
    }
}
