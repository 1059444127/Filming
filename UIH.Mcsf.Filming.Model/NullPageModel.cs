using System.Collections.Generic;
using UIH.Mcsf.Filming.Adapters;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.Model
{
    internal class NullPageModel : PageModel
    {
        #region Overrides of PageModel

        public override Layout Layout
        {
            get { return LayoutFactory.CreateLayout(); }
        }

        public override IList<ImageCell> ImageCells
        {
            get { return new List<ImageCell>(); }
            set { }
        }

        public override bool IsBreak
        {
            get { return false; }
            set { }
        }

        public override bool IsVisible
        {
            get { return false; }
            set { }
        }

        #endregion
    }
}