using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace wisecorp.Views.Components
{
    public partial class ZoomableCanvas : UserControl
    {
        // Zoom parameters
        private double zoomFactor = 1.0;
        public const double ZoomSpeed = 1.1;
        public const double MaxZoom = 5.0;
        public const double MinZoom = 0.4;

        // Panning variables
        private bool isDragging = false;
        private Point startDragPoint;
        private double translateX = 0;
        private double translateY = 0;

        public ZoomableCanvas()
        {
            InitializeComponent();
            
            // Set initial render transform
            GraphCanvas.RenderTransform = new TransformGroup
            {
                Children = new TransformCollection
                {
                    new ScaleTransform(1, 1),
                    new TranslateTransform(0, 0)
                }
            };
        }

        public void SceduleCenterCanvas()
        {
            Dispatcher.BeginInvoke(new Action(CenterCanvas), DispatcherPriority.Loaded);
        }

        // Center the canvas content
        private void CenterCanvas()
        {
            double canvasWidth = GraphCanvas.Width;
            double canvasHeight = GraphCanvas.Height;

            double viewportWidth = GraphScrollViewer.ViewportWidth;
            double viewportHeight = GraphScrollViewer.ViewportHeight;

            double offsetX = (canvasWidth - viewportWidth) / 2;
            double offsetY = (canvasHeight - viewportHeight) / 2;

            GraphScrollViewer.ScrollToHorizontalOffset(offsetX);
            GraphScrollViewer.ScrollToVerticalOffset(offsetY);
        }

        // Zoom and Pan Event Handlers
        private void GraphCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            Point mousePos = e.GetPosition(GraphCanvas);

            double oldZoomFactor = zoomFactor;
            zoomFactor *= (e.Delta > 0) ? ZoomSpeed : 1 / ZoomSpeed;

            // Clamp zoom factor
            zoomFactor = Math.Clamp(zoomFactor, MinZoom, MaxZoom);

            // Calculate the mouse offset from the top-left corner
            double offsetX = mousePos.X - translateX;
            double offsetY = mousePos.Y - translateY;

            // Apply zoom and translation
            UpdateCanvasTransform(oldZoomFactor, offsetX, offsetY);
        }

        private void GraphCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isDragging = true;
                startDragPoint = e.GetPosition(GraphCanvas.Parent as UIElement);
                GraphCanvas.CaptureMouse();
            }
        }

        private void GraphCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPoint = e.GetPosition(GraphCanvas.Parent as UIElement);

                double offsetX = currentPoint.X - startDragPoint.X;
                double offsetY = currentPoint.Y - startDragPoint.Y;

                // Update the translation
                translateX += offsetX;
                translateY += offsetY;

                // Apply the new translation and zoom
                GraphCanvas.RenderTransform = new TransformGroup
                {
                    Children = new TransformCollection
                    {
                        new ScaleTransform(zoomFactor, zoomFactor),
                        new TranslateTransform(translateX, translateY)
                    }
                };

                // Update the start point for the next move
                startDragPoint = currentPoint;
            }
        }

        private void GraphCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isDragging = false;
                GraphCanvas.ReleaseMouseCapture();
            }
        }

        private void GraphScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Prevent scrolling when not on the canvas
            var scrolledElement = e.OriginalSource as DependencyObject;
            while (scrolledElement != null && scrolledElement != GraphCanvas)
            {
                scrolledElement = VisualTreeHelper.GetParent(scrolledElement);
            }

            if (scrolledElement == null)
            {
                e.Handled = true;
            }
        }

        // Helper method to update canvas transform with zoom-aware translation
        private void UpdateCanvasTransform(double oldZoomFactor, double offsetX, double offsetY)
        {
            // Adjust the translation so the mouse position stays under the cursor after zooming
            double newOffsetX = offsetX * (zoomFactor / oldZoomFactor);
            double newOffsetY = offsetY * (zoomFactor / oldZoomFactor);

            translateX += offsetX - newOffsetX;
            translateY += offsetY - newOffsetY;

            // Apply the new translation
            GraphCanvas.RenderTransform = new TransformGroup
            {
                Children = new TransformCollection
                {
                    new ScaleTransform(zoomFactor, zoomFactor),
                    new TranslateTransform(translateX, translateY)
                }
            };
        }

        // Public method to reset zoom
        public void ResetZoom()
        {
            zoomFactor = 1.0;
            translateX = 0;
            translateY = 0;

            GraphCanvas.RenderTransform = new TransformGroup
            {
                Children = new TransformCollection
                {
                    new ScaleTransform(1, 1),
                    new TranslateTransform(0, 0)
                }
            };
        }

        // Public property to access the underlying canvas
        public Canvas GraphCanvas => movingCanvas;
    }
}