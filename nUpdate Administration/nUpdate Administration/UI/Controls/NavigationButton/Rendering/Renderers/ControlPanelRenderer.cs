using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace VisualStyleControls.Rendering
{
    /// <summary>
    /// Renders the Windows Control Panel
    /// </summary>
    public static class ControlPanelRenderer
    {
        /// <summary>
        /// Renders a Windows Control Panel NavigationPane
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderNavigationPane(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateNavigationPaneElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel NavigationPane Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateNavigationPaneElement()
        {
            return VisualStyleElement.CreateElement("ControlPanel", 1, 0);
        }

        /// <summary>
        /// Renders a Windows Control Panel ContentPane
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderContentPane(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateContentPaneElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel ContentPane Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateContentPaneElement()
        {
            return VisualStyleElement.CreateElement("ControlPanel", 2, 0);
        }

        /// <summary>
        /// Renders a Windows Control Panel NavigationPaneLabel
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderNavigationPaneLabel(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateNavigationPaneLabelElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel NavigationPaneLabel Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateNavigationPaneLabelElement()
        {
            return VisualStyleElement.CreateElement("ControlPanel", 3, 0);
        }

        /// <summary>
        /// Renders a Windows Control Panel ContentPaneLabel
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderContentPaneLabel(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateContentPaneLabelElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel ContentPaneLabel Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateContentPaneLabelElement()
        {
            return VisualStyleElement.CreateElement("ControlPanel", 4, 0);
        }

        /// <summary>
        /// Renders a Windows Control Panel Title
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderTitle(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateTitleElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel Title Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateTitleElement()
        {
            return VisualStyleElement.CreateElement("ControlPanel", 5, 0);
        }

        /// <summary>
        /// Renders a Windows Control Panel BodyText
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBodyText(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateBodyTextElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel BodyText Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBodyTextElement()
        {
            return VisualStyleElement.CreateElement("ControlPanel", 6, 0);
        }

        /// <summary>
        /// Renders a Windows Control Panel HelpLink
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderHelpLink(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateHelpLinkElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel HelpLink Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateHelpLinkElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("ControlPanel", 7, (int)state);
        }

        /// <summary>
        /// Renders a Windows Control Panel TaskLink
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderTaskLink(IDeviceContext dc, Rectangle bounds, TaskLinkState state)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateTaskLinkElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel TaskLink Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateTaskLinkElement(TaskLinkState state)
        {
            return VisualStyleElement.CreateElement("ControlPanel", 8, (int)state);
        }

        /// <summary>
        /// Renders a Windows Control Panel GroupText
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderGroupText(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateGroupTextElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel GroupText Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateGroupTextElement()
        {
            return VisualStyleElement.CreateElement("ControlPanel", 9, 0);
        }

        /// <summary>
        /// Renders a Windows Control Panel ContentLink
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderContentLink(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateContentLinkElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel ContentLink Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateContentLinkElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("ControlPanel", 10, (int)state);
        }

        /// <summary>
        /// Renders a Windows Control Panel SectionTitleLink
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSectionTitleLink(IDeviceContext dc, Rectangle bounds, NormalHotState state)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateSectionTitleLinkElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel SectionTitleLink Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSectionTitleLinkElement(NormalHotState state)
        {
            return VisualStyleElement.CreateElement("ControlPanel", 11, (int)state);
        }

        /// <summary>
        /// Renders a Windows Control Panel LargeCommandArea
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderLargeCommandArea(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateLargeCommandAreaElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel LargeCommandArea Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateLargeCommandAreaElement()
        {
            return VisualStyleElement.CreateElement("ControlPanel", 12, 0);
        }

        /// <summary>
        /// Renders a Windows Control Panel SmallCommandArea
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSmallCommandArea(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateSmallCommandAreaElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel SmallCommandArea Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSmallCommandAreaElement()
        {
            return VisualStyleElement.CreateElement("ControlPanel", 13, 0);
        }

        /// <summary>
        /// Renders a Windows Control Panel Button
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderButton(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateButtonElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel Button Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateButtonElement()
        {
            return VisualStyleElement.CreateElement("ControlPanel", 14, 0);
        }

        /// <summary>
        /// Renders a Windows Control Panel MessageText
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderMessageText(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateMessageTextElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel MessageText Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateMessageTextElement()
        {
            return VisualStyleElement.CreateElement("ControlPanel", 15, 0);
        }

        /// <summary>
        /// Renders a Windows Control Panel NavigationPaneLine
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderNavigationPaneLine(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateNavigationPaneLineElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel NavigationPaneLine Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateNavigationPaneLineElement()
        {
            return VisualStyleElement.CreateElement("ControlPanel", 16, 0);
        }

        /// <summary>
        /// Renders a Windows Control Panel ContentPaneline
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderContentPaneline(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateContentPanelineElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel ContentPaneline Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateContentPanelineElement()
        {
            return VisualStyleElement.CreateElement("ControlPanel", 17, 0);
        }

        /// <summary>
        /// Renders a Windows Control Panel BannerArea
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBannerArea(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateBannerAreaElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel BannerArea Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBannerAreaElement()
        {
            return VisualStyleElement.CreateElement("ControlPanel", 18, 0);
        }

        /// <summary>
        /// Renders a Windows Control Panel BodyTitle
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderBodyTitle(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(ControlPanelRenderer.CreateBodyTitleElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Control Panel BodyTitle Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateBodyTitleElement()
        {
            return VisualStyleElement.CreateElement("ControlPanel", 19, 0);
        }
    }
}