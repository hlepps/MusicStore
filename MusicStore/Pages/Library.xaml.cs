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
        int savedID=-1; //-1 is default setting, preventing Load_Saved() from activation
        bool savedIdIsSong; //True - Object with saved ID is a song, False - Object with saved ID is an album

        public void RefreshItems()
        {
            foreach (DB.DBLibraryObject obj in DBConn.instance.currentUser.library.itemlist)
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
                    button.MouseEnter += Album_Hover;
                    button.MouseLeave += Mouse_Leave;
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
                    button.MouseEnter += Song_Hover;
                    button.MouseLeave += Mouse_Leave;
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
            RefreshItems();
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

        private void Refresh_Album(int id)
        {
            TrackDetailsScrollViewer.Visibility = Visibility.Hidden;
            AlbumDetailsScrollViewer.Visibility = Visibility.Visible;
            //Get reference to album with ID
            DB.DBAlbum reference = DB.DBAlbumsSaved.Get(id);
            //Refresh panel details
            AlbumLogoImage.Source = reference.image.bitmap;
            AlbumNameTextBlock.Text = reference.name;
            AlbumArtistTextBlock.Text = reference.author.name;
            TrackAmount.Text = reference.songs.Count().ToString();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < reference.songs.Count; i++)
            {
                sb.Append(reference.songs[i].name);
                if (i != (reference.songs.Count - 1))
                    sb.AppendLine();
            }
            AlbumTrackListTextBlock.Text = sb.ToString();
            AlbumPrice.Text = reference.price.ToString() + " PLN";
        }

        private void Refresh_Song(int id)
        {
            //Swap details panel visibility
            AlbumDetailsScrollViewer.Visibility = Visibility.Hidden;
            TrackDetailsScrollViewer.Visibility = Visibility.Visible;
            // Get reference to song with ID from button
            DB.DBSong reference = DB.DBSongsSaved.Get(id);
            //Refresh panel details
            TrackLogoImage.Source = reference.image.bitmap;
            TrackTitleTextBlock.Text = reference.name;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < reference.authors.Count; i++)
            {
                sb.Append(reference.authors[i].name);
                if (i != (reference.authors.Count - 1))
                    sb.AppendLine();
            }
            TrackArtistTextBlock.Text = sb.ToString();
            TrackPrice.Text = reference.price.ToString() + " PLN";
        }

        private void Load_Saved()
        {
            if (savedIdIsSong)
                Refresh_Song(savedID);
            else
                Refresh_Album(savedID);
        }
        private void Album_Click(object sender, RoutedEventArgs e)
        {
            savedID = GetIDFromObjectName(((Button)sender).Name);
            savedIdIsSong = false;
            Load_Saved();
            //MessageBox.Show("ALBUM", GetIDFromObjectName(((Button)sender).Name).ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void Song_Click(object sender, RoutedEventArgs e)
        {
            savedID = GetIDFromObjectName(((Button)sender).Name);
            savedIdIsSong = true;
            Load_Saved();
            //MessageBox.Show("SONG", GetIDFromObjectName(((Button)sender).Name).ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Song_Hover(object sender, RoutedEventArgs e)
        {
            Refresh_Song(GetIDFromObjectName(((Button)sender).Name));
        }
        
        private void Album_Hover(object sender, RoutedEventArgs e)
        {
            Refresh_Album(GetIDFromObjectName(((Button)sender).Name));
        }

        private void Mouse_Leave(object sender, RoutedEventArgs e)
        {
            if(savedID!=-1)
            Load_Saved();
            else
            {
                AlbumDetailsScrollViewer.Visibility = Visibility.Hidden;
                TrackDetailsScrollViewer.Visibility = Visibility.Hidden;
            }
        }
       private void MusicPlay(object sender, RoutedEventArgs e)
        {
            MusicPlayer musicPlayer = new MusicPlayer();
            musicPlayer.RefreshSong(savedID);
            musicPlayer.Show();

        }

    }
    
}
