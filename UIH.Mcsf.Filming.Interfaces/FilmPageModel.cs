using System;
using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Interfaces
{
    class FilmPageModel : PageModel
    {
        private Layout _layout;
        private IList<ImageCell> _imageCells;
        private bool _isVisible;
        private bool _isBreak;

        public FilmPageModel(Layout layout) 
        {
            _layout = layout;
            _imageCells = ImageCell.CreateCells(layout.Capacity);
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

        public override bool IsBreak
        {
            get { return _isBreak; }
            set
            {
                if(_isBreak == value) return;
                _isBreak = value;
                OnBreakChanged();
            }
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
