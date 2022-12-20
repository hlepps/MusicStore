using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicStore.Language
{
    public class LangManager
    {
        public static List<string> languages;
        private static string currentLanguage = "Style/Startowy.xaml";
        public static void RefreshLanguages()
        {
            languages = new List<string>();
            string[] langs = System.IO.Directory.GetFiles("Language/");
            foreach(string lang in langs)
            {
                languages.Add(lang);
            }
        }

        public static void UpdateLanguage()
        {
            App.application.Resources = (ResourceDictionary)Application.LoadComponent(new Uri(currentLanguage, UriKind.Relative));
            App.application.Resources.MergedDictionaries.Add((ResourceDictionary)Application.LoadComponent(new Uri(Style.StyleManager.GetCurrentStyle(), UriKind.Relative)));
        }

        public static void SetCurrentLanguage(string lang)
        {
            currentLanguage = lang;
        }

        public static string GetCurrentLanguage()
        {
            return currentLanguage;
        }
    }
}
