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
    /// Renders the Windows Task Dialog
    /// </summary>
    public static class TaskDialogRenderer
    {
        /// <summary>
        /// Renders a Windows Task Dialog PrimaryPanel
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderPrimaryPanel(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreatePrimaryPanelElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog PrimaryPanel Visual Style Element
        /// </summary>
        public static VisualStyleElement CreatePrimaryPanelElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 1, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog MainInstructionsPane
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderMainInstructionsPane(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateMainInstructionsPaneElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog MainInstructionsPane Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateMainInstructionsPaneElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 2, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog MainIcon
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderMainIcon(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateMainIconElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog MainIcon Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateMainIconElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 3, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog ContentPane
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderContentPane(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateContentPaneElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog ContentPane Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateContentPaneElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 4, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog ContentIcon
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderContentIcon(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateContentIconElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog ContentIcon Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateContentIconElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 5, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog ExpandedContent
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderExpandedContent(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateExpandedContentElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog ExpandedContent Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateExpandedContentElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 6, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog CommandLinkPane
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderCommandLinkPane(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateCommandLinkPaneElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog CommandLinkPane Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateCommandLinkPaneElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 7, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog SecondaryPane
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSecondaryPane(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateSecondaryPaneElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog SecondaryPane Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSecondaryPaneElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 8, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog ControlPane
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderControlPane(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateControlPaneElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog ControlPane Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateControlPaneElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 9, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog ButtonSection
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderButtonSection(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateButtonSectionElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog ButtonSection Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateButtonSectionElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 10, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog ButtonWrapper
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderButtonWrapper(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateButtonWrapperElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog ButtonWrapper Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateButtonWrapperElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 11, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog ExpandText
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderExpandText(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateExpandTextElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog ExpandText Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateExpandTextElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 12, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog ExpandButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderExpandButton(IDeviceContext dc, Rectangle bounds, ExpandButtonState state)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateExpandButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog ExpandButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateExpandButtonElement(ExpandButtonState state)
        {
            return VisualStyleElement.CreateElement("TaskDialog", 13, (int)state);
        }

        /// <summary>
        /// Renders a Windows Task Dialog VerificationText
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderVerificationText(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateVerificationTextElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog VerificationText Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateVerificationTextElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 14, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog FootnotePane
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderFootnotePane(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateFootnotePaneElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog FootnotePane Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateFootnotePaneElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 15, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog FootnoteArea
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderFootnoteArea(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateFootnoteAreaElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog FootnoteArea Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateFootnoteAreaElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 16, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog FootnoteSeperator
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderFootnoteSeperator(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateFootnoteSeperatorElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog FootnoteSeperator Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateFootnoteSeperatorElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 17, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog ExpandedFootArea
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderExpandedFootArea(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateExpandedFootAreaElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog ExpandedFootArea Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateExpandedFootAreaElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 18, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog ProgressBar
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderProgressBar(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateProgressBarElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog ProgressBar Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateProgressBarElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 19, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog ImageAlignment
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderImageAlignment(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateImageAlignmentElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog ImageAlignment Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateImageAlignmentElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 20, 0);
        }

        /// <summary>
        /// Renders a Windows Task Dialog RadioButtonPane
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderRadioButtonPane(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(TaskDialogRenderer.CreateRadioButtonPaneElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Task Dialog RadioButtonPane Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateRadioButtonPaneElement()
        {
            return VisualStyleElement.CreateElement("TaskDialog", 21, 0);
        }
    }
}