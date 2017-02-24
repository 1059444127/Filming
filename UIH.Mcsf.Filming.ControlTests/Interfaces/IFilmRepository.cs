using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IFilmRepository : IFilmBuffer, IDegree
    {
        void Add(ISelectableFilm film);

        int Focus { get; set; }
        event EventHandler FocusChanged;
        void Append();
    }
}