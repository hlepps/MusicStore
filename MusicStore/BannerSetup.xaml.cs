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

namespace MusicStore
{
    /// <summary>
    /// Interaction logic for BannerSetup.xaml
    /// </summary>
    public partial class BannerSetup : Window
    {
        public MainMenu instance;
        public BitmapImage logo = DB.DBConfig.studioLogo.bitmap;
        public BitmapImage banner = DB.DBConfig.studioBanner.bitmap;
        public BannerSetup()
        {
            InitializeComponent();
            //Load current values
            StudioNameInputTextBox.Text = DB.DBConfig.studioName;
            StudioLogoImage.Source = DB.DBConfig.studioLogo.bitmap;
            AppBannerImage.Source = DB.DBConfig.studioBanner.bitmap;
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyChangesButton_Click(object sender, RoutedEventArgs e)
        {
            //Send new info to database
            DB.DBConfig.UploadConfig(StudioNameInputTextBox.Text, new DB.DBImage(logo), new DB.DBImage(banner));
            //Run function to update banner;
            instance.RefreshBannerFunction();
            this.Close();
        }

        private void ImportLogoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
               "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
               "Portable Network Graphic (*.png)|*.png";
            if (openFileDialog.ShowDialog()==true)
            {
                logo = new BitmapImage(new Uri(openFileDialog.FileName));
                StudioLogoImage.Source = logo;
            }
        }
        private void ImportAppBannerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
               "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
               "Portable Network Graphic (*.png)|*.png";
            if (openFileDialog.ShowDialog()==true)
            {
               banner = new BitmapImage(new Uri(openFileDialog.FileName));
                AppBannerImage.Source = banner;
            }
        }
    }
}
