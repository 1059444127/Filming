using System;

namespace UIH.Mcsf.Filming.Interfaces
{
    public interface ICellCount
    {
        int CellCount { get; set; }
        event EventHandler CellCountChanged;
    }
}