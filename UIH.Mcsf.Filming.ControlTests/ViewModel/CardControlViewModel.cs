using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    class CardControlViewModel : TestViewModelBase
    {
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
            }
        }

        #endregion [--DisplayMode--]



        #region Overrides of TestViewModelBase

        protected override void UpdateViewModel()
        {
            MessageBox.Show("Middle Button pressed at CardControl");
        }

        #endregion
    }
}
