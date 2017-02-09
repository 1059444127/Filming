using System;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
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

        public ITitleSubject Title { get {return new NullTitleSubject();}}

        #endregion
    }
}