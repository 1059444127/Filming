using System.Collections.Generic;

namespace UIH.Mcsf.Filming.Interfaces
{
    internal class NullPageModel : PageModel
    {
        private readonly IList<ImageCell> _imageCells;

        public NullPageModel()
        {
            _imageCells = new List<ImageCell>();
        }

        #region Overrides of PageModel

        public override Layout Layout
        {
            get { return Layout.CreateLayout(); }
        }

        public override IList<ImageCell> ImageCells
        {
            get { return _imageCells; }
            set { }
        }

        #endregion
    }
}