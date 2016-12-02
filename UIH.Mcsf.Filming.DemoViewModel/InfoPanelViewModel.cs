using UIH.Mcsf.Filming.DataModel;
using UIH.Mcsf.Filming.Widgets;

namespace UIH.Mcsf.Filming.DemoViewModel
{
    public class InfoPanelViewModel : Notifier
    {
        private readonly IBoardPosition _boardPosition;

        public InfoPanelViewModel(IBoardPosition boardPosition)
        {
            _boardPosition = boardPosition;
            RegisterEventHandler();
        }

        #region [--Event Handler--]

        private void RegisterEventHandler()
        {
            _boardPosition.BoardCountChanged += BoardPositionOnBoardCountChanged;
            _boardPosition.BoardNoChanged += BoardPositionOnBoardNoChanged;
        }

        private void BoardPositionOnBoardNoChanged(object sender, IntEventArgs intEventArgs)
        {
            BoardNo = intEventArgs.Int;
        }

        private void BoardPositionOnBoardCountChanged(object sender, IntEventArgs intEventArgs)
        {
            BoardCount = intEventArgs.Int;
        }

        #endregion [--Event Handler--]

        #region BoardNo

        private int _boardNo = 1;

        public int BoardNo
        {
            get { return _boardNo; }
            set
            {
                if (_boardNo == value) return;
                _boardNo = value;
                NotifyPropertyChanged(() => BoardNo);
                _boardPosition.BoardNo = value - 1;
            }
        }

        #endregion //BoardNo

        #region BoardCount

        private int _boardCount = 1;

        public int BoardCount
        {
            get { return _boardCount; }
            set
            {
                if (_boardCount == value) return;
                _boardCount = value;
                NotifyPropertyChanged(() => BoardCount);
            }
        }

        #endregion //BoardCount
    }
}