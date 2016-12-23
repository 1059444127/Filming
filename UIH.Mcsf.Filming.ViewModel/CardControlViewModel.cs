using System.Windows.Controls;
using GalaSoft.MvvmLight;
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

    }
}