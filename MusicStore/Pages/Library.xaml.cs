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

namespace MusicStore.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy Library.xaml
    /// </summary>
    public partial class Library : Page
    {
        List<Border> items = new List<Border>();
        public int albumID = 0;

        public void RefreshItems(List<DB.DBLibraryObject> list)
        {
            foreach (DB.DBLibraryObject obj in list)
            {
                if (obj.GetType() == typeof(DB.DBAlbum))
                {
                    Border temp = new Border();
                    temp.Name = "A" + obj.id;
                    temp.CornerRadius = new CornerRadius(8, 8, 0, 8);
                    temp.SetValue(Grid.ColumnProperty, items.Count % 4);
                    temp.VerticalAlignment = VerticalAlignment.Top;
                    temp.Height = 100;
                    temp.Width = 100;
                    temp.Margin = new Thickness(10, 10, 10, 10);
                    temp.BorderThickness = new Thickness(2, 2, 4, 4);

                    LinearGradientBrush lgb = new LinearGradientBrush();
                    lgb.StartPoint = new Point(1, 1);
                    lgb.EndPoint = new Point(0, 0);
                    lgb.GradientStops = (GradientStopCollection)FindResource("rama");
                    temp.BorderBrush = lgb;

                    ImageBrush ib = new ImageBrush();
                    ib.ImageSource = ((DB.DBAlbum)obj).image.bitmap;
                    ib.Stretch = Stretch.Uniform;
                    temp.Background = ib;

                    Grid grid = new Grid();

                    Button button = new Button();
                    button.Name = "A" + obj.id;
                    button.Opacity = 0;
                    button.Click += Album_Click;
                    grid.Children.Add(button);

                    Border b = new Border();
                    b.CornerRadius = new CornerRadius(8, 0, 0, 0);
                    b.VerticalAlignment = VerticalAlignment.Bottom;
                    b.HorizontalAlignment = HorizontalAlignment.Right;
                    b.BorderThickness = new Thickness(2, 2, 2, 2);
                    b.Width = 30;
                    b.Height = 15;
                    b.BorderBrush = lgb;
                    b.Background = lgb;
                    grid.Children.Add(b);

                    Image img = new Image();
                    img.Source = new BitmapImage(new Uri(@"/obrazy/album.png", UriKind.Relative));
                    img.VerticalAlignment = VerticalAlignment.Bottom;
                    img.HorizontalAlignment = HorizontalAlignment.Right;
                    img.Width = 15;
                    img.Margin = new Thickness(0, 0, 5, 0);
                    grid.Children.Add(img);

                    temp.Child = grid;

                    items.Add(temp);
                    itemPanel.Children.Add(items[items.Count-1]);
                }
                else
                {
                    Border temp = new Border();
                    temp.Name = "S" + obj.id;
                    temp.CornerRadius = new CornerRadius(8, 8, 8, 8);
                    temp.SetValue(Grid.ColumnProperty, items.Count % 4);
                    temp.VerticalAlignment = VerticalAlignment.Top;
                    temp.Height = 100;
                    temp.Width = 100;
                    temp.Margin = new Thickness(10, 10, 10, 10);
                    temp.BorderThickness = new Thickness(2, 2, 2, 2);

                    LinearGradientBrush lgb = new LinearGradientBrush();
                    lgb.StartPoint = new Point(1, 1);
                    lgb.EndPoint = new Point(0, 0);
                    lgb.GradientStops = (GradientStopCollection)FindResource("rama");
                    temp.BorderBrush = lgb;

                    ImageBrush ib = new ImageBrush();
                    ib.ImageSource = ((DB.DBSong)obj).image.bitmap;
                    ib.Stretch = Stretch.Uniform;
                    temp.Background = ib;

                    Grid grid = new Grid();

                    Button button = new Button();
                    button.Name = "S" + obj.id;
                    button.Opacity = 0;
                    button.Click += Song_Click;
                    grid.Children.Add(button);

                    temp.Child = grid;

                    items.Add(temp);
                    itemPanel.Children.Add(items[items.Count - 1]);
                }
            }
        }

        public Library()
        {
            InitializeComponent();
            if (albumID == 0)
            {
                RefreshItems(DBConn.instance.currentUser.library.itemlist);
            }
            else
            {
                RefreshItems(DB.DBAlbumsSaved.Get(albumID).songs);
            }
        }

        private void LibraryListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = ContentScrollViewer;    
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void DetailsScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        private int GetIDFromObjectName(string name)
        {
            string s = name.Remove(0, 1);
            return Int32.Parse(s);
        }

        private void Album_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ALBUM", GetIDFromObjectName(((Button)sender).Name).ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void Song_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("SONG", GetIDFromObjectName(((Button)sender).Name).ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    
}
