using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IFilmRepository
    {
        ISelectableFilm this[int i] { get; }
        void Add(ISelectableFilm film);
        void AppendFilm();

        int Focus { get; }
        event EventHandler FocusChanged;
    }
}