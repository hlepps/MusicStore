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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicStore.Pages
{
    /// <summary>
    /// Interaction logic for Authors.xaml
    /// </summary>
    public partial class Authors : Page
    {
        public Authors()
        {
            InitializeComponent();
            ReloadAuthors();
        }

        public void ReloadAuthors()
        {
            AuthorStackPanel.Children.Clear();
            //Load Authors List
            //foreach Author in List -> Spawn Button from template in xaml
        }

        private int GetIDFromObjectName(string name)
        {
            string s = name.Remove(0, 1);
            return Int32.Parse(s);
        }

        private void EditAuthor_Click(object sender, RoutedEventArgs e)
        {
            if(UserFunctions.VerifyUserPermission(2))
            {
                AuthorManager authorManager = new AuthorManager();
                authorManager.artistID = GetIDFromObjectName(((Button)sender).Name);
                authorManager.ReloadWindow();
                authorManager.ShowDialog();
            }
        }
    }
}
