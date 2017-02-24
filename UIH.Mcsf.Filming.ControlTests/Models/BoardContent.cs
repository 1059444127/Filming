using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class BoardContent : IBoardContent
    {
        private readonly IFilmBuffer _films;

        public BoardContent(IFilmBuffer filmBuffer)
        {
            _films = filmBuffer;
            RegisterFilmBufferEvent();
        }

        ~BoardContent()
        {
            UnRegisterFilmBufferEvent();
        }

        #region [--Event Handler--]

        private void UnRegisterFilmBufferEvent()
        {
            _films.FilmChanged -= FilmsOnFilmChanged;
        }

        private void RegisterFilmBufferEvent()
        {
            _films.FilmChanged += FilmsOnFilmChanged;
        }

        private void FilmsOnFilmChanged(object sender, IntEventArgs intEventArgs)
        {
            CellChanged(this, intEventArgs);
        }

        #endregion

        #region Implementation of IBoardContent

        private int _visibleCount = 1;
        public int VisibleCount
        {
            get { return _visibleCount; }
            set
            {
                if(_visibleCount == value) return;
                _visibleCount = value;
                CountChanged(this, new EventArgs());
            }
        }

        public event EventHandler CountChanged = delegate { };
        
        public IFilm this[int i]
        {
            get { return _films[i]; }
        }

        public event EventHandler<IntEventArgs> CellChanged = delegate { };

        #endregion
    }
}