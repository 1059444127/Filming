using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Adapters
{
    internal class FilmingControlCell : MedViewerControlCell
    {
        private ImageCell _imageCell;
        //TODO: FilmingControlCell need DisplayData
        public void FillImage(ImageCell imageCell)
        {
            _imageCell = imageCell;
        }
    }
}