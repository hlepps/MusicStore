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
        public int savedID=-1; //-1 is default setting, preventing Load_Saved() from activation
        private bool savedIdIsSong; //True - Object with saved ID is a song, False - Object with saved ID is an album
        private List<Button> markedForDeletion;

        public enum Mode //Mode Library.xaml is launched on
        {
            UserLibrary, //Shows only user-owned objects
            Shop, //Shows whole library, contains shop functions
            Album //Shows only album-specific objects
        }
        public Mode pageMode = Mode.UserLibrary;
        private enum AdminMode
        {
            Browse,
            Edit,
            Delete
        }
        private AdminMode pageAdminMode = AdminMode.Browse;

        private void RefreshItems(List<DB.DBLibraryObject> list)
        {
            itemPanel.Children.Clear();
            items.Clear();
            
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
            //Lower part is only to be used as long as SQL Filter won't be made
            sqlFilterColumn.Width = new GridLength(0, GridUnitType.Star);
            contentListColumn.Width = new GridLength(6, GridUnitType.Star);
            //END OF TEMPORARY PART
            markedForDeletion = new List<Button>();
        }
        public void RefreshPage()
        {
            savedID = -1;
            AlbumDetailsScrollViewer.Visibility = Visibility.Hidden;
            TrackDetailsScrollViewer.Visibility = Visibility.Hidden;
            pageAdminMode = AdminMode.Browse;
            deleteStackPanel.Visibility = Visibility.Collapsed;
            SetUserLayout();
            SetDetailsPanelLayout();
            RefreshContent();
        }
        private void SetUserLayout()
        {
            if(UserFunctions.VerifyUserPermission(2))
            {
                adminPanelRow.Height = new GridLength(1, GridUnitType.Star);
                AdminModeTextBlock.Text = pageAdminMode.ToString();
            }
            else
            {
                adminPanelRow.Height = new GridLength(0, GridUnitType.Star);
            }
        }

        private void SetDetailsPanelLayout()
        {
            switch (pageMode)
            {
                case Mode.UserLibrary:
                    ShopTrackPanel.Visibility = Visibility.Collapsed;
                    ShopAlbumPanel.Visibility = Visibility.Collapsed;
                    TrackListenTextBlock.Text = (string)FindResource("playtrackcs");
                    break;

                case Mode.Shop:
                    ShopTrackPanel.Visibility = Visibility.Visible;
                    ShopAlbumPanel.Visibility = Visibility.Visible;
                    TrackListenTextBlock.Text = (string)FindResource("playsamplecs");
                    break;

                case Mode.Album:
                    ShopTrackPanel.Visibility = Visibility.Collapsed;
                    ShopAlbumPanel.Visibility = Visibility.Collapsed;
                    if(DBConn.instance.currentUser.library.itemlist.Contains(DB.DBAlbumsSaved.Get(albumID)))
                    {
                        TrackListenTextBlock.Text = (string)FindResource("playtrackcs");
                    }
                    else
                    {
                        TrackListenTextBlock.Text = (string)FindResource("playsamplecs");
                    }
                    break;
            }
        }
        public void RefreshContent()
        {
            switch(pageMode)
            {
                case Mode.UserLibrary:
                    RefreshItems(DBConn.instance.currentUser.library.itemlist);
                    break;

                case Mode.Shop:
                    DB.DBSongsSaved.GetAll();
                    DB.DBAlbumsSaved.GetAll();
                    List<DB.DBLibraryObject> all = new List<DB.DBLibraryObject>();
                    all.AddRange(DB.DBAlbumsSaved.GetAllFromDictionary());
                    all.AddRange(DB.DBSongsSaved.GetAllFromDictionary());
                    RefreshItems(all);
                    break;

                case Mode.Album:
                    RefreshItems(DB.DBAlbumsSaved.Get(albumID).songs);
                    break;
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
                sb.Append(((DB.DBSong)reference.songs[i]).name);
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
            switch (pageAdminMode)
            {
                case AdminMode.Browse:
                    Load_Saved();
                    break;
                case AdminMode.Edit:
                    AlbumManager albumManager = new AlbumManager();
                    albumManager.SetAlbumReference(savedID);
                    albumManager.ReloadWindow();
                    albumManager.ShowDialog();
                    break;
                case AdminMode.Delete:
                    ToggleDelete(sender);
                    break;
            }
        }
        private void Song_Click(object sender, RoutedEventArgs e)
        {
            savedID = GetIDFromObjectName(((Button)sender).Name);
            savedIdIsSong = true;
            switch (pageAdminMode)
            {
                case AdminMode.Browse:                 
                    Load_Saved();
                    break;
                case AdminMode.Edit:
                    TrackManager trackManager = new TrackManager();
                    trackManager.SetTrackReference(savedID);
                    trackManager.ReloadWindow();
                    trackManager.ShowDialog();
                    break;
                case AdminMode.Delete:
                    ToggleDelete(sender);
                    break;
            }    
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
            musicPlayer.PlaySong(savedID);
        }

        private void OpenAlbumLibrary_Click(object sender, RoutedEventArgs e)
        {
            Library albumLibrary = new Library();
            Window window = new Window();
            DB.DBAlbum reference = DB.DBAlbumsSaved.Get(savedID);
            window.Title = reference.name;
            window.Icon = reference.image.bitmap;
            LinearGradientBrush lgb = new LinearGradientBrush();
            lgb.StartPoint = new Point(1, 1);
            lgb.EndPoint = new Point(0, 0);
            lgb.GradientStops = (GradientStopCollection)FindResource("tlo");
            window.Background = lgb;
            albumLibrary.albumID = savedID;
            albumLibrary.pageMode = Mode.Album;
            albumLibrary.RefreshPage();
            Frame pageFrame = new Frame();
            pageFrame.Content = albumLibrary;
            window.Content = pageFrame;
            window.Show();
        }

        //Admin Panel Functions
        private void SetBrowseMode_Click(object sender, RoutedEventArgs e)
        {
            pageAdminMode = AdminMode.Browse;
            AdminModeTextBlock.Text = pageAdminMode.ToString();
            deleteStackPanel.Visibility = Visibility.Collapsed;
            ClearMarkedForDeletionList();
        } 
        private void SetEditMode_Click(object sender, RoutedEventArgs e)
        {
            pageAdminMode = AdminMode.Edit;
            AdminModeTextBlock.Text = pageAdminMode.ToString();
            deleteStackPanel.Visibility = Visibility.Collapsed;
            ClearMarkedForDeletionList();
        } 
        private void SetDeleteMode_Click(object sender, RoutedEventArgs e)
        {
            pageAdminMode = AdminMode.Delete;
            AdminModeTextBlock.Text = pageAdminMode.ToString();
            deleteStackPanel.Visibility = Visibility.Visible;
            markedForDeletionCountTextBlock.Text = markedForDeletion.Count.ToString();
        }


        private void ToggleDelete(object sender)
        {
            if(markedForDeletion.Contains((Button)sender))
            {
                markedForDeletion.Remove((Button)sender);
                ((LinearGradientBrush)((Border)((Grid)((Button)sender).Parent).Parent).BorderBrush).GradientStops = (GradientStopCollection)FindResource("rama");
            }
            else
            {
                markedForDeletion.Add((Button)sender);
                ((LinearGradientBrush)((Border)((Grid)((Button)sender).Parent).Parent).BorderBrush).GradientStops = (GradientStopCollection)FindResource("ramaWybrana");
            }
            if (markedForDeletion.Any())
            {
                markedForDeletionCountTextBlock.Text = markedForDeletion.Count.ToString();
            }
            else
            {
                markedForDeletionCountTextBlock.Text = "0";
            }
        }

        private void ClearMarkedForDeletionList()
        {
            if (markedForDeletion.Any())
            {
                foreach (Button obj in markedForDeletion)
                {
                    ((LinearGradientBrush)((Border)((Grid)obj.Parent).Parent).BorderBrush).GradientStops = (GradientStopCollection)FindResource("rama");
                }
                markedForDeletion.Clear();
            }
        }

        private void DeleteMessageBox_Click(object sender, RoutedEventArgs e)
        {
            if(markedForDeletion.Any())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append((string)FindResource("youareabouttoremove"));
                sb.Append(markedForDeletion.Count());
                sb.Append((string)FindResource("itemsfromthedatabase"));
                sb.AppendLine();
                sb.AppendLine();
                sb.Append((string)FindResource("inresults,these"));
                sb.AppendLine();
                sb.AppendLine();
                sb.Append((string)FindResource("THISACTION"));
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine((string)FindResource("proceed"));
                MessageBoxResult result = MessageBox.Show(sb.ToString(), (string)FindResource("delete"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch(result)
                {
                    case MessageBoxResult.Yes:
                        foreach(Button b in markedForDeletion)
                        {
                            char type = b.Name.First();
                            int id = int.Parse(b.Name.Substring(1));
                            switch (type)
                            {
                                case 'S':
                                    DB.DBSongsSaved.Remove(id);
                                    break;

                                case 'A':
                                    DB.DBAlbumsSaved.Remove(id);
                                    break;
                            }
                        }
                        markedForDeletion.Clear();
                        savedID = -1;
                        AlbumDetailsScrollViewer.Visibility = Visibility.Hidden;
                        TrackDetailsScrollViewer.Visibility = Visibility.Hidden;
                        RefreshContent();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }
        private void AddNewTrack_Click(object sender, RoutedEventArgs e)
        {
            TrackManager trackManager = new TrackManager();
            trackManager.ReloadWindow();
            trackManager.ShowDialog();
        }
        private void AddNewArtist_Click(object sender, RoutedEventArgs e)
        {
            AuthorManager authorManager = new AuthorManager();
            authorManager.ReloadWindow();
            authorManager.ShowDialog();
        }
        private void AddNewAlbum_Click(object sender, RoutedEventArgs e)
        {
            AlbumManager albumManager = new AlbumManager();
            albumManager.ReloadWindow();
            albumManager.ShowDialog();
        }

        private void BuyItem_Click(object sender, RoutedEventArgs e)
        {
            if(UserFunctions.VerifyUserPermission(1))
            {
                PaymentMethod paymentMethod = new PaymentMethod();
                double tmp;
                if(savedIdIsSong)
                {
                    DB.DBSong reference = DB.DBSongsSaved.Get(savedID);
                    tmp = reference.price;
                }
                else
                {
                    DB.DBAlbum reference = DB.DBAlbumsSaved.Get(savedID);
                    tmp = reference.price;
                }
                paymentMethod.itemPrice = tmp;
                paymentMethod.itsAlbum = !savedIdIsSong;
                paymentMethod.id = savedID;
                paymentMethod.RefreshWindow();
                paymentMethod.ShowDialog();
            }
            else
            {
                StringBuilder messageBoxText = new StringBuilder();
                messageBoxText.Append((string)FindResource("shoppurchasesare"));
                messageBoxText.AppendLine();
                messageBoxText.AppendLine();
                messageBoxText.Append((string)FindResource("wouldyouliketo"));
                messageBoxText.AppendLine();
                messageBoxText.AppendLine();
                messageBoxText.Append((string)FindResource("registernowand"));
                if (MessageBox.Show(messageBoxText.ToString(), (string)FindResource("insufficientpermissions"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    register nowyuser = new register();
                    //nowyuser = load data from current guest user for Register data inputs
                    if ((bool)nowyuser.ShowDialog())
                    {

                    }
                }
            }
        }
    }
}
