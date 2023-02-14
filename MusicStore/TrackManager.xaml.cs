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

namespace MusicStore
{


    /// <summary>
    /// Interaction logic for TrackManager.xaml
    /// </summary>
    public partial class TrackManager : Window
    {
        public int? trackID = null; //Imported manually, null value will create new track
        private DB.DBSong reference;
        private BitmapImage CoverImage;
        bool forceNewMP3 = false;
        bool forceNewImage = false;
        
        public TrackManager()
        {
            InitializeComponent();
            int nWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            int nHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
            this.LayoutTransform = new ScaleTransform(nWidth / 2, nHeight / 2);

            AuthorsStackPanel.Children.Clear();
            Grid g = ObjectGenerationHelper.GetAuthorEmptyGrid();
            var p = (UIElement)AddAuthorButton.Parent;
            ObjectGenerationHelper.RemoveParent(AddAuthorButton);
            g.Children.Add(AddAuthorButton);
            AuthorsStackPanel.Children.Add(g);
            RemoveAuthorButton.Visibility = Visibility.Collapsed;
        }

        public void ReloadWindow() //Manually called function to load site layout and values after assigning (or not) track ID
        {
            SetManagerLayout();
            LoadTrackInfo();
}

        public void SetTrackReference(int ID) //Manually set Track that is referenced by this window
        {
            reference = DB.DBSongsSaved.Get(ID);
            trackID = (int?)ID;
        }

        public void SetManagerLayout() //Changes from default Edit layout to Creation layout
        {
            if(trackID==null)
            {
                //Track Name
                RestoreTrackNameButton.Visibility = Visibility.Collapsed;
                //Cover Image
                RestorePreviousImageColumn.Width = new GridLength(0, GridUnitType.Star);
                //Authors
                RestoreAuthorsButton.Visibility = Visibility.Collapsed;
                //Track File
                RestoreFilePathColumn.Width = new GridLength(0, GridUnitType.Star);
                //Shop Price
                RestorePriceButton.Visibility = Visibility.Collapsed;
                //Confirmation Buttons
                ResetChangesRow.Width = new GridLength(0, GridUnitType.Star);
                SaveAsNewTrackRow.Width = new GridLength(0, GridUnitType.Star);
                SaveChangesButton.Content = "Add New Track";
            }
            //else - Values for editing mode are set by default
        }

        public void LoadTrackInfo() //Loads values of song from trackID
        {
            if (trackID != null)
            {           
                //Load Track Name
                ReloadTrackName(reference);
                //Load Cover Image
                ReloadCoverImage(reference);
                //Authors
                ReloadAuthors(reference);
                //Track File
                ReloadTrackFile(reference);
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
        private void RestoreTrackName_Click(object sender, RoutedEventArgs e)
        {
            ReloadTrackName(reference);
        }

        //Cover Image
        private void RestoreCoverImage_Click(object sender, RoutedEventArgs e)
        {
            ReloadCoverImage(reference);
            forceNewImage = false;
        }
        private void SetDefaultCoverImage_Click(object sender, RoutedEventArgs e)
        {
           //Błąd przy linii 104, trzeba znaleźć sposób na załadowanie "default" opcji
           // BitmapImage bitmapImage = new BitmapImage();
           // Uri uri = new Uri("obrazy/note-gb4fa8b680_640.png");
           // bitmapImage.UriSource = uri;
           // CoverPreviewImage.Source = bitmapImage;
            forceNewImage = true;
        }
        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
               "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
               "Portable Network Graphic (*.png)|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                CoverImage = new BitmapImage(new Uri(openFileDialog.FileName));
                CoverPreviewImage.Source = CoverImage;
                CoverImageFileTextBlock.Text = openFileDialog.FileName;
            }

            forceNewImage = true;
        }
        //Authors
        private void AddNewAuthor_Click(object sender, RoutedEventArgs e)
        {
            AuthorManager am = new AuthorManager();
            am.ShowDialog();
        }
        private void RestoreAuthors_Click(object sender, RoutedEventArgs e)
        {
            ReloadAuthors(reference);
        }
        private void RemoveAuthorFromList_Click(object sender, RoutedEventArgs e)
        {
            Grid g = (Grid)RemoveAuthorButton.Parent;
            ObjectGenerationHelper.RemoveParent(RemoveAuthorButton);
            AuthorsStackPanel.Children.Remove(g);
            if (AuthorsStackPanel.Children.Count > 2)
            {
                ((Grid)AuthorsStackPanel.Children[AuthorsStackPanel.Children.Count - 2]).Children.Add(RemoveAuthorButton);
            }
        }
        private void AddAuthorToList_Click(object sender, RoutedEventArgs e)
        {
            DBAuthorsSaved.GetAll();
            Grid a = (Grid)AuthorsStackPanel.Children[AuthorsStackPanel.Children.Count-1];

            ComboBox cb = new ComboBox();
            cb.SetValue(Grid.ColumnProperty, 1);
            a.Children.Add(cb);
            
            foreach(DBAuthor author in DBAuthorsSaved.dictionary.Values)
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
            ObjectGenerationHelper.RemoveParent(AddAuthorButton);
            b.Children.Add(AddAuthorButton);
            AuthorsStackPanel.Children.Add(b);

            RemoveAuthorButton.Visibility = Visibility.Visible;
            ObjectGenerationHelper.RemoveParent(RemoveAuthorButton);
            a.Children.Add(RemoveAuthorButton);


            
        }

