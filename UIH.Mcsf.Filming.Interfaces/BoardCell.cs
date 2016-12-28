using System;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class BoardCell
    {
        private bool _isBreak;
        private bool _isVisible;
        private readonly PageData _pageData;

        // TODO-Later: Replace BoardCell(pageData pageData) with PageData.setter
        public BoardCell(PageData pageData)
        {
            _pageData = pageData;
        }

        public BoardCell()
        {
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