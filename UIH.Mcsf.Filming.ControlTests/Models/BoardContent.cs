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
            RegisterFilmRepositoryEvent();

            var film = new Film(){IsVisible = true};
            film.FilmTitle.PatientName = "Nobody1";
            _films.Add(film);
        }

        ~BoardContent()
        {
            UnRegisterFilmRepositoryEvent();
        }

        #region [--Event Handler--]

        private void UnRegisterFilmRepositoryEvent()
        {
            _films.FocusChanged -= FilmsOnFocusChanged;
        }

        private void RegisterFilmRepositoryEvent()
        {
            _films.FocusChanged += FilmsOnFocusChanged;
        }

        private void FilmsOnFocusChanged(object sender, EventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IBoardContent

        public IFilm this[int i]
        {
            get { return _films[i]; }
        }

        #endregion

        #region Implementation of IAppend

        public void Append()
        {
            _films.Append();
        }

        #endregion
    }
}