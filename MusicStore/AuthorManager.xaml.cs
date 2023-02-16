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
using MusicStore.DB;

namespace MusicStore
{
    /// <summary>
    /// Interaction logic for AuthorManager.xaml
    /// </summary>
    public partial class AuthorManager : Window
    {
        public int? artistID = null; //Imported manually, null value will create new artist
        private DB.DBAuthor reference;
        private BitmapImage ArtistImage;
        bool forceNewImage = false;
        private ImageSource defaultImage;

        public AuthorManager()
        {
            InitializeComponent();
            int nWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            int nHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
            this.LayoutTransform = new ScaleTransform(nWidth / 2, nHeight / 4);
            defaultImage = CoverPreviewImage.Source;
            if(artistID == null)
            {
                RestoreArtistNameButton.Visibility = Visibility.Hidden;
                restoreImageButton.Visibility = Visibility.Hidden;
            }
        }

        public void ReloadWindow() //Manually called function to load site layout and values after assigning (or not) artist ID
        {
            if(artistID!=null)
            {
                SetArtistReference((int)artistID);
            }
            SetManagerLayout();
            LoadArtistInfo();
        }

        public void SetArtistReference(int ID) //Manually set Track that is referenced by this window
        {
            reference = DB.DBAuthorsSaved.Get(ID);
            artistID = (int?)ID;
        }

        public void SetManagerLayout() //Changes from default Edit layout to Creation layout
        {
            if (artistID == null)
            {
                //Track Name
                RestoreArtistNameButton.Visibility = Visibility.Collapsed;
                //Cover Image
                RestorePreviousImageColumn.Width = new GridLength(0, GridUnitType.Star);
                //Confirmation Buttons
                ResetChangesRow.Width = new GridLength(0, GridUnitType.Star);
                SaveAsNewArtistRow.Width = new GridLength(0, GridUnitType.Star);
                SaveChangesButton.Content = "Add New Author";
            }
            //else - Values for editing mode are set by default
        }

        public void LoadArtistInfo() //Loads values of author from authorID
        {
            if (artistID != null)
            {
                //Load author Name
                ReloadArtistName(reference);
                //Load author Image
                ReloadCoverImage(reference);
            }
        }

        //BUTTON FUNCTIONS

        //Track Name
        private void RestoreArtistName_Click(object sender, RoutedEventArgs e)
        {
            ReloadArtistName(reference);
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
            /* Błąd przy linijce Uri uri
            BitmapImage bitmapImage = new BitmapImage();
            Uri uri = new Uri("obrazy/note-gb4fa8b680_640.png");
            bitmapImage.UriSource = uri;
            CoverPreviewImage.Source = bitmapImage;
            forceNewImage = true;
            */
            CoverPreviewImage.Source = defaultImage;
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
                ArtistImage = new BitmapImage(new Uri(openFileDialog.FileName));
                CoverPreviewImage.Source = ArtistImage;
                CoverImageFileTextBlock.Text = openFileDialog.FileName;
                forceNewImage = true;
            }
        }

        //Confirmation Buttons
        private void ResetChanges_Click(object sender, RoutedEventArgs e)
        {
            LoadArtistInfo();
            forceNewImage = false;
        }
        private void SaveAsNewArtist_Click(object sender, RoutedEventArgs e)
        {
            if (TrackNameTextBox.Text.Any())
            {
                DBAuthorsSaved.Add(TrackNameTextBox.Text, DBImagesSaved.Add((BitmapImage)CoverPreviewImage.Source));
                MusicStore.MainMenu.instance.authors.ReloadAuthors();
                this.Close();
            }
            else
            {
                MessageBox.Show("ERROR: Missing required values", "Creating Artist Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (artistID != null)
            {
                if(forceNewImage)
                    DBAuthorsSaved.Update((int)artistID, TrackNameTextBox.Text, DBImagesSaved.Add((BitmapImage)CoverPreviewImage.Source));
                else
                    DBAuthorsSaved.Update((int)artistID, TrackNameTextBox.Text, DBAuthorsSaved.Get((int)artistID).id);
                MusicStore.MainMenu.instance.authors.ReloadAuthors();
            }
            else SaveAsNewArtist_Click(sender, e);
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //RELOAD FUNCTIONS
        private void ReloadArtistName(DB.DBAuthor reference)
        {
            TrackNameTextBox.Text = reference.name;
        }
        private void ReloadCoverImage(DB.DBAuthor reference)
        {
            CoverPreviewImage.Source = reference.image.bitmap;
            CoverImageFileTextBlock.Text = reference.image.ToString();
            forceNewImage = false;
        }
    }
}