        byte[] mp3bytes; //Track File
        private void ClearFilePath_Click(object sender, RoutedEventArgs e)
        {
            SelectedFileTextBlock.Text = null;
            mp3bytes = new byte[0];
            forceNewMP3 = false;
        }
        private void RestoreFilePath_Click(object sender, RoutedEventArgs e)
        {
            ReloadTrackFile(reference);
            forceNewMP3 = false;
        }

        private void SelectTrackFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 Audio file(*.mp3)| *.mp3";
            if (openFileDialog.ShowDialog() == true)
            {
                mp3bytes = File.ReadAllBytes(openFileDialog.FileName);
            }
            forceNewMP3 = true;
        }

        //Shop Price
        private void RestorePrice_Click(object sender, RoutedEventArgs e)
        {
            ReloadShopPrice(reference);
        }

        //Confirmation Buttons
        private void ResetChanges_Click(object sender, RoutedEventArgs e)
        {
            LoadTrackInfo();
        }


        List<int> GetAuthorsIDs()
        {
            List<int> ids = new List<int>();
            
            foreach(Grid g in AuthorsStackPanel.Children)
            {
                foreach(object cb in g.Children)
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

        private void SaveAsNewTrack_Click(object sender, RoutedEventArgs e)
        {
            if(TrackNameTextBox.Text != "" && mp3bytes != new byte[0] && ShopPriceTextBox.Text != "")
            {
                DBSongsSaved.Add(TrackNameTextBox.Text, DBImagesSaved.Add((BitmapImage)CoverPreviewImage.Source), double.Parse(ShopPriceTextBox.Text), DBSongsSaved.UploadMP3(mp3bytes), GetAuthorsIDs());
                this.Close();
            }
            else
            {
                MessageBox.Show("ERROR: Missing required values", "Creating Track Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (trackID != null)
            {
                DBSongsSaved.Update((int)trackID, TrackNameTextBox.Text, DBImagesSaved.Add((BitmapImage)CoverPreviewImage.Source), double.Parse(ShopPriceTextBox.Text), DBSongsSaved.UploadMP3(mp3bytes), GetAuthorsIDs());
            }
            else SaveAsNewTrack_Click(sender, e);
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //RELOAD FUNCTIONS
        private void ReloadTrackName(DB.DBSong reference)
        {
            TrackNameTextBox.Text = reference.name;
        }
        private void ReloadCoverImage(DB.DBSong reference)
        {          
            CoverPreviewImage.Source = reference.image.bitmap;
            CoverImageFileTextBlock.Text= reference.image.ToString();
        }
        private void ReloadAuthors(DB.DBSong reference)
        {
            //Clear extra authors
            for(int i=1;i<(AuthorsStackPanel.Children.Count-1);i++)
            {
                AuthorsStackPanel.Children.RemoveAt(1);
            }

            if (trackID != null)
            {
                foreach(DBAuthor au in DBSongsSaved.Get((int)trackID).authors)
                {
                    Grid a = (Grid)AuthorsStackPanel.Children[AuthorsStackPanel.Children.Count - 1];

                    ComboBox cb = new ComboBox();
                    cb.SetValue(Grid.ColumnProperty, 1);
                    a.Children.Add(cb);

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

                        cb.Items.Add(cbi);
                        if (author == au)
                            sav = c;

                        c++;
                    }
                    cb.SelectedIndex = sav;

                    Grid b = ObjectGenerationHelper.GetAuthorEmptyGrid();
                    ObjectGenerationHelper.RemoveParent(AddAuthorButton);
                    b.Children.Add(AddAuthorButton);
                    AuthorsStackPanel.Children.Add(b);

                    RemoveAuthorButton.Visibility = Visibility.Visible;
                    ObjectGenerationHelper.RemoveParent(RemoveAuthorButton);
                    a.Children.Add(RemoveAuthorButton);
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
        private void ReloadTrackFile(DB.DBSong reference)
        {
            SelectedFileTextBlock.Text = reference.songurlid.ToString();
        }
        private void ReloadShopPrice(DB.DBSong reference)
        {
            ShopPriceTextBox.Text = reference.price.ToString();
        }

    }
}
