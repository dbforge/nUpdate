// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Administration.Core.Win32;

namespace nUpdate.Administration.UI.Controls
{
    public class ExplorerTreeView : TreeView
    {
        #region InsertType enum

        public enum InsertType
        {
            BeforeNode,
            AfterNode,
            InsideNode
        }

        #endregion

        private long _mTicks;
        private TreeNode _selectedNode;
        private TvItem _tempTvItem;

        public ExplorerTreeView()
        {
            SetStyle(ControlStyles.Opaque, true);
            BorderStyle = BorderStyle.None;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawLine(
                SystemPens.ControlLight,
                new Point(e.ClipRectangle.Width - 1, 0),
                new Point(e.ClipRectangle.Width - 1, e.ClipRectangle.Height)
                );
        }

        private void PseudoSelectNode(TreeNode node, bool selected)
        {
            _tempTvItem.hItem = node.Handle;
            _tempTvItem.state = selected ? 2 : 0;
            NativeMethods.SendMessage(Handle, 0x113f, new IntPtr(0), ref _tempTvItem);
        }

        public void SetInsertionMark(TreeNode node, InsertType insertPostion)
        {
            _tempTvItem.mask = 8;
            _tempTvItem.stateMask = 2;
            if (_selectedNode != null)
            {
                PseudoSelectNode(_selectedNode, false);
                _selectedNode = null;
            }
            if (insertPostion == InsertType.InsideNode)
            {
                PseudoSelectNode(node, true);
                _selectedNode = node;
            }
            NativeMethods.SendMessage(Handle, 0x111a, insertPostion == InsertType.AfterNode,
                ((node == null) || (insertPostion == InsertType.InsideNode)) ? IntPtr.Zero : node.Handle);
        }

        protected override void WndProc(ref Message message)
        {
            switch (message.Msg)
            {
                case 15:
                {
                    var paintStruct = new PaintStruct();
                    var targetDc = NativeMethods.BeginPaint(message.HWnd, ref paintStruct);
                    var rectangle = new Rectangle(paintStruct.rcPaint_left, paintStruct.rcPaint_top,
                        paintStruct.rcPaint_right - paintStruct.rcPaint_left,
                        paintStruct.rcPaint_bottom - paintStruct.rcPaint_top);
                    if ((rectangle.Width > 0) && (rectangle.Height > 0))
                    {
                        using (
                            var graphics = BufferedGraphicsManager.Current.Allocate(targetDc,
                                ClientRectangle))
                        {
                            var hdc = graphics.Graphics.GetHdc();
                            var m = Message.Create(Handle, 0x318, hdc, IntPtr.Zero);
                            DefWndProc(ref m);
                            graphics.Graphics.ReleaseHdc(hdc);
                            graphics.Render();
                        }
                    }
                    NativeMethods.EndPaint(message.HWnd, ref paintStruct);
                    message.Result = IntPtr.Zero;
                    return;
                }
                case 20:
                    message.Result = (IntPtr) 1;
                    return;

                /*case 0x20:
                LinkLabel2.SetCursor(LinkLabel2.LoadCursor(0, 0x7f00));
                message.Result = IntPtr.Zero;
                return;*/
            }
            base.WndProc(ref message);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (Environment.OSVersion.Version.Major >= 6)
            {
                ShowLines = false;
                HotTracking = true;
                var lParam = NativeMethods.SendMessage(Handle, 0x112d, new IntPtr(0), new IntPtr(0));
                NativeMethods.SendMessage(Handle, 0x112c, new IntPtr(0), lParam);
                NativeMethods.SetWindowTheme(Handle, "explorer", null);
            }
            else
                HotTracking = false;
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
            var pt = PointToClient(new Point(drgevent.X, drgevent.Y));
            var nodeAt = GetNodeAt(pt);
            if (nodeAt == null)
                return;

            var span = new TimeSpan(DateTime.Now.Ticks - _mTicks);
            if (pt.Y < ItemHeight)
            {
                if (nodeAt.PrevVisibleNode != null)
                    nodeAt = nodeAt.PrevVisibleNode;
                nodeAt.EnsureVisible();
                _mTicks = DateTime.Now.Ticks;
            }
            else if ((pt.Y < (ItemHeight*2)) && (span.TotalMilliseconds > 250.0))
            {
                nodeAt = nodeAt.PrevVisibleNode;
                if (nodeAt.PrevVisibleNode != null)
                    nodeAt = nodeAt.PrevVisibleNode;
                nodeAt.EnsureVisible();
                _mTicks = DateTime.Now.Ticks;
            }
            if (pt.Y > ItemHeight)
            {
                if (nodeAt.NextVisibleNode != null)
                    nodeAt = nodeAt.NextVisibleNode;
                nodeAt.EnsureVisible();
                _mTicks = DateTime.Now.Ticks;
            }
            else if ((pt.Y > (ItemHeight*2)) && (span.TotalMilliseconds > 250.0))
            {
                nodeAt = nodeAt.NextVisibleNode;
                if (nodeAt.NextVisibleNode != null)
                    nodeAt = nodeAt.NextVisibleNode;
                nodeAt.EnsureVisible();
                _mTicks = DateTime.Now.Ticks;
            }
        }
    }
}