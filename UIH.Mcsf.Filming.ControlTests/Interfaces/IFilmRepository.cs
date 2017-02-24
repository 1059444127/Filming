using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IFilmRepository : IVisibleCountSubject, IDegree
    {
        ISelectableFilm this[int i] { get; }
        void Add(ISelectableFilm film);

        int Focus { get; set; }
        event EventHandler FocusChanged;
        void Append();
    }
}