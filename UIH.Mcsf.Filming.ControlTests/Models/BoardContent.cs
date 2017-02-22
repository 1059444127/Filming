using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class BoardContent : IBoardContent
    {
        private IFilmRepository _films;
        private FilmBuffer _filmBuffer;
        private int _visibleContentCount = 1;
        private int _no;
        private int _maxNO;

        public BoardContent(IFilmRepository filmRepository)
        {
            _films = filmRepository;
            RegisterFilmRepositoryEvent();

            _filmBuffer = new FilmBuffer(_films) {VisibleSize = Count};
        }

        ~BoardContent()
        {
            UnRegisterFilmRepositoryEvent();
        }

        #region [--Event Handler--]

        private void UnRegisterFilmRepositoryEvent()
        {
            _films.CountChanged -= FilmsOnCountChanged;
        }

        private void RegisterFilmRepositoryEvent()
        {
            _films.CountChanged += FilmsOnCountChanged;
        }

        private void FilmsOnCountChanged(object sender, EventArgs eventArgs)
        {
            var filmCount = _films.Count;
            if (filmCount <= 0) MaxNO = 0;
            else MaxNO = (int)Math.Ceiling((0.0+filmCount)/Count) - 1;
        }

        #endregion

        #region Implementation of IBoardContent

        public int Count
        {
            get { return _visibleContentCount; }
            set
            {
                if(_visibleContentCount == value) return;
                _visibleContentCount = value;
                CountChanged(this, new EventArgs());
            }
        }

        public event EventHandler CountChanged = delegate { };
        
        public IFilm this[int i]
        {
            get { return _films[NO*Count+i]; }
        }

        public event EventHandler<IntEventArgs> CellChanged = delegate { };
        public event EventHandler Changed = delegate { };

        public int NO
        {
            get { return _no; }
            set
            {
                if (_no == value) return;
                _no = value;
                NOChanged(this, new EventArgs());
                _films.Focus = NO*Count;

                // Hide old cell, Show new Cell
            }
        }

        public event EventHandler NOChanged = delegate { };

        public int MaxNO
        {
            get { return _maxNO; }
            private set
            {
                if (_maxNO == value) return;
                _maxNO = value;
                MaxNOChanged(this, new EventArgs());
            }
        }

        public event EventHandler MaxNOChanged = delegate { };

        #endregion
    }
}