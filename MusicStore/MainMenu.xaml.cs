using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace MusicStore
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public static MainMenu instance;
        Pages.Library library = new Pages.Library();
        Pages.Options options = new Pages.Options();
        Pages.AccountSettings accountSettings = new Pages.AccountSettings();
        Pages.Authors authors = new Pages.Authors();

        int styl;
        public void InitMainMenu()
        {
            RefreshBannerFunction();
            RefreshUserInfo();
            //Set Admin Layout
            if (UserFunctions.VerifyUserPermission(2))
            {
                OpenStudioDetailsButton.IsEnabled = true;
            }
            else
            {
                OpenStudioDetailsButton.IsEnabled = false;
            }
            instance = this;

            DBConn.instance.PrepareConnection();
            MySqlCommand cmd = new MySqlCommand($"SELECT lastStyle, lastLanguage FROM users WHERE username='{DBConn.instance.currentUser.username}'", DBConn.instance.conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            styl = (int)rdr[0];
            string lang = (string)rdr[1];

            if (styl == 0)
            {
                MusicStore.Style.StyleManager.SetCurrentStyle("Style/Darkmode.xaml");
            }
            if (styl == 1)
            {
                MusicStore.Style.StyleManager.SetCurrentStyle("Style/Lightmode.xaml");
            }

            MusicStore.Style.StyleManager.UpdateStyle();
            MusicStore.Language.LangManager.SetCurrentLanguage(lang);
            MusicStore.Language.LangManager.UpdateLanguage();


        }
        public MainMenu()
        {
            InitializeComponent();
            
        }

        private void btnminimalize(object sender, RoutedEventArgs e)
        {

            WindowState = WindowState.Minimized;
        }

        private void btnclose(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OpenStudioDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            BannerSetup bannerSetup = new BannerSetup();
            bannerSetup.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            bannerSetup.Height = (System.Windows.SystemParameters.PrimaryScreenHeight) / 2;
            bannerSetup.Width = (System.Windows.SystemParameters.PrimaryScreenWidth) / 2;
            bannerSetup.instance = this;
            bannerSetup.ShowDialog();
        }

        public void RefreshBannerFunction()
        {
            DB.DBConfig.RefreshConfig();
            StudioName.Text = DB.DBConfig.studioName;
            StudioLogo.ImageSource = DB.DBConfig.studioLogo.bitmap;
            BannerBackground.ImageSource = DB.DBConfig.studioBanner.bitmap;
        }

        public void RefreshUserInfo()
        {
            //Set current username text
            UsernameTextBlock.Text = DBConn.instance.currentUser.username;
            //Set permission type text
            switch(DBConn.instance.currentUser.permission)
            {
                case 0:
                    UsertypeTextBlock.Text = "Guest";
                    break;
                case 1:
                    UsertypeTextBlock.Text = "User";
                    break;
                case 2:
                    UsertypeTextBlock.Text = "Administartor";
                    break;
                case 3:
                    UsertypeTextBlock.Text = "Headadmin";
                    break;
            }
            //Set current funds text in PLN
            UserFundsTextBlock.Text = DBConn.instance.currentUser.wallet + " PLN";
            UserAvatar.ImageSource = DBConn.instance.currentUser.avatar.bitmap;
        }

        private void Btnbiblioteka_Click(object sender, RoutedEventArgs e)
        {
            library.pageMode = Pages.Library.Mode.UserLibrary;
            library.RefreshPage();
            mainFrame.Content = library;
        }

        private void Btnsklep_Click(object sender, RoutedEventArgs e)
        {
            //mainFrame.Content = shop;
            library.pageMode = Pages.Library.Mode.Shop;
            library.RefreshPage();
            mainFrame.Content = library;
        }

        private void Btnopcje_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = options;
            if (styl == 0)
            {
                options.UpdateCheckbox(false);
            }
            if (styl == 1)
            {
                options.UpdateCheckbox(true);
            }
        }
        private void Btnuser_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = accountSettings;
        }

        private void Btnauthor_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = authors;
        }

        private void btnlogout_Click(object sender, RoutedEventArgs e)
        {
            DBConn.instance.Logout();
        }
    }
}
