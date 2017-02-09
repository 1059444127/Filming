using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public class NullFilm : IFilm
    {
        #region Implementation of IFilm

        public IFilmTitleSubject FilmTitle { get; private set; }

        #endregion

        #region Implementation of IPage

        public bool IsVisible
        {
            get { return false; }
            set { }
        }

        public event EventHandler VisibleChanged;

        public ITitleSubject Title
        {
            get { return FilmTitle; }
        }

        #endregion
    }
}