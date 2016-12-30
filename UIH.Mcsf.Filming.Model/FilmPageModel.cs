using System.Collections.Generic;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.Model
{
    internal class FilmPageModel : PageModel
    {
        private IList<ImageCell> _imageCells;
        private bool _isBreak;
        private bool _isVisible;
        private readonly Layout _layout;

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
                if (_isBreak == value) return;
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