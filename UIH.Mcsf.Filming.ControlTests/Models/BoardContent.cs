using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class BoardContent : IBoardContent
    {
        private IFilmRepository _films;

        public BoardContent(IFilmRepository filmRepository)
        {
            _films = filmRepository;
            var film = new Film(){IsVisible = true};
            film.FilmTitle.PatientName = "Nobody1";
            _films.Add(film);
        }

        #region Implementation of IBoardContent

        public IFilm this[int i]
        {
            get { return _films[i]; }
        }

        public void AppendContent()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}