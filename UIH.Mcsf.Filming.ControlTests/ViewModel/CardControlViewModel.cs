using GalaSoft.MvvmLight;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    class CardControlViewModel : ViewModelBase
    {
        public CardControlViewModel()
        {
            DynamicGridViewModel = new DynamicGridViewModel();
        }

        #region [--CellCount--]

        private int _cellCount=1;

        public int CellCount
        {
            get { return _cellCount; }
            set
            {
                if (_cellCount == value) return;
                _cellCount = value;
                RaisePropertyChanged(() => CellCount);
            }
        }

        #endregion [--CellCount--]


        public object DynamicGridViewModel { get; private set; }

    }
}
