using System;
using UIH.Mcsf.Filming.Abstracts;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public class NullFilm : ISelectableFilm
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

        #region Implementation of ISelect

        public bool IsSelected { get; set; }

        public bool IsFocused { get; set; }

        public event EventHandler<ClickStatusEventArgs> Clicked;

        #endregion
    }
}