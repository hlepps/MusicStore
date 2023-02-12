using MusicStore.DB;
using NAudio.Utils;
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
    /// Logika interakcji dla klasy AccountSettings.xaml
    /// </summary>
    public partial class AccountSettings : Page
    {
        List<DBImage> images = new List<DBImage>();
        public AccountSettings()
        {
            InitializeComponent();


            foreach(DBLibraryObject obj in DBConn.instance.currentUser.library.itemlist)
            {
                if (obj.GetType() == typeof(DB.DBAlbum))
                {
                    DBImage dbi = ((DB.DBAlbum)obj).image;
                    if (!images.Contains(dbi))
                        images.Add(dbi);

                    if(!images.Contains(((DB.DBAlbum)obj).author.image))
                        images.Add(((DB.DBAlbum)obj).author.image);
                }
                else if (obj.GetType() == typeof(DB.DBSong))
                {
                    DBImage dbi = ((DB.DBSong)obj).image;
                    if (!images.Contains(dbi))
                        images.Add(dbi);

                }
            }

            pictureCombo.Items.Clear();
            foreach(DBImage image in images)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                StackPanel sp = new StackPanel();
                Image img = new Image();
                img.Source = image.bitmap;
                img.MaxWidth = 64;
                img.MaxHeight = 64;
                sp.Children.Add(img);
                cbi.Content = sp;
                pictureCombo.Items.Add(cbi);
            }
        }

        private void pictureCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = images[pictureCombo.Items.IndexOf(pictureCombo.SelectedItem)].id;
            DBConn.instance.currentUser.ChangeAvatar(id);

        }
    }
}
