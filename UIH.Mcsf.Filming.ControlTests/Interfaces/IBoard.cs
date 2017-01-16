using System;
using System.Collections.Generic;
using UIH.Mcsf.Filming.ControlTests.Models;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoard
    {
        int CellCount { get; set; }
        event EventHandler CellCountChanged;

        IList<BoardCell> BoardCells { get; set; } 
        void NewPage();
    }
}
