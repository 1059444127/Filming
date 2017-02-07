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

        #endregion
    }
}