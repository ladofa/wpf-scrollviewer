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
		static List<double> listModifyX = new List<double>
		{
			1, 0, 0,
			1,    0,
			1, 0, 0
		};

		static List<double> listModifyY = new List<double>
		{
			1, 1, 1,
			0,    0,
			0, 0, 0
		};

		static List<double> listModifyWidth = new List<double>
		{
			-1, 0, 1,
			-1,    1,
			-1, 0, 1
		};

		static List<double> listModifyHeight = new List<double>
		{
			-1, -1, -1,
			0,    0,
			1, 1, 1
		};

		List<Cursor> listCursor = new List<Cursor>
			{
				Cursors.SizeNWSE, Cursors.SizeNS, Cursors.SizeNESW,
				Cursors.SizeWE,                   Cursors.SizeWE,
				Cursors.SizeNESW, Cursors.SizeNS, Cursors.SizeNWSE
			};

		public Item()
		{
			InitializeComponent();

			const double minWidth = 30;

			List<Rectangle> listRectangle = new List<Rectangle>{
				Rectangle00,
				Rectangle01,
				Rectangle02,
				Rectangle10,
				Rectangle12,
				Rectangle20,
				Rectangle21,
				Rectangle22
			};



			bool isPushed = false;
			Point lastPos = new Point(0, 0);

			FrameworkElement parent = Parent as FrameworkElement;


			for (int i = 0; i < 8; i++)
			{
				Rectangle rectangle = listRectangle[i];
				Cursor cursor = listCursor[i];
				double modifyX = listModifyX[i];
				double modifyY = listModifyY[i];
				double modifyWidth = listModifyWidth[i];
				double modifyHeight = listModifyHeight[i];

				rectangle.MouseLeave += (sender, e) =>
				{
					Cursor = Cursors.Arrow;
					isPushed = false;
				};

				rectangle.MouseLeftButtonDown += (sender, e) =>
				{
					isPushed = true;
					lastPos = e.GetPosition(parent);
				};

				rectangle.MouseLeftButtonUp += (sender, e) =>
				{
					isPushed = false;
				};

				rectangle.MouseMove += delegate (object sender, MouseEventArgs e)
				{
					Cursor = cursor;
					if (isPushed)
					{
						var curPos = e.GetPosition(parent);
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
				};
			}
		}

		private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
