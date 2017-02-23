using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class FilmBuffer : IFilmBuffer
    {
        private IFilmRepository _films;
        private const int Capacity = GlobalDefinitions.MaxDisplayMode;
        private int _cursor;

        public FilmBuffer(IFilmRepository filmRepository)
        {
            _films = filmRepository;
            RegisterFilmRepositoryEvent();
        }

        ~FilmBuffer()
        {
            UnRegisterFilmRepositoryEvent();
        }

        #region [--FilmRepository Event Handler--]

        private void UnRegisterFilmRepositoryEvent()
        {
            _films.FocusChanged -= FilmsOnFocusChanged;
            _films.CountChanged -= FilmsOnCountChanged;
        }

        private void RegisterFilmRepositoryEvent()
        {
            _films.FocusChanged += FilmsOnFocusChanged;
            _films.CountChanged += FilmsOnCountChanged;
        }

        private void FilmsOnCountChanged(object sender, EventArgs eventArgs)
        {
            var filmCount = _films.Count;
            if (filmCount <= 0) MaxNO = 0;
            else MaxNO = (int)Math.Ceiling((0.0 + filmCount) / VisibleSize) - 1;
        }

        private void FilmsOnFocusChanged(object sender, EventArgs eventArgs)
        {
            
        }

        #endregion

        #region [--Implemented From IFilmBuffer--]

        public event EventHandler<IntEventArgs> FilmChanged;

        public IFilm this[int i]
        {
            get { return _films[NO*VisibleSize+i]; }
        }

        public int VisibleSize { get; set; }

        #endregion

        #region [--CardControlViewModel use--]

        private int _no;
        public int NO
        {
            get { return _no; }
            set
            {
                if (_no == value) return;
                _no = value;
                NOChanged(this, new EventArgs());
                _films.Focus = NO * VisibleSize;

                // Hide old cell, Show new Cell
            }
        }

        public event EventHandler NOChanged = delegate { };

        private int _maxNO;
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

        #endregion [--CardControlViewModel use--]

        

    }
}