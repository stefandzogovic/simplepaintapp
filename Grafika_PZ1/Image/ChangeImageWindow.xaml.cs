using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for ChangeImageWindow.xaml
    /// </summary>
    public partial class ChangeImageWindow : Window
    {
        private ImageWindow imageWindow;
        private MainWindow mainWindow;
        private bool bul = false;

        public ChangeImageWindow(ImageWindow imagewindow, MainWindow mainwindow)
        {

            InitializeComponent();
            Img.ImageSource = ImageWindow.ClickedImageBrush.ImageSource;

            MainWindow = mainwindow;
        }
        private void Change(object sender, RoutedEventArgs e)
        {

        }
        private void ButtonCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChangeBtn_Click(object sender, RoutedEventArgs e)
        {

            ImageBrush background = new ImageBrush();
            background.ImageSource = ImageWindow.ClickedImageBrush.ImageSource;
            ImageWindow.ClickedImageBrush.ImageSource = Img.ImageSource;
            UndoRedo.ChangeRepresentationObject changedobject = new UndoRedo.ChangeRepresentationObject();
            changedobject.shape = ImageWindow.ClickedRect;
            changedobject.brush = background;
            changedobject.height = ImageWindow.ClickedRect.ActualHeight;
            changedobject.Width = ImageWindow.ClickedRect.ActualWidth;
            changedobject.Action = UndoRedo.ActionType.Edit;
            MainWindow.Undoredo.InsertObjectforUndoRedo(changedobject);

            this.Close();
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
        public MainWindow MainWindow { get => mainWindow; set => mainWindow = value; }
        public ImageWindow ImageWindow { get => imageWindow; set => imageWindow = value; }


       
    }
}
