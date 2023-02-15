using MusicStore.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicStore
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow instance;
        public MainWindow()
        {
            InitializeComponent();
            instance = this;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnchowanie_click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnzamykanie_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            Close();
        }

        private void btnlogowanie_click(object sender, RoutedEventArgs e)
        {
            if (logintxt.Text != "" && haslotxt.Password != "")
            {
                Loading.instance.Show();
                System.Threading.Thread.Sleep(1000);
                if (DBConn.instance.Login(logintxt.Text, haslotxt.Password))
                {
                    DB.DBAlbumsSaved.PreloadAll();
                    DB.DBSongsSaved.PreloadAll();
                    MainMenu menu = new MainMenu();
                    menu.InitMainMenu();
                    menu.Show();
                    this.Hide();
                }
                Loading.instance.Hide();
            }
        }

        private void btnrejestracja_click(object sender, RoutedEventArgs e)
        {
            register nowyuser = new register();
            this.Hide();
            if((bool)nowyuser.ShowDialog())
            {

            }
            this.Show();
        }
    }
}
