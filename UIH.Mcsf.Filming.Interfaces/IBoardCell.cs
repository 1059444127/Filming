using System;

namespace UIH.Mcsf.Filming.Interfaces
{
    public interface IBoardCell
    {
        bool IsVisible { set; }
        PageModel PageModel { get; set; }
        event EventHandler<PageModelEventArgs> PageModelChanged;
    }
}