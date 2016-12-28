using System;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class PageDisplayModel
    {
        private bool _isBreak;
        private bool _isVisible;
        private readonly Page _page;

        // TODO: Rename Page to PageData
        public PageDisplayModel(Page page)
        {
            _page = page;
        }

        public PageDisplayModel()
        {
            // TODO: Create Class NullPageData To Replace new Page()
            // TODO: Create Class NullLayout
            _page = new Page();
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

        public Page Page
        {
            get { return _page; }
        }

        public event EventHandler<BoolEventArgs> VisibleChanged = delegate { };
        public event EventHandler<BoolEventArgs> IsBreakChanged = delegate { };
    }
}