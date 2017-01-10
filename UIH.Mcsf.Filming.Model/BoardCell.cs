using System;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.Model
{
    public class BoardCell : IBoardCell
    {
        private PageModel _pageModel = PageModelFactory.CreatePageModel();
        private int _row;
        private int _col;

        #region [--Implement From IBoardCell--]

        public PageModel PageModel
        {
            get { return _pageModel; }
            set
            {
                if (_pageModel == value) return;
                _pageModel = value;
                PageModelChanged(this, new EventArgs());
            }
        }

        public bool IsVisible
        {
            set { _pageModel.IsVisible = value; }
        }

        public event EventHandler PageModelChanged = delegate { };

        public int Row
        {
            get { return _row; }
            set
            {
                if(_row == value) return;
                _row = value;
                RowChanged(this, new EventArgs());
            }
        }

        public event EventHandler RowChanged = delegate { };

        public int Col
        {
            get { return _col; }
            set
            {
                if (_col == value) return;
                _col = value;
                ColChanged(this, new EventArgs());
            }
        }

        public event EventHandler ColChanged = delegate { };

        #endregion
    }
}