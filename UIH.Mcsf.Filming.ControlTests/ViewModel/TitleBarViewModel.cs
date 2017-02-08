using GalaSoft.MvvmLight;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    public class TitleBarViewModel : ViewModelBase, ITitle
    {
        public TitleBarViewModel()
        {
            _title = new NullTitleSubject();
        }

        #region [--ITitleSubject--]

        private ITitleSubject _title;
        public ITitleSubject Title
        {
            set
            {
                if(_title == value) return;
                //UnRegisterTitleEvent();
                _title = value;
                //RefreshProperty
            }
        }

        #endregion

        #region [--IsSelected--]

        private bool _isSelected = true;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }

        #endregion [--IsSelected--]

        #region [--IsFocused--]

        private bool _isFocused = true;

        public bool IsFocused
        {
            get { return _isFocused; }
            set
            {
                if (_isFocused == value) return;
                _isFocused = value;
                RaisePropertyChanged(() => IsFocused);
            }
        }

        #endregion [--IsFocused--]

        #region Implementation of ITitle

        #region [--PageNO--]

        private int _pageNO = 1;

        public int PageNO
        {
            get { return _pageNO; }
            set
            {
                if (_pageNO == value) return;
                _pageNO = value;
                RaisePropertyChanged(() => PageNO);
            }
        }

        #endregion [--PageNO--]

        #region [--PageCount--]

        private int _pageCount = 1;

        public int PageCount
        {
            get { return _pageCount; }
            set
            {
                if (_pageCount == value) return;
                _pageCount = value;
                RaisePropertyChanged(() => PageCount);
            }
        }

        #endregion [--PageCount--]

        #endregion
    }
}
