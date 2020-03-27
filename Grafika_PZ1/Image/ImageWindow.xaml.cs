using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Grafika_PZ1.Image
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        private MainWindow mainwindow;
        public double X;
        public double Y;

        public MainWindow Mainwindow { get => mainwindow; set => mainwindow = value; }
        public static ImageBrush ClickedImageBrush;
        public static System.Windows.Shapes.Rectangle ClickedRect;
        private bool bul = false;

        public ImageWindow(double x, double y, MainWindow mainwindow)
        {
            X = x;
            Y = y;
            Mainwindow = mainwindow;
            InitializeComponent();

            Width.PreviewTextInput += Common.NumberValidationTextBox;
            Height.PreviewTextInput += Common.NumberValidationTextBox;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Img.ImageSource = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void ButtonCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DrawBtn_Click(object sender, RoutedEventArgs e)
        {

            ImageBrush background = new ImageBrush();
            background.ImageSource = Img.ImageSource;
            if (background.ImageSource == null)
            {
                System.Windows.Forms.MessageBox.Show("Image is empty");


            }
            else
            {
                System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
                rect.Width = Int64.Parse(Width.Text);
                rect.Height = Int64.Parse(Height.Text);

                rect.Fill = background;
                rect.MouseLeftButtonDown += ChangeImage;
                Canvas.SetTop(rect, Y);
                Canvas.SetLeft(rect, X);
                UndoRedo.ChangeRepresentationObject newrect = new UndoRedo.ChangeRepresentationObject();
                newrect.Action = UndoRedo.ActionType.Insert;
                newrect.shape = rect;

                Mainwindow.MyCanvas.Children.Add(rect);
                Mainwindow.Undoredo.InsertObjectforUndoRedo(newrect);
                ClickedImageBrush = background;
                ClickedRect = rect;
                this.Close();
            }
        }

        private void ChangeImage(object sender, RoutedEventArgs e)
        {
            var clickedImage = e.Source as System.Windows.Shapes.Rectangle;

            ClickedImageBrush = (ImageBrush)clickedImage.Fill;


            ChangeImageWindow changewindow = new ChangeImageWindow(this, Mainwindow);
            Common.MoveWindowToCursor(changewindow, Mainwindow);
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
