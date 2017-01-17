using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface ISelect
    {
        bool IsSelected { get; set; }
        bool IsFocused { get; set; }
        event EventHandler<ClickStatusEventArgs> Clicked;
    }
}