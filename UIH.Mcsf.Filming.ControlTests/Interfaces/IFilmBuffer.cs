using System;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IFilmBuffer
    {
        event EventHandler<IntEventArgs> FilmChanged;
        IFilm this[int i] { get; }
        int VisibleSize { set; }
    }
}