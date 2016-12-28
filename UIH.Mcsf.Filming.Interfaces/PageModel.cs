using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class PageModel
    {
        public PageModel(Layout layout, IList<ImageCell> imageCells)
        {
            Layout = layout;
            ImageCells = imageCells;
        }

        public PageModel()
        {
            Layout = Layout.CreateDefaultLayout();
            ImageCells = ImageCell.CreateCells(Layout.Capacity);
        }

        public Layout Layout { get; private set; }
        public IList<ImageCell> ImageCells { get; private set; }
    }
}