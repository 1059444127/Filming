using System;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IFilmBuffer : IVisibleCountSubject
    {
        new int VisibleCount { get; set; }
        event EventHandler<IntEventArgs> FilmChanged;
        ISelectableFilm this[int i] { get; }        
    }
}