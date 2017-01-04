using System;

namespace UIH.Mcsf.Filming.Model
{
    public interface ICellCount
    {
        int CellCount { get; set; }
        event EventHandler CellCountChanged;
    }
}