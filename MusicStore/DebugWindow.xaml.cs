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
using System.Diagnostics;
using Microsoft.Win32;

namespace MusicStore
{
    /// <summary>
    /// Logika interakcji dla klasy DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        bool a = true;
        private void Style_Click(object sender, RoutedEventArgs e)
        {
            string resourcePath;
            if (a)
            {
                resourcePath = "Style/Darkmode.xaml";
                a = false;
            }
            else
            {
                resourcePath = "Style/Lightmode.xaml";
                a = true;
            }
            App.application.Resources = (ResourceDictionary)Application.LoadComponent(new Uri(resourcePath, UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(imgid.Text);
            img.Source = DB.DBImagesSaved.Get(id).bitmap;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string studio_name = "VINYLPR0";
            DB.DBImage logo = new DB.DBImage(), banner = new DB.DBImage();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png";
            openFileDialog.Title = "logo";
            if (openFileDialog.ShowDialog() == true)
            {
                logo.bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
            }

            openFileDialog.Title = "banner";
            if(openFileDialog.ShowDialog() == true)
            {
                banner.bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
            }

            DB.DBConfig.UploadConfig(studio_name, logo, banner);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DB.DBConfig.RefreshConfig();
            img.Source = DB.DBConfig.studioLogo.bitmap;
        }
    }
}
