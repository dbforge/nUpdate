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
    /// Renders the Windows Start Panel Previous
    /// </summary>
    public static class StartPanelPreviousRenderer
    {
        /// <summary>
        /// Renders a Windows Start Panel Previous UserPane
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderUserPane(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateUserPaneElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous UserPane Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateUserPaneElement()
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 1, 0);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous MorePrograms
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderMorePrograms(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateMoreProgramsElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous MorePrograms Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateMoreProgramsElement()
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 2, 0);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous MoreProgramsArrow
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMoreProgramsArrow(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateMoreProgramsArrowElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous MoreProgramsArrow Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMoreProgramsArrowElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 3, (int)state);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous ProgList
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderProgList(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateProgListElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous ProgList Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateProgListElement()
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 4, 0);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous ProgListSeperator
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderProgListSeperator(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateProgListSeperatorElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous ProgListSeperator Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateProgListSeperatorElement()
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 5, 0);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous PlaceList
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderPlaceList(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreatePlaceListElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous PlaceList Visual Style Element
        /// </summary>
        public static VisualStyleElement CreatePlaceListElement()
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 6, 0);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous PlaceListSeperator
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderPlaceListSeperator(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreatePlaceListSeperatorElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous PlaceListSeperator Visual Style Element
        /// </summary>
        public static VisualStyleElement CreatePlaceListSeperatorElement()
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 7, 0);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous LogOff
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderLogOff(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateLogOffElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous LogOff Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateLogOffElement()
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 8, 0);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous LogOffButtons
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderLogOffButtons(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateLogOffButtonsElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous LogOffButtons Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateLogOffButtonsElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 9, (int)state);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous UserPicture
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderUserPicture(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateUserPictureElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous UserPicture Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateUserPictureElement()
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 10, 0);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous Preview
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderPreview(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreatePreviewElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous Preview Visual Style Element
        /// </summary>
        public static VisualStyleElement CreatePreviewElement()
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 11, 0);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous MoreProgramsTab
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMoreProgramsTab(IDeviceContext dc, Rectangle bounds, NormalHotSelectedDisabledFocusedState state)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateMoreProgramsTabElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous MoreProgramsTab Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMoreProgramsTabElement(NormalHotSelectedDisabledFocusedState state)
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 12, (int)state);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous NscHost
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderNscHost(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateNscHostElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous NscHost Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateNscHostElement()
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 13, 0);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous SoftwareExplorer
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderSoftwareExplorer(IDeviceContext dc, Rectangle bounds, NormalHotSelectedDisabledFocusedState state)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateSoftwareExplorerElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous SoftwareExplorer Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateSoftwareExplorerElement(NormalHotSelectedDisabledFocusedState state)
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 14, (int)state);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous OpenBox
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderOpenBox(IDeviceContext dc, Rectangle bounds, NormalHotSelectedDisabledFocusedState state)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateOpenBoxElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous OpenBox Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateOpenBoxElement(NormalHotSelectedDisabledFocusedState state)
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 15, (int)state);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous SearchView
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderSearchView(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateSearchViewElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous SearchView Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateSearchViewElement()
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 16, 0);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous MoreProgramsArrowBack
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderMoreProgramsArrowBack(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateMoreProgramsArrowBackElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous MoreProgramsArrowBack Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateMoreProgramsArrowBackElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 17, (int)state);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous TopMatch
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        public static void RenderTopMatch(IDeviceContext dc, Rectangle bounds)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateTopMatchElement()).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous TopMatch Visual Style Element
        /// </summary>
        public static VisualStyleElement CreateTopMatchElement()
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 18, 0);
        }

        /// <summary>
        /// Renders a Windows Start Panel Previous LogOffSplitButton
        /// </summary>
        /// <param name="dc">The dc to draw to (you can also pass a <see cref="System.Drawing.Graphics"/> object).</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="state">The state.</param>
        public static void RenderLogOffSplitButton(IDeviceContext dc, Rectangle bounds, NormalHotPressedState state)
        {
            new VisualStyleRenderer(StartPanelPreviousRenderer.CreateLogOffSplitButtonElement(state)).DrawBackground(dc, bounds);
        }

        /// <summary>
        /// Creates a Windows Start Panel Previous LogOffSplitButton Visual Style Element
        /// </summary>
        /// <param name="state">The state.</param>
        public static VisualStyleElement CreateLogOffSplitButtonElement(NormalHotPressedState state)
        {
            return VisualStyleElement.CreateElement("StartPanelPriv", 19, (int)state);
        }
    }
}