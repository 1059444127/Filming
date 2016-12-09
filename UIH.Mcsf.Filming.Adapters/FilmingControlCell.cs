using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Adapters
{
    internal class FilmingControlCell : MedViewerControlCell
    {
        private ImageCell _imageCell;
        public FilmingControlCell()
        {
            Image.AddPage(new DisplayData());
        }

        public void FillImage(ImageCell imageCell)
        {
            if (_imageCell == imageCell) return;
            _imageCell = imageCell;
            // TODO: Debug to Test Performance of Image.ReplacePage
            Image.ReplacePage(_imageCell.DisplayData, 0);
            Refresh();
        }

    }
}