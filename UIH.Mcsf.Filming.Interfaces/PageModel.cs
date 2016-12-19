using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class PageModel
    {
        //TODO: PageModel.Constructor
        public Layout Layout { get; set; }
        public IList<ImageCell> ImageCells { get; set; }
    }
}
