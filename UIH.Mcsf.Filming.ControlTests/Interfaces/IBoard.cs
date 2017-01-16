using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoard
    {
        int CellCount { get; set; }
        event EventHandler CellCountChanged;

        IList<BoardCell> BoardCells { get; set; } 
        void NewPage();
    }

    public class BoardCell
    {
    }
}
