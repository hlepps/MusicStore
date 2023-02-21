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
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using MusicStore.DB;
using Org.BouncyCastle.Crypto;

namespace MusicStore
{


    /// <summary>
    /// Interaction logic for TrackManager.xaml
    /// </summary>
    public partial class AlbumManager : Window
    {
        public int? albumID = null; //Imported manually, null value will create new track
        private DB.DBAlbum reference;
        private BitmapImage CoverImage;
        private ImageSource defaultImage;

        public AlbumManager()
        {
            InitializeComponent();
            int nWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            int nHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
            this.LayoutTransform = new ScaleTransform(nWidth / 2, nHeight / 2);
            defaultImage = CoverPreviewImage.Source;

            TracksStackPanel.Children.Clear();
            Grid gt = ObjectGenerationHelper.GetAuthorEmptyGrid(); // CHANGE
            var pt = (UIElement)AddTrackButton.Parent;
            ObjectGenerationHelper.RemoveParent(AddTrackButton);
            gt.Children.Add(AddTrackButton);
            TracksStackPanel.Children.Add(gt);
            RemoveTrackButton.Visibility = Visibility.Collapsed;

            foreach (DBAuthor author in DBAuthorsSaved.dictionary.Values)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Name = "a" + author.id;
                DockPanel dp = new DockPanel();
                Image img = new Image();
                img.Height = 27;
                img.Width = 27;
                img.Source = author.image.bitmap;
                dp.Children.Add(img);
                Separator s = new Separator();
                s.Width = 25;
                s.Opacity = 0;
                dp.Children.Add(s);
                TextBlock tb = new TextBlock();
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Left;
                tb.Text = author.name;
                tb.Foreground = (Brush)FindResource("darknap");
                tb.Background = Brushes.Transparent;
                tb.FontWeight = FontWeights.Medium;
                dp.Children.Add(tb);
                cbi.Content = dp;

                authorComboBox.Items.Add(cbi);
            }
        }

        public void ReloadWindow() //Manually called function to load site layout and values after assigning (or not) album ID
        {
            SetManagerLayout();
            LoadAlbumInfo();
        }

        public void SetAlbumReference(int ID) //Manually set Album that is referenced by this window
        {
            reference = DB.DBAlbumsSaved.Get(ID);
            albumID = (int?)ID;
        }

        public void SetManagerLayout() //Changes from default Edit layout to Creation layout
        {
            if (albumID == null)
            {
                //Track Name
                RestoreAlbumNameButton.Visibility = Visibility.Collapsed;
                //Cover Image
                RestorePreviousImageColumn.Width = new GridLength(0, GridUnitType.Star);
                //Authors
                RestoreAuthorsButton.Visibility = Visibility.Collapsed;
                //Shop Price
                RestorePriceButton.Visibility = Visibility.Collapsed;
                //Confirmation Buttons
                ResetChangesRow.Width = new GridLength(0, GridUnitType.Star);
                SaveAsNewAlbumRow.Width = new GridLength(0, GridUnitType.Star);
                SaveChangesButton.Content = (string)FindResource("addnewalbum2");
            }
            //else - Values for editing mode are set by default
        }

