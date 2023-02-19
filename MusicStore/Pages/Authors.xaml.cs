using MusicStore.DB;
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
            DBAuthorsSaved.GetAll();
            foreach(DB.DBAuthor a in DB.DBAuthorsSaved.dictionary.Values)
            {
                Button b = new Button();
                b.Name = "a" + a.id;
                b.Margin = new Thickness(10);
                b.Click += EditAuthor_Click;
                b.Style = (System.Windows.Style)FindResource("button1");
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.Margin = new Thickness(5);
                Image i = new Image();
                i.Source = a.image.bitmap;
                i.Width = 54;
                i.Height = 54;
                i.HorizontalAlignment = HorizontalAlignment.Left;
                i.VerticalAlignment = VerticalAlignment.Center;
                i.Margin = new Thickness(0, 0, 10, 0);
                sp.Children.Add(i);
                TextBlock tb = new TextBlock();
                tb.Text = a.name;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.Foreground = (SolidColorBrush)FindResource("tekstAutorow");
                tb.FontWeight = FontWeights.Bold;
                tb.FontSize = 36;
                sp.Children.Add(tb);
                b.Content = sp;
                AuthorStackPanel.Children.Add(b);
            }
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
