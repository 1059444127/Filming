using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public abstract class ImageCell : ISelect
    {
        private bool _isFocused;
        private bool _isSelected;
        public abstract DisplayData DisplayData { get; }
        public event EventHandler<BoolEventArgs> SelectStatusChanged = delegate { };
        public event EventHandler<BoolEventArgs> FocusStatusChanged = delegate { };

        public void OnClicked(IClickStatus clickStatus)
        {
            Clicked(this, new ClickStatusEventArgs(clickStatus));
        }

        #region Implementation of ISelect

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                SelectStatusChanged(this, new BoolEventArgs(value));
            }
        }

        public bool IsFocused
        {
            get { return _isFocused; }
            set
            {
                if (_isFocused == value) return;
                _isFocused = value;
                FocusStatusChanged(this, new BoolEventArgs(value));
            }
        }

        public event EventHandler<ClickStatusEventArgs> Clicked = delegate { };

        #endregion
    }

    public class NullImageCell : ImageCell
    {
        private readonly DisplayData _displayData;

        public NullImageCell()
        {
            _displayData = DisplayDataFactory.Instance.CreateDisplayData();
        }

        #region Overrides of ImageCell

        public override DisplayData DisplayData
        {
            get { return _displayData; }
        }

        #endregion
    }

    public class FilmingImageCell : ImageCell
    {
        private readonly DisplayData _displayData;
        // TODO-later: try 异步加载方式
        public FilmingImageCell(string sopInstanceUid)
        {
            _displayData = DisplayDataFactory.Instance.CreateDisplayData(sopInstanceUid);
            FillImageInfo();
        }

        #region Overrides of ImageCell

        public override DisplayData DisplayData
        {
            get { return _displayData; }
        }

        #endregion

        private string GetTagByName(Dictionary<uint, string> dicomHeader, uint tag)
        {
            return !dicomHeader.ContainsKey(tag) ? string.Empty : dicomHeader[tag];
        }

        private void FillImageInfo()
        {
            Debug.Assert(_displayData != null);
            var imageHeader = _displayData.ImageHeader;
            if (imageHeader == null) return;
            var dicomHeader = imageHeader.DicomHeader;
            if (dicomHeader == null) return;

            GetTagByName(dicomHeader, ServiceTagName.StudyInstanceUID);
            GetTagByName(dicomHeader, ServiceTagName.StudyDate);
            GetTagByName(dicomHeader, ServiceTagName.AccessionNumber);

            GetTagByName(dicomHeader, ServiceTagName.PatientName);
            GetTagByName(dicomHeader, ServiceTagName.PatientID);
            GetTagByName(dicomHeader, ServiceTagName.PatientAge);
            GetTagByName(dicomHeader, ServiceTagName.PatientSex);
        }
    }

    public class ImageCellFactory
    {
        public static IList<ImageCell> CreateCells(int cellCount)
        {
            var cells = new List<ImageCell>();
            for (var i = 0; i < cellCount; i++)
            {
                cells.Add(new NullImageCell());
            }
            return cells;
        }

        public static ImageCell CreateCell()
        {
            return new NullImageCell();
        }

        public static ImageCell CreateCell(string sopInstanceUid)
        {
            return new FilmingImageCell(sopInstanceUid);
        }
    }
}