        public void LoadAlbumInfo() //Loads values of song from albumID
        {
            if (albumID != null)
            {
                //Load Album Name
                ReloadAlbumName(reference);
                //Load Cover Image
                ReloadCoverImage(reference);
                //Authors
                ReloadAuthors(reference);
                //Tracks
                ReloadTracks(reference);
                //Shop Price
                ReloadShopPrice(reference);
            }
        }
        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            {
                //Poprawny Regex (według edytora Javascript) - ^(\d{ 1,})(\.{ 0,1})(\d{ 0,2})$
                Regex regex = new Regex(@"^\\d*.\\d\\d|^\\d*");
                //Regex regex = new Regex(@"^(\\d{ 1,})(\\.{ 0,1})(\\d{ 0,2})$");
                e.Handled = regex.IsMatch(e.Text);
            }
        }

        //BUTTON FUNCTIONS

        //Track Name
        private void RestoreAlbumName_Click(object sender, RoutedEventArgs e)
        {
            ReloadAlbumName(reference);
        }

        //Cover Image
        private void RestoreCoverImage_Click(object sender, RoutedEventArgs e)
        {
            ReloadCoverImage(reference);
        }
        private void SetDefaultCoverImage_Click(object sender, RoutedEventArgs e)
        {
            //Błąd przy linii 104, trzeba znaleźć sposób na załadowanie "default" opcji
            // BitmapImage bitmapImage = new BitmapImage();
            // Uri uri = new Uri("obrazy/note-gb4fa8b680_640.png");
            // bitmapImage.UriSource = uri;
            // CoverPreviewImage.Source = bitmapImage;
            CoverPreviewImage.Source = defaultImage;
            //forceNewImage = true;
        }
        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = (string)FindResource("allsuportedgraphics") +
               (string)FindResource("JPEG") +
               (string)FindResource("PNG");
            if (openFileDialog.ShowDialog() == true)
            {
                CoverImage = new BitmapImage(new Uri(openFileDialog.FileName));
                CoverPreviewImage.Source = CoverImage;
                CoverImageFileTextBlock.Text = openFileDialog.FileName;
            }
        }
        //Authors
        private void AddNewAuthor_Click(object sender, RoutedEventArgs e)
        {
            AuthorManager am = new AuthorManager();
            am.ShowDialog();
        }
        private void AddNewTrack_Click(object sender, RoutedEventArgs e)
        {
            TrackManager tm = new TrackManager();
            tm.ShowDialog();
        }
        private void RestoreAuthors_Click(object sender, RoutedEventArgs e)
        {
            ReloadAuthors(reference);
        }
        private void RestoreTracks_Click(object sender, RoutedEventArgs e)
        {
            ReloadTracks(reference);
        }
        private void RemoveAuthorFromList_Click(object sender, RoutedEventArgs e)
        {
            if (AuthorsStackPanel.Children.Count > 2)
            {
            }
        }
        private void RemoveTrackFromList_Click(object sender, RoutedEventArgs e)
        {
            Grid gt = (Grid)RemoveTrackButton.Parent;
            ObjectGenerationHelper.RemoveParent(RemoveTrackButton);
            TracksStackPanel.Children.Remove(gt);
            if (TracksStackPanel.Children.Count > 2)
            {
                ((Grid)TracksStackPanel.Children[TracksStackPanel.Children.Count - 2]).Children.Add(RemoveTrackButton);
            }
        }
        private void AddAuthorToList_Click(object sender, RoutedEventArgs e)
        {
            DBAuthorsSaved.GetAll();
            Grid a = (Grid)AuthorsStackPanel.Children[AuthorsStackPanel.Children.Count - 1];

            ComboBox cb = new ComboBox();
            cb.SetValue(Grid.ColumnProperty, 1);
            a.Children.Add(cb);

            foreach (DBAuthor author in DBAuthorsSaved.dictionary.Values)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Name = "a" + author.id;
                DockPanel dp = new DockPanel();
                Image img = new Image();
                img.Height = 27;
                img.Width = 27;
                img.Source = author.image.bitmap;
                dp.Children.Add(img);
                Separator s = new Separator();
                s.Width = 25;
                s.Opacity = 0;
                dp.Children.Add(s);
                TextBlock tb = new TextBlock();
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Left;
                tb.Text = author.name;
                tb.Foreground = (Brush)FindResource("darknap");
                tb.Background = Brushes.Transparent;
                tb.FontWeight = FontWeights.Medium;
                dp.Children.Add(tb);
                cbi.Content = dp;

                cb.Items.Add(cbi);
            }

            Grid b = ObjectGenerationHelper.GetAuthorEmptyGrid();
            AuthorsStackPanel.Children.Add(b);

        }
        private void AddTrackToList_Click(object sender, RoutedEventArgs e)
        {
            DBAuthorsSaved.GetAll(); //CHANGE
            Grid a = (Grid)TracksStackPanel.Children[TracksStackPanel.Children.Count - 1];

            ComboBox cb = new ComboBox();
            cb.SetValue(Grid.ColumnProperty, 1);
            a.Children.Add(cb);

            foreach (DBSong song in DBSongsSaved.dictionary.Values)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Name = "s" + song.id;
                DockPanel dp = new DockPanel();
                Image img = new Image();
                img.Height = 27;
                img.Width = 27;
                img.Source = song.image.bitmap;
                dp.Children.Add(img);
                Separator s = new Separator();
                s.Width = 25;
                s.Opacity = 0;
                dp.Children.Add(s);
                TextBlock tb = new TextBlock();
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Left;
                tb.Text = song.name;
                tb.Foreground = (Brush)FindResource("darknap");
                tb.Background = Brushes.Transparent;
                tb.FontWeight = FontWeights.Medium;
                dp.Children.Add(tb);
                cbi.Content = dp;

                cb.Items.Add(cbi);
            }

            Grid b = ObjectGenerationHelper.GetAuthorEmptyGrid(); //CHANGE
            ObjectGenerationHelper.RemoveParent(AddTrackButton);
            b.Children.Add(AddTrackButton);
            TracksStackPanel.Children.Add(b);

            RemoveTrackButton.Visibility = Visibility.Visible;
            ObjectGenerationHelper.RemoveParent(RemoveTrackButton);
            a.Children.Add(RemoveTrackButton);
        }

        //Shop Price
        private void RestorePrice_Click(object sender, RoutedEventArgs e)
        {
            ReloadShopPrice(reference);
        }

        //Confirmation Buttons
        private void ResetChanges_Click(object sender, RoutedEventArgs e)
        {
            LoadAlbumInfo();
        }

        int GetAuthorID()
        {
            string s = ((ComboBoxItem)((ComboBox)authorComboBox).SelectedItem).Name;
            s = s.Substring(1);
            return int.Parse(s);
        }

        List<int> GetSongsIDs()
        {
            List<int> ids = new List<int>();

            foreach (Grid g in TracksStackPanel.Children)
            {
                foreach (object cb in g.Children)
                {
                    if (cb is ComboBox)
                    {
                        string s = ((ComboBoxItem)((ComboBox)cb).SelectedItem).Name;
                        s = s.Substring(1);
                        ids.Add(int.Parse(s));
                    }

                }
            }

            return ids;
        }
        private void SaveAsNewAlbum_Click(object sender, RoutedEventArgs e)
        {
            if (AlbumNameTextBox.Text.Any() && ShopPriceTextBox.Text.Any())
            {
                DBAlbumsSaved.Add(AlbumNameTextBox.Text, DBImagesSaved.Add((BitmapImage)CoverPreviewImage.Source), double.Parse(ShopPriceTextBox.Text.Replace('.', ',')), GetAuthorID(), GetSongsIDs());
                MainMenu.instance.library.RefreshPage();
                this.Close();
            }
            else
            {
                MessageBox.Show((string)FindResource("ERROR"), (string)FindResource("creatingalbumfailed"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (albumID != null)
            {
                DBAlbumsSaved.Update((int)albumID, AlbumNameTextBox.Text, DBImagesSaved.Add((BitmapImage)CoverPreviewImage.Source), double.Parse(ShopPriceTextBox.Text.Replace('.', ',')), GetAuthorID(), GetSongsIDs());
                MainMenu.instance.library.RefreshPage();
                this.Close();
            }
            else SaveAsNewAlbum_Click(sender, e);
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //RELOAD FUNCTIONS
        private void ReloadAlbumName(DB.DBAlbum reference)
        {
            AlbumNameTextBox.Text = reference.name;
        }
        private void ReloadCoverImage(DB.DBAlbum reference)
        {
            CoverPreviewImage.Source = reference.image.bitmap;
            CoverImageFileTextBlock.Text = reference.image.ToString();
        }
        private void ReloadAuthors(DB.DBAlbum reference)
        {
            //Clear extra authors
            for (int i = 1; i < (AuthorsStackPanel.Children.Count - 1); i++)
            {
                AuthorsStackPanel.Children.RemoveAt(1);
            }

            if (albumID != null)
            {
                int c = 0;
                int sav = 0;
                foreach (DBAuthor author in DBAuthorsSaved.dictionary.Values)
                {
                    ComboBoxItem cbi = new ComboBoxItem();
                    cbi.Name = "a" + author.id;
                    DockPanel dp = new DockPanel();
                    Image img = new Image();
                    img.Height = 27;
                    img.Width = 27;
                    img.Source = author.image.bitmap;
                    dp.Children.Add(img);
                    Separator s = new Separator();
                    s.Width = 25;
                    s.Opacity = 0;
                    dp.Children.Add(s);
                    TextBlock tb = new TextBlock();
                    tb.VerticalAlignment = VerticalAlignment.Center;
                    tb.HorizontalAlignment = HorizontalAlignment.Left;
                    tb.Text = author.name;
                    tb.Foreground = (Brush)FindResource("darknap");
                    tb.Background = Brushes.Transparent;
                    tb.FontWeight = FontWeights.Medium;
                    dp.Children.Add(tb);
                    cbi.Content = dp;

                    authorComboBox.Items.Add(cbi);
                    if (author == DBAlbumsSaved.Get((int)albumID).author)
                        sav = c;

                    c++;
                }
                authorComboBox.SelectedIndex = sav;

                Grid b = ObjectGenerationHelper.GetAuthorEmptyGrid();

            }
            /*
             This part of the function needs to in order:
            1. Clear ComboBox_0 ComboBoxItems;
            2. Create a ComboBoxItem List;
            2a. ComboBoxItem needs to consist of:
                <DockPanel>
                    <Image Height="27" Width="27" Source="{ARTIST_i IMAGE}"/>
                    <Separator Width="25" Opacity="0"/>
                    <TextBlock VerticalAlignment="Center" HorizontalAlignment="Left" Text="{ARTIST_i NAME}"/>
                </DockPanel>
                ;
            3. For each item in ComboBoxItem List, add it to ComboBox_0;
            4. Set IsSelected="True" for ComboBoxItem in ComboBox_0 that matches Artist Name with first Artist in Track;
            5. For each missing artist, generate new ComboBoxItem, insert it to AuthorsStackPanel, then for new ComboBoxItem repeat step 3 and 4.
             */
        }

        private void ReloadTracks(DB.DBAlbum reference)
        {
            //Clear extra tracks
            for (int i = 1; i < (TracksStackPanel.Children.Count - 1); i++)
            {
                TracksStackPanel.Children.RemoveAt(1);
            }

            if (albumID != null)
            {
                foreach (DBSong songAlbum in DBAlbumsSaved.Get((int)albumID).songs)
                {
                    Grid a = (Grid)TracksStackPanel.Children[TracksStackPanel.Children.Count - 1];

                    ComboBox cb = new ComboBox();
                    cb.SetValue(Grid.ColumnProperty, 1);
                    a.Children.Add(cb);

                    int c = 0;
                    int sav = 0;
                    foreach (DBSong song in DBSongsSaved.dictionary.Values)
                    {
                        ComboBoxItem cbi = new ComboBoxItem();
                        cbi.Name = "s" + song.id;
                        DockPanel dp = new DockPanel();
                        Image img = new Image();
                        img.Height = 27;
                        img.Width = 27;
                        img.Source = song.image.bitmap;
                        dp.Children.Add(img);
                        Separator s = new Separator();
                        s.Width = 25;
                        s.Opacity = 0;
                        dp.Children.Add(s);
                        TextBlock tb = new TextBlock();
                        tb.VerticalAlignment = VerticalAlignment.Center;
                        tb.HorizontalAlignment = HorizontalAlignment.Left;
                        tb.Text = song.name;
                        tb.Foreground = (Brush)FindResource("darknap");
                        tb.Background = Brushes.Transparent;
                        tb.FontWeight = FontWeights.Medium;
                        dp.Children.Add(tb);
                        cbi.Content = dp;

                        cb.Items.Add(cbi);
                        if (song == songAlbum)
                            sav = c;

                        c++;
                    }
                    cb.SelectedIndex = sav;

                    Grid b = ObjectGenerationHelper.GetAuthorEmptyGrid(); //Change
                    ObjectGenerationHelper.RemoveParent(AddTrackButton);
                    b.Children.Add(AddTrackButton);
                    TracksStackPanel.Children.Add(b);

                    RemoveTrackButton.Visibility = Visibility.Visible;
                    ObjectGenerationHelper.RemoveParent(RemoveTrackButton);
                    a.Children.Add(RemoveTrackButton);
                }
            }
            /*
             This part of the function needs to in order:
            1. Clear ComboBox_0 ComboBoxItems;
            2. Create a ComboBoxItem List;
            2a. ComboBoxItem needs to consist of:
                <DockPanel>
                    <Image Height="27" Width="27" Source="{ARTIST_i IMAGE}"/>
                    <Separator Width="25" Opacity="0"/>
                    <TextBlock VerticalAlignment="Center" HorizontalAlignment="Left" Text="{ARTIST_i NAME}"/>
                </DockPanel>
                ;
            3. For each item in ComboBoxItem List, add it to ComboBox_0;
            4. Set IsSelected="True" for ComboBoxItem in ComboBox_0 that matches Artist Name with first Artist in Track;
            5. For each missing artist, generate new ComboBoxItem, insert it to AuthorsStackPanel, then for new ComboBoxItem repeat step 3 and 4.
             */
        }

        private void ReloadShopPrice(DB.DBAlbum reference)
        {
            ShopPriceTextBox.Text = reference.price.ToString();
        }

    }
}
