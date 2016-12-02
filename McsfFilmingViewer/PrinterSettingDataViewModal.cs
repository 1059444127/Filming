using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Printing;
using System.Windows;
using System.Xml;
using UIH.Mcsf.Filming.Command;
using System.Diagnostics;

namespace UIH.Mcsf.Filming
{
    public class PrinterSettingDataViewModal : INotifyPropertyChanged
    {
        public static readonly int MaxFilmCount = 9;
        public PrinterSettingDataViewModal()
        {
            Printers configures = Printers.Instance;
            //configures.ReloadDefaultConfig();            
            InitFilmPrinterDPI();

            InitPrinterAE();

            InitFilmSizeInfoByAE();

            InitMediumTypeInfoByAE();

            InitFilmDestinationInfoByAE();

            //init film orientation
            InitFilmOrientation();

            //init film annotation
            InitFilmAnnotation();

            InitFilmCopyCount();

            InitRealSizePrintCorrectRatioByAE();

            IfClearAfterAddFilmingJob = configures.IfClearAfterAddFilmingJob;

            IfClearAfterSaveEFilm = configures.IfClearAfterSaveEFilm;

            IfSaveEFilmWhenFilming = configures.IfSaveEFilmsAvailable && configures.IfSaveEFilmWhenFilming;

            //IfColorPrint = configures.IfColorPrint;
            InitIsColorPrintingOptionChecked();

            IfShutDownAfterPrint = configures.IfShutDownAfterPrint;

            GeneralPrinterDPI = configures.DefaultPaperPrintDPI;

        }


        public void ReloadPrinterSetting()
        {
            Printers configures = Printers.Instance;
            InitFilmPrinterDPI();

            InitPrinterAE();

            InitFilmSizeInfoByAE();

            InitMediumTypeInfoByAE();

            InitFilmDestinationInfoByAE();

            //init film orientation
            InitFilmOrientation();

            //init film annotation
            InitFilmAnnotation();

            InitFilmCopyCount();

            InitRealSizePrintCorrectRatioByAE();

            IfClearAfterAddFilmingJob = configures.IfClearAfterAddFilmingJob;

            IfClearAfterSaveEFilm = configures.IfClearAfterSaveEFilm;

            IfSaveEFilmWhenFilming = configures.IfSaveEFilmsAvailable && configures.IfSaveEFilmWhenFilming;

            //IfColorPrint = configures.IfColorPrint;
            InitIsColorPrintingOptionChecked();

            IfShutDownAfterPrint = configures.IfShutDownAfterPrint;

            GeneralPrinterDPI = configures.DefaultPaperPrintDPI;
        }

        private void InitFilmPrinterDPI()
        {
            try
            {
                var debugConfig = Configure.Environment.Instance.GetDebugConfigure();
                _filmPrinterDPI = debugConfig.PrinterDPI;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }

        }
        //public PrinterSettingDataViewModal Clone()
        //{
        //    PrinterSettingDataViewModal newDataViewModel = null;

        //    MemoryStream stream = new MemoryStream();
        //    BinaryFormatter binFormatter = new BinaryFormatter();
        //    binFormatter.Serialize(stream, this);
        //    stream.Position = 0;
        //    newDataViewModel = (PrinterSettingDataViewModal)binFormatter.Deserialize(stream);
        //    stream.Close();
        //    return newDataViewModel;
        //}

