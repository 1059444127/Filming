using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using UIH.Mcsf.Filming.Adapters;
using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Filming.Model;
using UIH.Mcsf.Filming.ViewModel;

namespace UIH.Mcsf.Filming.UserControlTests
{
    public class MainWindowViewModel
    {
        // TODO-New-Feature: Command/Event From other application

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
            return _007CreateGridTestViewModel();
        }

        private void StartTest()
        {
            _007GridTestSubUserControlPositionBinding();
        }

        private void _007GridTestSubUserControlPositionBinding()
        {
            var viewModel = _userControlViewModel as GridTestViewModel;
            viewModel.Row = 1;
            viewModel.Col = 0;
        }

        private object _007CreateGridTestViewModel()
        {
            return new GridTestViewModel();
        }

        private object _004CreateCardControlViewModel()
        {
            return new CardControlViewModel();
        }

        private void _006CardControlTestDisplayMode()
        {
            var viewModel = _userControlViewModel as CardControlViewModel;
            var boardModel = new BoardModel(new PageRepository());
            viewModel.BoardModel = boardModel;

            viewModel.DisplayMode = 3;
        }

        private void _005CardControlTestBoardModel()
        {
            var viewModel = _userControlViewModel as CardControlViewModel;
            var boardModel = new BoardModel(new PageRepository());
            boardModel.BoardCells = CreatePages(GlobalDefinitions.MaxDisplayMode);

            viewModel.BoardModel = boardModel;

            boardModel.CellCount = 3;

        }

        private void _004CardControlTest()
        {
            var viewModel = _userControlViewModel as CardControlViewModel;
            //viewModel.DisplayMode = 3;

            var pageCount = _random.Next(GlobalDefinitions.MaxDisplayMode*2);
            var pages = CreatePages(pageCount);

            //viewModel.Pages = pages;
        }

        private object _002CreatePageControlViewModel()
        {
            return new PageControlViewModel(CreatePageModel());
        }

        private void _002PageControlTest()
        {
            var viewModel = _userControlViewModel as PageControlViewModel;
            viewModel.Layout = LayoutFactory.CreateLayout(3, 3);
            var cells = CreateCells();

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
            viewModel.Layout = LayoutFactory.CreateLayout(3, 3);
            MessageBox.Show("Hello F5");
        }

        #endregion [--UserControl Test--]

        private static IList<ImageCell> CreateCells()
        {
            var sopInstanceUid = @"1.2.156.112605.161340985965.20140523064111.4.15276.1";

            var cells = new SelectableList<ImageCell>();
            for (int i = 0; i < 16; i++)
            {
                cells.Add(ImageCellFactory.CreateCell(sopInstanceUid));
            }
            return cells;
        }

        private static BoardCell CreatePageModel()
        {
            var page = PageModelFactory.CreatePageModel(LayoutFactory.CreateLayout(3, 3));
            return new BoardCell {PageModel = page};
        }

        private static List<IBoardCell> CreatePages(int pageCount)
        {
            var pages = new List<IBoardCell>();
            for (int i = 0; i < pageCount; i++)
            {
                var page = CreatePageModel();
                pages.Add(page);
            }
            return pages;
        }
    }
}