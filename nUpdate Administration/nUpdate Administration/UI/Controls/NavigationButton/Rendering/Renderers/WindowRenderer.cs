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
    /// Renders the Windows Window
    /// </summary>
    public static class WindowRenderer
    {
        /// <summary>
        /// Renders a Windows Window Caption
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderCaption(IDeviceContext dc, Rectangle bounds, ActiveInactiveDisabledState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateCaptionElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window Caption Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateCaptionElement(ActiveInactiveDisabledState state)
        {
            return VisualStyleElement.CreateElement("Window", 1, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window SmallCaption
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSmallCaption(IDeviceContext dc, Rectangle bounds, ActiveInactiveDisabledState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateSmallCaptionElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window SmallCaption Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSmallCaptionElement(ActiveInactiveDisabledState state)
        {
            return VisualStyleElement.CreateElement("Window", 2, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window MinCaption
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMinCaption(IDeviceContext dc, Rectangle bounds, ActiveInactiveDisabledState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateMinCaptionElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window MinCaption Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMinCaptionElement(ActiveInactiveDisabledState state)
        {
            return VisualStyleElement.CreateElement("Window", 3, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window SmallMinCaption
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSmallMinCaption(IDeviceContext dc, Rectangle bounds, ActiveInactiveDisabledState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateSmallMinCaptionElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window SmallMinCaption Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSmallMinCaptionElement(ActiveInactiveDisabledState state)
        {
            return VisualStyleElement.CreateElement("Window", 4, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window MaxCaption
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMaxCaption(IDeviceContext dc, Rectangle bounds, ActiveInactiveDisabledState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateMaxCaptionElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window MaxCaption Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMaxCaptionElement(ActiveInactiveDisabledState state)
        {
            return VisualStyleElement.CreateElement("Window", 5, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window SmallMaxCaption
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSmallMaxCaption(IDeviceContext dc, Rectangle bounds, ActiveInactiveDisabledState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateSmallMaxCaptionElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window SmallMaxCaption Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSmallMaxCaptionElement(ActiveInactiveDisabledState state)
        {
            return VisualStyleElement.CreateElement("Window", 6, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window FrameLeft
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderFrameLeft(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(WindowRenderer.CreateFrameLeftElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window FrameLeft Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateFrameLeftElement()
        {
            return VisualStyleElement.CreateElement("Window", 7, 0);
        }

        /// <summary>
        /// Renders a Windows Window FrameRight
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderFrameRight(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(WindowRenderer.CreateFrameRightElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window FrameRight Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateFrameRightElement()
        {
            return VisualStyleElement.CreateElement("Window", 8, 0);
        }

        /// <summary>
        /// Renders a Windows Window FrameBottom
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderFrameBottom(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(WindowRenderer.CreateFrameBottomElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window FrameBottom Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateFrameBottomElement()
        {
            return VisualStyleElement.CreateElement("Window", 9, 0);
        }

        /// <summary>
        /// Renders a Windows Window SmallFrameLeft
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSmallFrameLeft(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(WindowRenderer.CreateSmallFrameLeftElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window SmallFrameLeft Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSmallFrameLeftElement()
        {
            return VisualStyleElement.CreateElement("Window", 10, 0);
        }

        /// <summary>
        /// Renders a Windows Window SmallFrameRight
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSmallFrameRight(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(WindowRenderer.CreateSmallFrameRightElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window SmallFrameRight Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSmallFrameRightElement()
        {
            return VisualStyleElement.CreateElement("Window", 11, 0);
        }

        /// <summary>
        /// Renders a Windows Window SmallFrameBottom
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSmallFrameBottom(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(WindowRenderer.CreateSmallFrameBottomElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window SmallFrameBottom Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSmallFrameBottomElement()
        {
            return VisualStyleElement.CreateElement("Window", 12, 0);
        }

        /// <summary>
        /// Renders a Windows Window SystemButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSystemButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateSystemButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window SystemButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSystemButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 13, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window MdiSystemButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMdiSystemButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateMdiSystemButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window MdiSystemButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMdiSystemButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 14, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window MinimizeButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMinimizeButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateMinimizeButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window MinimizeButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMinimizeButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 15, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window MdiMinimizeButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMdiMinimizeButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateMdiMinimizeButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window MdiMinimizeButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMdiMinimizeButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 16, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window MaximizeButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMaximizeButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateMaximizeButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window MaximizeButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMaximizeButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 17, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window CloseButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderCloseButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateCloseButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window CloseButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateCloseButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 18, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window SmallCloseButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSmallCloseButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateSmallCloseButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window SmallCloseButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSmallCloseButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 19, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window MdiCloseButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMdiCloseButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateMdiCloseButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window MdiCloseButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMdiCloseButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 20, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window RestoreButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderRestoreButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateRestoreButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window RestoreButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateRestoreButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 21, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window MdiRestoreButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMdiRestoreButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateMdiRestoreButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window MdiRestoreButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMdiRestoreButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 22, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window HelpButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderHelpButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateHelpButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window HelpButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateHelpButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 23, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window MdiHelpButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMdiHelpButton(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateMdiHelpButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window MdiHelpButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMdiHelpButtonElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 24, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window HorizontalScroll
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderHorizontalScroll(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateHorizontalScrollElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window HorizontalScroll Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateHorizontalScrollElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 25, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window HorizontalThumb
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderHorizontalThumb(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateHorizontalThumbElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window HorizontalThumb Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateHorizontalThumbElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 26, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window VerticalScroll
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderVerticalScroll(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateVerticalScrollElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window VerticalScroll Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateVerticalScrollElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 27, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window VerticalThumb
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderVerticalThumb(IDeviceContext dc, Rectangle bounds, ButtonState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateVerticalThumbElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window VerticalThumb Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateVerticalThumbElement(ButtonState state)
        {
            return VisualStyleElement.CreateElement("Window", 28, (int)state);
        }

        /// <summary>
        /// Renders a Windows Window Dialog
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderDialog(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(WindowRenderer.CreateDialogElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window Dialog Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateDialogElement()
        {
            return VisualStyleElement.CreateElement("Window", 29, 0);
        }

        /// <summary>
        /// Renders a Windows Window CaptionSizingTemplate
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderCaptionSizingTemplate(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(WindowRenderer.CreateCaptionSizingTemplateElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window CaptionSizingTemplate Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateCaptionSizingTemplateElement()
        {
            return VisualStyleElement.CreateElement("Window", 30, 0);
        }

        /// <summary>
        /// Renders a Windows Window SmallCaptionSizingTemplate
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSmallCaptionSizingTemplate(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(WindowRenderer.CreateSmallCaptionSizingTemplateElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window SmallCaptionSizingTemplate Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSmallCaptionSizingTemplateElement()
        {
            return VisualStyleElement.CreateElement("Window", 31, 0);
        }

        /// <summary>
        /// Renders a Windows Window FrameLeftSizingTemplate
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderFrameLeftSizingTemplate(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(WindowRenderer.CreateFrameLeftSizingTemplateElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window FrameLeftSizingTemplate Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateFrameLeftSizingTemplateElement()
        {
            return VisualStyleElement.CreateElement("Window", 32, 0);
        }

        /// <summary>
        /// Renders a Windows Window SmallFrameLeftSizingTemplate
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSmallFrameLeftSizingTemplate(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(WindowRenderer.CreateSmallFrameLeftSizingTemplateElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window SmallFrameLeftSizingTemplate Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSmallFrameLeftSizingTemplateElement()
        {
            return VisualStyleElement.CreateElement("Window", 33, 0);
        }

        /// <summary>
        /// Renders a Windows Window FrameRightSizingTemplate
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderFrameRightSizingTemplate(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(WindowRenderer.CreateFrameRightSizingTemplateElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window FrameRightSizingTemplate Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateFrameRightSizingTemplateElement()
        {
            return VisualStyleElement.CreateElement("Window", 34, 0);
        }

        /// <summary>
        /// Renders a Windows Window SmallFrameRightSizingTemplate1
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSmallFrameRightSizingTemplate1(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(WindowRenderer.CreateSmallFrameRightSizingTemplate1Element()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window SmallFrameRightSizingTemplate1 Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSmallFrameRightSizingTemplate1Element()
        {
            return VisualStyleElement.CreateElement("Window", 35, 0);
        }

        /// <summary>
        /// Renders a Windows Window FrameBottomSizingTemplate
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderFrameBottomSizingTemplate(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(WindowRenderer.CreateFrameBottomSizingTemplateElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window FrameBottomSizingTemplate Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateFrameBottomSizingTemplateElement()
        {
            return VisualStyleElement.CreateElement("Window", 36, 0);
        }

        /// <summary>
        /// Renders a Windows Window SmallFrameRightSizingTemplate2
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSmallFrameRightSizingTemplate2(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(WindowRenderer.CreateSmallFrameRightSizingTemplate2Element()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window SmallFrameRightSizingTemplate2 Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSmallFrameRightSizingTemplate2Element()
        {
            return VisualStyleElement.CreateElement("Window", 37, 0);
        }

        /// <summary>
        /// Renders a Windows Window Frame
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderFrame(IDeviceContext dc, Rectangle bounds, ActiveInactiveState state)
        {
            new VisualStyleRenderer(WindowRenderer.CreateFrameElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Window Frame Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateFrameElement(ActiveInactiveState state)
        {
            return VisualStyleElement.CreateElement("Window", 28, (int)state);
        }
    }
}