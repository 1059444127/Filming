using System;
using System.Windows;
using UIH.Mcsf.Filming.Models;
using UIH.Mcsf.Filming.Widgets;

namespace UIH.Mcsf.Filming.ViewModels
{
    public class FilmTitleViewModel : Notifier
    {

        #region [--Notify Properties--]

        private int _displayFont;

        public int DisplayFont
        {
            get { return _displayFont; }
            set
            {
                if (_displayFont == value) return;
                _displayFont = value;
                NotifyPropertyChanged(() => DisplayFont);
            }
        }

        private string _patientName;

        public string PatientName
        {
            get { return _patientName; }
            set
            {
                if (_patientName == value) return;
                _patientName = value;
                NotifyPropertyChanged(() => PatientName);
            }
        }

        public Visibility PatientNameAndSexAndAgeVisibility
        {
            get 
            { 
                if(PatientNameVisibility == Visibility.Visible)
                    return Visibility.Visible;
                if(AccessionNoLableVisibility == Visibility.Visible)
                {
                    if (_config.SexFlag == "1" || _config.AgeFlag == "1")
                        return Visibility.Visible;
                }
                return Visibility.Hidden;
            }
        }

        private Visibility _patientNameVisibility;

        public Visibility PatientNameVisibility
        {
            get { return _patientNameVisibility; }
            set
            {
                if (_patientNameVisibility == value) return;
                _patientNameVisibility = value;
                NotifyPropertyChanged(() => PatientNameVisibility);
            }
        }

        private string _accessionNo;

        public string AccessionNo
        {
            get { return _accessionNo; }
            set
            {
                if (_accessionNo == value) return;
                _accessionNo = value;
                NotifyPropertyChanged(()=>AccessionNo);
                NotifyPropertyChanged(()=>AccessionNoLableVisibility);
            }
        }

        private Visibility _accessionNoVisibility;

        public Visibility AccessionNoVisibility
        {
            get { return _accessionNoVisibility; }
            set
            {
                if (_accessionNoVisibility == value) return;
                _accessionNoVisibility = value;
                NotifyPropertyChanged(()=> AccessionNoVisibility);
            }
        }

        private Visibility _accessionNoLableVisibility;
        public Visibility AccessionNoLableVisibility
        {
            get { return _accessionNoLableVisibility; }
            set
            {
                if(_accessionNoLableVisibility == value) return;
                _accessionNoLableVisibility = value;
                NotifyPropertyChanged(()=>AccessionNoLableVisibility);
            }
        }

        private Visibility _patientSexAndAgeLableVisibility;
        public Visibility PatientSexAndAgeLableVisibility
        {
            get { return _patientSexAndAgeLableVisibility; }
            set
            {
                if (_patientSexAndAgeLableVisibility == value) return;
                _patientSexAndAgeLableVisibility = value;
                NotifyPropertyChanged(() => PatientSexAndAgeLableVisibility);
            }
        }

        private string _patientSexAndAge;

        public string PatientSexAndAge
        {
            get { return _patientSexAndAge; }
            set
            {
                if (_patientSexAndAge == value) return;
                _patientSexAndAge = value;
                NotifyPropertyChanged(() => PatientSexAndAge);
            }
        }

        //private Visibility _patientSexAndAgeVisibility;

        //public Visibility PatientSexAndAgeVisibility
        //{
        //    get
        //    {
        //        if (AccessionNoVisibility==Visibility.Visible)
        //            return Visibility.Hidden;
        //    }
        //    //set
        //    //{
        //    //    if (_patientSexAndAgeVisibility == value) return;
        //    //    _patientSexAndAgeVisibility = value;
        //    //    NotifyPropertyChanged(() => PatientSexAndAgeVisibility);
        //    //}
        //}

        private string _patientId;

        public string PatientId
        {
            get { return _patientId; }
            set
            {
                if (_patientId == value) return;
                _patientId = value;
                NotifyPropertyChanged(() => PatientId);
            }
        }

        private Visibility _patientIdVisibility;

        public Visibility PatientIdVisibility
        {
            get { return _patientIdVisibility; }
            set
            {
                if (_patientIdVisibility == value) return;
                _patientIdVisibility = value;
                NotifyPropertyChanged(() => PatientIdVisibility);
            }
        }

        private Visibility _logoVisibility;

        public Visibility LogoVisibility
        {
            get { return _logoVisibility; }
            set
            {
                if (_logoVisibility == value) return;
                _logoVisibility = value;
                NotifyPropertyChanged(() => LogoVisibility);
            }
        }

        private string _studyDate;

        public string StudyDate
        {
            get { return _studyDate; }
            set
            {
                if (_studyDate == value) return;
                _studyDate = value;
                NotifyPropertyChanged(() => StudyDate);
            }
        }

        private Visibility _studyDateVisibility;

