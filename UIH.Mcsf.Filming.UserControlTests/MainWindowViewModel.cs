using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using UIH.Mcsf.Filming.ViewModel;

namespace UIH.Mcsf.Filming.UserControlTests
{
    public class MainWindowViewModel
    {
        #region [--UserControlViewModel--]

        private object _userControlViewModel;

        public object UserControlViewModel
        {
            get
            {
                return _userControlViewModel ?? (_userControlViewModel = CreateUserControlViewModel());
            }
        }

        private object CreateUserControlViewModel()
        {
            return _001CreateViewerControlAdapterViewModel();
        }

        #endregion [--UserControlViewModel--]

        #region [--StartTestCommand--]

        private ICommand _startTestCommand;

        public ICommand StartTestCommand
        {
            get { return _startTestCommand = _startTestCommand ?? new RelayCommand(StartTest); }
        }

        private void StartTest()
        {
            _001ViewerControlAdapterTest();
        }

        #endregion [--StartTestCommand--]    

        #region [--UserControl Test--]

        private object _001CreateViewerControlAdapterViewModel()
        {
            return new ViewerControlAdapterViewModel();
        }

        private void _001ViewerControlAdapterTest()
        {
            var viewModel = _userControlViewModel as ViewerControlAdapterViewModel;
            // TODO£ºCreate available Layout
            viewModel.Layout = null;

            MessageBox.Show("Hello F5");
        }

        #endregion [--UserControl Test--]
    }
}