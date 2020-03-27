using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Grafika_PZ1.UndoRedo
{
    public partial class UndoRedo : IUndoRedo
    {

        private Stack<ChangeRepresentationObject> _UndoActionsCollection = new Stack<ChangeRepresentationObject>();
        private Stack<ChangeRepresentationObject> _RedoActionsCollection = new Stack<ChangeRepresentationObject>();
        private List<Shape> _ShapesCollection = new List<Shape>();

        public event EventHandler EnableDisableUndoRedoFeature;

        private Canvas _Container;

        public Canvas Container
        {
            get { return _Container; }
            set { _Container = value; }
        }
        #region IUndoRedo Members

        public void Undo(int level)
        {
            for (int i = 1; i <= level; i++)
            {
                if (_UndoActionsCollection.Count == 0) return;

                ChangeRepresentationObject Undostruct = _UndoActionsCollection.Pop();
                if (Undostruct.Action == ActionType.Delete)
                {
                    Container.Children.Add(Undostruct.shape);
                    this.RedoPushInUnDoForDelete(Undostruct.shape);
                }
                else if (Undostruct.Action == ActionType.Insert)
                {
                    Container.Children.Remove(Undostruct.shape);
                    this.RedoPushInUnDoForInsert(Undostruct.shape);
                    if (Undostruct.shape.GetType().Equals(typeof(System.Windows.Shapes.Polygon)))
                    {
                        System.Windows.Shapes.Polygon poly = (System.Windows.Shapes.Polygon)Undostruct.shape;

                        foreach (Shape shape in Container.Children)
                        {
                            if (shape.GetType().Equals(typeof(System.Windows.Shapes.Path)))
                            {
                                Path shapetemp = (Path)shape;
                                EllipseGeometry temp = (EllipseGeometry)shapetemp.Data;
                                foreach (System.Windows.Point p in poly.Points)
                                {
                                    if (temp.Center == p)
                                    {
                                        shape.Name = "enabled";
                                    }
                                }
                            }
                        }
                    }
                }
                else if (Undostruct.Action == ActionType.Edit)
                {
                    if (_UndoActionsCollection.Count != 0)
                    {
                        this.RedoPushInUnDoForEdit(Undostruct);
                        if (Undostruct.shape.GetType().Equals(typeof(Path)))
                        {
                            Path tempPath = (Path)Undostruct.shape;
                            EllipseGeometry tempeg = (EllipseGeometry)tempPath.Data;

                            tempeg.RadiusY = Undostruct.height;
                            tempeg.RadiusX = Undostruct.Width;
                            tempPath.Fill = Undostruct.brush;
                            tempPath.Data = tempeg;
                            tempPath.Stroke = Undostruct.brushborder;
                            tempPath.StrokeThickness = Undostruct.borderThickness;

                            Undostruct.shape = tempPath;
                        }
                        else if (Undostruct.shape.GetType().Equals(typeof(System.Windows.Shapes.Polygon)))
                        {
                            Undostruct.shape.Fill = Undostruct.brush;
                            Undostruct.shape.Stroke = Undostruct.brushborder;
                            Undostruct.shape.StrokeThickness = Undostruct.borderThickness;

                        }
                        else
                        {
                            Undostruct.shape.Height = Undostruct.height;
                            Undostruct.shape.Width = Undostruct.Width;
                            Undostruct.shape.Fill = Undostruct.brush;
                            Undostruct.shape.Stroke = Undostruct.brushborder;
                            Undostruct.shape.StrokeThickness = Undostruct.borderThickness;

                        }
                    }
                }
                else if (Undostruct.Action == ActionType.Clear)
                {
                    foreach (Shape shape in Undostruct.ShapesCollection)
                    {
                        if (!Container.Children.Contains(shape))
                            Container.Children.Add(shape);
                    }
                    _RedoActionsCollection.Push(Undostruct);

                }
            }

            if (EnableDisableUndoRedoFeature != null)
            {
                EnableDisableUndoRedoFeature(null, null);
            }

        }

        public void Redo(int level)
        {
            for (int i = 1; i <= level; i++)
            {
                if (_RedoActionsCollection.Count == 0) return;

                ChangeRepresentationObject Undostruct = _RedoActionsCollection.Pop();
                if (Undostruct.Action == ActionType.Delete)
                {
                    Container.Children.Remove(Undostruct.shape);
                    ChangeRepresentationObject ChangeRepresentationObjectForDelete = this.MakeChangeRepresentationObjectForDelete(Undostruct.shape);
                    _UndoActionsCollection.Push(ChangeRepresentationObjectForDelete);
                }
                else if (Undostruct.Action == ActionType.Insert)
                {
                    Container.Children.Add(Undostruct.shape);
                    ChangeRepresentationObject ChangeRepresentationObjectForInsert = this.MakeChangeRepresentationObjectForInsert(Undostruct.shape);
                    _UndoActionsCollection.Push(ChangeRepresentationObjectForInsert);
                    if (Undostruct.shape.GetType().Equals(typeof(System.Windows.Shapes.Polygon)))
                    {
                        System.Windows.Shapes.Polygon poly = (System.Windows.Shapes.Polygon)Undostruct.shape;

                        foreach (Shape shape in Container.Children)
                        {
                            if (shape.GetType().Equals(typeof(System.Windows.Shapes.Path)))
                            {
                                Path shapetemp = (Path)shape;
                                EllipseGeometry temp = (EllipseGeometry)shapetemp.Data;
                                foreach (System.Windows.Point p in poly.Points)
                                {
                                    if (temp.Center == p)
                                    {
                                        shape.Name = "disabled";
                                    }
                                }
                            }
                        }
                    }
                }
                else if (Undostruct.Action == ActionType.Edit)
                {
                    ChangeRepresentationObject ChangeRepresentationObjectforEdit = this.MakeChangeRepresentationObjectForEdit(Undostruct);
                    _UndoActionsCollection.Push(ChangeRepresentationObjectforEdit);
                    if (Undostruct.shape.GetType().Equals(typeof(Path)))
                    {
                        Path tempPath = (Path)Undostruct.shape;
                        EllipseGeometry tempeg = (EllipseGeometry)tempPath.Data;

                        tempeg.RadiusY = Undostruct.height;
                        tempeg.RadiusX = Undostruct.Width;
                        tempPath.Fill = Undostruct.brush;
                        tempPath.Data = tempeg;
                        tempPath.Stroke = Undostruct.brushborder;
                        tempPath.StrokeThickness = Undostruct.borderThickness;

                        Undostruct.shape = tempPath;
                    }
                    else if (Undostruct.shape.GetType().Equals(typeof(System.Windows.Shapes.Polygon)))
                    {
                        Undostruct.shape.Fill = Undostruct.brush;
                        Undostruct.shape.Stroke = Undostruct.brushborder;
                        Undostruct.shape.StrokeThickness = Undostruct.borderThickness;


                    }
                    else
                    {

                        Undostruct.shape.Height = Undostruct.height;
                        Undostruct.shape.Width = Undostruct.Width;
                        Undostruct.shape.Fill = Undostruct.brush;
                        Undostruct.shape.Stroke = Undostruct.brushborder;
                        Undostruct.shape.StrokeThickness = Undostruct.borderThickness;
                    }

                }

                else if (Undostruct.Action == ActionType.Clear)
                {
                    foreach (Shape shape in Undostruct.ShapesCollection)
                    {
                        Container.Children.Remove(shape);
                    }
                    _UndoActionsCollection.Push(Undostruct);

                }
            }
            if (EnableDisableUndoRedoFeature != null)
            {
                EnableDisableUndoRedoFeature(null, null);
            }

        }

        public void InsertObjectforUndoRedo(ChangeRepresentationObject dataobject)
        {
            _UndoActionsCollection.Push(dataobject);

            _RedoActionsCollection.Clear();

            if (EnableDisableUndoRedoFeature != null)
            {
                EnableDisableUndoRedoFeature(null, null);
            }
        }

        #endregion

        #region UndoHelperFunctions

        public ChangeRepresentationObject MakeChangeRepresentationObjectForInsert(Shape ApbOrDevice)
        {
            ChangeRepresentationObject dataObject = new ChangeRepresentationObject();
            dataObject.Action = ActionType.Insert;
            dataObject.shape = ApbOrDevice;
            dataObject.disablepoints = ApbOrDevice.Name;
            return dataObject;
        }

        public ChangeRepresentationObject MakeChangeRepresentationObjectForDelete(Shape ApbOrDevice)
        {
            ChangeRepresentationObject dataobject = new ChangeRepresentationObject();
            dataobject.Action = ActionType.Delete;
            dataobject.shape = ApbOrDevice;
            dataobject.disablepoints = ApbOrDevice.Name;
            return dataobject;

        }

        public ChangeRepresentationObject MakeChangeRepresentationObjectForEdit(ChangeRepresentationObject undostruct)
        {
            ChangeRepresentationObject ResizeStruct = new ChangeRepresentationObject();
            ResizeStruct.Action = ActionType.Edit;
            if (undostruct.shape.GetType().Equals(typeof(Path)))
            {
                Path p = (Path)undostruct.shape;
                EllipseGeometry eg = (EllipseGeometry)p.Data;
                ResizeStruct.height = eg.RadiusY;
                ResizeStruct.Width = eg.RadiusX;

            }
            else
            {
                ResizeStruct.height = undostruct.shape.Height;
                ResizeStruct.Width = undostruct.shape.Width;
            }
            ResizeStruct.brush = undostruct.shape.Fill;

            ResizeStruct.shape = undostruct.shape;
            ResizeStruct.borderThickness = undostruct.shape.StrokeThickness;
            ResizeStruct.brushborder = undostruct.shape.Stroke;

            return ResizeStruct;
        }


        public void clearUnRedo()
        {
            _UndoActionsCollection.Clear();
        }

        #endregion

        #region RedoHelperFunctions

        public void RedoPushInUnDoForInsert(Shape ApbOrDevice)
        {

            ChangeRepresentationObject dataobject = new ChangeRepresentationObject();
            dataobject.Action = ActionType.Insert;
            dataobject.shape = ApbOrDevice;
            dataobject.disablepoints = ApbOrDevice.Name;
            _RedoActionsCollection.Push(dataobject);

        }

        public void RedoPushInUnDoForDelete(Shape ApbOrDevice)
        {
            ChangeRepresentationObject dataobject = new ChangeRepresentationObject();
            dataobject.Action = ActionType.Delete;
            dataobject.disablepoints = ApbOrDevice.Name;
            dataobject.shape = ApbOrDevice;
            _RedoActionsCollection.Push(dataobject);

        }

        public void RedoPushInUnDoForEdit(ChangeRepresentationObject undostruct)
        {
            ChangeRepresentationObject EditStruct = new ChangeRepresentationObject();
            EditStruct.Action = ActionType.Edit;
            if (undostruct.shape.GetType().Equals(typeof(Path)))
            {
                Path p = (Path)undostruct.shape;
                EllipseGeometry eg = (EllipseGeometry)p.Data;
                EditStruct.height = eg.RadiusY;
                EditStruct.Width = eg.RadiusX;
            }
            else
            {
                EditStruct.height = undostruct.shape.Height;
                EditStruct.Width = undostruct.shape.Width;
            }
            EditStruct.brush = undostruct.shape.Fill;
            EditStruct.shape = undostruct.shape;
            EditStruct.brushborder = undostruct.shape.Stroke;
            EditStruct.borderThickness = undostruct.shape.StrokeThickness;
            _RedoActionsCollection.Push(EditStruct);
        }


        #endregion


        public bool IsUndoPossible()
        {
            if (_UndoActionsCollection.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool IsRedoPossible()
        {
            if (_RedoActionsCollection.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public enum ActionType
    {
        Delete = 0,
        Clear = 1,
        Edit = 2,
        Insert = 3
    }



    public class ChangeRepresentationObject
    {
        public ActionType Action;
        public double Width;
        public string disablepoints;
        public List<Shape> ShapesCollection;
        public System.Windows.Media.Brush brush;
        public System.Windows.Media.Brush brushborder;
        public double borderThickness;
        public double height;
        public Shape shape;

    }
}