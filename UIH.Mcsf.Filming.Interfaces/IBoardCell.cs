using System;

namespace UIH.Mcsf.Filming.Interfaces
{
    public interface IBoardCell
    {
        bool IsVisible { set; }
        
        PageModel PageModel { get; set; }
        event EventHandler PageModelChanged;
        
        int Row { get; set; }
        event EventHandler RowChanged;
        int Col { get; set; }
        event EventHandler ColChanged;
    }
}