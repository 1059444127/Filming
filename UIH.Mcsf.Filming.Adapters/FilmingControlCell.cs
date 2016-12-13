using System;
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

        private ImageCell ImageCell
        {
            set
            {
                if (_imageCell == value) return;
                _imageCell.SelectStatusChanged -= ImageCellOnSelectStatusChanged;
                _imageCell = value;
                _imageCell.SelectStatusChanged -= ImageCellOnSelectStatusChanged;
                _imageCell.SelectStatusChanged += ImageCellOnSelectStatusChanged;
            }
        }

        private void ImageCellOnSelectStatusChanged(object sender, BoolEventArgs boolEventArgs)
        {
            IsSelected = boolEventArgs.Bool;
        }

        public void FillImage(ImageCell imageCell)
        {
            ImageCell = imageCell;
            Image.ReplacePage(_imageCell.DisplayData, 0);
            Refresh();
        }

        public void OnClicked(IClickStatus clickStatus)
        {
            Debug.Assert(_imageCell != null);
            _imageCell.OnClicked(clickStatus);
        }
    }
}