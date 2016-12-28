using System;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class BoardCell
    {
        private bool _isBreak;
        private bool _isVisible;

        // TODO: BoardCell.PageDataChanged Event
        private readonly PageData _pageData;

        // TODO-Later: Replace BoardCell(pageData pageData) with PageData.setter
        public BoardCell(PageData pageData)
        {
            // TODO: Rename PageData To PageModel
            _pageData = pageData;
        }

        public BoardCell()
        {
            // TODO: Create Class NullLayout
            _pageData = new PageData();
        }
        // TODO: Keep BoardCell.IsVisible.Setter, Move to PageData
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

        // TODO: Move BoardCell.IsBreak to PageData
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

        // TODO: Move BoardCell.VisibleChanged to PageData
        public event EventHandler<BoolEventArgs> VisibleChanged = delegate { };
        // TODO: Move BoardCell.IsBreakChanged to PageData
        public event EventHandler<BoolEventArgs> IsBreakChanged = delegate { };
    }
}