using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Control = System.Windows.Forms.Control;

namespace Grafika_PZ1
{
    /// <summary>
    /// Interaction logic for ElipseWindow.xaml
    /// </summary>
    public partial class ElipseWindow : Window
    {
        private MainWindow mainwindow;
        private Path currentEllipse;
        public double X;
        public double Y;

        public MainWindow Mainwindow { get => mainwindow; set => mainwindow = value; }
        public Path CurrentEllipse { get => currentEllipse; set => currentEllipse = value; }
        private bool bul = false;


        public ElipseWindow(double x, double y, MainWindow mainwindow, string draworchange)
        {
            InitializeComponent();
            X = x;
            Y = y;

            Mainwindow = mainwindow;

            Width.PreviewTextInput += Common.NumberValidationTextBox;
            Height.PreviewTextInput += Common.NumberValidationTextBox;
            BorderThickness.PreviewTextInput += Common.NumberValidationTextBox;


            if (draworchange == "draw")
                DrawBtn.Click += ButtonDraw;
            else
                DrawBtn.Click += ButtonChange;

            Common.FillColorComboBox(FillColor, BorderColor);
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

        private void ButtonDraw(object sender, RoutedEventArgs e)
        {

            System.Windows.Shapes.Ellipse eg= new System.Windows.Shapes.Ellipse();
            Path path = new Path();
            path.MouseLeftButtonDown += ChangeEllipseMouseButtonDown;
            path.Fill = FillColorRect.Fill;

            path.Stroke = BorderColorRect.Fill;
            path.StrokeThickness = Int64.Parse(BorderThickness.Text);

            EllipseGeometry egeometry = new EllipseGeometry();
            egeometry.RadiusX = Int64.Parse(Width.Text);
            egeometry.RadiusY = Int64.Parse(Height.Text);
            egeometry.Center = new Point(X, Y);

            path.Data = egeometry;


            UndoRedo.ChangeRepresentationObject newrect = new UndoRedo.ChangeRepresentationObject();
            newrect.Action = UndoRedo.ActionType.Insert;
            newrect.shape = path;

            Mainwindow.MyCanvas.Children.Add(path);
            Mainwindow.Undoredo.InsertObjectforUndoRedo(newrect);

            this.Close();
        }

        private void ButtonCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonChange(object sender, RoutedEventArgs e)
        {

            UndoRedo.ChangeRepresentationObject changedobject = new UndoRedo.ChangeRepresentationObject();

            EllipseGeometry egellipse = new EllipseGeometry();
            egellipse = (EllipseGeometry)CurrentEllipse.Data;
            changedobject.shape = CurrentEllipse;
            changedobject.brush = CurrentEllipse.Fill;
            changedobject.height = egellipse.RadiusY;
            changedobject.Width = egellipse.RadiusX;
            changedobject.Action = UndoRedo.ActionType.Edit;
            changedobject.brushborder = CurrentEllipse.Stroke;
            changedobject.borderThickness = currentEllipse.StrokeThickness;

            CurrentEllipse.Fill = FillColorRect.Fill;
            CurrentEllipse.Stroke = BorderColorRect.Fill;
            CurrentEllipse.StrokeThickness = Int64.Parse(BorderThickness.Text);
            EllipseGeometry temp = (EllipseGeometry)CurrentEllipse.Data;

            EllipseGeometry eg = new EllipseGeometry();
            eg.RadiusX = Int64.Parse(Width.Text);
            eg.RadiusY = Int64.Parse(Height.Text);
            eg.Center = temp.Center;

            CurrentEllipse.Data = eg;



            Mainwindow.Undoredo.InsertObjectforUndoRedo(changedobject);

            this.Close();
        }

        private void ChangeEllipseMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickedellipse = e.Source as Path;
            EllipseGeometry tempeometry = (EllipseGeometry)clickedellipse.Data;

            CurrentEllipse = clickedellipse;
            ElipseWindow windowChange = new ElipseWindow(0, 0, Mainwindow, "change");
            windowChange.CurrentEllipse = CurrentEllipse;
            windowChange.Title = "Change Elipse Window";
            windowChange.DrawBtn.Content = "Change";
            windowChange.Width.Text = tempeometry.RadiusX.ToString();
            windowChange.Height.Text = tempeometry.RadiusY.ToString();
            windowChange.BorderThickness.Text = CurrentEllipse.StrokeThickness.ToString();
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Background = CurrentEllipse.Fill;
            Color color = ((SolidColorBrush)CurrentEllipse.Fill).Color;
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

            foreach (ComboBoxItem item in  FillColor.Items)
                if (item.Content.ToString() == selectedcolorname)
                {
                    int x = FillColor.Items.IndexOf(item);
                    windowChange.FillColor.SelectedIndex = x;
                    break;
                }


            cbi = new ComboBoxItem();
            cbi.Background = CurrentEllipse.Stroke;
            color = ((SolidColorBrush)CurrentEllipse.Stroke).Color;
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

        private void TextChanged1(object sender, TextChangedEventArgs e)
        {
            var textbox = e.Source as System.Windows.Controls.TextBox;
            if (this.IsLoaded)
            {
                if (textbox.Text == "")
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
