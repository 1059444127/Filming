using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoard
    {
        int CellCount { get; set; }
        event EventHandler CellCountChanged;

        void AppendBoardCell();
        object this[int i] { get; }
    }
}
