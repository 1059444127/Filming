using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.XPath;
using UIH.Mcsf.Database;
using UIH.Mcsf.Filming.Configure;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.MainFrame;
using UIH.Mcsf.Pipeline.Data;
using UIH.Mcsf.Viewer;
using UIH.Mcsf.Pipeline.Dictionary;
using System.ComponentModel;

namespace UIH.Mcsf.Filming
{
    public sealed class FilmingPageTitle : INotifyPropertyChanged
    {
        private readonly MedViewerControl _medViewerControl;
        public FilmingPageTitle(MedViewerControl medViewerControl)
        {
            _medViewerControl = medViewerControl;
            SetElementDisplay();
        }

        private string _patientNameFlag = "1";
        private string _patientIdFlag = "1";
        private string _studyIdFlag = "0";
        private string _ageFlag = "1";
        private string _sexFlag = "1";
        private string _studyDateFlag = "1";
        private string _hospitalInfoFlag = "1";
        private string _operatorNameFlag = "0";
        private Double _displayFont = 10 ;
        private string _displayPosition = "1";
        private string _commentFlag = "1";
        private string _pageNoFlag = "0";
        private string _accessionNoFlag = "1";
        private string _logoFlag = "1";

        public Visibility PatientNameVisibility
        {
            get
            {
                if (_patientNameFlag == "1")
                {
                    return Visibility.Visible;
                }
                if (AccessionNoVisibility == Visibility.Visible && (_ageFlag == "1" || _sexFlag == "1"))
                {
                    return Visibility.Visible;
                }
                return Visibility.Hidden;
            }
        }

