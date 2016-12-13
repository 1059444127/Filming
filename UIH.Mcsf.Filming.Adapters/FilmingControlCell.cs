using System.Diagnostics;
using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Adapters
{
    internal class FilmingControlCell : MedViewerControlCell
    {
        private ImageCell _imageCell;
        public FilmingControlCell()
        {
            Image.AddPage(DisplayDataFactory.Instance.CreateDisplayData());
        }

        public void FillImage(ImageCell imageCell)
        {
            if (_imageCell == imageCell) return;
            _imageCell = imageCell;
            Image.ReplacePage(_imageCell.DisplayData, 0);
            Refresh();
        }

        public void OnClicked(IClickStatus clickStatus)
        {
            // TODO: FilmingControlCell.ImageCell.Clicked
            Debug.Assert(_imageCell != null);
            _imageCell.OnClicked(clickStatus);
        }
    }
}