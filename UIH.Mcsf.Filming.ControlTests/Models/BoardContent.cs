using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    internal class BoardContent : IBoardContent
    {
        #region Implementation of IBoardContent

        public IPage this[int i]
        {
            get { return new PageStub{IsVisible = true}; }
        }

        public void AppendContent()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal class PageStub : IPage
    {
        #region Implementation of IPage

        public bool IsVisible { get; set; }

        #endregion
    }
}