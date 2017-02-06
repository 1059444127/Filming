using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    internal class PageControlViewModel : ViewModelBase, IGridCell
    {
        public PageControlViewModel()
        {
            TitleBarViewModel = new TitleBarViewModel {PatientName = "NobodyInPage", PageNO = 1, PageCount = 1};
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
            get { return _page; }
            set
            {
                if (_page == value) return;
                _page = value;
                RaisePropertyChanged(() => Page);
            }
        }

        #endregion [--IPage--]
    }
}