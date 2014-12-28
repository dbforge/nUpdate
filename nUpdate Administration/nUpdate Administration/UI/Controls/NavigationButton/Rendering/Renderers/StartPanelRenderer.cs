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
    /// Renders the Windows StartPanel
    /// </summary>
    public static class StartPanelRenderer
    {
        /// <summary>
        /// Renders a Windows StartPanel UserPane
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderUserPane(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateUserPaneElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel UserPane Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateUserPaneElement()
        {
            return VisualStyleElement.CreateElement("StartPanel", 1, 0);
        }

        /// <summary>
        /// Renders a Windows StartPanel MorePrograms
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderMorePrograms(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateMoreProgramsElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel MorePrograms Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateMoreProgramsElement()
        {
            return VisualStyleElement.CreateElement("StartPanel", 2, 0);
        }

        /// <summary>
        /// Renders a Windows StartPanel MoreProgramsArrow
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMoreProgramsArrow(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateMoreProgramsArrowElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel MoreProgramsArrow Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMoreProgramsArrowElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("StartPanel", 3, (int)state);
        }

        /// <summary>
        /// Renders a Windows StartPanel ProgList
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderProgList(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateProgListElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel ProgList Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateProgListElement()
        {
            return VisualStyleElement.CreateElement("StartPanel", 4, 0);
        }

        /// <summary>
        /// Renders a Windows StartPanel ProgListSeperator
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderProgListSeperator(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateProgListSeperatorElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel ProgListSeperator Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateProgListSeperatorElement()
        {
            return VisualStyleElement.CreateElement("StartPanel", 5, 0);
        }

        /// <summary>
        /// Renders a Windows StartPanel PlaceList
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderPlaceList(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreatePlaceListElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel PlaceList Visual Style Element
        /// </summary>
        public static VisualStyleElement CreatePlaceListElement()
        {
            return VisualStyleElement.CreateElement("StartPanel", 6, 0);
        }

        /// <summary>
        /// Renders a Windows StartPanel PlaceListSeperator
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderPlaceListSeperator(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreatePlaceListSeperatorElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel PlaceListSeperator Visual Style Element
        /// </summary>
        public static VisualStyleElement CreatePlaceListSeperatorElement()
        {
            return VisualStyleElement.CreateElement("StartPanel", 7, 0);
        }

        /// <summary>
        /// Renders a Windows StartPanel LogOff
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderLogOff(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateLogOffElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel LogOff Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateLogOffElement()
        {
            return VisualStyleElement.CreateElement("StartPanel", 8, 0);
        }

        /// <summary>
        /// Renders a Windows StartPanel LogOffButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderLogOffButton(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateLogOffButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel LogOffButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateLogOffButtonElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("StartPanel", 9, (int)state);
        }

        /// <summary>
        /// Renders a Windows StartPanel UserPicture
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderUserPicture(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateUserPictureElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel UserPicture Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateUserPictureElement()
        {
            return VisualStyleElement.CreateElement("StartPanel", 10, 0);
        }

        /// <summary>
        /// Renders a Windows StartPanel Preview
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderPreview(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreatePreviewElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel Preview Visual Style Element
        /// </summary>
        public static VisualStyleElement CreatePreviewElement()
        {
            return VisualStyleElement.CreateElement("StartPanel", 11, 0);
        }

        /// <summary>
        /// Renders a Windows StartPanel MoreProgramsTab
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMoreProgramsTab(IDeviceContext dc, Rectangle bounds, NormalHotSelectedDisabledFocusedState state)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateMoreProgramsTabElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel MoreProgramsTab Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMoreProgramsTabElement(NormalHotSelectedDisabledFocusedState state)
        {
            return VisualStyleElement.CreateElement("StartPanel", 12, (int)state);
        }

        /// <summary>
        /// Renders a Windows StartPanel NscHost
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderNscHost(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateNscHostElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel NscHost Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateNscHostElement()
        {
            return VisualStyleElement.CreateElement("StartPanel", 13, 0);
        }

        /// <summary>
        /// Renders a Windows StartPanel SoftwareExplorer
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSoftwareExplorer(IDeviceContext dc, Rectangle bounds, NormalHotSelectedDisabledFocusedState state)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateSoftwareExplorerElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel SoftwareExplorer Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSoftwareExplorerElement(NormalHotSelectedDisabledFocusedState state)
        {
            return VisualStyleElement.CreateElement("StartPanel", 14, (int)state);
        }

        /// <summary>
        /// Renders a Windows StartPanel OpenBox
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderOpenBox(IDeviceContext dc, Rectangle bounds, NormalHotSelectedDisabledFocusedState state)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateOpenBoxElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel OpenBox Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateOpenBoxElement(NormalHotSelectedDisabledFocusedState state)
        {
            return VisualStyleElement.CreateElement("StartPanel", 15, (int)state);
        }

        /// <summary>
        /// Renders a Windows StartPanel SearchView
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSearchView(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateSearchViewElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel SearchView Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSearchViewElement()
        {
            return VisualStyleElement.CreateElement("StartPanel", 16, 0);
        }

        /// <summary>
        /// Renders a Windows StartPanel MoreProgramsArrowBack
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMoreProgramsArrowBack(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateMoreProgramsArrowBackElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel MoreProgramsArrowBack Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMoreProgramsArrowBackElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("StartPanel", 17, (int)state);
        }

        /// <summary>
        /// Renders a Windows StartPanel TopMatch
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderTopMatch(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateTopMatchElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel TopMatch Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateTopMatchElement()
        {
            return VisualStyleElement.CreateElement("StartPanel", 18, 0);
        }

        /// <summary>
        /// Renders a Windows StartPanel LogOffSplitButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderLogOffSplitButton(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(StartPanelRenderer.CreateLogOffSplitButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows StartPanel LogOffSplitButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateLogOffSplitButtonElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("StartPanel", 19, (int)state);
        }
    }
}