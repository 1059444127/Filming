﻿using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.ControlTests.Models;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    class CardControlViewModel : TestViewModelBase
    {
        private readonly IFilmRepository _filmRepository;

        public CardControlViewModel()
        {
            _filmRepository = new FilmRepository();
            _board = new Board(_filmRepository);

            RegisterFilmBufferEvent();
        }

        ~CardControlViewModel()
        {
            UnRegisterFilmBufferEvent();
        }

        #region [--Board Content Event Handler--]

        private void RegisterFilmBufferEvent()
        {
            _filmRepository.MaxNOChanged += BoardContentOnMaxNOChanged;
            _filmRepository.NOChanged += BoardContentOnNOChanged;
        }

        private void UnRegisterFilmBufferEvent()
        {
            _filmRepository.MaxNOChanged -= BoardContentOnMaxNOChanged;
            _filmRepository.NOChanged -= BoardContentOnNOChanged;
        }

        private void BoardContentOnNOChanged(object sender, EventArgs eventArgs)
        {
            BoardNO = _filmRepository.NO;
        }

        private void BoardContentOnMaxNOChanged(object sender, EventArgs eventArgs)
        {
            BoardMaxNO = _filmRepository.MaxNO;
        }

        #endregion

        #region [--Board--]

        private IBoard _board;

        //For CardControl Binding
        public IBoard Board
        {
            get { return _board; }
        }



        #endregion [--Board--]

        #region [--BoardNO--]

        private int _boardNO;

        public int BoardNO
        {
            get { return _boardNO; }
            set
            {
                if (_boardNO == value) return;
                _boardNO = value;
                RaisePropertyChanged(() => BoardNO);
                _filmRepository.NO = value;
            }
        }

        #endregion [--BoardNO--]

        #region [--BoardMaxNO--]

        private int _boardMaxNO;

        public int BoardMaxNO
        {
            get { return _boardMaxNO; }
            set
            {
                if (_boardMaxNO == value) return;
                _boardMaxNO = value;
                RaisePropertyChanged(() => BoardMaxNO);
            }
        }

        #endregion [--BoardMaxNO--]



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
                _filmRepository.VisibleCount = value;
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
            _filmRepository.Append();
        }

        #endregion [--NewPageCommand--]    


        #region Overrides of TestViewModelBase

        // TODO-New-Feature-working-on: CardControlViewModel.NewPage
        // TODO-New-Feature: CardControlViewModel.DeleteSelectedPage
        // TODO-Later: undo/redo

        protected override void UpdateViewModel()
        {
            MessageBox.Show("Middle Button pressed at CardControl");
        }

        #endregion
    }

}
