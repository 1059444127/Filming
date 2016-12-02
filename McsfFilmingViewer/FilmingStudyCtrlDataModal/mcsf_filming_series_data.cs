////////////////////////////////////////////////////////////////////////////
///// Copyright, (c) Shanghai United Imaging Healthcare Inc., 2012 
///// All rights reserved. 
///// 
///// \author  Juanyong.Yang  juanyong.yang@united-imaging.com
////
///// \file    mcsf_review_series_data.cs
/////
///// \brief   the series data model
///// 
///// \version 1
///// \date    2012/02/22
///////////////////////////////////////////////////////////////////////////

//using System;
//using System.Collections.Generic;
//using System.Windows.Media;

//namespace UIH.Mcsf.Filming
//{
//    public class SeriesData
//    {
//        private readonly List<int> _selectedViewport = new List<int>();

//        private ReviewDataStatus _status = ReviewDataStatus.Initializing;

//        public SeriesData(string seriesUid)
//        {
//            _seriesUid = seriesUid;
//            Images = new List<ImageData>();
//        }

//        public SeriesData(String seriesUid, String seriesDesc, ImageSource source)
//            : this(seriesUid)
//        {
//            _seriesDesc = seriesDesc;
//            SeriesImage = source;
//        }

//        public List<ImageData> Images { get; set; }

//        private string _seriesUid = "";
//        public string SeriesInstanceUID
//        {
//            get { return _seriesUid; }
//            set
//            {
//                _seriesUid = value;
//            }
//        }

//        private string _studyUid = "";
//        public string StudyInstanceUID
//        {
//            get { return _studyUid; }
//            set
//            {
//                _studyUid = value;
//            }
//        }

//        private string _patientId = "";
//        public string PatientID
//        {
//            get { return _patientId; }
//            set
//            {
//                _patientId = value;
//            }
//        }

//        private string _modality = "";
//        public string Modality
//        {
//            get { return _modality; }
//            set { _modality = value; }
//        }

//        public string SeriesNumber { get; set; }

//        public ImageSource SeriesImage { get; set; }

//        private string _seriesDesc = "";
//        public string SeriesDescription
//        {
//            get { return _seriesDesc; }
//            set
//            {
//                _seriesDesc = value;
//            }
//        }

//        public bool IsSelected { get; set; }

//        public void SetViewportSelected(int index, bool isSelected)
//        {
//            if (isSelected)
//            {
//                if (!_selectedViewport.Contains(index))
//                    _selectedViewport.Add(index);
//            }
//            else
//            {
//                if (_selectedViewport.Contains(index))
//                    _selectedViewport.Remove(index);
//            }
//        }

//        public bool CanSupport3D { get; set; }

//        public int ClientNum { get; set; }

//        public ReviewDataStatus Status
//        {
//            get { return _status; }
//            set 
//            {
//                _status = value;
//            }
//        }

//        public override string ToString()
//        {
//            return "SeriesUID=" + SeriesInstanceUID + ";SeriesNumber" + SeriesNumber;
//        }
//    }
//}
