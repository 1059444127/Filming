using System;
using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Adapters
{
    internal class FilmingControlCell : MedViewerControlCell
    {
        private ImageCell _imageCell;
        private bool _isFocused;
        public FilmingControlCell()
        {
            Image.AddPage(DisplayDataFactory.Instance.CreateDisplayData());
        }

        private ImageCell ImageCell
        {
            set
            {
                if (_imageCell == value) return;
                DetachImageCell();
                _imageCell = value;
                AttachImageCell();
            }
        }

        private void AttachImageCell()
        {
            if (_imageCell == null) return;
            RegisterImageCellEvent();
            _imageCell.IsFocused = _isFocused;
        }

        private void DetachImageCell()
        {
            if (_imageCell == null) return;
            UnRegisterImageCellEvent();
            _imageCell.IsFocused = false;
        }

        private void RegisterImageCellEvent()
        {
            _imageCell.SelectStatusChanged -= ImageCellOnSelectStatusChanged;
            _imageCell.SelectStatusChanged += ImageCellOnSelectStatusChanged;
            _imageCell.FocusStatusChanged -= ImageCellOnFocusStatusChanged;
            _imageCell.FocusStatusChanged += ImageCellOnFocusStatusChanged;
        }

        private void UnRegisterImageCellEvent()
        {
            _imageCell.SelectStatusChanged -= ImageCellOnSelectStatusChanged;
            _imageCell.FocusStatusChanged -= ImageCellOnFocusStatusChanged;
        }

        private void ImageCellOnSelectStatusChanged(object sender, EventArgs args)
        {
            IsSelected = _imageCell.IsSelected;
        }

        private void ImageCellOnFocusStatusChanged(object sender, EventArgs args)
        {
            _isFocused = _imageCell.IsFocused;
        }

        public void FillImage(ImageCell imageCell)
        {
            ImageCell = imageCell;
            Image.ReplacePage(_imageCell.DisplayData, 0);
            Refresh();
        }

        public void OnClicked(IClickStatus clickStatus)
        {
            if (_imageCell == null) return; //TODO-Later-Log
            _imageCell.OnClicked(clickStatus);
        }

        
    }
}