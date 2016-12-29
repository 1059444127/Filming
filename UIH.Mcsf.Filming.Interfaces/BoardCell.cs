using System;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class BoardCell
    {
        // TODO: BoardCell.PageDataChanged Event
        private PageModel _pageModel;
        // TODO-Later: Replace BoardCell(PageModel PageModel) with PageModel.setter
        public BoardCell(PageModel pageModel)
        {
            _pageModel = pageModel;
        }

        public BoardCell()
        {
            // TODO: Create Class NullLayout
            _pageModel = PageModel.CreatePageModel();
        }

        public bool IsVisible
        {
            set { _pageModel.IsVisible = value; }
        }

        public PageModel PageModel
        {
            get { return _pageModel; }
            set
            {
                if (_pageModel == value) return;
                _pageModel = value;
                PageModelChanged(this, new PageModelEventArgs(value));
            }
        }

        public event EventHandler<PageModelEventArgs> PageModelChanged = delegate { };
    }
}