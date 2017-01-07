using System;
using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Interfaces
{
    public abstract class PageModel : ISelect
    {
        public abstract Layout Layout { get; }
        public abstract IList<ImageCell> ImageCells { get; set; }
        public abstract bool IsBreak { get; set; }
        public abstract bool IsVisible { get; set; }
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