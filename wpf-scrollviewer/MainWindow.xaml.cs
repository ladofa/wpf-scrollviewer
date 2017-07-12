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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			Image image = new Image();
			BitmapImage source = new BitmapImage();
			source.BeginInit();
			source.UriSource = new Uri("Clipboard01.png", UriKind.RelativeOrAbsolute);
			source.EndInit();
			image.Source = source;
			//ScrollViewer.Content = image;
		}

		private void Item_Loaded(object sender, RoutedEventArgs e)
		{

		}
	}
}
