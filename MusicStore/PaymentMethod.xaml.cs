using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
using System.Xml.Linq;

namespace MusicStore
{
    /// <summary>
    /// Interaction logic for PaymentMethod.xaml
    /// </summary>
    public partial class PaymentMethod : Window
    {
        public double itemPrice;
        public bool itsAlbum;
        public int id;
        public PaymentMethod()
        {
            InitializeComponent();
            int nWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            int nHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
            base.Width = nWidth * 0.2;
            base.Height = nHeight * 0.2;
        }

        public void RefreshWindow()
        {
            LoadWallet();
            LoadCardInfo();
        }

        private void LoadWallet()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            stringBuilder.Append(DBConn.instance.currentUser.wallet);
            stringBuilder.Append(" PLN}");
            walletValue.Text = stringBuilder.ToString();

            double tmp = DBConn.instance.currentUser.wallet - itemPrice;
            if(tmp>=0)
            {
                fundsCheck.Text = (string)FindResource("totalafter") + tmp + " PLN";
            }
            else
            {
                fundsCheck.Text = (string)FindResource("notenoughfunds");
            }
        }

        private void LoadCardInfo()
        {
            if(DBConn.instance.currentUser.creditInfo[0].Any())
            {
                string tmpcardNumber = "XXXX XXXX XXXX ";
                tmpcardNumber = tmpcardNumber + DBConn.instance.currentUser.creditInfo[0].Substring(12);
                cardNumber.Text = tmpcardNumber;
            }
            else
            {
                cardNumber.Text = (string)FindResource("nocardsaved");
            }           
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            if(RadioButtonWallet.IsChecked.Value)
            {
                double tmp = DBConn.instance.currentUser.wallet - itemPrice;
                if (tmp >= 0)
                {
                    MySqlCommand usr = new MySqlCommand($"UPDATE users SET library=@lib, wallet={tmp.ToString().Replace(',','.')} WHERE username='{DBConn.instance.currentUser.username}'", DBConn.instance.conn);
                    usr.Parameters.AddWithValue("lib", DBConn.instance.currentUser.GetRawLibrary() + "," + (itsAlbum ? "a" : "s") + id);
                    DBConn.instance.PrepareConnection();
                    usr.ExecuteNonQuery();


                    //Remove funds from account, assign item to user library
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append((string)FindResource("paymentsuccesfull")); 
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.Append((string)FindResource("thisitemhasbeen"));
                    MessageBox.Show(stringBuilder.ToString(), (string)FindResource("paymentsuccesfull"), MessageBoxButton.OK, MessageBoxImage.Information);
                    DBConn.instance.currentUser.RefreshInfo();
                    MainMenu.instance.library.RefreshPage();
                    MainMenu.instance.RefreshUserInfo();
                    this.Close();
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append((string)FindResource("notfundsselected")); 
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.Append((string)FindResource("pleaseselectanothercard")); 
                    MessageBox.Show(stringBuilder.ToString(), (string)FindResource("paymentfailed"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if(RadioButtonCard.IsChecked.Value)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append((string)FindResource("currentversionnot"));
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.Append((string)FindResource("pleaseselectanothercard"));
                MessageBox.Show(stringBuilder.ToString(), (string)FindResource("paymentfailed"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EditCard_Click(object sender, RoutedEventArgs e)
        {
            CardInfo cardInfo = new CardInfo();
            if ((bool)cardInfo.ShowDialog())
            {

            }
            DBConn.instance.currentUser.RefreshInfo();
            LoadCardInfo();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
