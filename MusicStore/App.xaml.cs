using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using System.Text;

namespace MusicStore
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            // tworzenie instancji polaczenia z baza danych
            DBConn dbconn = new DBConn();
            // instancja jest dostepna globalnie w DBConn.instance aby nie laczyc
            // sie za kazdym razem z baza tylko utrzymywac jedno polaczenie


            // otwieranie pierwszego okna - login - w trybie "bezpiecznym", 
            // czyli po wywaleniu jakiegokolwiek bledu aplikacja sie nie wywali
            // klasycznie tylko zarejestruje blad, wyswietli skrot bledu w okienku
            // i zapisze logi w pliku, pod warunkiem kompilowania w trybie 'release'
            // 
            // pomocne gdy podczas uruchamiania aplikacji poza visual studio
            // wywali błąd - mamy wtedy informacje co i gdzie sie wywaliło
            Application a = new Application();
            a.StartupUri = new Uri("Login.xaml", System.UriKind.Relative);
#if DEBUG
            a.Run();
#else
            try
            {
                a.Run();
            }
            catch (Exception exc)
            {
                FileStream file = File.OpenWrite(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + "\\" + DateTime.Now.ToString().Replace(':', ' ') + ".error");
                string message = exc.Message + "\n\n" + exc.StackTrace;
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                file.Write(bytes, 0, bytes.Length);
                MessageBox.Show(exc.Message + "\n\n" + exc.StackTrace.Split('\n')[0], "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
#endif
        }
    }
}
