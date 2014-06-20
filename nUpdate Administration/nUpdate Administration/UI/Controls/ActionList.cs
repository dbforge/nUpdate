using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace nUpdate.Administration.UI.Controls
{
    public partial class ActionList : ListBox
    {
        public ActionList()
        {
            InitializeComponent();

            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.ItemHeight = 60;
            this.IntegralHeight = false;

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        private VisualStyleRenderer ren;

        #region Flags

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

        #endregion

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            e.Graphics.Clip = new Region(e.Bounds);
            e.Graphics.Clear(Color.White);

            if (e.State.HasFlag(DrawItemState.Selected))
            {
                this.ren = new VisualStyleRenderer(VisualStyleElement.CreateElement("LISTVIEW", (int)ListViewParts.LVP_GROUPHEADER, (int)GroupHeaderStates.LVGH_OPENSELECTEDNOTFOCUSEDHOT));
            }
            else
            {
                this.ren = new VisualStyleRenderer(VisualStyleElement.CreateElement("LISTVIEW", (int)ListViewParts.LVP_GROUPHEADER, (int)GroupHeaderStates.LVGH_OPEN));
            }

            this.ren.DrawBackground(e.Graphics, e.Bounds);

            if (this.Items.Count != 0)
            {
                ActionListItem itm = null;
                itm = ActionListItem.TryParse(this.Items[e.Index]);

                if (itm != null)
                {
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    e.Graphics.DrawString(itm.HeaderText, Font, new SolidBrush(itm.HeaderColor), 40, e.Bounds.Y + 10);
                    e.Graphics.DrawString(itm.ItemText, this.Font, new SolidBrush(this.ForeColor), 20, e.Bounds.Y + 43);
                    e.Graphics.DrawImage(itm.HeaderImage, 20, e.Bounds.Y + 10);
                }
                else
                {
                    this.Items.RemoveAt(e.Index);
                }
            }
        }
    }

    public class ActionListItem
    {
        public string HeaderText { get; set; }
        public string Description { get; set; }
        public string ItemText { get; set; }
        public Image HeaderImage { get; set; }

        private Color headerColor = SystemColors.ControlText;

        /// <summary>
        /// Sets the header color.
        /// </summary>
        public Color HeaderColor
        {
            get
            {
                return this.headerColor;
            }

            set
            {
                this.headerColor = value;
            }
        }

        public static ActionListItem TryParse(object source)
        {
            ActionListItem dest = null;
            try
            {
                dest = (ActionListItem)source;
            }
            catch
            {
            }

            return dest;
        }

        public static ActionListItem Parse(object source)
        {
            ActionListItem dest = (ActionListItem)source;
            return dest;
        }
    }
}