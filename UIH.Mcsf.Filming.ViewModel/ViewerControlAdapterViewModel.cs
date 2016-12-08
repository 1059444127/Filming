﻿using GalaSoft.MvvmLight;
using UIH.Mcsf.Filming.DataModel;

namespace UIH.Mcsf.Filming.ViewModel
{
    public class ViewerControlAdapterViewModel : ViewModelBase
    {
        #region [--Layout--]
        // TODO: ViewerControlAdapterViewModel.Layout 使用Model.Layout替代
        private Layout _layout;

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
