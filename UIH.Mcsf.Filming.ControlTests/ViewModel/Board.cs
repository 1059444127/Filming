using System;
using System.Diagnostics;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    public class Board : IBoard
    {
        private int _visibleCount = 1;
        private readonly IFilmBuffer _filmBuffer;
        private readonly FilmControlViewModel[] _films = new FilmControlViewModel[GlobalDefinitions.MaxDisplayMode];

        public Board(IFilmBuffer filmBuffer)
        {
            _filmBuffer = filmBuffer;
            RegisterBoardContentEvent();
            for (int i = 0; i < _films.Length; i++)
            {
                _films[i] = new FilmControlViewModel{Film = filmBuffer[i]};
            }
        }

        ~Board()
        {
            UnRegisterBoardContentEvent();
        }

        #region [--BoardContent Event Handler--]

        private void RegisterBoardContentEvent()
        {
            _filmBuffer.VisibleCountChanged += FilmBufferOnVisibleCountChanged;
            _filmBuffer.FilmChanged += FilmBufferOnFilmChanged;
        }

        private void UnRegisterBoardContentEvent()
        {
            _filmBuffer.VisibleCountChanged -= FilmBufferOnVisibleCountChanged;
            _filmBuffer.FilmChanged -= FilmBufferOnFilmChanged;
        }

        private void FilmBufferOnFilmChanged(object sender, IntEventArgs intEventArgs)
        {
            var filmIndex = intEventArgs.Int;
            Debug.Assert(filmIndex >=0);
            Debug.Assert(filmIndex < GlobalDefinitions.MaxDisplayMode);

            _films[filmIndex].Film = _filmBuffer[filmIndex];
        }

        private void FilmBufferOnVisibleCountChanged(object sender, EventArgs eventArgs)
        {
            VisibleCount = _filmBuffer.VisibleCount;
        }

        #endregion

        #region Implementation of IBoard


        public object this[int i]
        {
            get
            {
                Debug.Assert(i<GlobalDefinitions.MaxDisplayMode);
                return _films[i];
            }
        }

        #endregion

        #region Implementation of IVisibleCountSubject

        public int VisibleCount
        {
            get { return _visibleCount; }
            private set
            {
                if (_visibleCount == value) return;
                _visibleCount = value;
                VisibleCountChanged(this, new EventArgs());
            }
        }

        public event EventHandler VisibleCountChanged = delegate { };

        #endregion
    }
}