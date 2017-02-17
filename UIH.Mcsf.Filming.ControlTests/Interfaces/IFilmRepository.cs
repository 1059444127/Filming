using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IFilmRepository : IAppend
    {
        ISelectableFilm this[int i] { get; }
        void Add(ISelectableFilm film);

        int Focus { get; }
        event EventHandler FocusChanged;
    }
}