using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Adapters
{
    internal class FilmingControlCell : MedViewerControlCell
    {
        private ImageCell _imageCell;
        public FilmingControlCell()
        {
            Image.AddPage(GlobalDefinitions.EmptyDisplayData);
        }

        public void FillImage(ImageCell imageCell)
        {
            if (_imageCell == imageCell) return;
            _imageCell = imageCell;
            Image.ReplacePage(_imageCell.DisplayData, 0);
            Refresh();
        }

    }
}