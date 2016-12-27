using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class Page
    {
        public Page(Layout layout, IList<ImageCell> imageCells)
        {
            Layout = layout;
            ImageCells = imageCells;
        }

        public Page()
        {
            Layout = Layout.CreateDefaultLayout();
            ImageCells = ImageCell.CreateCells(Layout.Capacity);
        }

        public Layout Layout { get; private set; }
        public IList<ImageCell> ImageCells { get; private set; }
    }
}