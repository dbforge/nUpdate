// Copyright © Dominic Beger 2018

using System;
using System.Drawing;
using System.Windows.Forms;
using nUpdate.Administration.Core.Win32;

namespace nUpdate.Administration.UI.Controls
{
    public class ExplorerTreeView : TreeView
    {
        public enum InsertType
        {
            BeforeNode,
            AfterNode,
            InsideNode
        }

        private long _mTicks;
        private TreeNode _selectedNode;
        private ExplorerTreeNode _tempTreeViewItem;

        public ExplorerTreeView()
        {
            SetStyle(ControlStyles.Opaque, true);
            BorderStyle = BorderStyle.None;
        }

        protected override void OnDragOver(DragEventArgs dragEventArgs)
        {
            base.OnDragOver(dragEventArgs);
            var dragPoint = PointToClient(new Point(dragEventArgs.X, dragEventArgs.Y));
            var nodeAt = GetNodeAt(dragPoint);
            if (nodeAt == null)
                return;

            var span = new TimeSpan(DateTime.Now.Ticks - _mTicks);
            if (dragPoint.Y < ItemHeight)
            {
                if (nodeAt.PrevVisibleNode != null)
                    nodeAt = nodeAt.PrevVisibleNode;
                nodeAt.EnsureVisible();
                _mTicks = DateTime.Now.Ticks;
            }
            else if (dragPoint.Y < ItemHeight * 2 && span.TotalMilliseconds > 250.0)
            {
                nodeAt = nodeAt.PrevVisibleNode;
                if (nodeAt.PrevVisibleNode != null)
                    nodeAt = nodeAt.PrevVisibleNode;
                nodeAt.EnsureVisible();
                _mTicks = DateTime.Now.Ticks;
            }

            if (dragPoint.Y > ItemHeight)
            {
                if (nodeAt.NextVisibleNode != null)
                    nodeAt = nodeAt.NextVisibleNode;
                nodeAt.EnsureVisible();
                _mTicks = DateTime.Now.Ticks;
            }
            else if (dragPoint.Y > ItemHeight * 2 && span.TotalMilliseconds > 250.0)
            {
                nodeAt = nodeAt.NextVisibleNode;
                if (nodeAt.NextVisibleNode != null)
                    nodeAt = nodeAt.NextVisibleNode;
                nodeAt.EnsureVisible();
                _mTicks = DateTime.Now.Ticks;
            }
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
            {
                HotTracking = false;
            }
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
            _tempTreeViewItem.HItem = node.Handle;
            _tempTreeViewItem.State = selected ? 2 : 0;
            NativeMethods.SendMessage(Handle, 0x113f, new IntPtr(0), ref _tempTreeViewItem);
        }

        public void SetInsertionMark(TreeNode node, InsertType insertPostion)
        {
            _tempTreeViewItem.Mask = 8;
            _tempTreeViewItem.StateMask = 2;
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

            NativeMethods.SendMessage(Handle, 0x111a, new IntPtr(insertPostion == InsertType.AfterNode ? 1 : 0),
                node == null || insertPostion == InsertType.InsideNode ? IntPtr.Zero : node.Handle);
        }

        protected override void WndProc(ref Message message)
        {
            switch (message.Msg)
            {
                case 15:
                {
                    var paintStruct = new PaintStruct();
                    var targetDc = NativeMethods.BeginPaint(message.HWnd, ref paintStruct);
                    var rectangle = new Rectangle(paintStruct.RcPaint_left, paintStruct.RcPaint_top,
                        paintStruct.RcPaint_right - paintStruct.RcPaint_left,
                        paintStruct.RcPaint_bottom - paintStruct.RcPaint_top);
                    if (rectangle.Width > 0 && rectangle.Height > 0)
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

                    NativeMethods.EndPaint(message.HWnd, ref paintStruct);
                    message.Result = IntPtr.Zero;
                    return;
                }
                case 20:
                    message.Result = (IntPtr) 1;
                    return;
            }

            base.WndProc(ref message);
        }
    }
}