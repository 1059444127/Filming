using System;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.ControlTests.Models;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    public class PageControlViewModel : ViewModelBase
    {
        public PageControlViewModel()
        {
            ViewerControlAdapterViewModel = new FooControlViewModel();
        }

        #region [--IsVisible--]

        private bool _isVisible;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                RaisePropertyChanged(() => IsVisible);
            }
        }

        #endregion [--IsVisible--]

        public TitleBarViewModel TitleBarViewModel { get; set; }

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
            if (_page == null) return;

            IsVisible = _page.IsVisible;
            TitleBarViewModel.Title = _page.Title;
        }

        private void RegisterPageEvent()
        {
            if(_page == null) return;

            _page.VisibleChanged += PageOnVisibleChanged;
        }

        private void UnRegisterPageEvent()
        {
            if(_page == null) return;

            _page.VisibleChanged -= PageOnVisibleChanged;
        }

        #region [--Page Event Handler--]

        private void PageOnVisibleChanged(object sender, EventArgs eventArgs)
        {
            IsVisible = _page.IsVisible;
        }

        #endregion

        #endregion [--IPage--]


        private Visibility BoolToVisibility(bool b)
        {
            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        ~PageControlViewModel()
        {
            UnRegisterPageEvent();
        }
    }
}