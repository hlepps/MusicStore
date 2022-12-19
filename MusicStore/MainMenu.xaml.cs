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

namespace MusicStore
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public MainMenu()
        {
            InitializeComponent();
            RefreshBannerFunction();
            RefreshUserInfo();
            //Set Admin Layout
            if (DBConn.instance.currentUser.permission >= 2)
            {
                OpenStudioDetailsButton.IsEnabled = true;
            }
            else
            {
                OpenStudioDetailsButton.IsEnabled = false;
            }
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
            StudioLogo.Source = DB.DBConfig.studioLogo.bitmap;
            BannerBackground.Source = DB.DBConfig.studioBanner.bitmap;
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
        }
    }
}
