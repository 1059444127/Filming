using System;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class BoardCell
    {
        private bool _isBreak;
        private bool _isVisible;

        // TODO: BoardCell.PageDataChanged Event
        private readonly PageModel _pageModel;

        // TODO-Later: Replace BoardCell(PageModel PageModel) with PageModel.setter
        public BoardCell(PageModel pageModel)
        {
            // TODO: Rename PageModel To PageModel
            _pageModel = pageModel;
        }

        public BoardCell()
        {
            // TODO: Create Class NullLayout
            _pageModel = new PageModel();
        }
        // TODO: Keep BoardCell.IsVisible.Setter, Move to PageModel
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

        // TODO-working-on: Move BoardCell.IsBreak to PageModel
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

        public PageModel PageModel
        {
            get { return _pageModel; }
        }

        // TODO: Move BoardCell.VisibleChanged to PageModel
        public event EventHandler<BoolEventArgs> VisibleChanged = delegate { };
        // TODO-working-on: Move BoardCell.IsBreakChanged to PageModel
        public event EventHandler<BoolEventArgs> IsBreakChanged = delegate { };
    }
}