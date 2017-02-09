using System;
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
                if (_title == value) return;
                UnRegisterTitleEvent();
                _title = value;
                RefreshProperties();
                RegisterTitleEvent();
            }
        }

        private void RefreshProperties()
        {
            if (_title == null) return;

            NO = _title.NO;
            Count = _title.Count;
        }

        private void RegisterTitleEvent()
        {
            if (_title == null) return;

            _title.NOChanged += TitleOnNOChanged;
            _title.CountChanged += TitleOnCountChanged;
        }

        private void UnRegisterTitleEvent()
        {
            if (_title == null) return;

            _title.NOChanged -= TitleOnNOChanged;
            _title.CountChanged -= TitleOnCountChanged;
        }

        #region [--Title Event Handler--]

        private void TitleOnCountChanged(object sender, EventArgs eventArgs)
        {
            Count = _title.Count;
        }

        private void TitleOnNOChanged(object sender, EventArgs eventArgs)
        {
            NO = _title.NO;
        }

        #endregion

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

        #region [--NO--]

        private int _no = 1;

        public int NO
        {
            get { return _no; }
            set
            {
                if (_no == value) return;
                _no = value;
                RaisePropertyChanged(() => NO);
            }
        }

        #endregion [--NO--]

        #region [--Count--]

        private int _count = 1;

        public int Count
        {
            get { return _count; }
            set
            {
                if (_count == value) return;
                _count = value;
                RaisePropertyChanged(() => Count);
            }
        }

        #endregion [--Count--]

        #endregion
    }
}