using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using UIH.Mcsf.Filming.Interfaces;
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

        #endregion [--UserControlViewModel--]

        #region [--UserControl Test--]

        #region [--StartTestCommand--]

        private ICommand _startTestCommand;

        public ICommand StartTestCommand
        {
            get { return _startTestCommand = _startTestCommand ?? new RelayCommand(StartTest); }
        }

        #endregion [--StartTestCommand--]    

        private object CreateUserControlViewModel()
        {
            return _002CreatePageControlViewModel();
        }

        private void StartTest()
        {
            _002PageControlTest();
        }

        private object _002CreatePageControlViewModel()
        {
            return new PageControlViewModel();
        }

        private void _002PageControlTest()
        {
            var viewModel = _userControlViewModel as PageControlViewModel;
            viewModel.Layout = Layout.CreateDefaultLayout();
        }

        private object _001CreateViewerControlAdapterViewModel()
        {
            return new ViewerControlAdapterViewModel();
        }

        private void _001ViewerControlAdapterTest()
        {
            var viewModel = _userControlViewModel as ViewerControlAdapterViewModel;
            viewModel.Layout = Layout.CreateDefaultLayout();
            MessageBox.Show("Hello F5");
        }

        #endregion [--UserControl Test--]
    }
}