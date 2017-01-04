using System;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.Model
{
    public class BoardCell : IBoardCell
    {
        private PageModel _pageModel = PageModel.CreatePageModel();

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

        public bool IsVisible
        {
            set { _pageModel.IsVisible = value; }
        }

        public event EventHandler<PageModelEventArgs> PageModelChanged = delegate { };
    }
}