using System;
using System.Collections.Generic;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoard
    {
        int CellCount { get; set; }
        event EventHandler CellCountChanged;

        IList<BoardCell> BoardCells { get; set; } 
    }

    public class BoardCell
    {
    }
}
