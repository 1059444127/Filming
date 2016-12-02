using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.MHC;
using UIH.Mcsf.Viewer;
using System.Windows.Media;
using System.ComponentModel;

namespace UIH.Mcsf.Filming
{
    //public enum StudyContentType
    //{
    //    Series,
    //    Image
    //}

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propName)
        {
            VerifyPropertyName(propName);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                throw new Exception(msg);
            }
        }
    }

    //public class StudyListCtrlViewModel : ViewModelBase
    //{
    //    public StudyListCtrlViewModel()
    //    {
    //    }

    //    //public StudyListCtrlViewMode(DataRepositry data)
    //    //{
    //    //    this._source = data;
    //    //    foreach (var studyData in data.Studys)
    //    //    {
    //    //        AddStudy(studyData);
    //    //    }
    //    //}

    //    readonly ObservableCollection<StudyViewModel> _studyVMList = new ObservableCollection<StudyViewModel>();
    //    public ObservableCollection<StudyViewModel> StudyViewModelList
    //    {
    //        get 
    //        {
    //            //_studyVMList.Clear();
    //            //foreach (StudyData study in _source.Studys)
    //            //{
    //            //    StudyViewModel studyVM = new StudyViewModel(study);
    //            //    _studyVMList.Add(studyVM);
    //            //}

    //            return _studyVMList;
    //        }
    //    }

    //    StudyListDisplayMode _studyListCtrlDisplayMode;
    //    public StudyListDisplayMode StudyListCtrlDisplayMode
    //    {
    //        get
    //        {
    //            return _studyListCtrlDisplayMode;
    //        }
    //        set
    //        {
    //            _studyListCtrlDisplayMode = value;
    //            foreach (StudyViewModel svm in _studyVMList)
    //            {
    //                svm.StudyListDisplayMode = value;
    //            }
    //        }
    //    }

    //    private IList<StudyData> _studyDataList = new List<StudyData>();
    //    public void AddStudies(IList<StudyData> studyDataList)
    //    {
    //        if (studyDataList == null || studyDataList.Count() == 0)
    //        {
    //            return;
    //        }

    //        IList<string> vmStudyUIDList = new List<string>();
    //        if (_studyVMList != null)
    //        {
    //            foreach (var study in _studyVMList)
    //            {
    //                vmStudyUIDList.Add(study.Source.StudyUid);
    //            } 
    //        }

    //        _studyDataList = studyDataList;

    //        //SSFS key 107267:SSFS_Filming_FilmSheet_DifferentPatientImageInOneFilm
    //        if ((_studyVMList == null || _studyVMList.Count == 0) && studyDataList.Count > 1
    //                || _studyVMList.Count() > 0 && studyDataList.Any(study => !vmStudyUIDList.Contains(study.StudyUid)))
    //        {
    //            MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Warning_AddDifferentPatientToSameFilmCard", new MsgResponseHander(AddStudiesHandler));
    //        }
    //        else
    //        {
    //            AddStudiesHandler(MessageBoxResponse.YES);
    //        }
    //    }

    //    private void AddStudiesHandler(MessageBoxResponse res)
    //    {
    //        if (res == MessageBoxResponse.YES)
    //        {
    //            foreach (var studyData in _studyDataList)
    //            {
    //                AddStudy(studyData);
    //            }
    //        }

    //    }

    //    /// <key>\n 
    //    /// PRA:Yes \n
    //    /// Traced from: SSFS_PRA_Filming_DifferentPatientImageInOnePicture_caution \n
    //    /// Tests: N/A \n
    //    /// Description: when load image of different patient into a filming card, pop up a warning \n
    //    /// Short Description: DifferentPatientImageInOneFilmingCardCaution \n
    //    /// Component: Filming \n
    //    /// </key> \n
    //    public void AddStudy(StudyData studyData)
    //    {

    //        try
    //        {
    //            if (_studyVMList.Any(n => n.Source.StudyUid == studyData.StudyUid))
    //            {
    //                //the study has exist, just update it
    //                var studyVM = _studyVMList.First(n => n.Source.StudyUid == studyData.StudyUid);
    //                _studyVMList.Remove(studyVM);

    //                AddStudyToVM(studyData);
    //            }
    //            else
    //            {
    //                AddStudyToVM(studyData);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Logger.LogFuncException(ex.Message+ex.StackTrace);
    //        }
    //    }

    //    private void AddStudyToVM(StudyData studyData)
    //    {
    //        var svm = new StudyViewModel(studyData, _studyListCtrlDisplayMode);
    //        _studyVMList.Add(svm);
    //        FillSeriesListData(svm);

    //        OnPropertyChanged("StudyViewModelList");
    //    }

    //    /// <summary>
    //    /// add the thumbnail image in study list control tree view
    //    /// </summary>
    //    private void FillSeriesListData(StudyViewModel studyvm)
    //    {
    //        var filmCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
    //        if (filmCard == null)
    //        {
    //            return;
    //        }

    //        if(studyvm == null)
    //        {
    //            return;
    //        }
    //        //foreach (StudyViewModel studyvm in filmCard.StudyListCtrlViewModel.StudyViewModelList)
    //        {
    //            foreach (SeriesViewModel svm in studyvm.Children)
    //            {
    //                foreach (ImageViewModel ivm in svm.Children)
    //                {
    //                    svm.ImageSource = ivm.ImageSource;
    //                    break;
    //                }
    //            }
    //        }
    //    }

    //    public void RemoveStudyAt(int index)
    //    {
    //        try
    //        {
    //            _source.Studys.RemoveAt(index);
    //            _studyVMList.RemoveAt(index);

    //            OnPropertyChanged("StudyViewModelList");
    //        }
    //        catch (Exception ex)
    //        {
    //            Logger.LogFuncException(ex.Message+ex.StackTrace);
    //        }
    //    }

    //    readonly DataRepositry _source = DataRepositry.Instance;
    //}

    //public class StudyViewModel : ViewModelBase
    //{
    //    public StudyViewModel(StudyData study, StudyListDisplayMode mode)
    //    {
    //        _source = study;
    //        _children = new ObservableCollection<SeriesViewModel>();

    //        StudyListDisplayMode = mode;

    //        foreach (SeriesData s in study.SeriesItems)
    //        {
    //            AddSeries(s);
    //        }
    //    }

    //    public string PatientName
    //    {
    //        get { return _source.PatientName; }
    //        set
    //        {
    //            _source.PatientName = value;
    //            OnPropertyChanged("PatientName");
    //        }
    //    }

    //    public string PatientID
    //    {
    //        get { return _source.PatientID; }
    //        set
    //        {
    //            _source.PatientID = value;
    //            OnPropertyChanged("PatientID");
    //        }
    //    }

    //    //private string _position;
    //    //public string Position
    //    //{
    //    //    get { return _position; }//"Head"; }// 
    //    //    set
    //    //    {
    //    //        _position = value;
    //    //        OnPropertyChanged("Position");
    //    //    }
    //    //}

    //    public string Gender
    //    {
    //        get { return _source.PatientSex; }
    //        set
    //        {
    //            _source.PatientSex = value;
    //            OnPropertyChanged("Gender");
    //        }
    //    }

    //    private string _age;
    //    public string Age
    //    {
    //        get
    //        {
    //            if(_age == null)
    //            {
    //                if (!string.IsNullOrEmpty(_source.PatientStudyAge))
    //                {
    //                    try
    //                    {
    //                        uint iAge = Convert.ToUInt16(_source.PatientStudyAge);
    //                        if (0 == iAge)
    //                        {
    //                            _age = "";
    //                        }
    //                        else
    //                        {
    //                            _age = iAge.ToString();
    //                        }

    //                    }
    //                    catch (Exception)
    //                    {
    //                        _age = "";
    //                    }
    //                }
    //                else
    //                {

    //                    _age = ConvertBirthdayToAge(_source.StudyDate, ConvertStringToDateTime(_source.PatientBirthday));
    //                }

    //                _age = UpdateAgeSuffix(_age);
    //            }
                
    //            return _age;
    //        }
    //        set
    //        {
    //            _age = value;
    //            OnPropertyChanged("Age");
    //        }
    //    }

    //    private DateTime? ConvertStringToDateTime(string birthdayString)
    //    {
    //        string YMR_birthdayString = null;
    //        string[] trimTime = birthdayString.Split(' ');
    //        if (trimTime.Length > 1)
    //        {
    //            YMR_birthdayString = trimTime[0];
    //        }
    //        if (YMR_birthdayString != null)
    //        {
    //            string[] birthdayArrayString = YMR_birthdayString.Split('/');
    //            if (birthdayArrayString.Length != 3)
    //            {
    //                Logger.LogError("Wrong birthday! :" + birthdayString);
    //                return null;
    //            }

    //            int year = Convert.ToInt32(birthdayArrayString[0]);
    //            int month = Convert.ToInt32(birthdayArrayString[1]);
    //            int day = Convert.ToInt32(birthdayArrayString[2]);

    //            var birthDay = new DateTime(year, month, day);

    //            return birthDay;
    //        }
    //        return null;
    //    }

    //    /// <summary>
    //    /// calculate the age of the study date point of the patient
    //    /// </summary>
    //    /// <param name="studyDate"> </param>
    //    /// <param name="birthdayDate"></param>
    //    /// <returns></returns>
    //    private string ConvertBirthdayToAge(DateTime? studyDate, DateTime? birthdayDate)
    //    {
    //        try
    //        {
    //            if(studyDate == null || birthdayDate == null)
    //            {
    //                return "";
    //            }

    //            string ageNumber = "";
    //            string ageType = "";

    //            TimeSpan ts = studyDate.Value- birthdayDate.Value;

    //            double days = Math.Floor(ts.TotalDays);
    //            if (days<0)
    //            {
    //                return "";
    //            }
    //            double weeks = Math.Floor(ts.TotalDays / 7);

    //            if (0 == days)
    //            {
    //                ageNumber = "1";
    //                ageType = "D";
    //            }
    //            else if (0 < days && 14 > days)
    //            {
    //                ageNumber = days.ToString();
    //                ageType = "D";
    //            }
    //            else if (2 <= weeks && 8 > weeks)
    //            {
    //                ageNumber = weeks.ToString();
    //                ageType = "W";
    //            }
    //            else
    //            {
    //                //Months ( Because Month calculate by floor number, so it's not need to subtract one month when the days' number is less than one month.
    //                double months = (studyDate.Value.Year-birthdayDate.Value.Year) * 12
    //                    + (studyDate.Value.Month-birthdayDate.Value.Month)
    //                    + ((studyDate.Value.Day-birthdayDate.Value.Day) >= 0 ? 0 : -1);
    //                months = Math.Floor(months);

    //                if (2 <= months && 24 > months)
    //                {
    //                    ageNumber = months.ToString();
    //                    ageType = "M";
    //                }
    //                else if (24 <= months)
    //                {
    //                    double years = Math.Floor(months / 12);
    //                    if (2 <= years && 999 > years)
    //                    {
    //                        ageNumber = years.ToString();
    //                        ageType = "Y";
    //                    }
    //                    else if (2 > years)
    //                    {
    //                        //
    //                        ageNumber = weeks.ToString();
    //                        //ageType = GlobalDefinition.ToLocalLanguge("UID_PA_AgeUnit_Week");
    //                    }
    //                    else
    //                    {
    //                        Logger.LogError("Your input age is larger than 999 years, Please check it.");

    //                        return "";
    //                    }
    //                }
    //                else
    //                {
    //                    ageNumber = weeks.ToString();
    //                    //ageType = GlobalDefinition.ToLocalLanguge("UID_PA_AgeUnit_Week");
    //                }
    //            }

    //            return ageNumber + ageType;
    //        }
    //        catch (Exception ex)
    //        {
    //            Logger.LogError(ex.StackTrace);
    //            //throw;

    //            return "";
    //        }
    //    }
    //    /// <summary>
    //    /// replace the suffix of DICOM files to the NLS format.
    //    /// </summary>
    //    /// <param name="patientAge"></param>
    //    /// <returns></returns>
    //    private string UpdateAgeSuffix(string patientAge)
    //    {
    //        string newAgeWithSuffix = patientAge;
    //        if(string.IsNullOrEmpty(patientAge))
    //        {
    //            return "";
    //        }

    //        if (FilmingViewerContainee.FilmingResourceDict == null)
    //        {
    //            return "";
    //        }

    //        patientAge = patientAge.TrimStart('0');

    //        if(patientAge.Contains("Y"))
    //        {
    //            string yearSuffix = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Ages_Year"] as string;
    //            newAgeWithSuffix = patientAge.Replace("Y", yearSuffix);
    //        }

    //        if(patientAge.Contains("M"))
    //        {
    //            string yearSuffix = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Ages_Month"] as string;
    //            newAgeWithSuffix = patientAge.Replace("M", yearSuffix);
    //        }

    //        if (patientAge.Contains("W"))
    //        {
    //            string yearSuffix = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Ages_Week"] as string;
    //            newAgeWithSuffix = patientAge.Replace("W", yearSuffix);
    //        }

    //        if (patientAge.Contains("D"))
    //        {
    //            string yearSuffix = FilmingViewerContainee.FilmingResourceDict["UID_Filming_Ages_Day"] as string;
    //            newAgeWithSuffix = patientAge.Replace("D", yearSuffix);
    //        }

    //        return newAgeWithSuffix;
    //    }

    //    public string Description
    //    {
    //        get { return _source.Description; }
    //        set
    //        {
    //            _source.Description = value;
    //            OnPropertyChanged("Description");
    //        }
    //    }

    //    public bool IsSelected
    //    {
    //        get { return false; }
    //        set
    //        {
    //            OnPropertyChanged("IsSelected");
    //        }
    //    }

    //    StudyListDisplayMode _listMode;
    //    public StudyListDisplayMode StudyListDisplayMode
    //    {
    //        get { return _listMode; }
    //        set
    //        {
    //            _listMode = value;
    //            OnPropertyChanged("StudyListDisplayMode");
    //        }
    //    }

    //    readonly ObservableCollection<SeriesViewModel> _children;
    //    public ObservableCollection<SeriesViewModel> Children
    //    {
    //        get { return _children; }
    //    }

    //    StudyData _source;
    //    public StudyData Source
    //    {
    //        get { return _source; }
    //        set { _source = value; }
    //    }

    //    public void AddSeries(SeriesData series)
    //    {
    //        var child = new SeriesViewModel(series, this);
    //        _children.Add(child);

    //        OnPropertyChanged("Children");
    //    }

    //    public void RemoveSeries(SeriesData series)
    //    {
    //        _source.SeriesItems.Remove(series);

    //        SeriesViewModel svm = _children.FirstOrDefault(s => s.Source == series);
    //        if(null != svm)
    //            _children.Remove(svm);

    //        OnPropertyChanged("Children");
    //    }

    //    public void RemoveSeriesAt(int index)
    //    {
    //        _source.SeriesItems.RemoveAt(index);
    //        _children.RemoveAt(index);

    //        OnPropertyChanged("Children");
    //    }
    //}

    //public class SeriesViewModel : ViewModelBase
    //{
    //    public SeriesViewModel(SeriesData series, StudyViewModel parent)
    //    {
    //        _source = series;
    //        _parent = parent;
    //        _children = new ObservableCollection<ImageViewModel>();

    //        foreach (ImageData img in series.Images)
    //        {
    //            AddImage(img);
    //        }
    //    }

    //    public string SeriesDescription
    //    {
    //        get { return _source.SeriesDescription; }
    //        set
    //        {
    //            _source.SeriesDescription = value;
    //            OnPropertyChanged("SeriesDescription");
    //        }
    //    }

    //    public bool CanSupport3D
    //    {
    //        get { return _source.CanSupport3D; }
    //        set 
    //        { 
    //            _source.CanSupport3D =value;
    //            OnPropertyChanged("CanSupport3D");
    //        }
    //    }

    //    public bool IsSelected
    //    {
    //        get { return _source.IsSelected; }
    //        set
    //        {
    //            //if (Keyboard.IsKeyDown(Key.LeftCtrl)
    //            //    || Keyboard.IsKeyDown(Key.RightCtrl))
    //            //{
    //            //    if (value != false)
    //            //        _source.IsSelected = value;
    //            //}
    //            //else
    //            //{
    //            _source.IsSelected = value;
    //            //}

    //            OnPropertyChanged("IsSelected");
    //        }
    //    }

    //    private Brush _background = Brushes.Transparent;
    //    public Brush Background
    //    {
    //        get { return _background; }
    //        set
    //        {
    //            _background = value;
    //            OnPropertyChanged("Background");
    //        }
    //    }

    //    private Brush _foreground = Brushes.White;
    //    public Brush Foreground
    //    {
    //        get { return _foreground; }
    //        set
    //        {
    //            _foreground = value;
    //            OnPropertyChanged("Foreground");
    //        }
    //    }

    //    readonly ObservableCollection<ImageViewModel> _children;
    //    public ObservableCollection<ImageViewModel> Children
    //    {
    //        get { return _children; }
    //    }

    //    public int ChildrenCount
    //    {
    //        get { return _source.Images.Count; }
    //    }

    //    public ImageSource ImageSource
    //    {
    //        get { return _source.SeriesImage; }
    //        set
    //        {
    //            _source.SeriesImage = value;
    //            OnPropertyChanged("ImageSource");
    //        }
    //    }

    //    SeriesData _source;
    //    public SeriesData Source
    //    {
    //        get { return _source; }
    //        set { _source = value; }
    //    }

    //    readonly StudyViewModel _parent;
    //    public StudyViewModel Parent
    //    {
    //        get { return _parent; }
    //    }

    //    public void AddImage(ImageData image)
    //    {
    //        var ivm = new ImageViewModel(image, this);
    //        _children.Add(ivm);

    //        OnPropertyChanged("Children");
    //        OnPropertyChanged("ChildrenCount");
    //    }
    //}

    //public class ImageViewModel : ViewModelBase
    //{
    //    public ImageViewModel(ImageData image, SeriesViewModel parent)
    //    {
    //        _source = image;
    //        _parent = parent;
    //    }

    //    public string Description
    //    {
    //        get { return _source.Description; }
    //        set 
    //        {
    //            OnPropertyChanged("Description");
    //        }
    //    }

    //    public ImageSource ImageSource
    //    {
    //        get { return _source.ImageSource; }
    //    }

    //    ImageData _source;
    //    public ImageData Source
    //    {
    //        get { return _source; }
    //        set { _source = value; }
    //    }

    //    readonly SeriesViewModel _parent;
    //    public SeriesViewModel Parent
    //    {
    //        get { return _parent; }
    //    }

    //    public void CreateDisplayData(string sharedMemName)
    //    {
    //        var accessor = new DataAccessor();
    //        _source.DisplayData = accessor.CreateImageData(sharedMemName);

    //        _source.IsDisplayDataCreated = true;
    //        OnPropertyChanged("ImageSource");
    //    }
    //}
}
