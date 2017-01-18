using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.ControlTests.Models;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    class CardControlViewModel : TestViewModelBase
    {
        public CardControlViewModel()
        {
            Board = new Board();
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
                // TODO-Later: CardControlViewModel Register CellCountChanged Event From Board
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

        #region [--NewPageCommand--]

        private ICommand _newPageCommand;

        public ICommand NewPageCommand
        {
            get { return _newPageCommand = _newPageCommand ?? new RelayCommand(NewPage); }
        }

        private void NewPage()
        {
            Board.NewPage();
        }

        #endregion [--NewPageCommand--]    


        #region Overrides of TestViewModelBase

        // TODO-New-Feature-working-on: CardControlViewModel.NewPage
        // TODO-New-Feature: CardControlViewModel.DeleteSelectedPage
        // TODO-Later: undo/redo

        protected override void UpdateViewModel()
        {
            MessageBox.Show("Middle Button pressed at CardControl");
            (Board[3] as PageControlViewModel).Visibility = Visibility.Visible;
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


        public void NewPage()
        {
            throw new NotImplementedException();
        }

        public object this[int i]
        {
            get
            {
                return new PageControlViewModel
                {
                    TitleBarViewModel = new TitleBarViewModel {PatientName = "NoBody", PageNO = i + 1, PageCount = 8},
                    Visibility = Visibility.Visible
                };
            }
        }

        #endregion
    }
}
