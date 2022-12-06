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
    /// Interaction logic for BannerSetup.xaml
    /// </summary>
    public partial class BannerSetup : Window
    {
        public BannerSetup()
        {
            InitializeComponent();
            //Wyciągnięcie aktualnej nazwy Studia do wartości StudioNameInputTextBox
            //StudioNameInputTextBox.Text=DBConn.
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyChangesButton_Click(object sender, RoutedEventArgs e)
        {
            //Send new info to database;
            //Run function to update banner;
            this.Close();
        }

        private void ImportLogoButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
