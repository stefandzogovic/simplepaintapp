using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Grafika_PZ1.Polygon
{
    /// <summary>
    /// Interaction logic for PolgygonWindow.xaml
    /// </summary>
    public partial class PolgygonWindow : Window
    {
        private MainWindow mainwindow;
        private System.Windows.Shapes.Polygon currentPolygon;
        public double X;
        public double Y;

        public MainWindow Mainwindow { get => mainwindow; set => mainwindow = value; }
        public System.Windows.Shapes.Polygon CurrentPolygon { get => currentPolygon; set => currentPolygon = value; }
        private bool bul = false;
        public PolgygonWindow(double x, double y, MainWindow mainwindow, string draworchange)
        {
            InitializeComponent();

            X = x;
            Y = y;

            Mainwindow = mainwindow;
            BorderThickness.PreviewTextInput += Common.NumberValidationTextBox;

            Common.FillColorComboBox(FillColor, BorderColor);


            if (draworchange == "draw")
                DrawBtn.Click += ButtonDraw;
            else
                DrawBtn.Click += ButtonChange;
        }

        private void FillColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem cbi = (ComboBoxItem)FillColor.SelectedItem;
            FillColorRect.Fill = cbi.Background;
        }

        private void BorderColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem cbi = (ComboBoxItem)BorderColor.SelectedItem;
            BorderColorRect.Fill = cbi.Background;
        }

        private void ButtonCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonDraw(object sender, RoutedEventArgs e)
        {

            System.Windows.Shapes.Polygon poly = new System.Windows.Shapes.Polygon();
            poly.MouseLeftButtonDown += ChangePolyMouseButtonDown;
            poly.Fill = FillColorRect.Fill;

            poly.Stroke = BorderColorRect.Fill;
            poly.StrokeThickness = Int64.Parse(BorderThickness.Text);

            foreach(Shape sh in Mainwindow.MyCanvas.Children)
            {
                 if(sh.Name == "enabled")
                {
                    Path p = (Path)sh;
                    EllipseGeometry eg = (EllipseGeometry)p.Data;
                    p.Name = "disabled";
                    poly.Points.Add(eg.Center);
                }
            }

            UndoRedo.ChangeRepresentationObject newrect = new UndoRedo.ChangeRepresentationObject();
            newrect.Action = UndoRedo.ActionType.Insert;
            newrect.shape = poly;

            Mainwindow.MyCanvas.Children.Add(poly);
            Mainwindow.Undoredo.InsertObjectforUndoRedo(newrect);

            this.Close();
        }


        private void ButtonChange(object sender, RoutedEventArgs e)
        {
            UndoRedo.ChangeRepresentationObject changedobject = new UndoRedo.ChangeRepresentationObject();

            changedobject.shape = CurrentPolygon;
            changedobject.brush = CurrentPolygon.Fill;
            changedobject.Action = UndoRedo.ActionType.Edit;
            changedobject.brushborder = CurrentPolygon.Stroke;
            changedobject.borderThickness = CurrentPolygon.StrokeThickness;

            CurrentPolygon.Fill = FillColorRect.Fill;
            CurrentPolygon.Stroke = BorderColorRect.Fill;
            CurrentPolygon.StrokeThickness = Int64.Parse(BorderThickness.Text);

            Mainwindow.Undoredo.InsertObjectforUndoRedo(changedobject);
            this.Close();
        }

        private void ChangePolyMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickedrectangle = e.Source as System.Windows.Shapes.Polygon;

            CurrentPolygon = clickedrectangle;
            PolgygonWindow windowChange = new PolgygonWindow(0, 0, Mainwindow, "change");
            windowChange.CurrentPolygon = CurrentPolygon;
            windowChange.Title = "Change Polygon Window";
            windowChange.DrawBtn.Content = "Change";
            windowChange.BorderThickness.Text = CurrentPolygon.StrokeThickness.ToString();
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Background = CurrentPolygon.Fill;
            Color color = ((SolidColorBrush)CurrentPolygon.Fill).Color;
            string selectedcolorname = "";

            foreach (var colorvalue in typeof(Colors).GetRuntimeProperties())
            {
                if ((Color)colorvalue.GetValue(null) == color)
                {
                    selectedcolorname = colorvalue.Name;
                    break;
                }
            }
            cbi.Content = selectedcolorname;

            foreach (ComboBoxItem item in FillColor.Items)
                if (item.Content.ToString() == selectedcolorname)
                {
                    int x = FillColor.Items.IndexOf(item);
                    windowChange.FillColor.SelectedIndex = x;
                    break;
                }


            cbi = new ComboBoxItem();
            cbi.Background = CurrentPolygon.Stroke;
            color = ((SolidColorBrush)CurrentPolygon.Stroke).Color;
            selectedcolorname = "";
            foreach (var colorvalue in typeof(Colors).GetRuntimeProperties())
            {
                if ((Color)colorvalue.GetValue(null) == color)
                {
                    selectedcolorname = colorvalue.Name;
                    break;
                }
            }
            cbi.Content = selectedcolorname;

            foreach (ComboBoxItem item in BorderColor.Items)
                if (item.Content.ToString() == selectedcolorname)
                {
                    int x = BorderColor.Items.IndexOf(item);
                    windowChange.BorderColor.SelectedIndex = x;
                    break;
                }

            Common.MoveWindowToCursor(windowChange, Mainwindow);
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = e.Source as System.Windows.Controls.TextBox;
            if (this.IsLoaded)
            {
                if (textbox.Text == "" || textbox.Text == "0")
                {
                    if (!bul)
                    {
                        bul = true;
                        DrawBtn.IsEnabled = false;

                    }
                }
                else
                {
                    if (bul)
                    {
                        DrawBtn.IsEnabled = true;
                        bul = false;
                    }
                }
            }
        }
    }
}
