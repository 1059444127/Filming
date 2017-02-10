using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class BoardContent : IBoardContent
    {
        private IFilmRepository _films;

        public BoardContent()
        {
            _films = new FilmRepository();
            _films.Add(new Film(){IsVisible = true});
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