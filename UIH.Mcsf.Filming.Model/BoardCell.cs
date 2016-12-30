using System;

namespace UIH.Mcsf.Filming.Model
{
    public class BoardCell
    {
        private PageModel _pageModel = PageModel.CreatePageModel();

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