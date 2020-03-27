using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Grafika_PZ1
{
    public class Common
    {
        public static RoutedCommand MyCommand = new RoutedCommand();

        public static void MoveWindowToCursor(Window window, MainWindow mainwindow)
        {
            System.Drawing.Point point = System.Windows.Forms.Cursor.Position;
            var transform = PresentationSource.FromVisual(mainwindow).CompositionTarget.TransformFromDevice;
            var mouse = transform.Transform(GetMousePosition());
            window.Left = mouse.X - window.ActualWidth - 5;
            window.Top = mouse.Y - window.ActualHeight;
            window.ShowDialog();
        }


        private static System.Windows.Point GetMousePosition()
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            return new System.Windows.Point(point.X, point.Y);
        }

        public static void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public static void FillColorComboBox(ComboBox cb1, ComboBox cb2)
        {
            Type brushesType = typeof(System.Windows.Media.Brushes);

            var properties = brushesType.GetProperties(BindingFlags.Static | BindingFlags.Public);

            foreach (var prop in properties)
            {
                string name = prop.Name;
                SolidColorBrush brush = (SolidColorBrush)prop.GetValue(null, null);

                Color color = brush.Color;

                ComboBoxItem cbi1 = new ComboBoxItem();
                ComboBoxItem cbi2 = new ComboBoxItem();
                cbi2.Background = brush;
                cbi2.Content = name;
                cbi2.Name = name;
                cbi1.Name = name;
                cbi1.Background = brush;
                cbi1.Content = name;
                cb1.Items.Add(cbi1);
                cb2.Items.Add(cbi2);
            }

            cb1.SelectedIndex = 0;
            cb2.SelectedIndex = 0;

        }
    }
}
