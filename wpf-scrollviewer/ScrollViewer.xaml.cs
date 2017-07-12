using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpf_scrollviewer
{
	/// <summary>
	/// Interaction logic for ScrollViewer.xaml
	/// </summary>
	public partial class ScrollViewer : UserControl
	{
		public System.Windows.FrameworkElement Content
		{
			get => (System.Windows.FrameworkElement)GridContent.Children[0];
			set 
			{
				GridContent.Children.Clear();
				GridContent.Children.Add(value);
				Rect view = new Rect(0, 0, value.ActualWidth, value.ActualHeight);
				ViewArea = view;
				value.SizeChanged += Value_SizeChanged;
			}
		}

		private void Value_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ZoomInFull();
		}

		Point? lastCenterPositionOnTarget;
		Point? lastMousePositionOnTarget;
		Point? lastDragPoint;

		public ScrollViewer()
		{
			InitializeComponent();

			scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;
			scrollViewer.MouseLeftButtonUp += OnMouseLeftButtonUp;
			scrollViewer.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
			scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;

			scrollViewer.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
			scrollViewer.MouseMove += OnMouseMove;

			slider.ValueChanged += OnSliderValueChanged;

			ZoomInFull();
		}


		Rect viewArea = new Rect();

		public double Scale
		{
			get => scaleTransform.ScaleX;
		}

		public Rect ViewArea
		{
			set
			{
				double windowWidth = scrollViewer.ViewportWidth;
				double windowHeight = scrollViewer.ViewportHeight;
				double windowRate = windowWidth / windowHeight;

				if (windowWidth == 0)
				{
					windowWidth = scrollViewer.ActualWidth;
					windowHeight = scrollViewer.ActualHeight;
				}

				double a = GridContent.Width;

				//double contentWidth = scrollViewer.ExtentWidth;
				//double contentHeight = scrollViewer.ExtentHeight; 
				double contentWidth = grid.ActualWidth;
				double contentHeight = grid.ActualHeight;
				double contentRate = contentWidth / contentHeight;

				//oriented in content.
				Rect rect = value;

				if (rect.Width == 0 || contentWidth == 0 || windowWidth == 0)
				{
					viewArea = rect;
					return;
				}

				//--decide scale
				//allowed by scrollViewer
				double minScale = Math.Min(windowWidth / contentWidth, windowHeight / contentHeight);
				

				double scaleX = Math.Max(windowWidth / rect.Width, minScale);
				double scaleY = Math.Max(windowHeight / rect.Height, minScale);

				double scale;
				//(x or y) axis should be extended.
				if (scaleX > scaleY)
				{
					scale = scaleY;
					double oldWidth = rect.Width;
					rect.Width = windowWidth / scale;
					rect.X -= (rect.Width - oldWidth) / 2;//extend from center
				}
				else
				{
					scale = scaleX;
					double oldHeight = rect.Height;
					rect.Height = windowHeight / scale;
					rect.Y -= (rect.Height - oldHeight) / 2;
				}

				scaleTransform.ScaleX = scale;
				scaleTransform.ScaleY = scale;

				

				//scale = scaleTransform.ScaleX;


				//double extendedWidth = contentWidth * scale;
				//double extendedHeight = contentHeight * scale;

				scrollViewer.ScrollToHorizontalOffset(rect.X * scale);
				scrollViewer.ScrollToVerticalOffset(rect.Y * scale);

				//viewArea = rect;
			}

			get
			{
				return viewArea;
			}
		}

		void ZoomInFull()
		{
			ViewArea = new Rect(0, 0, GridContent.ActualWidth, GridContent.ActualHeight);
		}


		

		void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (lastDragPoint.HasValue)
			{
				Point posNow = e.GetPosition(scrollViewer);

				double dX = posNow.X - lastDragPoint.Value.X;
				double dY = posNow.Y - lastDragPoint.Value.Y;

				lastDragPoint = posNow;

				//scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - dX);
				//scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - dY);

				Rect rect = ViewArea;

				rect.X -= dX / Scale;
				rect.Y -= dY / Scale;

				ViewArea = rect;

				Point pos = e.GetPosition(GridContent);
			}
		}

		void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var mousePos = e.GetPosition(scrollViewer);
			if (mousePos.X <= scrollViewer.ViewportWidth && mousePos.Y <
				scrollViewer.ViewportHeight) //make sure we still can use the scrollbars
			{
				scrollViewer.Cursor = Cursors.SizeAll;
				lastDragPoint = mousePos;
				Mouse.Capture(scrollViewer);
			}
		}

		void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			double scale = 1;
			if (e.Delta > 0)
			{
				scale /= 1.2;
			}
			if (e.Delta < 0)
			{
				scale *= 1.2;
			}

			lastMousePositionOnTarget = Mouse.GetPosition(grid);

			Point pos = e.GetPosition(GridContent);

			Rect view = ViewArea;

			double nuWidth = view.Width * scale;
			double nuHeight = view.Height * scale;

			// leftSide / total width
			double rateX = (pos.X - view.X) / view.Width;
			view.X -= (nuWidth - view.Width) * rateX;

			//topSide / total height
			double rateY = (pos.Y - view.Y) / view.Height;
			view.Y -= (nuHeight - view.Height) * rateY;

			view.Width = nuWidth;
			view.Height = nuHeight;

			ViewArea = view;

			e.Handled = true;
		}

		void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			scrollViewer.Cursor = Cursors.Arrow;
			scrollViewer.ReleaseMouseCapture();
			lastDragPoint = null;
		}

		void OnSliderValueChanged(object sender,
			 RoutedPropertyChangedEventArgs<double> e)
		{
			scaleTransform.ScaleX = e.NewValue;
			scaleTransform.ScaleY = e.NewValue;

			var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2,
											 scrollViewer.ViewportHeight / 2);
			lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, grid);
		}

		void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			double scale = Scale;
			if (double.IsNaN(scale))
			{
				//scale = 1;
			}

			
			if (scale != 0)
			{
				viewArea.X = scrollViewer.HorizontalOffset / scale;
				viewArea.Y = scrollViewer.VerticalOffset / scale;
				viewArea.Width = scrollViewer.ViewportWidth / scale;
				viewArea.Height = scrollViewer.ViewportHeight / scale;

				double contentWidth = GridContent.ActualWidth;
				double contentHeight = GridContent.ActualHeight;

				if (viewArea.Width > contentWidth)
				{
					viewArea.X -= (viewArea.Width - contentWidth) / 2;
				}

				if (viewArea.Height > contentHeight)
				{
					viewArea.Y -= (viewArea.Height - contentHeight) / 2;
				}

				Console.WriteLine(viewArea);
			}
		}
	}
}
