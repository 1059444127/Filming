using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.MainFrame;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming
{
    [Serializable]
    public class ImagePatientInfo
    {

        public string PatientName { get; set; }
        public string PatientAge { get; set; }
        public string PatientSex { get; set; }
        public string PatientBirthDate { get; set; }
        public string PatientId { get; set; }
        public string StudyInstanceUid { get; set; }
        public string AccessionNo { get; set; }
        public string InstitutionName { get; set; }
        public string OperatorName { get; set; }
        public string Manufacturer { get; set; }
        public string StudyId { get; set; }
        public string StudyDate { get; set; }
        public bool SamePatient { get; set; }
        public string AcquisitionDateTime { get; set;}
        public string SeriesInstanceUID { get; set;}
        public string Modality { get; set;}
        public ImagePatientInfo()
        {
            PatientName = "";
            PatientAge = "0";
            PatientSex = "";
            PatientBirthDate = "";
            StudyInstanceUid = "";
            AccessionNo = "";
            InstitutionName = "";
            OperatorName = "";
            Manufacturer = "";
            StudyId = "";
            StudyDate = "";
            AcquisitionDateTime = "";
            SeriesInstanceUID = "";
            Modality = "OT";
            SamePatient = true;
        }

        public void Clear()
        {
            PatientName = "";
            PatientAge = "0";
            PatientSex = "";
            PatientBirthDate = "";
            StudyInstanceUid = "";
            AccessionNo = "";
            InstitutionName = "";
            OperatorName = "";
            Manufacturer = "";
            StudyId = "";
            StudyDate = "";
            AcquisitionDateTime = "";
            SeriesInstanceUID = "";
            Modality = "OT";
            SamePatient = true;
            PatientId = "";
        }
        public void AppendPatientInfo(DisplayData displayData)
        {
            if (displayData.Tag != null)
                return;
            ImagePatientInfo pi = new ImagePatientInfo();
            pi.PatientName = FilmingHelper.GetDicomHeaderInfoByTagName(displayData.ImageHeader.DicomHeader,
                                                                       ServiceTagName.PatientName).Trim();
            pi.PatientAge = FilmingHelper.GetDicomHeaderInfoByTagName(displayData.ImageHeader.DicomHeader,
                                                                       ServiceTagName.PatientAge).Trim().TrimStart('0');
            pi.PatientSex = FilmingHelper.GetDicomHeaderInfoByTagName(displayData.ImageHeader.DicomHeader,
                                                                       ServiceTagName.PatientSex).Trim();
            pi.PatientBirthDate = FilmingHelper.GetDicomHeaderInfoByTagName(displayData.ImageHeader.DicomHeader,
                                                                       ServiceTagName.PatientBirthDate).Trim();
            pi.PatientId =
                FilmingHelper.GetDicomHeaderInfoByTagName(displayData.ImageHeader.DicomHeader, ServiceTagName.PatientID)
                    .Trim();
            pi.OperatorName = FilmingHelper.GetDicomHeaderInfoByTagName(displayData.ImageHeader.DicomHeader,
                                                                       ServiceTagName.OperatorsName).Trim();
            pi.InstitutionName = FilmingHelper.GetDicomHeaderInfoByTagName(displayData.ImageHeader.DicomHeader,
                                                                       ServiceTagName.InstitutionName).Trim();
            pi.AccessionNo = FilmingHelper.GetDicomHeaderInfoByTagName(displayData.ImageHeader.DicomHeader,
                                                                       ServiceTagName.AccessionNumber).Trim();
            pi.Manufacturer = FilmingHelper.GetDicomHeaderInfoByTagName(displayData.ImageHeader.DicomHeader,
                                                                       ServiceTagName.Manufacturer).Trim();
            pi.StudyInstanceUid = FilmingHelper.GetDicomHeaderInfoByTagName(displayData.ImageHeader.DicomHeader,
                                                                       ServiceTagName.StudyInstanceUID).Trim();
            pi.StudyId = FilmingHelper.GetDicomHeaderInfoByTagName(displayData.ImageHeader.DicomHeader,
                                                                      ServiceTagName.StudyID).Trim();
            pi.Modality = FilmingHelper.GetDicomHeaderInfoByTagName(displayData.ImageHeader.DicomHeader,
                                                                      ServiceTagName.Modality).Trim();
            pi.StudyDate = FilmingHelper.GetDicomHeaderInfoByTagName(displayData.ImageHeader.DicomHeader,
                                                                      ServiceTagName.StudyDate).Trim();
            pi.AcquisitionDateTime = FilmingHelper.GetDicomHeaderInfoByTagName(displayData.ImageHeader.DicomHeader,
                                                                     ServiceTagName.AcquisitionDate).Trim();
            pi.SeriesInstanceUID = FilmingHelper.GetDicomHeaderInfoByTagName(displayData.ImageHeader.DicomHeader,
                                                                    ServiceTagName.SeriesInstanceUID).Trim();
            displayData.Tag = pi;
        }

        //this function is mainly for update the final result 
        public void CompareAndUpdateValue(ImagePatientInfo pInfo, bool first)
        {

            PatientName = ProcessInfo(PatientName, pInfo.PatientName, first, FilmingHelper.MixedPatientName);

            if (PatientName == FilmingHelper.MixedPatientName)
            {
                SamePatient = false;
            }

            PatientAge = ProcessInfo(PatientAge, pInfo.PatientAge, first, FilmingHelper.StarsString);
            PatientSex = ProcessInfo(PatientSex, pInfo.PatientSex, first, FilmingHelper.StarsString);
            PatientBirthDate = ProcessInfo(PatientBirthDate, pInfo.PatientBirthDate, first, "");
            PatientId = ProcessInfo(PatientId, pInfo.PatientId, first, FilmingHelper.StarsString);
            StudyInstanceUid = ProcessInfo(StudyInstanceUid, pInfo.StudyInstanceUid, first, FilmingHelper.StarsString);
            AccessionNo = ProcessInfo(AccessionNo, pInfo.AccessionNo, first, FilmingHelper.StarsString);
            InstitutionName = ProcessInstitutionName(InstitutionName, pInfo.InstitutionName, first, FilmingHelper.StarsString);
            OperatorName = ProcessInfo(OperatorName, pInfo.OperatorName, first, FilmingHelper.StarsString);
            Manufacturer = ProcessInfo(Manufacturer, pInfo.Manufacturer, first, FilmingHelper.StarsString);
            StudyId = ProcessInfo(StudyId, pInfo.StudyId, first, FilmingHelper.StarsString);
            StudyDate = ProcessInfo(StudyDate, pInfo.StudyDate, first, FilmingHelper.StarsString);
            AcquisitionDateTime = ProcessInfo(AcquisitionDateTime, pInfo.AcquisitionDateTime, first, FilmingHelper.StarsString);
            SeriesInstanceUID = ProcessInfo(SeriesInstanceUID, pInfo.SeriesInstanceUID, first, FilmingHelper.StarsString);
            Modality = ProcessInfo(Modality, pInfo.Modality, first, "OT");
        }

     
        private string ProcessInfo(string str1,string str2,bool first,string mixValue)
        {
            
            string result="";
            if (str1 != str2 && !first)
                result = mixValue;
            else
            {
                result = str2;
            }
            return result;
        }
        private string ProcessInstitutionName(string str1,string str2,bool first,string mixValue)
        {
            string result = "";
            if (str1 != str2 && !first)
            {
                if (str2 != "")
                    result = str2;
            }
            else
            {
                result = str2;
            }
            return result;
        }
    }
}
