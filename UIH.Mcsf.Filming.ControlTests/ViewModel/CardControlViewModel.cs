using System;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    public class CardControlViewModel : ViewModelBase
    {
        public CardControlViewModel()
        {
            DynamicGridViewModel = new DynamicGridViewModel();
        }

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
                DynamicGridViewModel.CellCount = value;
            }
        }

        #endregion [--DisplayMode--]


        public DynamicGridViewModel DynamicGridViewModel { get; private set; }


    }
}
