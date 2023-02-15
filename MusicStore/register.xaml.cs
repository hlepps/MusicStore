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
    /// Logika interakcji dla klasy register.xaml
    /// </summary>
    public partial class register : Window
    {
        public register()
        {
            InitializeComponent();
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
            this.Close();
        }

        private void btnnowekonto_click(object sender, RoutedEventArgs e)
        {           
            if (logintxt.Text != "" &&
                haslotxt.Password != "" &&
                powtorzhaslotxt.Password == haslotxt.Password &&
                nrcardtxt.Text != "" &&
                cvvtxt.Text != "" &&
                datetxt.Text != "" &&
                TaCCheckBox.IsChecked.Value
                )
            {
                if(DBConn.instance.Register(logintxt.Text, haslotxt.Password, nrcardtxt.Text, cvvtxt.Text, datetxt.Text))
                {
                    /*
                    if (DBConn.instance.Login(logintxt.Text, haslotxt.Password))
                    {
                        DB.DBAlbumsSaved.PreloadAll();
                        DB.DBSongsSaved.PreloadAll();
                        MainMenu menu = new MainMenu();
                        menu.Show();
                        this.Hide();
                    }
                    */
                    this.Close();
                }

            }
            else
            {
                System.Windows.MessageBox.Show("Please enter every needed information", "Login", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void cardDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            string txt = cardDate.SelectedDate.Value.Month.ToString("00") + "/" + cardDate.SelectedDate.Value.Year.ToString()[2] + cardDate.SelectedDate.Value.Year.ToString()[3];
            datetxt.Text = txt;
        }

        private void OpenTaC_Click(object sender, RoutedEventArgs e)
        {
            TermsAndConditions TaC = new TermsAndConditions();
            TaC.Show();
        }
    }
}