        public PrinterSettingDataViewModal Clone()
        {
            PrinterSettingDataViewModal newDataViewModel = new PrinterSettingDataViewModal();
            //newDataViewModel.AnnotationTypeList = new ObservableCollection<ImgTxtDisplayState>(this.AnnotationTypeList);
            newDataViewModel.CurrentAnnotationType = this.CurrentAnnotationType;
            newDataViewModel.CurrentCopyCount = this.CurrentCopyCount;
            newDataViewModel.CurrentFilmSize = this.CurrentFilmSize;
            newDataViewModel.CurrentFilmOrientation = this.CurrentFilmOrientation;
            newDataViewModel.CurrentPrinterAE = this.CurrentPrinterAE;
            newDataViewModel.CurrentPrinterFilmSizeList = new ObservableCollection<object>(this.CurrentPrinterFilmSizeList);
            //newDataViewModel.CurrentScaleMode = this.CurrentScaleMode;
            newDataViewModel.FilmCopyCountList = new ObservableCollection<uint>(this.FilmCopyCountList);
            //newDataViewModel.FilmOrientationList = new ObservableCollection<FilmOrientationEnum>(this.FilmOrientationList);
            //newDataViewModel.FilmScaleModeList = new ObservableCollection<FilmScaleModeEnum>(this.FilmScaleModeList);
            newDataViewModel.FilmSize = this.FilmSize;
            newDataViewModel.CurrentFilmSize = this.CurrentFilmSize;

            newDataViewModel.CurrentMediumType = this.CurrentMediumType;
            newDataViewModel.CurrentPrinterMediumTypeList =
                new ObservableCollection<object>(this.CurrentPrinterMediumTypeList);
            newDataViewModel.CurrentMediumType = this.CurrentMediumType;

            newDataViewModel.CurrentFilmDestination = this.CurrentFilmDestination;
            newDataViewModel.CurrentPrinterFilmDestinationList =
                new ObservableCollection<object>(this.CurrentPrinterFilmDestinationList);
            newDataViewModel.CurrentFilmDestination = this.CurrentFilmDestination;

            newDataViewModel.RegisterPrinterAEList = new ObservableCollection<string>(this.RegisterPrinterAEList);
            newDataViewModel.IfClearAfterAddFilmingJob = this.IfClearAfterAddFilmingJob;
            newDataViewModel.IfClearAfterSaveEFilm = this.IfClearAfterSaveEFilm;
            newDataViewModel.IfSaveEFilmWhenFilming = this.IfSaveEFilmWhenFilming;
            newDataViewModel.IfShutDownAfterPrint = this.IfShutDownAfterPrint;
            newDataViewModel.IfColorPrint = this.IfColorPrint;
            newDataViewModel.IfShutDownAfterPrint = this.IfShutDownAfterPrint;
            newDataViewModel.IsEnableAnnotationSelection = this.IsEnableAnnotationSelection;
            newDataViewModel.IsEnableOrientationSelection = this.IsEnableOrientationSelection;
            newDataViewModel.IsEnableSaveEFilmSelection = this.IsEnableSaveEFilmSelection;
            newDataViewModel.IsEnableClearFilmSelection = this.IsEnableClearFilmSelection;
            newDataViewModel.IsEnableColorPrintSelection = this.IsEnableColorPrintSelection;

            return newDataViewModel;
        }

        public void CopyTo(PrinterSettingDataViewModal newDataViewModel)
        {
            //newDataViewModel.AnnotationTypeList = new ObservableCollection<ImgTxtDisplayState>(this.AnnotationTypeList);
            newDataViewModel.CurrentAnnotationType = this.CurrentAnnotationType;
            newDataViewModel.CurrentCopyCount = this.CurrentCopyCount;
            newDataViewModel.CurrentFilmSize = this.CurrentFilmSize;
            newDataViewModel.CurrentFilmOrientation = this.CurrentFilmOrientation;
            newDataViewModel.CurrentPrinterAE = this.CurrentPrinterAE;
            newDataViewModel.CurrentPrinterFilmSizeList = new ObservableCollection<object>(this.CurrentPrinterFilmSizeList);
            //newDataViewModel.CurrentScaleMode = this.CurrentScaleMode;
            newDataViewModel.FilmCopyCountList = new ObservableCollection<uint>(this.FilmCopyCountList);
            //newDataViewModel.FilmOrientationList = new ObservableCollection<FilmOrientationEnum>(this.FilmOrientationList);
            //newDataViewModel.FilmScaleModeList = new ObservableCollection<FilmScaleModeEnum>(this.FilmScaleModeList);
            newDataViewModel.FilmSize = this.FilmSize;
            newDataViewModel.CurrentFilmSize = this.CurrentFilmSize;

            newDataViewModel.CurrentMediumType = this.CurrentMediumType;
            newDataViewModel.CurrentPrinterMediumTypeList =
                new ObservableCollection<object>(this.CurrentPrinterMediumTypeList);
            newDataViewModel.CurrentMediumType = this.CurrentMediumType;

            newDataViewModel.CurrentFilmDestination = this.CurrentFilmDestination;
            newDataViewModel.CurrentPrinterFilmDestinationList =
                new ObservableCollection<object>(this.CurrentPrinterFilmDestinationList);
            newDataViewModel.CurrentFilmDestination = this.CurrentFilmDestination;

            newDataViewModel.RegisterPrinterAEList = new ObservableCollection<string>(this.RegisterPrinterAEList);
            newDataViewModel.IfClearAfterAddFilmingJob = this.IfClearAfterAddFilmingJob;
            newDataViewModel.IfClearAfterSaveEFilm = this.IfClearAfterSaveEFilm;

            newDataViewModel.IfSaveEFilmWhenFilming = this.IfSaveEFilmWhenFilming;
            newDataViewModel.IfColorPrint = this.IfColorPrint;
            newDataViewModel.IfShutDownAfterPrint = this.IfShutDownAfterPrint;
            newDataViewModel.IsEnableAnnotationSelection = this.IsEnableAnnotationSelection;
            newDataViewModel.IsEnableOrientationSelection = this.IsEnableOrientationSelection;
            newDataViewModel.IsEnableSaveEFilmSelection = this.IsEnableSaveEFilmSelection;
            newDataViewModel.IsEnableClearFilmSelection = this.IsEnableClearFilmSelection;
            newDataViewModel.IsEnableColorPrintSelection = this.IsEnableColorPrintSelection;
        }
        //public void SaveConfigToFile()
        //{
        //    try
        //    {
        //        SavePostFilmingConfigToFile();
        //        SaveDefaultConfigToFile();
        //    }
        //    catch
        //    {
        //        //System.Windows.MessageBox.Show(ex.StackTrace);
        //        MessageBoxHandler.Instance.ShowWarning("UID_Filming_Warning_CannotSaveConfigure");
        //    }
        //}

