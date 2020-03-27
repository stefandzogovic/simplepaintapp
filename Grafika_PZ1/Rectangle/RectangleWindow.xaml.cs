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

namespace Grafika_PZ1.Rectangle
{
    /// <summary>
    /// Interaction logic for RectangleWindow.xaml
    /// </summary>
    public partial class RectangleWindow : Window
    {
        private MainWindow mainwindow;
        private System.Windows.Shapes.Rectangle currentRectangle;
        public double X;
        public double Y;

        public MainWindow Mainwindow { get => mainwindow; set => mainwindow = value; }
        public System.Windows.Shapes.Rectangle CurrentRectangle { get => currentRectangle; set => currentRectangle = value; }
        private bool bul = false;

        public RectangleWindow(double x, double y, MainWindow mainwindow, string draworchange)
        {

            InitializeComponent();

            X = x;
            Y = y;

            Mainwindow = mainwindow;

            Width.PreviewTextInput += Common.NumberValidationTextBox;
            Height.PreviewTextInput += Common.NumberValidationTextBox;
            BorderThickness.PreviewTextInput += Common.NumberValidationTextBox;

            Common.FillColorComboBox(FillColor, BorderColor);


            if (draworchange == "draw")
                DrawBtn.Click += ButtonDraw;
            else
                DrawBtn.Click += ButtonChange;

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

            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
            rect.Width = Int64.Parse(Width.Text);
            rect.Height = Int64.Parse(Height.Text);
            rect.Fill = FillColorRect.Fill;
            rect.Stroke = BorderColorRect.Fill;
            rect.StrokeThickness = Int64.Parse(BorderThickness.Text);
            rect.MouseLeftButtonDown += ChangeRect;
            Canvas.SetTop(rect, Y);
            Canvas.SetLeft(rect, X);

            UndoRedo.ChangeRepresentationObject newrect = new UndoRedo.ChangeRepresentationObject();
            newrect.Action = UndoRedo.ActionType.Insert;
            newrect.shape = rect;

            Mainwindow.MyCanvas.Children.Add(rect);
            Mainwindow.Undoredo.InsertObjectforUndoRedo(newrect);
            CurrentRectangle = rect;

            this.Close();
        }

        private void ChangeRect(object sender, RoutedEventArgs e)
        {
            var clickedrectangle = e.Source as System.Windows.Shapes.Rectangle;

            CurrentRectangle = clickedrectangle;
            RectangleWindow windowChange = new RectangleWindow(0, 0, Mainwindow, "change");
            windowChange.CurrentRectangle = CurrentRectangle;
            windowChange.Title = "Change Rectangle Window";
            windowChange.DrawBtn.Content = "Change";
            windowChange.Width.Text = CurrentRectangle.Width.ToString();
            windowChange.Height.Text = CurrentRectangle.Height.ToString();
            windowChange.BorderThickness.Text = CurrentRectangle.StrokeThickness.ToString();
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Background = CurrentRectangle.Fill;
            Color color = ((SolidColorBrush)CurrentRectangle.Fill).Color;
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
            cbi.Background = CurrentRectangle.Stroke;
            color = ((SolidColorBrush)CurrentRectangle.Stroke).Color;
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

        private void ButtonChange(object sender, RoutedEventArgs e)
        {
            UndoRedo.ChangeRepresentationObject changedobject = new UndoRedo.ChangeRepresentationObject();

            changedobject.shape = CurrentRectangle;
            changedobject.brush = CurrentRectangle.Fill;
            changedobject.height = CurrentRectangle.ActualHeight;
            changedobject.Width = CurrentRectangle.ActualWidth;
            changedobject.Action = UndoRedo.ActionType.Edit;
            changedobject.brushborder = CurrentRectangle.Stroke;
            changedobject.borderThickness = CurrentRectangle.StrokeThickness;

            CurrentRectangle.Fill = FillColorRect.Fill;
            CurrentRectangle.Stroke = BorderColorRect.Fill;
            CurrentRectangle.StrokeThickness = Int64.Parse(BorderThickness.Text);
            CurrentRectangle.Width = Int64.Parse(Width.Text);
            CurrentRectangle.Height = Int64.Parse(Height.Text);



            Mainwindow.Undoredo.InsertObjectforUndoRedo(changedobject);
            this.Close();
        }

        private void FillColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem cbi = (ComboBoxItem)FillColor.SelectedItem;
            FillColorRect.Fill = cbi.Background;
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
