using System.Collections.Generic;
using GalaSoft.MvvmLight;
using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Filming.Model;

namespace UIH.Mcsf.Filming.ViewModel
{
    public class PageControlViewModel : ViewModelBase
    {
        #region [--Layout--]

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

        #region [--ImageCells--]

        private IList<ImageCell> _imageCells;

        public IList<ImageCell> ImageCells
        {
            get { return _imageCells; }
            set
            {
                if (_imageCells == value) return;
                _imageCells = value;
                RaisePropertyChanged(() => ImageCells);
            }
        }

        #endregion [--ImageCells--]

        #endregion [--ImageCells--]
 
    }
}
