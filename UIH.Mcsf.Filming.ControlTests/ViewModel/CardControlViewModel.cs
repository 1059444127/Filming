﻿using System.Windows;
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
            Board = new Board(new BoardContent());
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
            Board.AppendBoardCell();
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

}