        public Visibility StudyDateVisibility
        {
            get { return _studyDateVisibility; }
            set
            {
                if (_studyDateVisibility == value) return;
                _studyDateVisibility = value;
                NotifyPropertyChanged(() => StudyDateVisibility);
            }
        }

        private string _institutionName;

        public string InstitutionName
        {
            get { return _institutionName; }
            set
            {
                if (_institutionName == value) return;
                _institutionName = value;
                NotifyPropertyChanged(() => InstitutionName);
            }
        }

        private Visibility _institutionNameVisibility;

        public Visibility InstitutionNameVisibility
        {
            get { return _institutionNameVisibility; }
            set
            {
                if (_institutionNameVisibility == value) return;
                _institutionNameVisibility = value;
                NotifyPropertyChanged(() => InstitutionNameVisibility);
            }
        }

        private string  _comment;

        public string  Comment
        {
            get { return _comment; }
            set
            {
                if (_comment == value) return;
                _comment = value;
                NotifyPropertyChanged(() => Comment);
            }
        }

        private Visibility _commentVisibility;

        public Visibility CommentVisibility
        {
            get { return _commentVisibility; }
            set
            {
                if (_commentVisibility == value) return;
                _commentVisibility = value;
                NotifyPropertyChanged(() => CommentVisibility);
            }
        }

        private  string _pageNo;

        public string PageNo
        {
            get { return _pageNo; }
            set
            {
                if (_pageNo == value) return;
                _pageNo = value;
                NotifyPropertyChanged(() => PageNo);
            }
        }

        private Visibility _pageNoVisibility;

        public Visibility PageNoVisibility
        {
            get { return _pageNoVisibility; }
            set
            {
                if (_pageNoVisibility == value) return;
                _pageNoVisibility = value;
                NotifyPropertyChanged(() => PageNoVisibility);
            }
        }

        private string _displayPosition = "1";
        private PageTitleConfigInfoModel _config;

        public string DisplayPosition
        {
            get { return _displayPosition; }
        }

        #endregion

        #region [---Public Methods---]

        public void ResetModel(PageTitleInfoModel model)
        {
            try
            {
                Logger.LogFuncUp();

                if (null == model)
                {
                    return;
                }

                this.PatientName = model.PatientName;
                this.PatientId = model.PatientId;

                var sex = _config.SexFlag != "1" ? string.Empty : model.Sex;
                sex = string.IsNullOrEmpty(sex) ? string.Empty : sex + " ";
                var age = _config.AgeFlag != "1" ? string.Empty : model.Age;

                this.PatientSexAndAge = sex + age;
                this.StudyDate = model.StudyDate;
                this.Comment = model.Comment;
                this.PageNo = model.PageNo;
                this.InstitutionName = model.HospitalInfo;
                this.AccessionNo = model.AccessionNo;

                SetPatientSexAndAgeLableVisibility();     
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }

        }

        public void SetPageTitleConfigInfo(PageTitleConfigInfoModel config)
        {
            try
            {
                Logger.LogFuncUp();

                this._config = config;
                this.DisplayFont = int.Parse(config.DisplayFont);
                //this.DisplayPosition = config.Comment == "1" ? Visibility.Visible : Visibility.Hidden;
                this.PatientNameVisibility = config.PatientNameFlag == "1" ? Visibility.Visible : Visibility.Hidden;
                this.PatientIdVisibility = config.PatientIdFlag == "1" ? Visibility.Visible : Visibility.Hidden;
                //this.PatientSexAndAgeVisibility = config.AgeFlag == "1" || config.SexFlag == "1"
                //                                      ? Visibility.Visible
                //                                      : Visibility.Hidden;
                this.StudyDateVisibility = config.StudyDateFlag == "1" ? Visibility.Visible : Visibility.Hidden;
                this.InstitutionNameVisibility = config.HospitalInfoFlag == "1" ? Visibility.Visible : Visibility.Hidden;
                this.CommentVisibility = config.Comment == "1" ? Visibility.Visible : Visibility.Hidden;
                this.PageNoVisibility = config.PageNo == "1" ? Visibility.Visible : Visibility.Hidden;
                this.AccessionNoVisibility = config.AccessionNoFlag == "1" ? Visibility.Visible : Visibility.Hidden;
                this.LogoVisibility = config.Logo == "1" ? Visibility.Visible : Visibility.Hidden;
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void SetPatientSexAndAgeLableVisibility()    //根据AccessionNumber的显示与否、存在与否，设置PatientSex和PatientAge的显示位置，Key435891。
        {
            if (this.AccessionNoVisibility == Visibility.Visible && this.AccessionNo != string.Empty)
                this.AccessionNoLableVisibility = Visibility.Visible;
            else
                this.AccessionNoLableVisibility = Visibility.Collapsed;
            if (AccessionNoLableVisibility == Visibility.Visible)
                this.PatientSexAndAgeLableVisibility = Visibility.Collapsed;
            else
                this.PatientSexAndAgeLableVisibility = Visibility.Visible;
        }
    }

    #endregion

}
