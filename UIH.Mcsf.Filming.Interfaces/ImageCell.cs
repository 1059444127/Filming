using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Interfaces
{
    public class ImageCell : ISelect
    {
        private DisplayData _displayData;
        private bool _isFocused;
        // TODO: ImageCell.DisplayDataChanged
        private bool _isSelected;

        public ImageCell()
        {
            DisplayData = DisplayDataFactory.Instance.CreateDisplayData();
        }

        // TODO-later: try 异步加载方式

        public ImageCell(string sopInstanceUid)
        {
            DisplayData = DisplayDataFactory.Instance.CreateDisplayData(sopInstanceUid);
        }

        public DisplayData DisplayData
        {
            get { return _displayData; }
            private set
            {
                if (_displayData == value) return;
                _displayData = value;
                FillImageInfo();
            }
        }

        private string GetTagByName(Dictionary<uint, string> dicomHeader, uint tag)
        {
            return !dicomHeader.ContainsKey(tag) ? string.Empty : dicomHeader[tag];
        }

        public event EventHandler<BoolEventArgs> SelectStatusChanged = delegate { };
        public event EventHandler<BoolEventArgs> FocusStatusChanged = delegate { };

        public void OnClicked(IClickStatus clickStatus)
        {
            Clicked(this, new ClickStatusEventArgs(clickStatus));
        }

        private void FillImageInfo()
        {
            Debug.Assert(_displayData != null);
            var imageHeader = _displayData.ImageHeader;
            if (imageHeader == null) return;
            var dicomHeader = imageHeader.DicomHeader;
            if (dicomHeader == null) return;

            StudyInstanceUID = GetTagByName(dicomHeader, ServiceTagName.StudyInstanceUID);
            StudyDate = GetTagByName(dicomHeader, ServiceTagName.StudyDate);
            AccessionNumber = GetTagByName(dicomHeader, ServiceTagName.AccessionNumber);

            PatientName = GetTagByName(dicomHeader, ServiceTagName.PatientName);
            PatientID = GetTagByName(dicomHeader, ServiceTagName.PatientID);
            PatientAge = GetTagByName(dicomHeader, ServiceTagName.PatientAge);
            PatientSex = GetTagByName(dicomHeader, ServiceTagName.PatientSex);
        }

        public static IList<ImageCell> CreateCells(int cellCount)
        {
            var cells = new List<ImageCell>();
            for (var i = 0; i < cellCount; i++)
            {
                cells.Add(new ImageCell());
            }
            return cells;
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

        #region Implementation of IImage

        public virtual string StudyInstanceUID { get; private set; }
        public virtual string StudyDate { get; private set; }
        public virtual string AccessionNumber { get; private set; }
        public virtual string PatientName { get; private set; }
        public virtual string PatientID { get; private set; }
        public virtual string PatientAge { get; private set; }
        public virtual string PatientSex { get; private set; }

        #endregion
    }
}