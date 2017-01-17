using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class PageRepository : IPageRepository
    {
        #region Implementation of IPageRepository

        public void AppendPage()
        {
            throw new System.NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public event EventHandler CountChanged;

        public PageModel this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}