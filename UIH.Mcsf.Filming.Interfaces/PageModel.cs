using System;
using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Interfaces
{
    public abstract class PageModel
    {
        private bool _isBreak;

        public static PageModel CreatePageModel(Layout layout, IList<ImageCell> imageCells)
        {
            return new FilmPageModel(layout, imageCells);
        }

        public static PageModel CreatePageModel()
        {
            return new NullPageModel();
        }

        public abstract Layout Layout { get; }
        public abstract IList<ImageCell> ImageCells { get; set; }

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

        public abstract bool IsVisible { get; set; }


        public event EventHandler<BoolEventArgs> IsBreakChanged = delegate { };
        public event EventHandler<BoolEventArgs> VisibleChanged = delegate { };

        protected void OnVisibleChanged()
        {
            VisibleChanged(this, new BoolEventArgs(IsVisible));
        }
    }
}