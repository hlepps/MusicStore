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

namespace MusicStore
{
    /// <summary>
    /// Interaction logic for AlbumManager.xaml
    /// </summary>
    public partial class AlbumManager : Window
    {
            public int? trackID = null; //Imported manually, null value will create new track
            private DB.DBSong reference;
            private BitmapImage CoverImage;
            private List<char> allowedChar = new List<char>();

            public AlbumManager()
            {
                InitializeComponent();
                int nWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
                int nHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
                this.LayoutTransform = new ScaleTransform(nWidth / 2, nHeight / 2);
                allowedChar.Add('0');
                allowedChar.Add('1');
                allowedChar.Add('2');
                allowedChar.Add('3');
                allowedChar.Add('4');
                allowedChar.Add('5');
                allowedChar.Add('6');
                allowedChar.Add('7');
                allowedChar.Add('8');
                allowedChar.Add('9');
                allowedChar.Add('0');
                allowedChar.Add('.');
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
                if (trackID == null)
                {
                    //Track Name
                    RestoreTrackNameButton.Visibility = Visibility.Collapsed;
                    //Cover Image
                    RestorePreviousImageColumn.Width = new GridLength(0, GridUnitType.Star);
                    //Authors
                    RestoreAuthorsButton.Visibility = Visibility.Collapsed;
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
                    //Shop Price
                    ReloadShopPrice(reference);
                }
            }
            private void PreviewTextInput(object sender, TextCompositionEventArgs e)
            {
                {
                    //Poprawny Regex (według edytora Javascript) - ^(\d{ 1,})(\.{ 0,1})(\d{ 0,2})$
                    Regex regex = new Regex("[^0-9]+");
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
            }
            private void SetDefaultCoverImage_Click(object sender, RoutedEventArgs e)
            {
                //Błąd przy linii 104, trzeba znaleźć sposób na załadowanie "default" opcji
                // BitmapImage bitmapImage = new BitmapImage();
                // Uri uri = new Uri("obrazy/note-gb4fa8b680_640.png");
                // bitmapImage.UriSource = uri;
                // CoverPreviewImage.Source = bitmapImage;
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
            }
            //Authors
            private void AddNewAuthor_Click(object sender, RoutedEventArgs e)
            {
                /*
                 1. Open new Author Manager
                 2. If new Author was created, load and add it to Artist list
                 */
            }
            private void RestoreAuthors_Click(object sender, RoutedEventArgs e)
            {
                ReloadAuthors(reference);
            }
            private void RemoveAuthorFromList_Click(object sender, RoutedEventArgs e)
            {
                //Delete ComboBox that the button belonged to
            }
            private void AddAuthorToList_Click(object sender, RoutedEventArgs e)
            {
                //Generate new Author ComboBox
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
            private void SaveAsNewTrack_Click(object sender, RoutedEventArgs e)
            {
                if (TrackNameTextBox.Text.Any() && ShopPriceTextBox.Text.Any())
                {
                    //Create new song to database from data in this window
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
                    //Update song in database with ID equal to trackID with data from this window
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
                CoverImageFileTextBlock.Text = reference.image.ToString();
            }
            private void ReloadAuthors(DB.DBSong reference)
            {
                //Clear extra authors
                for (int i = 1; i < (AuthorsStackPanel.Children.Count - 1); i++)
                {
                    AuthorsStackPanel.Children.RemoveAt(1);
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
            private void ReloadShopPrice(DB.DBSong reference)
            {
                ShopPriceTextBox.Text = reference.price.ToString();
            }

        }
    }
