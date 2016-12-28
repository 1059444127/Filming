﻿using System;
using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Interfaces
{
    public abstract class PageModel
    {

        public static PageModel CreatePageModel(Layout layout, IList<ImageCell> imageCells)
        {
            return new FilmPageModel(layout, imageCells);
        }

        public static PageModel CreatePageModel()
        {
            return new NullPageModel();
        }

        public abstract Layout Layout { get; }
        public abstract IList<ImageCell> ImageCells { get; set; }

        public abstract bool IsBreak { get; set; }

        public abstract bool IsVisible { get; set; }


        public event EventHandler<BoolEventArgs> IsBreakChanged = delegate { };
        public event EventHandler<BoolEventArgs> VisibleChanged = delegate { };

        protected void OnBreakChanged()
        {
            IsBreakChanged(this, new BoolEventArgs(IsBreak));
        }

        protected void OnVisibleChanged()
        {
            VisibleChanged(this, new BoolEventArgs(IsVisible));
        }
    }
}