        public void SaveDefaultConfigToFile()
        {
            try
            {
                Logger.LogFuncUp();

                Printers.Instance.DefaultAE = CurrentPrinterAE;
                Printers.Instance.DefaultFilmSize = CurrentFilmSize == null
                                                        ? (string)CurrentFilmSize
                                                        : CurrentFilmSize.ToString();
                //Printers.Instance.DefaultMediumType = CurrentMediumType == null
                //                                          ? (string)CurrentMediumType
                //                                          : CurrentMediumType.ToString();
                //Printers.Instance.DefaultFilmDestination = CurrentFilmDestination == null
                //                                               ? (string)CurrentFilmDestination
                //                                               : CurrentFilmDestination.ToString();
              //  Printers.Instance.DefaultPaperPrintDPI = GeneralPrinterDPI;
                Printers.Instance.DefaultOrientation = CurrentFilmOrientation;

               // Printers.Instance.IfClearAfterAddFilmingJob = IfClearAfterAddFilmingJob;
                Printers.Instance.IfColorPrint = IfColorPrint;         //设置自动打印？自动打印为灰度打印
               // Printers.Instance.IfShutDownAfterPrint = IfShutDownAfterPrint;
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void SavePostFilmingConfigToFile()
        {
            try
            {
                Logger.LogFuncUp();
                
                Printers.Instance.IfSaveEFilmWhenFilming = IfSaveEFilmWhenFilming;
                //Printers.Instance.IfColorPrint = IfColorPrint;
                
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void InitFilmCopyCount()
        {
            for (uint i = (uint)MaxFilmCount; i >= 1; i--)
            {
                FilmCopyCountList.Add(i);
            }
            CurrentCopyCount = 1;
        }

        //private void InitScaleMode()
        //{
        //    FilmScaleModeList.Add(FilmScaleModeEnum.FitToFilm);
        //    FilmScaleModeList.Add(FilmScaleModeEnum.Scale_1X);
        //    FilmScaleModeList.Add(FilmScaleModeEnum.Scale_2X);
        //    //FilmScaleModeList.Add(FilmScaleModeEnum.TrueSize);

        //    CurrentScaleMode = FilmScaleModeEnum.Scale_1X;
        //}

        private void InitFilmAnnotation()
        {
            //AnnotationTypeList.Add(ImgTxtDisplayState.All);
            ////AnnotationTypeList.Add(ImgTxtDisplayState.Customization);
            //AnnotationTypeList.Add(ImgTxtDisplayState.Anonymity);
            //AnnotationTypeList.Add(ImgTxtDisplayState.None);
            var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            if (filmingCard != null)
            {
                CurrentAnnotationType = (int)filmingCard.ImageTextDisplayMode;

            }
        }

        private void InitFilmOrientation()
        {
            CurrentFilmOrientation = Printers.Instance.DefaultOrientation;
        }

        private void InitPrinterAE()
        {
            CurrentPrinterAE = Printers.Instance.DefaultAE;
            RegisterPrinterAEList.Clear();
            bool find = false;
            foreach (var peerNode in Printers.Instance.PeerNodes)
            {
                if (peerNode.PeerAE == CurrentPrinterAE)
                {
                    find = true;
                }
                RegisterPrinterAEList.Add(peerNode.PeerAE);
            }
            if (find == false && Printers.Instance.PeerNodes.Count > 0)
            {
                CurrentPrinterAE = Printers.Instance.PeerNodes[0].PeerAE;
            }
        }

        private void InitFilmSizeInfoByAE()
        {
            CurrentPrinterFilmSizeList.Clear();

            foreach (var peerNode in Printers.Instance.PeerNodes)
            {
                if (peerNode.PeerAE == CurrentPrinterAE)
                {
                    foreach (var filmSize in peerNode.SupportFilmSizeList)
                    {
                        CurrentPrinterFilmSizeList.Add(filmSize);
                    }
                }
            }


            var isExist=false;
            foreach (var item in CurrentPrinterFilmSizeList)
            {
                if(item.ToString()== Printers.Instance.DefaultFilmSize)
                {                     
                     isExist=true;
                     break;
                }
            }

            if (isExist)
            {
                CurrentFilmSize =(object) Printers.Instance.DefaultFilmSize;
            }
            else if (CurrentPrinterFilmSizeList.Count > 0)
            {
                CurrentFilmSize = CurrentPrinterFilmSizeList[0];
            }
        }

        private void InitMediumTypeInfoByAE()
        {
            CurrentPrinterMediumTypeList.Clear();

            foreach (var peerNode in Printers.Instance.PeerNodes)
            {
                if (peerNode.PeerAE == CurrentPrinterAE)
                {
                    foreach (var mediumType in peerNode.SupportMediumTypeList)
                    {
                        CurrentPrinterMediumTypeList.Add(mediumType);
                    }
                }
            }

            if (CurrentPrinterMediumTypeList.Contains(Printers.Instance.DefaultMediumType))
            {
                CurrentMediumType = Printers.Instance.DefaultMediumType;
            }
            else if (CurrentPrinterMediumTypeList.Count > 0)
            {
                CurrentMediumType = CurrentPrinterMediumTypeList[0];
            }
        }

        private void InitRealSizePrintCorrectRatioByAE()
        {
            foreach (var peerNode in Printers.Instance.PeerNodes)
            {
                if (peerNode.PeerAE == CurrentPrinterAE)
                {
                    if(peerNode.CorrectedRatioForRealSizeConfig.ContainsKey(CurrentFilmSize.ToString()))
                        CorrectedRatioForRealSize = peerNode.CorrectedRatioForRealSizeConfig[CurrentFilmSize.ToString()];
					else
						CorrectedRatioForRealSize = 1.000;
                    return;
                }
            }
            CorrectedRatioForRealSize = 1.000;
        }
        private void InitFilmDestinationInfoByAE()
        {
            CurrentPrinterFilmDestinationList.Clear();

            foreach (var peerNode in Printers.Instance.PeerNodes)
            {
                if (peerNode.PeerAE == CurrentPrinterAE)
                {
                    foreach (var filmDestination in peerNode.SupportFilmDestinationList)
                    {
                        CurrentPrinterFilmDestinationList.Add(filmDestination);
                    }
                }
            }

            if (CurrentPrinterFilmDestinationList.Count < 1)
            {
                CurrentPrinterFilmDestinationList.Insert(0, string.Empty);
            }
            CurrentFilmDestination = CurrentPrinterFilmDestinationList[0];
        }

        public void InitIsColorPrintingOptionChecked()
        {
            foreach (var peerNode in Printers.Instance.PeerNodes)
            {
                if (peerNode.PeerAE == CurrentPrinterAE)
                {
                    IfColorPrint = peerNode.IsColorPrintingOptionChecked;
                    break;
                }
            }
        }

        public bool ChangePrinterAE = false;
        private string _currentPrinterAE;
        public string CurrentPrinterAE
        {
            get { return _currentPrinterAE; }
            set
            {
                var initPrinterAE = _currentPrinterAE==null?true:false;
                if (_currentPrinterAE != value)
                {
                    ChangePrinterAE = true;
                    _currentPrinterAE = value;
                    InitFilmSizeInfoByAE();
                    InitMediumTypeInfoByAE();
                    InitFilmDestinationInfoByAE();
                    InitRealSizePrintCorrectRatioByAE();
                    InitGeneralPrinterDPIVisibility();
                    InitIsColorPrintingOptionChecked();
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentPrinterAE"));
                    ChangePrinterAE = false;
                    if (!initPrinterAE)
                    {
                        SavePrintSet();
                    }
                }
            }
        }

        private int _generalPrinterDPI = 300;

        public int GeneralPrinterDPI
        {
            get { return _generalPrinterDPI; }
            set
            {
                if (_generalPrinterDPI == value) return;
                if (value > 20 && value < 900)
                    _generalPrinterDPI = value;
                else if (value <= 20)
                    _generalPrinterDPI = 20;
                else
                    _generalPrinterDPI = 900;
                OnPropertyChanged(new PropertyChangedEventArgs("GeneralPrinterDPI"));
            }
        }

        private int _filmPrinterDPI = 300; 

        private Visibility _generalPrinterDPIVisibility;

        public Visibility GeneralPrinterDpiVisibility
        {
            get { return _generalPrinterDPIVisibility; }
            set
            {
                if (value == _generalPrinterDPIVisibility) return;
                _generalPrinterDPIVisibility = value;
                OnPropertyChanged(new PropertyChangedEventArgs("GeneralPrinterDpiVisibility"));
            }
        }

        private void InitGeneralPrinterDPIVisibility()
        {
            var ae = Printers.Instance.PeerNodes.FirstOrDefault(p => p.PeerAE == CurrentPrinterAE);
            GeneralPrinterDpiVisibility = ae != null && ae.NodeType == PeerNodeType.FILM_PRINTER
                 ? Visibility.Collapsed
                 : Visibility.Visible;

        }

        private ObservableCollection<string> _registerPrinterAEList =
            new ObservableCollection<string>();
        public ObservableCollection<string> RegisterPrinterAEList
        {
            get { return _registerPrinterAEList; }
            set
            {
                if (_registerPrinterAEList != value)
                {
                    _registerPrinterAEList = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("RegisterPrinterAEList"));
                }
            }
        }

        private ObservableCollection<object> _currentPrinterFilmSizeList =
            new ObservableCollection<object>();
        public ObservableCollection<object> CurrentPrinterFilmSizeList
        {
            get { return _currentPrinterFilmSizeList; }
            set
            {
                if (_currentPrinterFilmSizeList != value)
                {
                    _currentPrinterFilmSizeList = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentPrinterFilmSizeList"));
                }
            }
        }

        private ObservableCollection<object> _currentPrinterFilmDestinationList =
            new ObservableCollection<object>();
        public ObservableCollection<object> CurrentPrinterFilmDestinationList
        {
            get { return _currentPrinterFilmDestinationList; }
            set
            {
                if (_currentPrinterFilmDestinationList != value)
                {
                    _currentPrinterFilmDestinationList = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentPrinterFilmDestinationList"));
                }
            }
        }

        private ObservableCollection<object> _currentPrinterMediumTypeList =
            new ObservableCollection<object>();
        public ObservableCollection<object> CurrentPrinterMediumTypeList
        {
            get { return _currentPrinterMediumTypeList; }
            set
            {
                if (_currentPrinterMediumTypeList != value)
                {
                    _currentPrinterMediumTypeList = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentPrinterMediumTypeList"));
                }
            }
        }

        private object _filmSize;
        public object FilmSize
        {
            get { return _filmSize; }
            set
            {
                if (_filmSize != value)
                {
                    _filmSize = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentFilmSizeRatioOfPortrait"));
                }
            }
        }

        private object _currentMediumType = string.Empty;
        public object CurrentMediumType
        {
            get { return _currentMediumType; }
            set
            {
                _currentMediumType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentMediumType"));
            }
        }



        private object _currentFilmDestination = string.Empty;
        public object CurrentFilmDestination
        {
            get { return _currentFilmDestination; }
            set
            {
                _currentFilmDestination = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentFilmDestination"));
            }
        }

        //private ObservableCollection<FilmOrientationEnum> _filmOrientationList = 
        //    new ObservableCollection<FilmOrientationEnum>();
        //public ObservableCollection<FilmOrientationEnum> FilmOrientationList
        //{
        //    get { return _filmOrientationList; }
        //    set { _filmOrientationList = value;
        //    OnPropertyChanged(new PropertyChangedEventArgs("FilmOrientationList"));
        //    }
        //}

        private bool _IsEnableOrientationSelection = true;
        public bool IsEnableOrientationSelection
        {
            get
            {
                //          CurrentFilmOrientation = _isEnableAnnotationSelection ? 0 : -1;
                return _IsEnableOrientationSelection;
            }
            set
            {

                if (_IsEnableOrientationSelection != value)
                {
                    _IsEnableOrientationSelection = value;

                    OnPropertyChanged(new PropertyChangedEventArgs("IsEnableOrientationSelection"));
                }
            }
        }

        public PageOrientation CurrentPaperPrintOrientation { get; set; }




        private int _currentFilmOrientation;
        public int CurrentFilmOrientation
        {
            get { return _currentFilmOrientation; }
            set
            {
                if (_currentFilmOrientation != value)
                {
                    _currentFilmOrientation = value;

                    var filmOrientation = (FilmOrientationEnum)(value);
                    switch (filmOrientation)
                    {
                        case FilmOrientationEnum.Portrait:
                            CurrentPaperPrintOrientation = PageOrientation.Portrait;
                            break;
                        case FilmOrientationEnum.Landscape:
                            CurrentPaperPrintOrientation = PageOrientation.Landscape;
                            break;
                    }

                    var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                    if (filmingCard != null)
                    {
                        filmingCard.PrintAndSave.CurrentFilmOrientation = filmOrientation;
                    }
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentFilmOrientation"));
                }
            }
        }

        private int _currentFilmColorPrint;
        public int CurrentFilmColorPrint
        {
            get { return _currentFilmColorPrint; }  
           
        }

        private object _currentFilmSize;
        public object CurrentFilmSize
        {
            get { return _currentFilmSize; }
            set
            {
                if (_currentFilmSize != value)
                {
                    _currentFilmSize = value;
                    var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                    if (filmingCard != null && this.CurrentFilmSize != null)
                    {
                        filmingCard.PrintAndSave.CurrentFilmSize = (this.CurrentFilmSize.ToString());
                    }
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentFilmSize"));
                }
            }
        }

        private double _correctedRatioForRealSize;
        public double CorrectedRatioForRealSize
        {
            get { return _correctedRatioForRealSize; }
            set
            {
                if (Math.Abs(_correctedRatioForRealSize - value) > 0.0001)
                {
                    _correctedRatioForRealSize = value;
                    var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                    if (filmingCard != null)
                    {
                        filmingCard.PrintAndSave.CorrectedRatio = _correctedRatioForRealSize;
                    }
                }
            }
        }
        

        private bool _isEnableAnnotationSelection = true;
        public bool IsEnableAnnotationSelection
        {
            get
            {
                //CurrentAnnotationType = _isEnableAnnotationSelection ? 0 : -1;
                return _isEnableAnnotationSelection;
            }
            set
            {

                if (_isEnableAnnotationSelection != value)
                {
                    _isEnableAnnotationSelection = value;

                    OnPropertyChanged(new PropertyChangedEventArgs("IsEnableAnnotationSelection"));
                }
            }
        }

        private int _currentAnnotationType; // ImgTxtDisplayState.All;
        public int CurrentAnnotationType
        {
            get { return _currentAnnotationType; }
            set
            {
                if (_currentAnnotationType != value)
                {
                    _currentAnnotationType = value;
                    //var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                    //if (filmingCard != null)
                    //    filmingCard.UpdateCornerText((ImgTxtDisplayState) value);
                    //OnPropertyChanged(new PropertyChangedEventArgs("CurrentAnnotationType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("IsEnableToConfigureAnnotaionItems"));
                }
            }
        }



        //public bool IsEnableToConfigureAnnotaionItems
        //{
        //    get { return CurrentAnnotationType == (int)ImgTxtDisplayState.Customization; }
        //}

        //private ObservableCollection<FilmScaleModeEnum> _filmScaleModeList=
        //    new ObservableCollection<FilmScaleModeEnum>();
        //public ObservableCollection<FilmScaleModeEnum> FilmScaleModeList
        //{
        //    get { return _filmScaleModeList; }
        //    set { _filmScaleModeList = value;
        //    OnPropertyChanged(new PropertyChangedEventArgs("FilmScaleModeList"));
        //    }
        //}
        //private FilmScaleModeEnum _currentScaleMode;
        //public FilmScaleModeEnum CurrentScaleMode
        //{
        //    get { return _currentScaleMode; }
        //    set 
        //    {
        //        _currentScaleMode = value;

        //        switch (_currentScaleMode)
        //        {
        //            case FilmScaleModeEnum.FitToFilm :
        //                (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).FitWindow();
        //                break;
        //            case FilmScaleModeEnum.Scale_1X:
        //                (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).ZoomCells(1);
        //                break;
        //            case FilmScaleModeEnum.Scale_2X:
        //                (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).ZoomCells(2);
        //                break;
        //            //case FilmScaleModeEnum.TrueSize:
        //            //    (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).FilmingTrueSize();
        //            //    break;
        //            default:
        //                Logger.LogWarning("Not Supported ScaleMode");
        //                break;
        //        }

        //        OnPropertyChanged(new PropertyChangedEventArgs("CurrentScaleMode"));
        //    }
        //}

        private ObservableCollection<uint> _filmCopyCountList =
            new ObservableCollection<uint>();
        public ObservableCollection<uint> FilmCopyCountList
        {
            get { return _filmCopyCountList; }
            set
            {
                if (_filmCopyCountList != value)
                {
                    _filmCopyCountList = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("FilmCopyCountList"));
                }
            }
        }

        private uint _currentCopyCount = 1;
        public uint CurrentCopyCount
        {
            get { return _currentCopyCount; }
            set
            {
                if (_currentCopyCount != value)
                {
                    _currentCopyCount = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentCopyCount"));
                }
            }
        }

        private bool _isEnableClearFilmSelection = true;
        public bool IsEnableClearFilmSelection
        {
            get
            {
                //  CurrentAnnotationType = _IsEnableClearFilmSelection ? 0 : -1;
                return _isEnableClearFilmSelection;
            }
            set
            {

                if (_isEnableClearFilmSelection != value)
                {
                    _isEnableClearFilmSelection = value;

                    OnPropertyChanged(new PropertyChangedEventArgs("IsEnableClearFilmSelection"));
                }
            }
        }



        private bool _ifClearAfterAddFilmingJob;
        public bool IfClearAfterAddFilmingJob
        {
            get { return _ifClearAfterAddFilmingJob; }
            set
            {
                if (_ifClearAfterAddFilmingJob != value)
                {
                    _ifClearAfterAddFilmingJob = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IfClearAfterAddFilmingJob"));
                }
            }
        }

        private bool _ifClearAfterSaveEFilm;
        public bool IfClearAfterSaveEFilm
        {
            get { return _ifClearAfterSaveEFilm; }
            set
            {
                if (_ifClearAfterSaveEFilm != value)
                {
                    _ifClearAfterSaveEFilm = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IfClearAfterSaveEFilm"));
                }
            }
        }

        private bool _ifShutDownAfterPrint = false;
        public bool IfShutDownAfterPrint
        {
            get { return _ifShutDownAfterPrint; }
            set
            {
                if (_ifShutDownAfterPrint != value)
                {
                    _ifShutDownAfterPrint = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IfShutDownAfterPrint"));
                }
            }
        }

        private bool _isEnableSaveEFilmSelection = true;
        public bool IsEnableSaveEFilmSelection
        {
            get
            {
                //   CurrentAnnotationType = _IsEnableSaveEFilmSelection ? 0 : -1;
                return _isEnableSaveEFilmSelection;
            }
            set
            {

                if (_isEnableSaveEFilmSelection != value)
                {
                    _isEnableSaveEFilmSelection = value;

                    OnPropertyChanged(new PropertyChangedEventArgs("IsEnableSaveEFilmSelection"));
                }
            }
        }

        private bool _isEnableColorPrintSelection = true;
        public bool IsEnableColorPrintSelection
        {
            get
            {
                //   CurrentAnnotationType = _IsEnableSaveEFilmSelection ? 0 : -1;
                return _isEnableColorPrintSelection;
            }
            set
            {

                if (_isEnableColorPrintSelection != value)
                {
                    _isEnableColorPrintSelection = value;

                    OnPropertyChanged(new PropertyChangedEventArgs("IsEnableColorPrintSelection"));
                }
            }
        }
        //public bool IsEnableSaveEFilmSelection
        //{
        //    get
        //    {
        //        var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
        //        Debug.Assert(filmingCard != null);
        //        return filmingCard.FilmingCardModality != "SC";

        //    }
        //}

        private bool _ifSaveEFilmWhenFilming;
        public bool IfSaveEFilmWhenFilming
        {
            get { return _ifSaveEFilmWhenFilming; }
            set
            {
                if (_ifSaveEFilmWhenFilming != value)
                {
                    _ifSaveEFilmWhenFilming = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IfSaveEFilmWhenFilming"));
                }
            }
        }

        private bool _ifColorPrint;
        public bool IfColorPrint
        {
            get { return _ifColorPrint; }
            set
            {
                if (_ifColorPrint != value)
                {
                    _ifColorPrint = value;
                    _currentFilmColorPrint = _ifColorPrint  ? 1 : 0;
                  //  OnPropertyChanged(new PropertyChangedEventArgs("IfColorPrint"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #region [Jinyang.li Performance]

        public void SerializedToXml(XmlNode parentNode)
        {
            if (null == parentNode || null == parentNode.OwnerDocument)
            {
                return;
            }

            var printerSettingInfoNode = parentNode.OwnerDocument.CreateElement(OffScreenRenderXmlHelper.PRINTER_SETTING_INFO);
            parentNode.AppendChild(printerSettingInfoNode);
            
            OffScreenRenderXmlHelper.AppendChildNode(printerSettingInfoNode, OffScreenRenderXmlHelper.CURRENT_PRINTER_AE, this.CurrentPrinterAE ?? string.Empty);
            OffScreenRenderXmlHelper.AppendChildNode(printerSettingInfoNode, OffScreenRenderXmlHelper.CURRENT_FILM_SIZE, this.CurrentFilmSize == null ? string.Empty : this.CurrentFilmSize.ToString());
            OffScreenRenderXmlHelper.AppendChildNode(printerSettingInfoNode, OffScreenRenderXmlHelper.CURRENT_ORIENTATION, this.CurrentFilmOrientation.ToString());
            OffScreenRenderXmlHelper.AppendChildNode(printerSettingInfoNode, OffScreenRenderXmlHelper.CURRENT_COPY_COUNT, this.CurrentCopyCount.ToString());
            OffScreenRenderXmlHelper.AppendChildNode(printerSettingInfoNode, OffScreenRenderXmlHelper.CURRENT_MEDIUM_TYPE, this.CurrentMediumType == null ? string.Empty : this.CurrentMediumType.ToString());
            OffScreenRenderXmlHelper.AppendChildNode(printerSettingInfoNode, OffScreenRenderXmlHelper.CURRENT_FILM_DESTINATION, this.CurrentFilmDestination == null ? string.Empty : this.CurrentFilmDestination.ToString());
            OffScreenRenderXmlHelper.AppendChildNode(printerSettingInfoNode, OffScreenRenderXmlHelper.CURRENT_PRINTER_DPI, this.GeneralPrinterDpiVisibility == Visibility.Collapsed ? _filmPrinterDPI.ToString() : this.GeneralPrinterDPI.ToString());
            OffScreenRenderXmlHelper.AppendChildNode(printerSettingInfoNode, OffScreenRenderXmlHelper.IF_SAVE_HIGH_PRECISION_EFILM, Printers.Instance.IfSaveHighPrecisionEFilm.ToString());
            OffScreenRenderXmlHelper.AppendChildNode(printerSettingInfoNode, OffScreenRenderXmlHelper.IF_SAVE_EFILM_WHEN_FILMING, this.IfSaveEFilmWhenFilming.ToString());
            OffScreenRenderXmlHelper.AppendChildNode(printerSettingInfoNode, OffScreenRenderXmlHelper.IF_COLOR_PRINT, this.IfColorPrint.ToString());
        }

        #endregion

        public void SavePrintSet()
        {
            try
            {
                if (ChangePrinterAE) return;
                SaveDefaultConfigToFile();              
            }
            catch (Exception ex)
            {
                FilmingViewerContainee.Main.ShowStatusError("UID_Filming_PrinterSetting_Fail");
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }
    }

    public enum FilmOrientationEnum
    {
        Portrait = 0,
        Landscape = 1
    }

    public enum FilmScaleModeEnum
    {
        FitToFilm,
        Scale_1X,
        Scale_2X
        //TrueSize
    }
}
