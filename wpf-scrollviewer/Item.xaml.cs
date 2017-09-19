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
	/// Interaction logic for Item.xaml
	/// </summary>
	public partial class Item : UserControl
	{
		//0:size-lt 1:size-t 2:size-rt
		//3:size-l           4:size-r
		//5:size-lb 6:size-b 7:size-rb

		static List<double> listModifyX = new List<double>
		{
			1, 0, 0,
			1,    0,
			1, 0, 0,
			1
		};

		static List<double> listModifyY = new List<double>
		{
			1, 1, 1,
			0,    0,
			0, 0, 0,
			1
		};

		static List<double> listModifyWidth = new List<double>
		{
			-1, 0, 1,
			-1,    1,
			-1, 0, 1,
			0
		};

		static List<double> listModifyHeight = new List<double>
		{
			-1, -1, -1,
			0,    0,
			1, 1, 1,
			0
		};


		public static readonly double DefaultUiWidth = 10;

		Point lastPos = new Point(0, 0);
		List<Rectangle> listRectangle;

		public Item()
		{
			InitializeComponent();

			listRectangle = new List<Rectangle>{
				Rectangle00,
				Rectangle01,
				Rectangle02,
				Rectangle10,
				Rectangle12,
				Rectangle20,
				Rectangle21,
				Rectangle22
			};

			for (int i = 0; i < 8; i++)
			{
				Rectangle rectangle = listRectangle[i];
				int index = i;

				rectangle.MouseLeftButtonDown += (sender, e) =>
				{
					lastPos = e.GetPosition(Parent as FrameworkElement);
					ScrollViewer.MouseButtonDownHandler(this, e);
				};

				rectangle.MouseMove += delegate (object sender, MouseEventArgs e)
				{
					ScrollViewer.MoveMode = (ScrollViewer.MoveModes)index;
				};
			}

			List<Rectangle> listMoveRect = new List<Rectangle>
			{
				MoveRect00,
				MoveRect01,
				MoveRect02,
				MoveRect10,
				MoveRect12,
				MoveRect20,
				MoveRect21,
				MoveRect22,
			};

			for (int i = 0; i < 8; i++)
			{
				Rectangle moveRect = listMoveRect[i];

				moveRect.MouseMove += MoveRect_MouseMove;
				moveRect.MouseLeftButtonDown += MoveRect_MouseLeftButtonDown;
			}

			//Selected = false;
		}

		private void MoveRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			lastPos = e.GetPosition(Parent as FrameworkElement);
			ScrollViewer.MouseButtonDownHandler(this, e);
		}

		public ScrollViewer ScrollViewer;

		private void MoveRect_MouseMove(object sender, MouseEventArgs e)
		{
			ScrollViewer.MoveMode = ScrollViewer.MoveModes.MoveSelected;
		}

		public Rect Rect
		{
			get
			{
				return new Rect(Margin.Left, Margin.Top, Width, Height);
			}

			set
			{
				Margin = new Thickness(value.X, value.Y, 0, 0);
				Width = value.Width;
				Height = value.Height;
			}
		}

		public void Move(MouseEventArgs e, int i)
		{
			Point curPos = e.GetPosition(Parent as FrameworkElement);
			const double minWidth = 30;
			double modifyX = listModifyX[i];
			double modifyY = listModifyY[i];
			double modifyWidth = listModifyWidth[i];
			double modifyHeight = listModifyHeight[i];

			double dx = curPos.X - lastPos.X;
			double dy = curPos.Y - lastPos.Y;

			double x = Margin.Left;
			double y = Margin.Top;
			double width = Width;
			double height = Height;

			double nuWidth = width + dx * modifyWidth;
			double nuX = x;
			if (nuWidth < minWidth)
			{
				nuWidth = minWidth;
			}
			else
			{
				nuX = x + dx * modifyX;
			}

			double nuHeight = height + dy * modifyHeight;
			double nuY = y;
			if (nuHeight < minWidth)
			{
				nuHeight = minWidth;
			}
			else
			{
				nuY = y + dy * modifyY;
			}

			Margin = new Thickness(nuX, nuY, 0, 0);
			Width = nuWidth;
			Height = nuHeight;

			lastPos = curPos;
		}

		private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
		{
			throw new NotImplementedException();
		}

		public bool Selected
		{
			set
			{
				if (value)
				{
					GridSize.Visibility = Visibility.Visible;
				}
				else
				{
					GridSize.Visibility = Visibility.Collapsed;
				}
			}

			get
			{
				return GridSize.Visibility == Visibility.Visible;
			}
		}

		public double UiWidth
		{
			get
			{
				return row0.Height.Value;
			}

			set
			{

				double v1 = Item.DefaultUiWidth / value;

				row0.Height = new GridLength(v1);
				row1.Height = new GridLength(v1);
				row2.Height = new GridLength(v1);
				row3.Height = new GridLength(v1);
				row4.Height = new GridLength(v1);

				col0.Width = new GridLength(v1);
				col1.Width = new GridLength(v1);
				col2.Width = new GridLength(v1);
				col3.Width = new GridLength(v1);
				col4.Width = new GridLength(v1);

				double value2 = -v1 / 2;

				GridMove.Margin = new Thickness(value2, value2, value2, value2);
				GridSize.Margin = new Thickness(value2, value2, value2, value2);

				listRectangle.ForEach(rect => rect.StrokeThickness = 1 / value);
			}
		}
	}
}
