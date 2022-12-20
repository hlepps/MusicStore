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

            var listview = new ListView();
            libview.Content = listview;

            var item = new Border();
            var grid = new Grid();
            item.Child = grid;
            var image = new Image();
            image.Margin = new Thickness(6, 6, 308, 6);
            image.RenderSize = new Size(78, 78);
            image.Width = 78;
            image.Height = 78;
            image.Source = DB.DBImagesSaved.Get(8).bitmap;
            grid.Children.Add(image);
            var label = new Label();
            label.Margin = new Thickness(90, 6, 0, 0);
            label.Content = "ASDF - asdf";
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Top;
            grid.Children.Add(label);
            
            listview.Items.Add(item);

            jezykbox.Items.Clear();
            MusicStore.Language.LangManager.RefreshLanguages();
            foreach(string lang in MusicStore.Language.LangManager.languages)
            {
                jezykbox.Items.Add(lang.Remove(lang.Length-5).Remove(0,9));
            }
        }

        bool a = false;
        private void Style_Click(object sender, RoutedEventArgs e)
        {
            if (a)
            {
                MusicStore.Style.StyleManager.SetCurrentStyle("Style/Darkmode.xaml");
                a = false;
            }
            else
            {
                MusicStore.Style.StyleManager.SetCurrentStyle("Style/Lightmode.xaml");
                a = true;
            }
            MusicStore.Style.StyleManager.UpdateStyle();
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
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var listview = new ListView();
            libview.Content = listview;

            foreach(DB.DBLibraryObject obj in DBConn.instance.currentUser.library.itemlist)
            {
                var item = new Border();
                var grid = new Grid();
                item.Child = grid;
                var image = new Image();
                image.Margin = new Thickness(6, 6, 308, 6);
                image.RenderSize = new Size(78, 78);
                image.Width = 78;
                image.Height = 78;
                var label = new Label();
                if (obj.GetType() == typeof(DB.DBSong))
                {
                    image.Source = ((DB.DBSong)obj).image.bitmap;
                    label.Content = ((DB.DBSong)obj).authors[0].name + " - " + ((DB.DBSong)obj).name;
                }
                else if (obj.GetType() == typeof(DB.DBAlbum))
                {
                    image.Source = ((DB.DBAlbum)obj).image.bitmap;
                    label.Content = ((DB.DBAlbum)obj).author + " - " + ((DB.DBAlbum)obj).name;
                }
                grid.Children.Add(image);
                label.Margin = new Thickness(90, 6, 0, 0);
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignment = VerticalAlignment.Top;
                grid.Children.Add(label);

                listview.Items.Add(item);
            }

        }

        private void Jezykbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MusicStore.Language.LangManager.SetCurrentLanguage(MusicStore.Language.LangManager.languages[jezykbox.Items.IndexOf(jezykbox.SelectedValue)]);
            MusicStore.Language.LangManager.UpdateLanguage();
            Trace.WriteLine(MusicStore.Language.LangManager.GetCurrentLanguage());
        }
    }
}
