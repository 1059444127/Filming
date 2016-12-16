using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Filming.Model;
using UIH.Mcsf.Filming.ViewModel;

namespace UIH.Mcsf.Filming.UserControlTests
{
    public class MainWindowViewModel
    {
        private Random _random = new Random();

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
            return _004CreateCardControlViewModel();
        }

        private void StartTest()
        {
            _004CardControlTest();
        }

        private object _004CreateCardControlViewModel()
        {
            return new CardControlViewModel();
        }

        private void _004CardControlTest()
        {
            var viewModel = _userControlViewModel as CardControlViewModel;
            viewModel.DisplayMode = _random.Next(1,9);
        }

        private object _002CreatePageControlViewModel()
        {
            return new PageControlViewModel();
        }

        private void _002PageControlTest()
        {
            var viewModel = _userControlViewModel as PageControlViewModel;
            viewModel.Layout = Layout.CreateDefaultLayout();
            var sopInstanceUid = @"1.2.156.112605.161340985965.20140523064111.4.15276.1";

            var cells = new SelectableList<ImageCell>();
            for (int i = 0; i < 16; i++)
            {
                cells.Add(new ImageCell(sopInstanceUid));
            }

            viewModel.ImageCells = cells;

            viewModel.TitleBarVisibility = Visibility.Visible;
            viewModel.TitleBarPosition = Dock.Bottom;

            viewModel.PageNO = 1;
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