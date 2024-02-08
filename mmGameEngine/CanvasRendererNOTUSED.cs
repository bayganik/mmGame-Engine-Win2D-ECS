using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;

namespace mmGameEngine
{
    public class CanvasRendererNO
    {
        CoreWindow GameWindow;
        CanvasDevice device;
        SwapChainManager swapChainManager;

        Dictionary<uint, Point> currentPointsInContact = new Dictionary<uint, Point>();
        CanvasRenderTarget FrontBuffer { get { return accumulationBuffers[currentBuffer]; } }
        CanvasRenderTarget BackBuffer { get { return accumulationBuffers[(currentBuffer + 1) % 2]; } }
        CanvasRenderTarget[] accumulationBuffers = new CanvasRenderTarget[2];
        int currentBuffer = 0;

        HueRotationEffect inputEffect = new HueRotationEffect()
        {
            Angle = 0.5f
        };

        Transform2DEffect outputEffect;
        CanvasImageBrush imageBrush;

        int ticksSinceLastTouch = 0;


        public CanvasRendererNO(CoreWindow window)
        {
            GameWindow = window;
            GameWindow.PointerPressed += window_PointerPressed;
            GameWindow.PointerMoved += window_PointerMoved;
            GameWindow.PointerReleased += window_PointerReleased;


            device = new CanvasDevice();
            swapChainManager = new SwapChainManager(GameWindow, device);

            var effect = new GaussianBlurEffect()
            {
                BlurAmount = 5,
                Source = inputEffect
            };

            outputEffect = new Transform2DEffect()
            {
                Source = effect
            };

            imageBrush = new CanvasImageBrush(device);
            imageBrush.Opacity = 0.99f;
        }

        void window_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            UpdateIntermediatePoints(args);
        }

        void window_PointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            UpdateIntermediatePoints(args);
        }

        void window_PointerReleased(CoreWindow sender, PointerEventArgs args)
        {
            currentPointsInContact.Remove(args.CurrentPoint.PointerId);
            UpdateIntermediatePoints(args);
        }

        void UpdateIntermediatePoints(PointerEventArgs args)
        {
            foreach (var point in args.GetIntermediatePoints())
            {
                if (point.IsInContact)
                {
                    currentPointsInContact[point.PointerId] = point.Position;
                }
                else
                {
                    currentPointsInContact.Remove(point.PointerId);
                }
            }
        }

        public void Render()
        {
            //
            // Make the swapchain same size as entire window
            //
            swapChainManager.EnsureMatchesWindow(GameWindow);

            // Update the accumulation buffer
            SwapAccumulationBuffers();
            EnsureCurrentBufferMatchesWindow();
        }

        void SwapAccumulationBuffers()
        {
            currentBuffer = (currentBuffer + 1) % 2;
        }

        void EnsureCurrentBufferMatchesWindow()
        {
            var bounds = GameWindow.Bounds;
            Size windowSize = new Size(bounds.Width, bounds.Height);
            float dpi = DisplayInformation.GetForCurrentView().LogicalDpi;

            var buffer = accumulationBuffers[currentBuffer];

            if (buffer == null || !(SwapChainManager.SizeEqualsWithTolerance(buffer.Size, windowSize)) || buffer.Dpi != dpi)
            {
                if (buffer != null)
                    buffer.Dispose();

                buffer = new CanvasRenderTarget(device, (float)windowSize.Width, (float)windowSize.Height, dpi);
                accumulationBuffers[currentBuffer] = buffer;
            }
        }


         void AccumulateBackBufferOntoFrontBuffer(CanvasDrawingSession ds)
        {
            // If this is the first frame then there's no back buffer
            if (BackBuffer == null)
                return;

            inputEffect.Source = BackBuffer;

            // Adjust the scale, so that if the front and back buffer are different sizes (eg the window was resized) 
            // then the contents is scaled up as appropriate.
            var scaleX = FrontBuffer.Size.Width / BackBuffer.Size.Width;
            var scaleY = FrontBuffer.Size.Height / BackBuffer.Size.Height;

            var transform = Matrix3x2.CreateScale((float)scaleX, (float)scaleY);

            // we do a bit of extra scale for effect
            transform *= Matrix3x2.CreateScale(1.01f, 1.01f, FrontBuffer.Size.ToVector2() / 2);

            outputEffect.TransformMatrix = transform;

            imageBrush.Image = outputEffect;
            imageBrush.SourceRectangle = new Rect(0, 0, FrontBuffer.Size.Width, FrontBuffer.Size.Height);
            ds.FillRectangle(imageBrush.SourceRectangle.Value, imageBrush);
        }

        //----------------
        // Step 09
        //----------------
        public void Trim()
        {
            device.Trim();
        }
    }
}
