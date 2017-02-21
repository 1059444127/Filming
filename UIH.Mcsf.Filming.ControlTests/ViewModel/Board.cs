using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    public class Board : IBoard
    {
        private int _count = 1;
        private readonly IBoardContent _boardContent;
        private readonly FilmControlViewModel[] _films = new FilmControlViewModel[GlobalDefinitions.MaxDisplayMode];

        public Board(IBoardContent boardContent)
        {
            _boardContent = boardContent;
            RegisterBoardContentEvent();
            for (int i = 0; i < _films.Length; i++)
            {
                _films[i] = new FilmControlViewModel{Film = boardContent[i]};
            }
        }

        ~Board()
        {
            UnRegisterBoardContentEvent();
        }

        #region [--BoardContent Event Handler--]

        private void RegisterBoardContentEvent()
        {
            _boardContent.CountChanged += BoardContentOnCountChanged;
            _boardContent.CellChanged += BoardContentOnCellChanged;
            _boardContent.Changed += BoardContentOnChanged;
        }

        private void UnRegisterBoardContentEvent()
        {
            _boardContent.CountChanged -= BoardContentOnCountChanged;
            _boardContent.CellChanged -= BoardContentOnCellChanged;
            _boardContent.Changed -= BoardContentOnChanged;
        }

        private void BoardContentOnChanged(object sender, EventArgs eventArgs)
        {
            for (int i = 0; i < Count; i++)
            {
                _films[i].Film = _boardContent[i];
            }
        }

        private void BoardContentOnCellChanged(object sender, IntEventArgs intEventArgs)
        {
            var filmIndex = intEventArgs.Int;
            Debug.Assert(filmIndex >=0);
            Debug.Assert(filmIndex < GlobalDefinitions.MaxDisplayMode);

            _films[filmIndex].Film = _boardContent[filmIndex];
        }

        private void BoardContentOnCountChanged(object sender, EventArgs eventArgs)
        {
            Count = _boardContent.Count;
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

        #region Implementation of ICountSubject

        public int Count
        {
            get { return _count; }
            private set
            {
                if (_count == value) return;
                _count = value;
                CountChanged(this, new EventArgs());
            }
        }

        public event EventHandler CountChanged = delegate { };

        #endregion
    }
}