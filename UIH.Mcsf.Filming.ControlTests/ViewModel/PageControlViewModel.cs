using System;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.ControlTests.Models;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    public class PageControlViewModel : ViewModelBase, IGridCell
    {
        public PageControlViewModel()
        {
            _page = new NullPage();
            TitleBarViewModel = new FilmTitleBarViewModel {PatientName = "NobodyInPage", NO = 1, Count = 1};
            ViewerControlAdapterViewModel = new FooControlViewModel();
        }

        #region Implementation of IGridCell

        #region [--Visibility--]

        private Visibility _visibility = Visibility.Collapsed;

        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                if (_visibility == value) return;
                _visibility = value;
                RaisePropertyChanged(() => Visibility);
            }
        }

        #endregion [--Visibility--]

        #endregion

        public TitleBarViewModel TitleBarViewModel { private get; set; }

        public object ViewerControlAdapterViewModel { get; set; }

        #region [--TitleBarVisibility--]

        private Visibility _titleBarVisibility = Visibility.Visible;

        public Visibility TitleBarVisibility
        {
            get { return _titleBarVisibility; }
            set
            {
                if (_titleBarVisibility == value) return;
                _titleBarVisibility = value;
                RaisePropertyChanged(() => TitleBarVisibility);
            }
        }

        #endregion [--TitleBarVisibility--]

        #region [--TitleBarPosition--]

        private Dock _titleBarPosition = Dock.Top;

        public Dock TitleBarPosition
        {
            get { return _titleBarPosition; }
            set
            {
                if (_titleBarPosition == value) return;
                _titleBarPosition = value;
                RaisePropertyChanged(() => TitleBarPosition);
            }
        }

        #endregion [--TitleBarPosition--]

        #region [--BreakVisibility--]

        private Visibility _breakVisibility = Visibility.Visible;


        public Visibility BreakVisibility
        {
            get { return _breakVisibility; }
            set
            {
                if (_breakVisibility == value) return;
                _breakVisibility = value;
                RaisePropertyChanged(() => BreakVisibility);
            }
        }

        #endregion [--BreakVisibility--]


        #region [--IPage--]

        private IPage _page;

        public IPage Page
        {
            set
            {
                if (_page == value) return;
                UnRegisterPageEvent();
                _page = value;
                RefreshProperties();
                RegisterPageEvent();
            }
        }

        private void RefreshProperties()
        {
            Visibility = BoolToVisibility(_page.IsVisible);
            TitleBarViewModel.Title = _page.Title;
        }

        private void RegisterPageEvent()
        {
            _page.VisibleChanged += PageOnVisibleChanged;
        }

        private void UnRegisterPageEvent()
        {
            _page.VisibleChanged -= PageOnVisibleChanged;
        }

        #region [--Page Event Handler--]

        private void PageOnPageNOChanged(object sender, EventArgs eventArgs)
        {
            TitleBarViewModel.NO = _page.PageNO;
        }

        private void PageOnVisibleChanged(object sender, EventArgs eventArgs)
        {
            Visibility = BoolToVisibility(_page.IsVisible);
        }

        private void PageOnPageCountChanged(object sender, EventArgs eventArgs)
        {
            TitleBarViewModel.Count = _page.PageCount;
        }

        #endregion

        #endregion [--IPage--]


        private Visibility BoolToVisibility(bool b)
        {
            return b ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}