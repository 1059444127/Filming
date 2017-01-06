using System;

namespace UIH.Mcsf.Filming.Interfaces
{
    public interface IBoardCell
    {
        bool IsVisible { set; }
        
        PageModel PageModel { get; set; }
        // TODO: Change PageModelChanged Type From EventHandler<PageModelEventArgs> to EventHandler
        event EventHandler<PageModelEventArgs> PageModelChanged;
        
        // TODO: Add IBoardCell.Row
        // TODO: Add IBoardCell.Col
        //int Row { get; set; }
        //event EventHandler RowChanged;
        //int Col { get; set; }
        //event EventHandler ColChanged;
    }
}