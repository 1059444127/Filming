using GalaSoft.MvvmLight;
using UIH.Mcsf.Filming.DataModel;

namespace UIH.Mcsf.Filming.UserControlTests
{
    public class ViewerControlAdapterViewModel : ViewModelBase
    {
        #region [--Layout--]

        private Layout _layout;

        public ViewerControlAdapterViewModel(Layout layout)
        {
            _layout = layout;
        }

        public Layout Layout
        {
            get { return _layout; }
            set
            {
                if (_layout == value) return;
                _layout = value;
                RaisePropertyChanged(() => Layout);
            }
        }

        #endregion [--Layout--]

    }
}
