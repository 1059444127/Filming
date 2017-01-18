using System;
using UIH.Mcsf.Filming.Abstracts;

namespace UIH.Mcsf.Filming.ControlTests.Interfaces
{
    public class PageModel : ISelect
    {
        public bool IsBreak { get; set; }
        public bool IsVisible { get; set; }
        public event EventHandler IsBreakChanged = delegate { };
        public event EventHandler VisibleChanged = delegate { };

        protected void OnBreakChanged()
        {
            IsBreakChanged(this, new EventArgs());
        }

        protected void OnVisibleChanged()
        {
            VisibleChanged(this, new EventArgs());
        }

        #region Implementation of ISelect

        public bool IsSelected { get; set; }

        public bool IsFocused { get; set; }

        public event EventHandler<ClickStatusEventArgs> Clicked;

        #endregion
    }
}