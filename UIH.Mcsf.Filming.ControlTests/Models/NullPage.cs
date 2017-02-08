using System;
using UIH.Mcsf.Filming.ControlTests.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Models
{
    public class NullPage : IPage
    {
        #region Implementation of IPage

        public bool IsVisible
        {
            get { return false; }
            set { }
        }

        public event EventHandler VisibleChanged;

        public int PageNO { get; set; }

        public event EventHandler PageNOChanged;

        public int PageCount { get; set; }

        public event EventHandler PageCountChanged;

        #endregion
    }
}