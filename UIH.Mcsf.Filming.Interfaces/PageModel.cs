﻿using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class PageModel
    {
        public PageModel(Layout layout, IList<ImageCell> imageCells)
        {
            Layout = layout;
            ImageCells = imageCells;
        }

        //TODO: PageModel:ISelect

        public Layout Layout { get; private set; }
        public IList<ImageCell> ImageCells { get; private set; }
    }
}