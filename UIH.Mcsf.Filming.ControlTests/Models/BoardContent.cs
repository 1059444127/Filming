using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class BoardContent : IBoardContent
    {
        private IFilmRepository _films;
        private int _visibleContentCount = 1;
        private int _no;
        private int _maxNO;

        public BoardContent(IFilmRepository filmRepository)
        {
            _films = filmRepository;
            RegisterFilmRepositoryEvent();
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
            NO = _films.Focus/_films.Count;
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
            get { return _films[i]; }
        }

        public event EventHandler<IntEventArgs> CellChanged = delegate { };

        public int NO
        {
            get { return _no; }
            set
            {
                _no = value;
                NOChanged(this, new EventArgs());
            }
        }

        public int MaxNO
        {
            get { return _maxNO; }
            set
            {
                if (_maxNO == value) return;
                _maxNO = value;
                MaxNOChanged(this, new EventArgs());
            }
        }

        public event EventHandler NOChanged = delegate { };
        public event EventHandler MaxNOChanged = delegate { };

        #endregion
    }
}