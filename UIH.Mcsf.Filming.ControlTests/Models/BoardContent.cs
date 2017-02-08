using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    internal class BoardContent : IBoardContent
    {
        #region Implementation of IBoardContent

        public IPage this[int i]
        {
            get { return new PageStub(); }
        }

        public void AppendContent()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal class PageStub : IPage
    {
        private bool _isVisible;

        #region Implementation of IPage

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if(_isVisible == value) return;
                _isVisible = value;
                VisibleChanged(this, new EventArgs());
            }
        }

        public event EventHandler VisibleChanged = delegate { };

        public int PageNO { get; set; }

        public event EventHandler PageNOChanged = delegate { };

        public int PageCount { get; set; }

        public event EventHandler PageCountChanged;

        public ITitleSubject Title
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}