using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class NullTitleSubject : ITitleSubject
    {
        #region Implementation of ITitle

        public int PageNO { get; set; }

        public int PageCount { get; set; }

        #endregion

        #region Implementation of ITitleSubject

        public event EventHandler PageNOChanged;
        public event EventHandler PageCountChanged;

        #endregion
    }
}