        public Visibility PatientIDVisibility
        {
            get
            {
                if (_patientIdFlag == "1")
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
        }

        public Visibility StudyIDVisibility
        {
            get
            {
                if (_studyIdFlag == "1")
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
        }
        public Visibility PatientSexAndAgeVisibility
        {
            get
            {

                if ((_ageFlag != "1") && (_sexFlag != "1") || (Visibility.Visible == AccessionNoVisibility))
                    return Visibility.Hidden;

                return Visibility.Visible;
            }
        }

        public Visibility AgeVisibility
        {
            get
            {
                if (_ageFlag == "1")
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
        }

        public Visibility SexVisibility
        {
            get
            {
                if (_sexFlag == "1")
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
        }

        public Visibility StudyDateVisibility
        {
            get
            {
                if (_studyDateFlag == "1")
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
        }

        public Visibility CommentVisibility
        {
            get
            {
                if(_commentFlag=="1")
                    return Visibility.Visible;
                
                return Visibility.Hidden;
                
            }
                
        }
        public Visibility PageNoVisibility
        {
            get
            {
                if(_pageNoFlag=="1" )
                    return Visibility.Visible;
                
                return Visibility.Hidden;
                
            }
        }
        public Visibility AccessionNoVisibility
        {
            get
            {
                if (_accessionNoFlag == "1" && AccessionNo != string.Empty)
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }
        public Visibility HospitalInfoVisibility
        {
            get
            {
                if (_hospitalInfoFlag == "1")
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
        }
        public Visibility OperatorNameVisibility
        {
            get
            {
                if (_operatorNameFlag == "1") 
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }
		public Visibility HospitalInfoAndOperatorNameVisibility
        {
            get
            {
                if ((_hospitalInfoFlag !="1") && (_operatorNameFlag !="1"))
                   return Visibility.Hidden;
                
                return Visibility.Visible;
            }
            
        }

        public Visibility LogoVisibility
        {
            get
            {
                if (_logoFlag == "1")
                    return Visibility.Visible;
                return Visibility.Hidden;
            }
        }

        public Double DisplayFont
        {
            get
            {
                return _displayFont;
            }
        }
        public string DisplayPosition
        {
            get { return _displayPosition; }
        }
        public bool IsMixed
        {
            get { return StudyInstanceUid == FilmingHelper.StarsString; }
        }

        public string PatientName
        {
            get
            {
                string result = string.Empty;
				//todo: performance optimization begin pageTitle
               //Old--Start
               // result += GetPatientInfo("PatientsName");
               //Old--End 

               //New--Start
               result += GetPatientInfo(ServiceTagName.PatientName);
               //New--End
			   	//todo: performance optimization end
                return result;
            }
        }

        public string PatientNameAndAgeAndSex
        {
            get
            {
                string result = string.Empty;
                if (_patientNameFlag == "1") result += PatientName;
                if (result != FilmingHelper.MixedPatientName && AccessionNoVisibility == Visibility.Visible)
                {
                    if (SexVisibility == Visibility.Visible)
                    {
                        var sex = GetPatientInfo(ServiceTagName.PatientSex, FilmingHelper.EmptyString, FilmingHelper.EmptyString);
                        result = result + "  " + sex ;
                    }
                    if (AgeVisibility == Visibility.Visible)
                    {
                        string age = GetPatientInfo(ServiceTagName.PatientAge, FilmingHelper.EmptyString, FilmingHelper.StarsString);
                        age = age.TrimStart('0');
                        result = result + "  "+age;
                    }
                }
                result = ProcessLongPatientName(result);
                return result;
            }
        }

        public string PatientNameAndAgeAndSexToolTip
        {
            get
            {
                string result = string.Empty;
                if (_patientNameFlag == "1") result += PatientName;
                if (result != FilmingHelper.MixedPatientName && AccessionNoVisibility == Visibility.Visible)
                {
                    if (SexVisibility == Visibility.Visible)
                    {
                        var sex = GetPatientInfo(ServiceTagName.PatientSex, FilmingHelper.EmptyString, FilmingHelper.EmptyString);
                        result = result + "  " + sex ;
                    }
                    if (AgeVisibility == Visibility.Visible)
                    {
                        string age = GetPatientInfo(ServiceTagName.PatientAge, FilmingHelper.EmptyString, FilmingHelper.StarsString);
                        age = age.TrimStart('0');
                        result = result + "  "+age;
                    }
                }
                return result;
            }
        }

        public string StudyDate
        {
            get
            {
                string dateString, timeString;
                string dateFormatString;//, timeFormatString;
                dateString = string.Empty;
                timeString = string.Empty;
                dateFormatString = string.Empty;
                //timeFormatString = string.Empty;
                try
                {
                    if (PatientName == FilmingHelper.MixedPatientName)
                        return FilmingHelper.MixedPatientName;
					//todo: performance optimization begin pageTitle	
                    //Old--Start
                    //dateString = GetPatientInfo("StudyDate", string.Empty, string.Empty);
                    //Old--End

                    //New Start
                    dateString = FinalPatientInfo.StudyDate;
                    //New End
					//todo: performance optimization end
                    dateFormatString = dateString == string.Empty ? string.Empty : string.Format("{0}/{1}/{2}", dateString.Substring(0, 4), dateString.Substring(4, 2), dateString.Substring(6, 2)
                        );
                    
                }
                catch (Exception)
                {
                    Logger.LogWarning("StudyDate : " + dateString + " " + timeString);
                }
                return dateFormatString;
            }
        }

        public string AcquisitionDateTime
        {
            get
            {
                string dateString, timeString;
                string dateFormatString;//, timeFormatString;
                dateString = string.Empty;
                timeString = string.Empty;
                dateFormatString = string.Empty;
               
                try
                {
                    if (PatientName == FilmingHelper.MixedPatientName)
                        return FilmingHelper.MixedPatientName;
					//todo: performance optimization begin pageTitle
                    //Old Start
                   // dateString = GetPatientInfo("AcquisitionDate", "", "");
                    //End Start

                    //New Start
                    dateString = FinalPatientInfo.AcquisitionDateTime;
                    //New End
					//todo: performance optimization end
                    dateFormatString = dateString == string.Empty ? null : string.Format("{0}/{1}/{2}", dateString.Substring(0, 4), dateString.Substring(4, 2), dateString.Substring(6, 2)
                        );
                   
                }
                catch (Exception)
                {
                    Logger.LogWarning("AcquisitionDateTime : " + dateString + " " + timeString);
                }
                return dateFormatString; // +" " + timeFormatString;
            }
        }

        public string PatientSexAndAge
        { 
            get
            {
                string result = string.Empty;
                if (SexVisibility == Visibility.Visible)
                    result = result + PatientSex + "   ";
                if (AgeVisibility == Visibility.Visible)
                    result = result + PatientAge;
                if (String.IsNullOrWhiteSpace(result))
                    return null;
                return result;
            }
        }
        public string PatientAge
        {
            get
            {
                if (PatientName == FilmingHelper.MixedPatientName)
                    return FilmingHelper.StarsString;
				//todo: performance optimization begin pageTitle	
                //Old Start
                //string age = GetPatientInfo("PatientsAge", FilmingHelper.EmptyString, FilmingHelper.StarsString);
                //Old End
                //New Start
                string age = FinalPatientInfo.PatientAge;
                //New End
				//todo: performance optimization end
                age = age.TrimStart('0');

                return AgeNLS_Converter(age);
            }
        }


        //性别多语
        private string GenderNLS_Converter(string genderDICOM)
        {
            if (genderDICOM.ToLower() == "mixed" || genderDICOM == string.Empty) return genderDICOM;

            string nlsGender = genderDICOM;

            if (genderDICOM.ToUpper() == "F" || genderDICOM.ToLower() == "female")
            {
                nlsGender = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Gender_Female"] as string;
            }
            else if (genderDICOM.ToUpper() == "M" || genderDICOM.ToLower() == "male")
            {
                nlsGender = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Gender_Male"] as string;
            }
            else if (genderDICOM.ToUpper() == "O" || genderDICOM.ToLower() == "other")
            {
                nlsGender = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Gender_Other"] as string;
            }
            else
            {
                nlsGender = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Gender_Unknown"] as string;
            }

            return nlsGender;
        }
        public string PatientSex
        {
            get
            {
                if (PatientName == FilmingHelper.MixedPatientName)
                    return FilmingHelper.MixedPatientName;
                return GenderNLS_Converter(FinalPatientInfo.PatientSex);
            }
        }


        //年龄多语
        private string AgeNLS_Converter(string patientAge)
        {
            string newAgeWithSuffix = patientAge;
            if (string.IsNullOrEmpty(patientAge))
            {
                return "";
            }

            if (FilmingViewerContainee.FilmingResourceDict == null)
            {
                return patientAge;
            }

            if (patientAge.Contains("Y"))
            {
                string yearSuffix = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Ages_Year"] as string;
                newAgeWithSuffix = patientAge.Replace("Y", yearSuffix);
            }
            else if (patientAge.Contains("M"))
            {
                string yearSuffix = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Ages_Month"] as string;
                newAgeWithSuffix = patientAge.Replace("M", yearSuffix);
            }
            else if (patientAge.Contains("W"))
            {
                string yearSuffix = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Ages_Week"] as string;
                newAgeWithSuffix = patientAge.Replace("W", yearSuffix);
            }
            else if (patientAge.Contains("D"))
            {
                string yearSuffix = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Ages_Day"] as string;
                newAgeWithSuffix = patientAge.Replace("D", yearSuffix);
            }

            return newAgeWithSuffix;
        }


        public string PatientBirthDate
        {
            get
            {
                if (PatientName == FilmingHelper.MixedPatientName)
                    return "";
                return FinalPatientInfo.PatientBirthDate;
            }
        }

        public string PatientId
        {
            get
            {
                if (PatientName == FilmingHelper.MixedPatientName)
                    return FilmingHelper.StarsString;
				//todo: performance optimization begin pageTitle	
                //Old Start
                //var pid = GetPatientInfo("PatientID", FilmingHelper.EmptyString, FilmingHelper.StarsString);
                //Old End
                //new Start
                var pid = FinalPatientInfo.PatientId;
                //new End
				//todo: performance optimization end
                if (!string.IsNullOrWhiteSpace(pid)) 
                    pid = "PID:" + pid;
                else 
                    return null;
                return pid;
            }
        }

       
        public string StudyInstanceUid
        {
            get
            {
                if (PatientName == FilmingHelper.MixedPatientName)
                    return FilmingHelper.StarsString;
				//todo: performance optimization begin pageTitle	
                //Old Start
                //return GetPatientInfo("StudyInstanceUID", FilmingHelper.EmptyString, FilmingHelper.StarsString);
                //Old End
                //New Start
                return FinalPatientInfo.StudyInstanceUid;
                //New End
				//todo: performance optimization end
            }
        }

        public string StudyId
        {
            get
            {
                if (PatientName == FilmingHelper.MixedPatientName)
                    return FilmingHelper.StarsString;
				//todo: performance optimization begin pageTitle	
                //Old Start
                //return GetPatientInfo("StudyID", FilmingHelper.EmptyString, FilmingHelper.StarsString);
                //Old End

                //New Start
                return FinalPatientInfo.StudyId;
                //New End
				//todo: performance optimization end
            }
        }

        private string _pageNo;
        public string PageNo
        {
            get { return _pageNo; }
            set
            {
                if (value != _pageNo && value != "")
                    _pageNo = value;
            }
        }

        public string Comment { get; set; }
        public string AccessionNo
        {
            get
            {
                if (PatientName == FilmingHelper.MixedPatientName)
                    return "Acc Num:"+FilmingHelper.StarsString;
				//todo: performance optimization begin pageTitle	
                //Old Start
                //var accNum = GetPatientInfo("AccessionNumber", FilmingHelper.EmptyString, FilmingHelper.StarsString);
                //Old End
                //New Start
                var accNum = FinalPatientInfo.AccessionNo;
                //New End
				//todo: performance optimization end
                if (!string.IsNullOrWhiteSpace(accNum))
                    accNum = "Acc Num:" + accNum;
                return accNum;
            }
        }

        public string InstitutionName
        {
            get
            {
                //同一序列的图片有的有InstitutionName，有的没有，那么医院信息应该显示，而不是"Mixed"
                string sInstitutionName = GetPatientInfo(ServiceTagName.InstitutionName);
                if (sInstitutionName.Equals(FilmingHelper.MixedPatientName))
                {
                    IEnumerable<MedViewerControlCell> imageCells = _medViewerControl.Cells.Where(cell => !cell.IsEmpty);
                    if (imageCells.Count() > 0)
                    {
                        string sCellStuID = imageCells.FirstOrDefault().StudyIdOfCell();
                        if (!imageCells.Any(cell => cell.StudyIdOfCell() != sCellStuID))//同一序列
                        {
                            MedViewerControlCell imageCell = imageCells.FirstOrDefault(
                                cell =>
                                cell.Image.CurrentPage.ImageHeader.DicomHeader.ContainsKey(ServiceTagName.InstitutionName));
                            sInstitutionName = imageCell.Image.CurrentPage.ImageHeader.DicomHeader[ServiceTagName.InstitutionName];
                        }
                    }
                }
                return sInstitutionName;
            }
        }

        public string OperatorName
        {
            get
            {
				//todo: performance optimization begin pageTitle
                //Old Start
                //return GetPatientInfo("OperatorsName");
                //Old End
                //New Start
                return FinalPatientInfo.OperatorName;
                //New End
				//todo: performance optimization end
            }
        }

        public string InstitutionAndOperatorName
        {
            get
            {
                string result = string.Empty;
                if (HospitalInfoVisibility == Visibility.Visible)
                {
                    if (!string.IsNullOrEmpty(InstitutionName))
                    {
                        result = InstitutionName + "   ";
                    }
                }

                if (OperatorNameVisibility == Visibility.Visible)
                {
                    if (!string.IsNullOrEmpty(OperatorName))
                    {
                        result = result + OperatorName;
                    }
                }
                return result;
            }
        }

        public string Manufacturer
        {
            get
            {
				//todo: performance optimization begin pageTitle
                //Old Start
                //return GetPatientInfo("Manufacturer");
                //Old End
                //New Start
                return FinalPatientInfo.Manufacturer;
                //New End
				//todo: performance optimization end
            }
        }

        public string EFilmSeriesUid { get; set; }

        public string SeriesUid
        {
            get
            {
                if (PatientName == FilmingHelper.MixedPatientName)
                    return FilmingHelper.StarsString;
				//todo: performance optimization begin pageTitle	
                //Old Start
                //return GetPatientInfo("SeriesInstanceUID", FilmingHelper.EmptyString, FilmingHelper.StarsString);
                //Old End
                //New Start
                return FinalPatientInfo.SeriesInstanceUID;
                //New End
				//todo: performance optimization end
            }

        }

        

        public string FilmingPageModality
        {
            get
            {
			   //todo: performance optimization begin pageTitle
               //Old Start
                //var ret = GetPatientInfo("Modality", "OT", "OT");
                //Old End
                //New Start
                var ret = FinalPatientInfo.Modality;
                //New End
				//todo: performance optimization end
                if (ret == string.Empty) return "OT";
                return ret;
            }
        }

        private string ProcessLongPatientName(string patientName)
        {
            try
            {
                if (patientName.Length == PatientName.Length) return patientName;
                if (Math.Abs(DisplayFont - 15) < 0.0001)
                {
                    if (patientName.Length > 21)
                    {
                        var reststring = patientName.Remove(0, PatientName.Length);
                        var name = PatientName.Substring(0, 13);
                        patientName = name + "..." + reststring;
                    }
                }
                if (Math.Abs(DisplayFont - 10) < 0.0001)
                {
                    if (patientName.Length > 38)
                    {
                        var reststring = patientName.Remove(0, PatientName.Length);
                        var name = PatientName.Substring(0,29);
                        patientName = name + "..." + reststring;
                    }
                }
                if (Math.Abs(DisplayFont - 5) < 0.0001)
                {
                    if (patientName.Length > 50)
                    {
                        var reststring = patientName.Remove(0, PatientName.Length);
                        var name = PatientName.Substring(0,41);
                        patientName = name + "..." + reststring;
                    }
                }
                return patientName;
            }
            catch (Exception )
            {
                return patientName;
            }
        }

        private string GetPatientInfo(uint tag, string defaultValue = FilmingHelper.EmptyString, string mixValue = FilmingHelper.MixedPatientName)
        {
            return GetPatientInfo(_medViewerControl.Cells, tag, defaultValue, mixValue);
        }

        public string GetPatientInfo(IEnumerable<MedViewerControlCell> cells, uint tag, string defaultValue = FilmingHelper.EmptyString, string mixValue = FilmingHelper.MixedPatientName)
        {
            if (cells == null) return string.Empty;

            var temp = !cells.Any() ? "" : defaultValue;
            bool isFirstImageCellAccessed = true;

            foreach (var cell in cells)
            {
                if (cell.Image != null && cell.Image.CurrentPage != null)
                {
                    var dicomHeader = cell.Image.CurrentPage.ImageHeader.DicomHeader;

                    if (dicomHeader.ContainsKey(tag))
                    {
                        string tagInfo = FilmingHelper.GetDicomHeaderInfoByTagName(dicomHeader, tag);
                        if (ServiceTagName.PatientAge==tag)
                        {
                            tagInfo = tagInfo.TrimStart('0');
                        }
                        temp = ((temp == defaultValue && isFirstImageCellAccessed || temp == tagInfo) ? tagInfo : mixValue);
                    }
                    else
                    {
                        temp = ((temp == defaultValue || temp=="") ? defaultValue : mixValue);
                    }

                    if (temp == mixValue)
                        break;

                    isFirstImageCellAccessed = false;
                }
            }
            return temp ?? string.Empty;
        }
        
		//todo: performance optimization begin pageTitle
        private ImagePatientInfo FinalPatientInfo=new ImagePatientInfo();
        public void UpdatePatientInfo(IEnumerable<MedViewerControlCell> cells)
        {
            FinalPatientInfo.Clear();
            bool first = true;
            foreach (var cell in cells)
            {
                if (cell.Image != null && cell.Image.CurrentPage != null)
                {
                    var pi = cell.Image.CurrentPage.Tag as ImagePatientInfo;
                    if (pi != null)
                    {
                        FinalPatientInfo.CompareAndUpdateValue(pi, first);
                    }
                    first = false;
                }
            }
        }

        //public string GetPatientAge()
        //{
        //    if (cells == null) return string.Empty;

        //    var temp = !cells.Any() ? "" : defaultValue;
        //    bool isFirstImageCellAccessed = true;
        //    foreach (var cell in cells)
        //    {
        //        if (cell.Image != null && cell.Image.CurrentPage != null)
        //        {
        //            var pi = cell.Image.CurrentPage.Tag as PatientInfo;

        //            if (pi != null)
        //            {
        //                string tagInfo = pi.PatientName;
        //                temp = ((temp == defaultValue && isFirstImageCellAccessed || temp == tagInfo) ? tagInfo : mixValue);
        //            }
        //            else
        //            {
        //                temp = ((temp == defaultValue || temp == "") ? defaultValue : mixValue);
        //            }

        //            if (temp == mixValue)
        //                break;

        //            isFirstImageCellAccessed = false;
        //        }
        //    }
        //    return temp ?? string.Empty;
        //}
        //private string GetPatientInfo(string tagName)
		//todo: performance optimization end

        public event PropertyChangedEventHandler PropertyChanged;



        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        //only for PT
        public event EventHandler DrugInfoChangedEvent;
        public void PatientInfoChanged()
        {
            try
            {
                Logger.LogFuncUp();
                OnPropertyChanged(new PropertyChangedEventArgs("PatientName"));
                OnPropertyChanged(new PropertyChangedEventArgs("PatientAge"));
                OnPropertyChanged(new PropertyChangedEventArgs("PatientSex"));
                OnPropertyChanged(new PropertyChangedEventArgs("PatientNameAndAgeAndSex"));
                OnPropertyChanged(new PropertyChangedEventArgs("PatientNameAndAgeAndSexToolTip"));
                OnPropertyChanged(new PropertyChangedEventArgs("PatientSexAndAge"));
                OnPropertyChanged(new PropertyChangedEventArgs("PatientId"));
                OnPropertyChanged(new PropertyChangedEventArgs("StudyId"));
                OnPropertyChanged(new PropertyChangedEventArgs("StudyDate"));
                //OnPropertyChanged(new PropertyChangedEventArgs("AcquisitionDateTime"));
                OnPropertyChanged(new PropertyChangedEventArgs("Manufacturer"));
                OnPropertyChanged(new PropertyChangedEventArgs("InstitutionName"));
                OnPropertyChanged(new PropertyChangedEventArgs("OperatorName"));
                OnPropertyChanged(new PropertyChangedEventArgs("AccessionNo"));
                OnPropertyChanged(new PropertyChangedEventArgs("InstitutionAndOperatorName"));      
                OnPropertyChanged(new PropertyChangedEventArgs("PatientSexAndAgeVisibility"));
                OnPropertyChanged(new PropertyChangedEventArgs("PatientNameVisibility"));
                OnPropertyChanged(new PropertyChangedEventArgs("PatientIDVisibility"));
                OnPropertyChanged(new PropertyChangedEventArgs("StudyDateVisibility"));
                OnPropertyChanged(new PropertyChangedEventArgs("HospitalInfoAndOperatorNameVisibility"));                
                OnPropertyChanged(new PropertyChangedEventArgs("CommentVisibility"));
                OnPropertyChanged(new PropertyChangedEventArgs("PageNoVisibility"));
                OnPropertyChanged(new PropertyChangedEventArgs("AccessionNoVisibility"));

                if (null != DrugInfoChangedEvent)
                {
                    DrugInfoChangedEvent(this, new EventArgs());
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        private void FillDicomFileMetaInformationTags(DicomAttributeCollection dataHeader)
        {
            //string strLen = GetPatientInfo("PatientsBirthTime", string.Empty, string.Empty);
            //UInt32 nLen = Convert.ToUInt32(strLen);
            //FilmingHelper.InsertUInt32DicomElement(dataHeader, Tag.FileMetaInformationGroupLength, nLen);
            //byte[] btVersion = new byte[1];
            //btVersion[0] = 55;
            //FilmingHelper.InsertBytesDicomElement(dataHeader, Tag.FileMetaInformationVersion, btVersion);
            //FilmingHelper.InsertUInt16DicomElement(dataHeader, Tag.MediaStorageSOPClassUID, 0);
            //FilmingHelper.InsertUInt16DicomElement(dataHeader, Tag.MediaStorageSOPInstanceUID, 0);
            //FilmingHelper.InsertUInt16DicomElement(dataHeader, Tag.TransferSyntaxUID, 0);//"1.2.840.10008.1.2.1"
            //FilmingHelper.InsertUInt16DicomElement(dataHeader, Tag.ImplementationClassUID, 0);
            //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.ImplementationVersionName, string.Empty);
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SpecificCharacterSet, "ISO.IR 192");
        }

        private void FillPatientModuleTags(DicomAttributeCollection dataHeader)
        {
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsName, PatientName);
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientID, PatientId);
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsBirthDate, PatientBirthDate);
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsSex, PatientSex);
            uint PatientBirthTime = 0x00100032;
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsBirthTime, GetPatientInfo(PatientBirthTime, string.Empty, string.Empty));
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientComments, string.Empty);

            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsAge, GetPatientInfo(ServiceTagName.PatientAge, string.Empty, string.Empty));
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.InstitutionName, InstitutionName);
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.Manufacturer, Manufacturer);


        }

        private void FillGeneralStudyModuleTags(DicomAttributeCollection dataHeader)
        {
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.StudyInstanceUid, StudyInstanceUid);
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.StudyDate, GetPatientInfo(ServiceTagName.StudyDate, string.Empty, string.Empty));
            uint StudyTime = 0x00080030;
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.StudyTime, GetPatientInfo(StudyTime, string.Empty, string.Empty));
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.ReferringPhysiciansName, GetPatientInfo(ServiceTagName.ReferringPhysicianName, string.Empty, string.Empty));
            //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.StudyId, StudyId);
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.AccessionNumber, GetPatientInfo(ServiceTagName.AccessionNumber, string.Empty, string.Empty));
            //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.StudyDescription, GetPatientInfo("StudyDescription", string.Empty, string.Empty));

            var now = DateTime.Now;
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.AcquisitionDate, now.Date.ToShortDateString());
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.AcquisitionTime, now.TimeOfDay.ToString());
        }

        private void FillPatientStudyModuleTags(DicomAttributeCollection dataHeader)
        {
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsAge, GetPatientInfo(ServiceTagName.PatientAge, string.Empty, string.Empty));
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsSize, GetPatientInfo(ServiceTagName.PatientSize, string.Empty, string.Empty));
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientsWeight, GetPatientInfo(ServiceTagName.PatientWeight, string.Empty, string.Empty));
            uint Occupation = 0x00102180;
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.Occupation, GetPatientInfo(Occupation, string.Empty, string.Empty));
        }

        private void FillGeneralSeriesModuleTags(DicomAttributeCollection dataHeader)
        {
            //string str = GetPatientInfo("Modality", string.Empty, string.Empty);
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.Modality, FilmingPageModality);
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SeriesInstanceUid, EFilmSeriesUid);
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SeriesNumber, FilmingHelper.GetSerieNumber(StudyInstanceUid));
            uint Laterality = 0x00200060;
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.Laterality, GetPatientInfo(Laterality, string.Empty, string.Empty));
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SeriesDate, GetPatientInfo(ServiceTagName.SeriesDate, string.Empty, string.Empty));
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SeriesTime, GetPatientInfo(ServiceTagName.SeriesTime, string.Empty, string.Empty));
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.ProtocolName, GetPatientInfo(ServiceTagName.ProtocolName, string.Empty, string.Empty));

            string descreption = "Electronic film_" + string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SeriesDescription, descreption);

            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.BodyPartExamined, GetPatientInfo(ServiceTagName.BodyPartExamined, string.Empty, string.Empty));
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientPosition, GetPatientInfo(ServiceTagName.PatientPosition, string.Empty, string.Empty));
        }

        private void FillSCEquipmentModuleTags(DicomAttributeCollection dataHeader)
        {
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.ConversionType, "WSD");
            //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.Modality, "SC");
            //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SecondaryCaptureDeviceID, GetPatientInfo(ServiceTagName.SecondaryCaptureDeviceID, string.Empty, string.Empty));
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SecondaryCaptureDeviceManufacturer, FilmingUtility.SecondaryCaptureDeviceManufacturer);
            //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SecondaryCaptureDeviceManufacturersModelName,
                                                               //GetPatientInfo(ServiceTagName.SecondaryCaptureDeviceManufacturersModelName, string.Empty, string.Empty));
            //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SecondaryCaptureDeviceSoftwareVersions,
                                                               //GetPatientInfo(ServiceTagName.SecondaryCaptureDeviceSoftwareVersions, string.Empty, string.Empty));
        }

        private void FillGeneralImageModuleTags(DicomAttributeCollection dataHeader, int filmPageIndex)
        {
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.InstanceNumber, Convert.ToString(filmPageIndex + 1));
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.PatientOrientation, GetPatientInfo(ServiceTagName.PatientOrientation, string.Empty, string.Empty));

            const string imageType = FilmingUtility.EFilmImageType;
            var imageTypes = imageType.Split('\\');
            FilmingHelper.InsertStringArrayDicomElement(dataHeader, Tag.ImageType, imageTypes);
            uint AcquisitionNumber = 0x00200012;
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.AcquisitionNumber, GetPatientInfo(AcquisitionNumber, string.Empty, string.Empty));
            //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.AcquisitionDate, GetPatientInfo("AcquisitionDate", string.Empty, string.Empty));
            //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.AcquisitionTime, GetPatientInfo("AcquisitionTime", string.Empty, string.Empty));
            //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.AcquisitionDatetime, AcquisitionDateTime);
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.ImagesInAcquisition, GetPatientInfo(ServiceTagName.ImagesInAcquisition, string.Empty, string.Empty));
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.ImageComments, GetPatientInfo(ServiceTagName.ImageComments, string.Empty, string.Empty));
            uint BurnedInAnnotation = 0x00280301;
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.BurnedInAnnotation, GetPatientInfo(BurnedInAnnotation, string.Empty, string.Empty));
            //if Modality is not MR, Set RescaleIntercept to 0 and set RescaleSlope to 1
            //if ("MR" != FilmingPageModality)
            {
                FilmingHelper.InsertStringDicomElement(dataHeader, Tag.RescaleIntercept, "0");
                FilmingHelper.InsertStringDicomElement(dataHeader, Tag.RescaleSlope, "1");
            }
        }

        private void FillVOILUTMacroTags(DicomAttributeCollection dataHeader)
        {
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.WindowCenter, "127");
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.WindowWidth, "256");
        }

        private void FillSCImageModuleTags(DicomAttributeCollection dataHeader)
        {
            var time = DateTime.Now;
            var dateOfSecondaryCapture = string.Format("{0:D4}{1:D2}{2:D2}", time.Year, time.Month, time.Day);
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.DateOfSecondaryCapture, dateOfSecondaryCapture);

            var timeOfSecondaryCapture = string.Format("{0:D2}{1:D2}{2:D2}", time.Hour, time.Minute, time.Second);
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.TimeOfSecondaryCapture, timeOfSecondaryCapture);
        }

        private void FillSopCommonModuleTags(DicomAttributeCollection dataHeader)
        {
            var uidManager = McsfDatabaseDicomUIDManagerFactory.Instance().CreateUIDManager();

            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SOPClassUID, "1.2.840.10008.5.1.4.1.1.7");
            //string str = uidManager.CreateImageUID("");
            string str = string.Empty;
            if (_medViewerControl.Cells.Count() > 0)
            {
                MedViewerControlCell cell = _medViewerControl.Cells.First(n => n.Image != null && n.Image.CurrentPage != null && n.Image.CurrentPage.SOPInstanceUID != null);
                str = cell.Image.CurrentPage.SOPInstanceUID;
            } 
            else
            {
                str = uidManager.CreateImageUID("");
            }
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SOPInstanceUID, str);

            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.SpecificCharacterSet, "127");
            //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.InstanceCreationDate, "256");
            //FilmingHelper.InsertStringDicomElement(dataHeader, Tag.InstanceCreationTime, "256");
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.InstanceCreatorUID, "256");
            FilmingHelper.InsertStringDicomElement(dataHeader, Tag.InstanceNumber, "256");
        }

        public DicomAttributeCollection AssembleSendData(byte[] imageData, double imageWidth, double imageHeight, int filmPageIndex,bool ifSaveImageAsGreyScale = true)
        {
            try
            {
                Logger.LogFuncUp();

                var dataHeader = new DicomAttributeCollection();

                //DICOM File Meta Information
                FillDicomFileMetaInformationTags(dataHeader);

                //Patient Module
                FillPatientModuleTags(dataHeader);

                //General Study Module
                FillGeneralStudyModuleTags(dataHeader);

                //Patient Study Module
                FillPatientStudyModuleTags(dataHeader);

                //General Series Module
                FillGeneralSeriesModuleTags(dataHeader);

                //General Equipment Module

                //SC Equipment Module
                FillSCEquipmentModuleTags(dataHeader);

                //General Image Module
                FillGeneralImageModuleTags(dataHeader, filmPageIndex);

                //Image Pixel Module
                FilmingHelper.AddConstDICOMAttributes(dataHeader, ifSaveImageAsGreyScale);
                FilmingHelper.InsertUInt16DicomElement(dataHeader, Tag.Columns, (ushort)imageWidth);
                FilmingHelper.InsertUInt16DicomElement(dataHeader, Tag.Rows, (ushort)imageHeight);
                var element = DicomAttribute.CreateAttribute(Tag.PixelData, VR.OB);
                if (!element.SetBytes(0, imageData))
                {
                    Logger.LogWarning("Failed to Insert NULL  image Data to Data header");
                }
                dataHeader.AddDicomAttribute(element);

                //SC Image Module
                FillSCImageModuleTags(dataHeader);

                //VOI LUT Module
                FillVOILUTMacroTags(dataHeader);

                //SOP Common Module
                FillSopCommonModuleTags(dataHeader);
                
                Logger.LogFuncDown();
                return dataHeader;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        public delegate void UpdatePageTitleLayoutHandler();

        public event UpdatePageTitleLayoutHandler TitleVisibleChanged;
        public void SetElementDisplay()
        {
            var pageTitleConfigure = PageTitleConfigure.Instance;
            _displayFont = pageTitleConfigure.DisplayFont;
            _displayPosition = pageTitleConfigure.DisplayPosition;
            _patientNameFlag = pageTitleConfigure.PatientName;
            _patientIdFlag = pageTitleConfigure.PatientID;
            _logoFlag = pageTitleConfigure.Logo;
            //_studyIdFlag = 
            _ageFlag = pageTitleConfigure.Age;
            _sexFlag = pageTitleConfigure.Sex;
            _studyDateFlag = pageTitleConfigure.StudyDate;
            _hospitalInfoFlag = pageTitleConfigure.HospitalInfo;
            _operatorNameFlag = pageTitleConfigure.OperatorName;
            _commentFlag = pageTitleConfigure.Comment;
            _pageNoFlag = pageTitleConfigure.PageNo;
            _accessionNoFlag = pageTitleConfigure.AccessionNo;
            PatientInfoChanged();
            if (TitleVisibleChanged != null)
                TitleVisibleChanged();
        }


        #region [---Performance---]
        
        public void SerializedToXml(XmlNode parentNode)
        {
            if (null == parentNode || null == parentNode.OwnerDocument)
            {
                return;
            }

            OffScreenRenderXmlHelper.AppendChildNode(parentNode, OffScreenRenderXmlHelper.DISPLAY_FONT, this.DisplayFont.ToString());
            var displayPosition = string.IsNullOrEmpty(this.DisplayPosition) ? string.Empty : this.DisplayPosition;
            var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            if (filmingCard != null && filmingCard._filmingCardModality == FilmingUtility.EFilmModality)
                displayPosition = "0";
            OffScreenRenderXmlHelper.AppendChildNode(parentNode, OffScreenRenderXmlHelper.DISPLAY_POSITION, displayPosition);
            OffScreenRenderXmlHelper.AppendChildNode(parentNode, OffScreenRenderXmlHelper.PATIENT_NAME_FLAG, string.IsNullOrEmpty(this.PatientNameAndAgeAndSex) ? string.Empty : this.PatientNameAndAgeAndSex);
            OffScreenRenderXmlHelper.AppendChildNode(parentNode, OffScreenRenderXmlHelper.PATIENT_ID_FLAG, string.IsNullOrEmpty(this.PatientId) ? string.Empty : this.PatientId);
            OffScreenRenderXmlHelper.AppendChildNode(parentNode, OffScreenRenderXmlHelper.AGE_FLAG, string.IsNullOrEmpty(this.PatientAge) ? string.Empty : this.PatientAge);
            OffScreenRenderXmlHelper.AppendChildNode(parentNode, OffScreenRenderXmlHelper.SEX_FLAG, string.IsNullOrEmpty(this.PatientSex)?string.Empty : this.PatientSex);
            OffScreenRenderXmlHelper.AppendChildNode(parentNode, OffScreenRenderXmlHelper.STUDY_DATE_FLAG, string.IsNullOrEmpty(this.StudyDate)?string.Empty : this.StudyDate);
            OffScreenRenderXmlHelper.AppendChildNode(parentNode, OffScreenRenderXmlHelper.HOSPITAL_INFO_FLAG, string.IsNullOrEmpty(this.InstitutionAndOperatorName) ? string.Empty : this.InstitutionAndOperatorName);
            OffScreenRenderXmlHelper.AppendChildNode(parentNode, OffScreenRenderXmlHelper.COMMENT, string.IsNullOrEmpty(this.Comment) ? string.Empty : this.Comment);
            OffScreenRenderXmlHelper.AppendChildNode(parentNode, OffScreenRenderXmlHelper.PAGE_NO, string.IsNullOrEmpty(this.PageNo)?string.Empty : this.PageNo);
            OffScreenRenderXmlHelper.AppendChildNode(parentNode,OffScreenRenderXmlHelper.ACCESSION_NO,string.IsNullOrEmpty(this.AccessionNo)?string.Empty:this.AccessionNo);
            OffScreenRenderXmlHelper.AppendChildNode(parentNode, OffScreenRenderXmlHelper.FILMING_STUDY_INSTANCE_UID, string.IsNullOrEmpty(this.StudyInstanceUid) ? string.Empty : this.StudyInstanceUid);
        }

        #endregion
        //private void SetValueFromDicomString(string attrName,string attrValue)
        //{
        //    switch (attrName)
        //    {
        //        case "DisplayFont":
        //            _displayFont = Convert.ToDouble(attrValue) * FilmingUtility.FilmScale;
        //            break;
        //        case "DisplayPosition":
        //            _displayPosition = attrValue;
        //            break;
        //        case "PatientName":
        //            _patientNameFlag = attrValue;
        //            break;
        //        case "PatientID":
        //            _patientIdFlag = attrValue;
        //            break;
        //        case "StudyID":
        //            _studyIdFlag = attrValue;
        //            break;
        //        case "Age":
        //            _ageFlag = attrValue;
        //            break;
        //        case "Sex":
        //            _sexFlag = attrValue;
        //            break;
        //        case "StudyDate":
        //            _studyDateFlag = attrValue;
        //            break;
        //        case "HospitalInfo":
        //            _hospitalInfoFlag = attrValue;
        //            break;
        //    }
        //}
        //public void ParseFilmingPageConfig()
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();
        //        var doc = new XmlDocument();
        //        doc.Load(FilmingHelper.EntryFilmingPageConfigPath);
        //        var xNav = doc.CreateNavigator();
        //        var xPathIt = xNav.Select("//FilmingPage");
        //        //use the XPathNodeIterator to display the results
        //        if (xPathIt.Count > 0)
        //        {
        //            while (xPathIt.MoveNext())
        //            {
        //                var nodesNavigator = xPathIt.Current;
        //                var currentItem = nodesNavigator.SelectDescendants(XPathNodeType.All, false);

        //                while (currentItem.MoveNext())
        //                {
        //                    SetValueFromDicomString(currentItem.Current.Name, currentItem.Current.Value);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Logger.LogWarning("No titles found in catalog.");
        //        }
        //        Logger.LogFuncDown();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //    }
        //}
    }
}
