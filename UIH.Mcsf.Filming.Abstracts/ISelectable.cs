using System;

namespace UIH.Mcsf.Filming.Abstracts
{
    public interface ISelectable
    {
        bool IsSelected { get; set; }
        bool IsFocused { get; set; }
        event EventHandler<ClickStatusEventArgs> Clicked;
    }
}