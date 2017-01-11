using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    abstract class TestViewModelBase : ViewModelBase
    {
        #region [--UpdateViewModelCommand--]

        private ICommand _updateViewModelCommand;

        public ICommand UpdateViewModelCommand
        {
            get { return _updateViewModelCommand = _updateViewModelCommand ?? new RelayCommand(UpdateViewModel); }
        }

        protected abstract void UpdateViewModel();
    }

        #endregion [--UpdateViewModelCommand--]   
}
