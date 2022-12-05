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
            //Set Admin Layout
            if(DBConn.currentUser.permission>=2)
            {
                OpenStudioDetailsButton.IsEnabled = true;
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
            bannerSetup.ShowDialog();
        }
    }
}
