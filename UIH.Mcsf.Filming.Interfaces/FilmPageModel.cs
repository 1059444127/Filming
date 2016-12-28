using System;
using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Interfaces
{
    class FilmPageModel : PageModel
    {
        private Layout _layout;
        private IList<ImageCell> _imageCells;
        private bool _isVisible;

        public FilmPageModel(Layout layout, IList<ImageCell> imageCells) 
        {
            _layout = layout;
            _imageCells = imageCells;
        }

        #region Overrides of PageModel

        public override Layout Layout
        {
            get { return _layout; }
        }

        public override IList<ImageCell> ImageCells
        {
            get { return _imageCells; }
            set { _imageCells = value; }
        }

        public override bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                OnVisibleChanged();
            }
        }


        #endregion
    }
}
