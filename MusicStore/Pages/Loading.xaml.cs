using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MusicStore.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy Loading.xaml
    /// </summary>
    public partial class Loading : Window
    {
        public static Loading instance;
        public Loading tempWindow;
        Thread newWindowThread;
        public Loading()
        {
            InitializeComponent();
            instance = this;
        }
        public void NewWindowHandler()
        {
            try
            {
                newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
                newWindowThread.SetApartmentState(ApartmentState.STA);
                newWindowThread.IsBackground = true;
                newWindowThread.Start();
            }
            catch (Exception ex)
            {
                // logging it  
            }
        }

        private void ThreadStartingPoint()
        {
            try
            {
                tempWindow = new Loading();
                tempWindow.Show();
                System.Windows.Threading.Dispatcher.Run();
            }
            catch (Exception ex)
            {
                // logging it  
            }
        }
        public void CloseWindow()
        {
            tempWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(tempWindow.Close));
        }
        public void CloseWindowSafe()
        {
            try
            {
                tempWindow.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new ThreadStart(() =>
                {
                    tempWindow.Close();
                    if (tempWindow != null)
                    {
                    }
                }));

                tempWindow = null;
            }
            catch (Exception ex)
            {
                // log it  
                newWindowThread.Abort();
            }
        }
    }
}
