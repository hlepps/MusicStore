using MusicStore.DB;
using MySql.Data.MySqlClient;
using NAudio.Utils;
using System;
using System.Collections.Generic;
using System.Data;
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
                if (obj is DB.DBAlbum)
                {
                    DBImage dbi = ((DB.DBAlbum)obj).image;
                    if (!images.Contains(dbi))
                        images.Add(dbi);

                    if(!images.Contains(((DB.DBAlbum)obj).author.image))
                        images.Add(((DB.DBAlbum)obj).author.image);
                }
                else if (obj is DB.DBSong)
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

            pfp.Source = DBConn.instance.currentUser.avatar.bitmap;

            if(DBConn.instance.currentUser.permission < 2)
            {
                AdminSettingsBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                AdminSettingsBorder.Visibility = Visibility.Visible;
            }
        }

        private void pictureCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = images[pictureCombo.Items.IndexOf(pictureCombo.SelectedItem)].id;
            DBConn.instance.currentUser.ChangeAvatar(id);
            pfp.Source = DBConn.instance.currentUser.avatar.bitmap;

        }

        private void btnwyloguj_Click(object sender, RoutedEventArgs e)
        {
            DBConn.instance.Logout();
        }

        Window usersToChangePermissionWindow = new Window();
        bool resign;
        private void wybierzUsera(object sender, RoutedEventArgs e)
        {
            if (resign)
            {
                System.Windows.MessageBox.Show("Wybierz użytkownika, który odziedziczy uprawnienia administratora", "Rezygnacja", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

                string user = ((TextBlock)((ListBox)usersToChangePermissionWindow.Content).SelectedItem).Text;
                DBConn.instance.PrepareConnection();
                MySqlCommand a = new MySqlCommand($"UPDATE users SET permission=1 WHERE username='{DBConn.instance.currentUser.username}'", DBConn.instance.conn);
                a.ExecuteNonQuery();
                DBConn.instance.PrepareConnection();
                MySqlCommand b = new MySqlCommand($"UPDATE users SET permission=2 WHERE username='{user}'", DBConn.instance.conn);
                b.ExecuteNonQuery();
                DBConn.instance.Logout();
                usersToChangePermissionWindow.Close();
            }
            else
            {
                string user = ((TextBlock)((ListBox)usersToChangePermissionWindow.Content).SelectedItem).Text;
                int index = user.IndexOf(" ");
                user = user.Substring(0, index);
                int permission = int.Parse(((TextBlock)((ListBox)usersToChangePermissionWindow.Content).SelectedItem).Name.Substring(1));
                DBConn.instance.PrepareConnection();
                if (permission == 1)
                {
                    MySqlCommand a = new MySqlCommand($"UPDATE users SET permission=2 WHERE username='{user}'", DBConn.instance.conn);
                    a.ExecuteNonQuery();
                }
                else
                {
                    MySqlCommand a = new MySqlCommand($"UPDATE users SET permission=1 WHERE username='{user}'", DBConn.instance.conn);
                    a.ExecuteNonQuery();
                }
                usersToChangePermissionWindow.Hide();
            }
        }

        private void btnzrezygnuj_Click(object sender, RoutedEventArgs e)
        {
            if (DBConn.instance.currentUser.permission == 3)
                System.Windows.MessageBox.Show("Jako główny administrator nie możesz zrezygnować z uprawnień administratora", "Rezygnacja", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            else
            {
                MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz zrezygnować z uprawnień administratora?", "Rezygnacja", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        resign = true;
                        ListBox users = new ListBox();
                        usersToChangePermissionWindow.Content = users;
                        usersToChangePermissionWindow.Width = 300;
                        usersToChangePermissionWindow.Height = 400; 
                        var location = btnzrezygnuj.PointToScreen(new Point(0, 0));
                        usersToChangePermissionWindow.Left = location.X;
                        usersToChangePermissionWindow.Top = location.Y;
                        usersToChangePermissionWindow.ResizeMode = ResizeMode.NoResize;
                        usersToChangePermissionWindow.WindowStyle = WindowStyle.None;
                        LinearGradientBrush lgb = new LinearGradientBrush();
                        lgb.StartPoint = new Point(1, 1);
                        lgb.EndPoint = new Point(0, 0);
                        lgb.GradientStops = (GradientStopCollection)FindResource("tlo");
                        usersToChangePermissionWindow.Background = lgb;
                        users.Background = lgb;
                        usersToChangePermissionWindow.Title = "Nadaj uprawnienia administracyjne";
                        users.SelectionChanged += wybierzUsera;
                        DBConn.instance.PrepareConnection();
                        MySqlCommand encmd = new MySqlCommand($"SELECT username FROM users where permission=1", DBConn.instance.conn);
                        DataTable dt = new DataTable();
                        dt.Load(encmd.ExecuteReader());
                        foreach (DataRow row in dt.Rows)
                        {
                            TextBlock t = new TextBlock();
                            t.Foreground = (SolidColorBrush)FindResource("napisy");
                            t.Text = row[0].ToString();
                            t.FontSize = 32;
                            users.Items.Add(t);
                        }
                        usersToChangePermissionWindow.ShowDialog();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }

        private void btnnadaj_Click(object sender, RoutedEventArgs e)
        {
            resign = false;
            ListBox users = new ListBox();
            usersToChangePermissionWindow.Content = users;
            usersToChangePermissionWindow.Width = 700;
            usersToChangePermissionWindow.Height = 400;
            var location = btnzrezygnuj.PointToScreen(new Point(0, 0));
            usersToChangePermissionWindow.Left = location.X;
            usersToChangePermissionWindow.Top = location.Y;
            usersToChangePermissionWindow.ResizeMode = ResizeMode.NoResize;
            usersToChangePermissionWindow.WindowStyle = WindowStyle.None;
            LinearGradientBrush lgb = new LinearGradientBrush();
            lgb.StartPoint = new Point(1, 1);
            lgb.EndPoint = new Point(0, 0);
            lgb.GradientStops = (GradientStopCollection)FindResource("tlo");
            usersToChangePermissionWindow.Background = lgb;
            users.Background = lgb;
            usersToChangePermissionWindow.Title = "Wybierz użytkownika, który odziedziczy uprawnienia administratora";
            users.SelectionChanged += wybierzUsera;
            DBConn.instance.PrepareConnection();
            MySqlCommand encmd = new MySqlCommand($"SELECT username, permission FROM users where permission=1 or permission=2", DBConn.instance.conn);
            DataTable dt = new DataTable();
            dt.Load(encmd.ExecuteReader());
            foreach (DataRow row in dt.Rows)
            {
                TextBlock t = new TextBlock();
                t.Foreground = (SolidColorBrush)FindResource("napisy");
                t.Name = "u" + (int)row[1];
                t.Text = row[0].ToString();
                if (t.Text == DBConn.instance.currentUser.username)
                    continue;

                if ((int)row[1] == 1)
                    t.Text += " - nadaj uprawnienia administratora";
                else
                    t.Text += " - usuń uprawnienia administratora";

                t.FontSize = 32;
                users.Items.Add(t);
            }
            usersToChangePermissionWindow.ShowDialog();
        }

        private void btnusun_Click(object sender, RoutedEventArgs e)
        {
            if(DBConn.instance.currentUser.permission == 3)
                System.Windows.MessageBox.Show("Jako główny administrator nie możesz usunąć swojego konta", "Usuwanie konta", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            else
            {
                MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć swoje konto?", "Usuwanie konta", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        DBConn.instance.PrepareConnection();
                        MySqlCommand a = new MySqlCommand($"DELETE FROM users WHERE username = '{DBConn.instance.currentUser.username}'", DBConn.instance.conn);
                        a.ExecuteNonQuery();
                        DBConn.instance.Logout();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }

        private void btnTaC_Click(object sender, RoutedEventArgs e)
        {
            TermsAndConditions TaC = new TermsAndConditions();
            TaC.Show();
        }
    }
}
