using System;
using System.Windows;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    class CardControlViewModel : TestViewModelBase
    {
        public CardControlViewModel()
        {
            Board = new BoardStub();
        }

        #region [--Board--]

        private IBoard _board;

        public IBoard Board
        {
            get { return _board; }
            set
            {
                if (_board == value) return;
                _board = value;
                RaisePropertyChanged(() => Board);
            }
        }

        #endregion [--Board--]


        #region [--DisplayMode--]

        private int _displayMode=1;

        public int DisplayMode
        {
            get { return _displayMode; }
            set
            {
                if (_displayMode == value) return;
                _displayMode = value;
                RaisePropertyChanged(() => DisplayMode);
                Board.CellCount = value;
            }
        }

        #endregion [--DisplayMode--]



        #region Overrides of TestViewModelBase

        // TODO-New-Feature: CardControlViewModel.NewPage
        // TODO-New-Feature: CardControlViewModel.DeleteSelectedPage
        // TODO-Later: undo/redo

        protected override void UpdateViewModel()
        {
            MessageBox.Show("Middle Button pressed at CardControl");
        }

        #endregion
    }

    class BoardStub : IBoard
    {
        private int _cellCount;

        #region Implementation of IBoard

        public int CellCount
        {
            get { return _cellCount; }
            set
            {
                if(_cellCount == value) return;
                _cellCount = value;
                CellCountChanged(this, new EventArgs());
            }
        }

        public event EventHandler CellCountChanged = delegate { };

        #endregion
    }
}
