using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public interface IFilmRepository : IFilmBuffer, IDegree
    {
        void Add(ISelectableFilm film);
        void Append();
    }
}