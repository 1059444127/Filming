using GalaSoft.MvvmLight;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.ViewModel
{
    public class CardControlViewModel : ViewModelBase
    {
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
    }
}