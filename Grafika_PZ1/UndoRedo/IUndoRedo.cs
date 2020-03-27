using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace Grafika_PZ1.UndoRedo
{
    interface IUndoRedo
    {
        void Undo(int level);
        void Redo(int level);
        void InsertObjectforUndoRedo(ChangeRepresentationObject dataobject);
    }
}
