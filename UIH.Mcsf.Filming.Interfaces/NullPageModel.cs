using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Interfaces
{
    internal class NullPageModel : PageModel
    {
        #region Overrides of PageModel

        public override Layout Layout
        {
            get { return Layout.CreateLayout(); }
        }

        public override IList<ImageCell> ImageCells
        {
            get { return new List<ImageCell>(); }
            set { }
        }

        public override bool IsBreak
        {
            get { return false; }
            set {  }
        }

        public override bool IsVisible
        {
            get { return false; }
            set { }
        }

        #endregion
    }
}