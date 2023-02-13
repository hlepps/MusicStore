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
using Microsoft.Win32;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Asn1.Ocsp;

namespace MusicStore
{

    public class ObjectGenerationHelper
    {
        public static Grid GetAuthorEmptyGrid()
        {
            Grid grid = new Grid();
            grid.Margin = new Thickness(0.5);
            var temp = new ColumnDefinition();
            temp.Width = new GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(temp);
            var temp2 = new ColumnDefinition();
            temp2.Width = new GridLength(9, GridUnitType.Star);
            grid.ColumnDefinitions.Add(temp2);
            return grid;
        }

        public static void RemoveParent(DependencyObject child)
        {
            var parent = VisualTreeHelper.GetParent(child);
            var parentAsPanel = parent as Panel;
            if (parentAsPanel != null)
            {
                parentAsPanel.Children.Remove((UIElement)child);
            }
            var parentAsContentControl = parent as ContentControl;
            if (parentAsContentControl != null)
            {
                parentAsContentControl.Content = null;
            }
            var parentAsDecorator = parent as Decorator;
            if (parentAsDecorator != null)
            {
                parentAsDecorator.Child = null;
            }
        }
    }
}
