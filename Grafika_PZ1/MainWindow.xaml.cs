
using Grafika_PZ1.Image;
using Grafika_PZ1.Polygon;
using Grafika_PZ1.Rectangle;
using Grafika_PZ1.UndoRedo;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grafika_PZ1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class Tuple
    {
        private bool bul;
        private Button btn;

        public Tuple(Button btn, bool bul)
        {
            Btn = btn;
            Bul = bul;
        }

        public Button Btn { get => btn; set => btn = value; }
        public bool Bul { get => bul; set => bul = value; }
    }


    public partial class MainWindow : Window
    {
        private UndoRedo.UndoRedo undoredo;
        public List<Tuple> btns = new List<Tuple>();
        private double x;
        private double y;
        public List<Shape> dots = new List<Shape>();
        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public UndoRedo.UndoRedo Undoredo { get => undoredo; set => undoredo = value; }

        public static RoutedCommand MyCommand = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();
            MyCommand.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control));
            MyCommand.InputGestures.Add(new KeyGesture(Key.Y, ModifierKeys.Control));
            btns.Add(new Tuple(Elipse, false));
            btns.Add(new Tuple(Rectangle, false));
            btns.Add(new Tuple(Polygon, false));
            btns.Add(new Tuple(Image, false));
            Undoredo = new UndoRedo.UndoRedo();
            Undoredo.Container = MyCanvas;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (Tuple tup in btns)
            {
                if (sender.Equals(tup.Btn))
                {
                    tup.Btn.Background = Brushes.Yellow;
                    tup.Bul = true;
                }
                else
                {
                    tup.Btn.Background = Brushes.LightGray;
                    tup.Bul = false;
                }
            }

        }


        private void Button_Click_Redo(object sender, RoutedEventArgs e)
        {
            Undoredo.Redo(1);
        }
        private void Button_Click_Undo(object sender, RoutedEventArgs e)
        {
            Undoredo.Undo(1);
        }
        private void Button_Click_Clear(object sender, RoutedEventArgs e)
        {
            ChangeRepresentationObject change = new ChangeRepresentationObject();
            change.ShapesCollection = new List<Shape>();
            change.Action = ActionType.Clear;
            foreach(Shape shape in MyCanvas.Children)
            {
                change.ShapesCollection.Add(shape);
            }
            Undoredo.InsertObjectforUndoRedo(change);
            MyCanvas.Children.Clear();
        }
        private void Canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            foreach (Tuple tup in btns)
            {
                if (tup.Bul)
                {
                    if (tup.Btn.Name.ToString() == "Elipse")
                    {
                        ElipseWindow elipsewindow = new ElipseWindow(X, Y, this, "draw");
                        Common.MoveWindowToCursor(elipsewindow, this);
                        break;
                    }
                    else if (tup.Btn.Name.ToString() == "Rectangle")
                    {
                        RectangleWindow rectwindow = new RectangleWindow(X, Y, this, "draw");
                        Common.MoveWindowToCursor(rectwindow, this);
                        break;
                    }
                    else if (tup.Btn.Name.ToString() == "Polygon")
                    {

                        ChangeRepresentationObject novatacka = new ChangeRepresentationObject();
                        Path p = new Path();
                        EllipseGeometry tacka = new EllipseGeometry();
                        tacka.RadiusX = 1;
                        tacka.RadiusY = 1;
                        tacka.Center = new Point(X, Y);
                        p.Name = "enabled";
                        p.Fill = Brushes.Black;
                        p.Data = tacka;
                        novatacka.shape = p;
                        novatacka.Action = ActionType.Insert;
                        novatacka.disablepoints = p.Name;
                        Undoredo.InsertObjectforUndoRedo(novatacka);
                        MyCanvas.Children.Add(p);
                        break;
                    }
                    else if (tup.Btn.Name.ToString() == "Image")
                    {
                        ImageWindow imagewindow = new ImageWindow(X, Y, this);
                        Common.MoveWindowToCursor(imagewindow, this);
                        break;
                    }
                }
            }
        }

        private void MyCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            X = e.GetPosition(MyCanvas).X;
            Y = e.GetPosition(MyCanvas).Y;
        }

        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (Tuple tup in btns)
            {
                if (tup.Bul)
                {
                    int brojac = 0;
                    foreach(Shape sh in MyCanvas.Children)
                    {
                        if(sh.Name == "enabled")
                        {
                            brojac++;
                        }
                    }
                    if (tup.Btn.Name.ToString() == "Polygon" && brojac != 0)
                    {
                        PolgygonWindow polywindow = new PolgygonWindow(X, Y, this, "draw");
                        Common.MoveWindowToCursor(polywindow, this);
                        break;

                    }
                }
            }
        }
    }
}
