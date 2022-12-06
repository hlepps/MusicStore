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
using System.Diagnostics;

namespace MusicStore
{
    /// <summary>
    /// Logika interakcji dla klasy DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        bool a = false;
        private void Style_Click(object sender, RoutedEventArgs e)
        {
            string resourcePath;
            if (a)
            {
                resourcePath = "Style/Darkmode.xaml";
                a = false;
            }
            else
            {
                resourcePath = "Style/Lightmode.xaml";
                a = true;
            }
            App.application.Resources = (ResourceDictionary)Application.LoadComponent(new Uri(resourcePath, UriKind.Relative));
        }
    }
}
