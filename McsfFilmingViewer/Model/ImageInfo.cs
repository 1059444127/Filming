using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Model
{
    class ImageInfo
    {
        public ImageInfo(MedViewerControlCell cell)
        {
            if (cell == null) return;
            var image = cell.Image; if (image == null) return;
            var page = image.CurrentPage; if (page == null) return;
            var imageHeader = page.ImageHeader; if (imageHeader == null) return;
            var dicomHeader = imageHeader.DicomHeader; if (dicomHeader == null) return;

            if (dicomHeader.ContainsKey(ServiceTagName.StudyInstanceUID)) StudyInstanceUID = dicomHeader[ServiceTagName.StudyInstanceUID];
            if (dicomHeader.ContainsKey(ServiceTagName.PatientName)) PatientName = dicomHeader[ServiceTagName.PatientName];
            if (dicomHeader.ContainsKey(ServiceTagName.PatientID)) PatientID = dicomHeader[ServiceTagName.PatientID];
            if (dicomHeader.ContainsKey(ServiceTagName.StudyID)) StudyID = dicomHeader[ServiceTagName.StudyID];
            if (dicomHeader.ContainsKey(ServiceTagName.PatientBirthDate)) Birthday = dicomHeader[ServiceTagName.PatientBirthDate];
            if (dicomHeader.ContainsKey(ServiceTagName.PatientSex)) Sex = dicomHeader[ServiceTagName.PatientSex];
            if (dicomHeader.ContainsKey(ServiceTagName.PatientAge)) Age = dicomHeader[ServiceTagName.PatientAge];
        }

        #region	[--Properties--]

        private string _studyInstanceUID = string.Empty;
        public string StudyInstanceUID
        {
            get { return _studyInstanceUID; }
            set { _studyInstanceUID = value; }
        }

        private string _patientName = string.Empty;
        public string PatientName
        {
            get { return _patientName; }
            set { _patientName = value; }
        }

        private string _patientID = string.Empty;
        public string PatientID
        {
            get { return _patientID; }
            set { _patientID = value; }
        }

        private string _studyID = string.Empty;
        public string StudyID
        {
            get { return _studyID; }
            set { _studyID = value; }
        }

        private string _birthday = string.Empty;
        public string Birthday
        {
            get { return _birthday; }
            set { _birthday = value; }
        }

        private string _sex = string.Empty;
        public string Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }

        private string _age = string.Empty;
        public string Age
        {
            get { return _age; }
            set { _age = value; }
        }

        #endregion	[--Properties--]

    }

    class StudyInstanceUIDComparer : IEqualityComparer<ImageInfo>
    {
        public bool Equals(ImageInfo x, ImageInfo y)
        {
            if (x == null) return y == null;
            if (y == null) return false;

            return x.StudyInstanceUID == y.StudyInstanceUID;
        }

        public int GetHashCode(ImageInfo obj)
        {
            if (obj == null) return 0;
            return obj.StudyInstanceUID.GetHashCode();
        }
    }

    class PatientComparer : IEqualityComparer<ImageInfo>
    {
        public bool Equals(ImageInfo x, ImageInfo y)
        {
            if (x == null) return y == null;
            if (y == null) return false;

            return x.PatientName == y.PatientName
                && x.PatientID == y.PatientID
                && x.Birthday == y.Birthday
                && x.Sex == y.Sex;
        }

        public int GetHashCode(ImageInfo obj)
        {
            if (obj == null) return 0;
            string source = obj.PatientName + obj.PatientID + obj.Birthday + obj.Sex;
            return source.GetHashCode();
        }
    }
}
