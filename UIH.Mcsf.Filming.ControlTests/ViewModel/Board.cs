﻿using System;
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
        }

        private void UnRegisterBoardContentEvent()
        {
            _boardContent.CountChanged -= BoardContentOnCountChanged;
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

        #region Implementation of IAppend

        public void Append()
        {
            _boardContent.Append();
        }

        #endregion

        #region Implementation of IVariableCollection

        public int Count
        {
            get { return _count; }
            set
            {
                if (_count == value) return;
                _count = value;
                CountChanged(this, new EventArgs());
                _boardContent.Count = value;
            }
        }

        public event EventHandler CountChanged = delegate { };

        #endregion
    }
}