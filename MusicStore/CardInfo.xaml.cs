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
    /// Interaction logic for CreditInfo.xaml
    /// </summary>
    public partial class CardInfo : Window
    {
        public CardInfo()
        {
            InitializeComponent();
            LoadCardInfo();
        }
        private void LoadCardInfo()
        {
            nrcardtxt.Text = DBConn.instance.currentUser.creditInfo[0];
            cvvtxt.Text = DBConn.instance.currentUser.creditInfo[1];
            datetxt.Text = DBConn.instance.currentUser.creditInfo[2];
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //If(poprawnie wypełnione pola)
            //Prześlij nowe dane karty użytkownika do bazy
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
