//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Collections.ObjectModel;

//using UIH.Mcsf.Viewer;

//namespace UIH.Mcsf.Filming
//{
//    public class StudyData //: NotifyPropertyChangedObject
//    {
//        private string _studyUid;
//        private string _studyDesc;
//        private string _modality;
//        private List<SeriesData> _seriesItems;

//        public StudyData()
//        {
//            _seriesItems = new List<SeriesData>();
//        }

//        public StudyData(string studyUid)
//            :this()
//        {
//            this._studyUid = studyUid;
//        }

//        public StudyData(string studyUid, string studyDesc)
//            :this(studyUid)
//        {
//            this._studyDesc = studyDesc;
//        }

//        public List<SeriesData> SeriesItems
//        {
//            get { return _seriesItems; }
//        }

//        public string PatientID { get; set; }

//        public string PatientName { get; set; }

//        public string PatientSex { get; set; }

//        public string PatientBirthday { get; set; }

//        public string PatientStudyAge { get; set; }

//        public DateTime? StudyDate { get; set; }

//        public string AccessionNumber { get; set; }

//        public string Modality
//        {
//            get { return _modality; }
//            set
//            {
//                _modality = value;
//                //OnPropertyChanged("Modality");
//            }
//        }

//        public string Description
//        {
//            get { return _studyDesc; }
//            set
//            {
//                _studyDesc = value;
//                //OnPropertyChanged("Description");
//            }
//        }

//        public string StudyUid
//        {
//            get { return _studyUid; }
//            set { _studyUid = value; }
//        }

//        public override string ToString()
//        {
//            return "PatientID=" + PatientID + ";PatientName=" + PatientName + ";AccessionNumber=" + AccessionNumber;
//        }
//    }
//}
