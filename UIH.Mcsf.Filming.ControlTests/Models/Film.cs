﻿using System;
using UIH.Mcsf.Filming.Abstracts;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    class Film : ISelectableFilm
    {
        public Film()
        {
            FilmTitle = new FilmTitle();
        }

        #region Implementation of IPage
        
        private bool _isVisible;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if(_isVisible == value) return;
                _isVisible = value;
                VisibleChanged(this, new EventArgs());
            }
        }

        public event EventHandler VisibleChanged = delegate { };

        public ITitleSubject Title { get {return FilmTitle;} }

        #endregion

        #region Implementation of IFilm

        public IFilmTitleSubject FilmTitle { get; private set; }

        #endregion

        #region Implementation of ISelectable

        public bool IsSelected { get; set; }

        public bool IsFocused { get; set; }

        public event EventHandler<ClickStatusEventArgs> Clicked = delegate {};

        #endregion
    }
}
