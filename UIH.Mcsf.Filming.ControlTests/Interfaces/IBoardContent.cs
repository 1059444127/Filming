using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IBoardContent 
    {
        int Count { get; set; }
        event EventHandler CountChanged;
        IFilm this[int i] { get; }
    }
}