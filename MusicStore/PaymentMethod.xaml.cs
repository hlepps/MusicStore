﻿using System;
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
    /// Interaction logic for PaymentMethod.xaml
    /// </summary>
    public partial class PaymentMethod : Window
    {
        public double itemPrice;
        public PaymentMethod()
        {
            InitializeComponent();
            int nWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            int nHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
            this.LayoutTransform = new ScaleTransform(nWidth * 0.2, nHeight * 0.2);
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
                fundsCheck.Text = "Total after: " + tmp + " PLN";
            }
            else
            {
                fundsCheck.Text = "NOT ENOUGH FUNDS";
            }
        }

        private void LoadCardInfo()
        {
            string tmpcardNumber="XXXX XXXX XXXX ";
            tmpcardNumber = tmpcardNumber + DBConn.instance.currentUser.creditInfo[0].Substring(12);
            cardNumber.Text = tmpcardNumber;
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            if(RadioButtonWallet.IsChecked.Value)
            {
                double tmp = DBConn.instance.currentUser.wallet - itemPrice;
                if (tmp >= 0)
                {
                    //Remove funds from account, assign item to user library
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("Payment succesful.");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.Append("This item has been assigned to your Library and funds had been withdrawn.");
                    MessageBox.Show(stringBuilder.ToString(), "Payment Succesful", MessageBoxButton.OK, MessageBoxImage.Information);
                    MusicStore.MainMenu.instance.library.RefreshPage();
                    this.Close();
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("Not enough funds for selected item.");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.Append("Please select another form of payment.");
                    MessageBox.Show(stringBuilder.ToString(), "Payment Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if(RadioButtonCard.IsChecked.Value)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Current version does not support paying by Payment Cards.");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.Append("Please select another form of payment.");
                MessageBox.Show(stringBuilder.ToString(), "Payment Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EditCard_Click(object sender, RoutedEventArgs e)
        {
            CardInfo cardInfo = new CardInfo();
            if ((bool)cardInfo.ShowDialog())
            {

            }
            LoadCardInfo();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
