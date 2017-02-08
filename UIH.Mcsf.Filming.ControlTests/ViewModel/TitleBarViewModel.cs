using System.Windows;
using GalaSoft.MvvmLight;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    public class TitleBarViewModel : ViewModelBase
    {
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

        #region [--PatientName--]

        private string _patientName = "Nobody";

        public string PatientName
        {
            get { return _patientName; }
            set
            {
                if (_patientName == value) return;
                _patientName = value;
                RaisePropertyChanged(() => PatientName);
            }
        }

        #endregion [--PatientName--]

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

        #region [--Visibility--]

        private Visibility _visibility = Visibility.Visible;

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

    }
}
