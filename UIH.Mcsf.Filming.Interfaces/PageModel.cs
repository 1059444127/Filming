using System;
using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class PageModel
    {
        private bool _isBreak;
        private bool _isVisible;

        public PageModel(Layout layout, IList<ImageCell> imageCells)
        {
            Layout = layout;
            ImageCells = imageCells;
        }

        public PageModel()
        {
        }

        public Layout Layout { get; private set; }
        public IList<ImageCell> ImageCells { get; private set; }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                VisibleChanged(this, new BoolEventArgs(value));
            }
        }

        public bool IsBreak
        {
            get { return _isBreak; }
            set
            {
                if (_isBreak == value) return;
                _isBreak = value;
                IsBreakChanged(this, new BoolEventArgs(value));
            }
        }

        public event EventHandler<BoolEventArgs> VisibleChanged = delegate { };
        public event EventHandler<BoolEventArgs> IsBreakChanged = delegate { };
    }
}