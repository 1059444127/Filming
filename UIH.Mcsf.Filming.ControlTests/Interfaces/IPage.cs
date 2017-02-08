using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IPage
    {
        bool IsVisible { get; set; }
        event EventHandler VisibleChanged;
        int PageNO { get; set; }
        int PageCount { get; set; }
        event EventHandler PageNOChanged;
        event EventHandler PageCountChanged;

        ITitleSubject Title { get;}
    }
}