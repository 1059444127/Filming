using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;
using UIH.Mcsf.Filming.Utilities;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class PageRepository : SelectableList<PageModel>, IRepository
    {
        #region Implementation of IRepository

        public void Append()
        {
            throw new System.NotImplementedException();
        }


        public event EventHandler CountChanged;

 

        #endregion
    }
}