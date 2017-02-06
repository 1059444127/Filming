using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    internal class BoardContent : IBoardContent
    {
        #region Implementation of IBoardContent

        public IPage this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}