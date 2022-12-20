using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicStore.Style
{
    public class StyleManager
    {
        public static List<string> styles;
        private static string currentStyle = "Style/Startowy.xaml";
        public static void RefreshStyles()
        {
            styles = new List<string>();
            string[] langs = System.IO.Directory.GetFiles("Style/");
            foreach (string lang in langs)
            {
                styles.Add(lang);
            }
        }

        public static void UpdateStyle()
        {
            App.application.Resources = (ResourceDictionary)Application.LoadComponent(new Uri(currentStyle, UriKind.Relative));
            App.application.Resources.MergedDictionaries.Add((ResourceDictionary)Application.LoadComponent(new Uri(Language.LangManager.GetCurrentLanguage(), UriKind.Relative)));
        }


        public static void SetCurrentStyle(string _style)
        {
            currentStyle = _style;
        }

        public static string GetCurrentStyle()
        {
            return currentStyle;
        }
    }
}
