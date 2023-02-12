using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Logika interakcji dla klasy Options.xaml
    /// </summary>
    public partial class Options : Page
    {
        public Options()
        {
            InitializeComponent(); 
            
            jezykbox.Items.Clear();
            MusicStore.Language.LangManager.RefreshLanguages();
            foreach (string lang in MusicStore.Language.LangManager.languages)
            {
                jezykbox.Items.Add(lang.Remove(lang.Length - 5).Remove(0, 9));
            }
        }

        bool a = true;
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            int stl;
            if (a)
            {
                MusicStore.Style.StyleManager.SetCurrentStyle("Style/Darkmode.xaml");
                a = false;
                stl = 0;
            }
            else
            {
                MusicStore.Style.StyleManager.SetCurrentStyle("Style/Lightmode.xaml");
                a = true;
                stl = 1;
            }
            MusicStore.Style.StyleManager.UpdateStyle();
            string sql = $"UPDATE users SET lastStyle = '{stl}' WHERE username = '{DBConn.instance.currentUser.username}'";

            MySqlCommand cmd = new MySqlCommand(sql, DBConn.instance.conn);
            DBConn.instance.PrepareConnection();
            cmd.ExecuteNonQuery();
        }
        private void Jezykbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MusicStore.Language.LangManager.SetCurrentLanguage(MusicStore.Language.LangManager.languages[jezykbox.Items.IndexOf(jezykbox.SelectedValue)]);
            MusicStore.Language.LangManager.UpdateLanguage();
            Trace.WriteLine(MusicStore.Language.LangManager.GetCurrentLanguage()); 


            string sql = $"UPDATE users SET lastLanguage = '{MusicStore.Language.LangManager.languages[jezykbox.Items.IndexOf(jezykbox.SelectedValue)]}' WHERE username = '{DBConn.instance.currentUser.username}'";

            MySqlCommand cmd = new MySqlCommand(sql, DBConn.instance.conn);
            DBConn.instance.PrepareConnection();
            cmd.ExecuteNonQuery();
        }
    }
}
