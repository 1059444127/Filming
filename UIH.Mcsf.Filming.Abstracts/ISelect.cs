using System;

namespace UIH.Mcsf.Filming.Abstracts
{
    public interface ISelect
    {
        bool IsSelected { get; set; }
        bool IsFocused { get; set; }
        event EventHandler<ClickStatusEventArgs> Clicked;
    }
}