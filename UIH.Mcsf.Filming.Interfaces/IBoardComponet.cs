using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Model
{
    public interface IBoardComponet : ICellCount
    {
        List<BoardCell> BoardCells { get; }
    }
}