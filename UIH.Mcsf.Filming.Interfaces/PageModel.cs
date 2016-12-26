using System;
using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class PageModel
    {
        private bool _isVisible;

        public PageModel(Layout layout, IList<ImageCell> imageCells)
        {
            Layout = layout;
            ImageCells = imageCells;
        }

        public PageModel()
        {
            
        }

        public event EventHandler<BoolEventArgs> VisibleChanged = delegate { };

        public Layout Layout { get; private set; }
        public IList<ImageCell> ImageCells { get; private set; }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if(_isVisible == value) return;
                _isVisible = value;
                VisibleChanged(this, new BoolEventArgs(value));
            }
        }
    }
}