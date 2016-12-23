using System;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.ViewModel
{
    public class CardControlViewModel : ViewModelBase
    {
        public CardControlViewModel()
        {
            // TODO-Later: CardControlViewModel.ControlPanelDock Read from configure
            ControlPanelDock = Dock.Right;
        }

        public Dock ControlPanelDock { get; private set; }

        #region [--BoardModel--]

        private BoardModel _boardModel;

        public BoardModel BoardModel
        {
            get { return _boardModel; }
            set
            {
                if (_boardModel == value) return;
                _boardModel = value;
                RaisePropertyChanged(() => BoardModel);
            }
        }

        #endregion [--BoardModel--]

        #region [--DisplayMode--]

        private int _displayMode;

        public int DisplayMode
        {
            get { return _displayMode; }
            set
            {
                if (_displayMode == value) return;
                _displayMode = value;
                RaisePropertyChanged(() => DisplayMode);
                BoardModel.DisplayMode = value;
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
            // TODO: NewPage in Model
            BoardModel.NewPage();
        }

        #endregion [--NewPageCommand--]    
    }
}