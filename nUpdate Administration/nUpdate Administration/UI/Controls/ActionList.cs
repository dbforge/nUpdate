// Author: Dominic Beger (Trade/ProgTrade)
// License: Creative Commons Attribution NoDerivs (CC-ND)
// Created: 01-08-2014 12:11
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace nUpdate.Administration.UI.Controls
{
    public partial class ActionList : ListBox
    {
        private VisualStyleRenderer _ren;

        #region Flags

        public enum GroupHeaderStates
        {
            LVGH_OPEN = 1,
            LVGH_OPENHOT = 2,
            LVGH_OPENSELECTED = 3,
            LVGH_OPENSELECTEDHOT = 4,
            LVGH_OPENSELECTEDNOTFOCUSED = 5,
            LVGH_OPENSELECTEDNOTFOCUSEDHOT = 6,
            LVGH_OPENMIXEDSELECTION = 7,
            LVGH_OPENMIXEDSELECTIONHOT = 8,
            LVGH_CLOSE = 9,
            LVGH_CLOSEHOT = 10,
            LVGH_CLOSESELECTED = 11,
            LVGH_CLOSESELECTEDHOT = 12,
            LVGH_CLOSESELECTEDNOTFOCUSED = 13,
            LVGH_CLOSESELECTEDNOTFOCUSEDHOT = 14,
            LVGH_CLOSEMIXEDSELECTION = 15,
            LVGH_CLOSEMIXEDSELECTIONHOT = 16,
        }

        public enum ListViewParts
        {
            LVP_LISTITEM = 1,
            LVP_LISTGROUP = 2,
            LVP_LISTDETAIL = 3,
            LVP_LISTSORTEDDETAIL = 4,
            LVP_EMPTYTEXT = 5,
            LVP_GROUPHEADER = 6,
            LVP_GROUPHEADERLINE = 7,
            LVP_EXPANDBUTTON = 8,
            LVP_COLLAPSEBUTTON = 9,
            LVP_COLUMNDETAIL = 10,
        }

        #endregion

        public ActionList()
        {
            InitializeComponent();

            DrawMode = DrawMode.OwnerDrawFixed;
            ItemHeight = 60;
            IntegralHeight = false;

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            e.Graphics.Clip = new Region(e.Bounds);
            e.Graphics.Clear(Color.White);

            if (e.State.HasFlag(DrawItemState.Selected))
            {
                _ren =
                    new VisualStyleRenderer(VisualStyleElement.CreateElement("LISTVIEW",
                        (int) ListViewParts.LVP_GROUPHEADER, (int) GroupHeaderStates.LVGH_OPENSELECTEDNOTFOCUSEDHOT));
            }
            else
            {
                _ren =
                    new VisualStyleRenderer(VisualStyleElement.CreateElement("LISTVIEW",
                        (int) ListViewParts.LVP_GROUPHEADER, (int) GroupHeaderStates.LVGH_OPEN));
            }

            _ren.DrawBackground(e.Graphics, e.Bounds);

            if (Items.Count != 0)
            {
                ServerListItem itm;
                ServerListItem.TryParse(Items[e.Index], out itm);

                if (itm != null)
                {
                    e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    e.Graphics.DrawString(itm.HeaderText, Font, new SolidBrush(itm.HeaderColor), 40, e.Bounds.Y + 10);
                    e.Graphics.DrawString(itm.ItemText, Font, new SolidBrush(ForeColor), 20, e.Bounds.Y + 43);
                    e.Graphics.DrawImage(itm.ServerImage, 20, e.Bounds.Y + 10);
                }
                else
                    Items.RemoveAt(e.Index);
            }
        }
    }

    public class ActionListItem
    {
        private Color _headerColor = SystemColors.ControlText;
        public string HeaderText { get; set; }
        public string ItemText { get; set; }
        public Image HeaderImage { get; set; }

        /// <summary>
        ///     Sets the header color.
        /// </summary>
        public Color HeaderColor
        {
            get { return _headerColor; }

            set { _headerColor = value; }
        }

        public static bool TryParse(object source, out ActionListItem item)
        {
            ActionListItem dest;
            try
            {
                dest = (ActionListItem) source;
            }
            catch
            {
                item = null;
                return false;
            }

            item = dest;
            return true;
        }

        public static ActionListItem Parse(object source)
        {
            var dest = (ActionListItem) source;
            return dest;
        }
    }
}