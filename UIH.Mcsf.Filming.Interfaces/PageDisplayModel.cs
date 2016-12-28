using System;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class PageDisplayModel
    {
        private bool _isBreak;
        private bool _isVisible;
        private readonly PageData _pageData;

        // TODO: Rename Page to PageData
        public PageDisplayModel(PageData pageData)
        {
            _pageData = pageData;
        }

        public PageDisplayModel()
        {
            // TODO: Create Class NullPageData To Replace new Page()
            // TODO: Create Class NullLayout
            _pageData = new PageData();
        }

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

        public PageData PageData
        {
            get { return _pageData; }
        }

        public event EventHandler<BoolEventArgs> VisibleChanged = delegate { };
        public event EventHandler<BoolEventArgs> IsBreakChanged = delegate { };
    }
}