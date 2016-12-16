using System.Collections.Generic;
using GalaSoft.MvvmLight;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.ViewModel
{
    public class CardControlViewModel : ViewModelBase
    {
        #region [--DisplayMode--]

        private int _displayMode;

        public int DisplayMode
        {
            get { return _displayMode; }
            set
            {
                if (_displayMode == value) return;
                _displayMode = value;
                RaisePropertyChanged(() => DisplayMode);
            }
        }

        #endregion [--DisplayMode--]

        #region [--Pages--]

        private IList<PageModel> _pages;

        public IList<PageModel> Pages
        {
            get { return _pages; }
            set
            {
                if (_pages == value) return;
                _pages = value;
                RaisePropertyChanged(() => Pages);
            }
        }

        #endregion [--Pages--]

    }
}
