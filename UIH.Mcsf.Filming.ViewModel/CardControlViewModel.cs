using System;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Filming.Model;

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

        private IBoardModel _boardModel;

        public IBoardModel BoardModel
        {
            private get { return _boardModel; }
            set
            {
                if (_boardModel == value) return;
                _boardModel = value;
                _boardModel.BoardNOChanged += BoardModelOnBoardNOChanged; // BoardModel 只有一个
                _boardModel.BoardCountChanged += BoardModelOnBoardCountChanged;
                RaisePropertyChanged(() => BoardModel);
            }
        }

        private void BoardModelOnBoardCountChanged(object sender, EventArgs args)
        {
            BoardMaxNO = _boardModel.BoardCount - 1;
        }

        private void BoardModelOnBoardNOChanged(object sender, EventArgs args)
        {
            BoardNO = _boardModel.BoardNO;
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
                BoardModel.CellCount = value;
            }
        }

        #endregion [--DisplayMode--]

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
            }
        }

        #endregion [--BoardNO--]

        #region [--BoardMaxNO--]

        private int _boardMaxNO = 0;

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