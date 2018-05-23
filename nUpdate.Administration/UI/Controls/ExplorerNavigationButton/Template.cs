// Copyright © Dominic Beger 2018

using System;
using System.Drawing;

namespace ExplorerNavigationButton
{
    public partial class ExplorerNavigationButton
    {
        private abstract class Template : IDisposable
        {
            private bool _disposed;

            public void Dispose()
            {
                if (!_disposed)
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);

                    _disposed = true;
                }
            }

            protected virtual void Dispose(bool disposing)
            {
            }

            public void Draw(Graphics g, ArrowDirection direction, ButtonState state)
            {
                switch (state)
                {
                    case ButtonState.Hover:
                        DrawHover(g, direction);
                        break;
                    case ButtonState.Disabled:
                        DrawDisabled(g, direction);
                        break;
                    case ButtonState.Pressed:
                        DrawPressed(g, direction);
                        break;
                    default:
                        DrawNormal(g, direction);
                        break;
                }
            }

            protected abstract void DrawDisabled(Graphics g, ArrowDirection direction);
            protected abstract void DrawHover(Graphics g, ArrowDirection direction);

            protected abstract void DrawNormal(Graphics g, ArrowDirection direction);
            protected abstract void DrawPressed(Graphics g, ArrowDirection direction);

            ~Template()
            {
                Dispose(false);
            }
        }
    }
}