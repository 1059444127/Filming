using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Interfaces
{
    public interface IBoardComponet : ICellCount
    {
        List<IBoardCell> BoardCells { get; }
    }
}