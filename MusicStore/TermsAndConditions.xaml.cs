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
using System.IO;

namespace MusicStore
{
    /// <summary>
    /// Interaction logic for TermsAndConditions.xaml
    /// </summary>
    public partial class TermsAndConditions : Window
    {
        string content;
        public TermsAndConditions()
        {
            InitializeComponent();
            int nWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
            int nHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
            this.LayoutTransform = new ScaleTransform(nWidth * 0.8, nHeight * 0.25);
            switch (MusicStore.Language.LangManager.GetCurrentLanguage())
            {
                case "Language/Chinese.xaml":
                    content = File.ReadAllText("Resources/TermsAndConditions/LoremIpsumChinese.txt");
                    break;
                case "Language/English.xaml":
                    content = File.ReadAllText("Resources/TermsAndConditions/LoremIpsumEnglish.txt");
                    break;
                case "Language/Polski.xaml":
                    content = File.ReadAllText("Resources/TermsAndConditions/LoremIpsumPolski.txt");
                    break;
                default:
                    content = File.ReadAllText("Resources/TermsAndConditions/LoremIpsumEnglish.txt");
                    break;
            }
            ContentTextBlock.Text = content;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
