using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class PageRepository : SelectableList<PageModel>, IPageRepository
    {
        #region Implementation of IPageRepository

        public void AppendPage()
        {
            throw new System.NotImplementedException();
        }


        public event EventHandler CountChanged;

 

        #endregion
    }
}