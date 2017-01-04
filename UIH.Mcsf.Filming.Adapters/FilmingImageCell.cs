using System.Collections.Generic;
using System.Diagnostics;
using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Adapters
{
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
}