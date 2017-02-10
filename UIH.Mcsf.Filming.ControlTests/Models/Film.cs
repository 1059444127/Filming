using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    class Film : IFilm
    {

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

        public ITitleSubject Title
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Implementation of IFilm

        public IFilmTitleSubject FilmTitle
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
