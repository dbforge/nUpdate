using System;
using System.Drawing;

namespace ExplorerNavigationButton
{
    public partial class ExplorerNavigationButton
    {
        private abstract class Template : IDisposable
        {
            public void Draw(Graphics g, ArrowDirection direction, ButtonState state)
            {
                switch (state)
                {
                    case ButtonState.Hover:
                        this.DrawHover(g, direction);
                        break;
                    case ButtonState.Disabled:
                        this.DrawDisabled(g, direction);
                        break;
                    case ButtonState.Pressed:
                        this.DrawPressed(g, direction);
                        break;
                    default:
                        this.DrawNormal(g, direction);
                        break;
                }
            }

            protected abstract void DrawNormal(Graphics g, ArrowDirection direction);
            protected abstract void DrawHover(Graphics g, ArrowDirection direction);
            protected abstract void DrawPressed(Graphics g, ArrowDirection direction);
            protected abstract void DrawDisabled(Graphics g, ArrowDirection direction);

            private bool disposed;

            public void Dispose()
            {
                if (!disposed)
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);

                    disposed = true;
                }
            }

            protected virtual void Dispose(bool disposing)
            { }

            ~Template()
            {
                Dispose(false);
            }
        }
    }
}
