using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using McsfCommonSave;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.Core;

namespace UIH.Mcsf.Filming.Auto
{
    public class Job
    {
        #region [--Construtor--]
        /// <summary>
        /// Constructor
        /// </summary>
        public Job()
        {
            try
            {
                Logger.LogFuncUp();

                init();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }

        }

        #endregion [--Construtor--]

        #region [--Static--]

        private int _timeOfWaitingForFilmBurning = MiscellaneousConfigureReader.Instance.TimeOfWaitingForFirstFilmBurning;

        #endregion [--Static--]

        #region Setters

        ///// <summary>
        ///// use default printer
        ///// </summary>
        //public void SetPrinter()
        //{
        //    PeerNode node = new PeerNode();
        //    _printer.GetPacsNodeParametersByAE(_printer.DefaultAE, ref node);
        //    _printerAE = node.PeerAE;
        //    _printerIP = node.PeerIP;
        //    _printerPort = node.PeerPort;
        //}

        /// <summary>
        /// use an AENode as printer
        /// </summary>
        /// <param name="node"></param>
        public void SetPrinter(PeerNode node)
        {
            _printerAE = node.PeerAE;
            _printerIP = node.PeerIP;
            _printerPort = node.PeerPort;
        }

        /// <summary>
        /// set printing layout
        /// </summary>
        /// <param name="eLayoutType"></param>
        /// <param name="column"></param>
        /// <param name="row"></param>
        public void SetLayout(LAYOUT_TYPE eLayoutType, uint column, uint row)
        {
            _column = column;
            _row = row;
            switch(eLayoutType)
            {
            case LAYOUT_TYPE.STANDARD:
                _layout = "STANDARD\\"+column.ToString()
                    +","+row.ToString();
                _sheetImageCount= column*row;
                break;
            case LAYOUT_TYPE.ROW:
                _layout = "ROW\\"+column.ToString()
                    +","+row.ToString();
                _sheetImageCount= column*row;
                Logger.LogWarning("don't support ROW model now!");
                break;
            case LAYOUT_TYPE.COL:
                _layout = "COL\\"+column.ToString()
                    +","+row.ToString();
                _sheetImageCount= column*row;
                Logger.LogWarning("don't support COL model now!");
                break;
            }
        }

        #endregion

        #region Getters

        /// <summary>
        /// Get auto filming strategy, incremental or complete
        /// </summary>
        /// <returns></returns>
        public EnumAutoFilmStrategy GetAutoFilmStrategy()
        {
            return _printer.AutoFilmStrategy;
        }

        /// <summary>
        /// Get Printer List from configure file
        /// </summary>
        /// <returns></returns>
        public List<PeerNode> GetPrinterList()
        {
            return _printer.PeerNodes;
        }

        /// <summary>
        /// Get Printer List for auto filming
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PeerNode> GetAutoFilmingPrinterList()
        {
            return
                from printer in GetPrinterList()
                where printer.AllowAutoFilming
                select printer;
        }

        #endregion


        #region Properties

        public string FilmSize { get; set; }

        /// <summary>
        /// dicom file to be printed
        /// </summary>
        public IList<string> DicomFilePathList
        {
            set {_dicomFilePathList = value;}
            get { return _dicomFilePathList; }
        }

        public PRINT_PRIORITY PrintPriority
        {
            set { _printPriority = value; }
            get { return _printPriority; }
        }

        public string PatientID
        {
            set { _patientID = value; }
        }

        public string PatientName
        {
            set {_patientName = value;}
        }

        public string PatientSex
        {
            set {_patientSex = value;}
        }

        public string PatientAge
        {
            set {_patientAge = value;}
        }

        public string StudyID
        {
            set { _studyID = value; }
        }

        /// <summary>
        /// how many copies to print
        /// </summary>
        public uint Copies
        {
            set {_copies = value;}
            get {return _copies;}
        }

        public string AccessionNO
        {
            set {_accessionNo = value;}
        }
    
        public string OperatorName
        {
            set {_operatorName = value;}
        }

        public bool CanAutoFilming
        {
            get { return _printer.CanAutoFilming; }
            set { _printer.CanAutoFilming = value; }
        }

        public List<string> LutFiles
        {
            get { return _printer.LutFiles; }
        }

        public string LutFile
        {
            set { _lutFile = _printer.LutFileDirectory + value; }
        }

        /// <summary>
        /// For update print status, if not filled, will try to fetch image sopinstanceUID tag in dicom file
        /// </summary>
        public IList<string> OriginalImageUIDList
        {
            //get { return _originalImageUIDList; }
            set { _originalImageUIDList = value; }
        }

        public Orientation Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        public string StudyInstanceUid
        {
            set { _studyInstanceUid = value; }
        }

        public bool IfSaveEFilm
        {
            set { _ifSaveEFilm = value; }
        }

        #endregion

        #region Interface

        /// <summary>
        /// send filming command to filming module
        /// </summary>
        /// <param name="proxy">communication proxy of the filming command sender</param>
        public void SendFilmingJobCommand(ICommunicationProxy proxy, bool withAnnotations=true)
        {
            try
            {
                Logger.LogFuncUp();

                _proxy = proxy;
                CommandContext cs = new CommandContext();
                cs.iCommandId = MCSF_AUTO_FILMING_COMMAND_ID;//7088
                //cs.sSender = param.GetValue(2).ToString();// "FilmingFE";
                //cs.sReceiver = param.GetValue(3).ToString();// "FilmingBE";
                //cs.sReceiver = "FilmingBE";
                cs.sReceiver = CommunicationNodeName.CreateCommunicationProxyName(MCSF_FILMING_NAME);//CreateCommunicationProxyName(MCSF_FILMING_NAME, FRONT_END);
                //cs.pCommandCallback = (ICommandCallbackHandler)param.GetValue(4);
                //cs.bServiceAsyncDispatch = true;
                byte[] serializedJob = CreateFilmingJobInstance(withAnnotations);
                cs.sSerializeObject = serializedJob;

               // cs.bServiceAsyncDispatch = true;

                if (-1 == _proxy.AsyncSendCommand(cs))
                {
                    throw new Exception("failed to send auto filming command to filming module!");
                }

                FilmingUtility.UpdatePrintStatus(new List<string>{_studyInstanceUid}, DBWrapperHelper.DBWrapper, _proxy);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
            }
        }
        public void SuspendAllFilmingJob(ICommunicationProxy proxy)
        {
            try
            {
                Logger.LogFuncUp();

                _proxy = proxy;
                CommandContext cs = new CommandContext();
                cs.iCommandId = SUSPEND_ALL_PRINT_JOB_COMMAND;//7092
                cs.sReceiver = CommunicationNodeName.CreateCommunicationProxyName(MCSF_FILMING_NAME);
               // cs.bServiceAsyncDispatch = true;
                if(-1 == _proxy.AsyncSendCommand(cs))
                {
                    throw new Exception("failed to send SuspendAllFilmingJob command to filming module!");
                }

                Logger.LogFuncDown();

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
            }
        }
        public void ResumeAllFilmingJob(ICommunicationProxy proxy)
        {
            try
            {
                Logger.LogFuncUp();

                _proxy = proxy;
                CommandContext cs = new CommandContext();
                cs.iCommandId = RESUME_ALL_PRINT_JOB_COMMAND;//7093
                cs.sReceiver = CommunicationNodeName.CreateCommunicationProxyName(MCSF_FILMING_NAME);
                //cs.bServiceAsyncDispatch = true;
                if (-1 == _proxy.AsyncSendCommand(cs))
                {
                    throw new Exception("failed to send ResumeAllFilmingJob command to filming module!");
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
            }
        }

        #endregion
    
        #region Fields
        //private FilmingPrintJob _filmingPrintJob;

        private string _printerAE = string.Empty;

        private string _ourAE = string.Empty;

        private string _printerIP = string.Empty;

        private ushort _printerPort;

        private PRINT_PRIORITY _printPriority;

        private string _patientID = "N/A";

        private string _patientName = "N/A";

        private string _patientSex = "N/A";

        private string _patientAge = "N/A";

        private string _operatorName = "N/A";

        private string _accessionNo = "N/A";

        private string _studyID = "N/A";

        private string _studyInstanceUid = string.Empty;

        private bool _ifSaveEFilm = false;

        private uint _copies;

        private string _layout = string.Empty;

        private IList<string> _dicomFilePathList;

        private uint _sheetImageCount;

        private Printers _printer = Printers.Instance;

        private string _lutFile = string.Empty;

        private IList<string> _originalImageUIDList = new List<string>();

        private Orientation _orientation = Orientation.Portrait;

        #endregion

        #region Command Parameters
        private const int MCSF_AUTO_FILMING_COMMAND_ID = 7088;//7076
        private const int SUSPEND_ALL_PRINT_JOB_COMMAND = 7092;
        private const int RESUME_ALL_PRINT_JOB_COMMAND = 7093;
        //private const int MCSF_ADD_FILMING_JOB_COMMAND_ID = 7088;
        private const string MCSF_FILMING_NAME = "FilmingService";
        private const string FRONT_END = "FE";
        private const string BACK_END = "BE";

        static List<string> filmImageList = new List<string>();

        private uint _column = 1;
        private uint _row = 1;
        #endregion

        #region Private Methods
        
        void init()
        {
            try
            {
                Logger.LogFuncUp();
                #region Fields initialization
                _printerAE = "";
                _ourAE = "";
                _printerIP = "";
                _printerPort = 0;
                _printPriority = PRINT_PRIORITY.MEDIUM;
                _copies = 1;
                _layout = "";
                _sheetImageCount = 1;
                #endregion

                #region Fields setting by parsing configure file

                _ourAE = _printer.OurAE;
                _printerAE= _printer.DefaultAE;
                Orientation = (Orientation)_printer.DefaultOrientation;
                PeerNode peerNode = new PeerNode();
                if (-1 == _printer.GetPacsNodeParametersByAE(_printerAE, ref peerNode))
                {
                    Logger.LogError("Not found default Pacs Node for filming");
                }

                _printerPort = peerNode.PeerPort;
                _printerIP = peerNode.PeerIP;

                #endregion

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        private ICommunicationProxy _proxy;
        private string _efilmFullPath;
        private static string _efilmOriginalSopInstanceUID = string.Empty;
        private static string _efilmOriginalStudyInstanceUid = string.Empty;
        private static ManualResetEvent lockEvent = new ManualResetEvent(false);

        /// <summary>
        /// after call Set* methods init instance, then call this 
        /// function to create a ProtoBuffer serialized object
        /// </summary>
        /// <returns>
        /// a serialized filming job instance
        /// if there is an exception, return null
        /// </returns>
        byte[] CreateFilmingJobInstance(bool withAnnotations)
        {
            Logger.LogFuncUp();
            byte[] serializedJob = null;
            try
            {
                Logger.LogFuncUp("Is AutoFilming : " + withAnnotations.ToString());

                FilmingPrintJob filmingPrintJob = new FilmingPrintJob();
                FilmingPrintJob.Builder filmingPrintJobBuilder = new FilmingPrintJob.Builder();

                filmingPrintJobBuilder.SetPrinterAE(_printerAE);
                filmingPrintJobBuilder.SetOurAE(_ourAE);
                filmingPrintJobBuilder.SetPrinterIP(_printerIP);
                filmingPrintJobBuilder.SetPort(_printerPort);

                filmingPrintJobBuilder.SetPrintPriority((FilmingPrintJob.Types.PrintPriority)_printPriority);
                filmingPrintJobBuilder.SetPrintTiming(FilmingPrintJob.Types.PrintTiming.IMMEDIATELY);

                filmingPrintJobBuilder.SetPatientId(_patientID);
                filmingPrintJobBuilder.SetPatientName(_patientName);
                filmingPrintJobBuilder.SetPatientSex(_patientSex);
                filmingPrintJobBuilder.SetPatientAge(_patientAge);
                filmingPrintJobBuilder.SetOperatorName(_operatorName);
                filmingPrintJobBuilder.SetAccessionNo(_accessionNo);
                filmingPrintJobBuilder.SetStudyId(_studyID);

                filmingPrintJobBuilder.SetCopies((int)_copies);

                filmingPrintJobBuilder.SetFilmingDate(DateTime.Now.ToShortDateString());
                filmingPrintJobBuilder.SetFilmingTime(DateTime.Now.ToShortTimeString());

                {//DR Filming : Set Medium Type,  FilmDestination
                    var peerNodes = Printers.Instance.PeerNodes;
                    var peerNode = peerNodes.FirstOrDefault(node => node.PeerAE == _printerAE);
                    if (peerNode != null)
                    {
                        var mediumType = peerNode.SupportMediumTypeList.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.ToString()));
                        if (mediumType != null)
                        {
                            filmingPrintJobBuilder.SetMediaType(mediumType.ToString());
                            Logger.Instance.LogDevInfo("[XR Print Parameter][MediumType]" + mediumType.ToString() );
                        }

                        var filmDestination =
                            peerNode.SupportFilmDestinationList.FirstOrDefault(
                                m => !string.IsNullOrWhiteSpace(m.ToString()));
                        if (filmDestination != null)
                        {
                            filmingPrintJobBuilder.SetFilmDestination(filmDestination.ToString());
                            Logger.Instance.LogDevInfo("[XR Print Parameter][FilmDestination]" + filmDestination.ToString());
                        }
                    }
                }

                //the total image of per sheet
                int iSheetCount = 0;
                //the total image of this print job
                int iTotalImage = _dicomFilePathList.Count;

                iSheetCount = (int) Math.Ceiling( (0.0+iTotalImage)/_sheetImageCount );

                for (int i = 0, iCurrentIndex = 0; i < iSheetCount; i++)
                {

                    FilmingPrintJob.Types.FilmBox.Builder filmBoxBuilder = new FilmingPrintJob.Types.FilmBox.Builder();
                    if (FilmSize != null) filmBoxBuilder.SetFilmSize(FilmSize);

                    if (withAnnotations)
                    {
                        filmBoxBuilder.Orientation = (FilmingPrintJob.Types.Orientation) Printers.Instance.GetDefaultOrientation(_printerAE);
                        var defaultFilmSize = Printers.Instance.GetDefaultFilmSizeOf(_printerAE);
                        if(defaultFilmSize != null)
                        {
                            filmBoxBuilder.SetFilmSize(defaultFilmSize);
                            FilmSize = defaultFilmSize;
                        }
                            
                        //----------------------------new code begin------------------------------------
                        filmImageList.Clear();
                        for (int j = 0; j < _sheetImageCount && iCurrentIndex < iTotalImage; j++, iCurrentIndex++)
                        {
                            filmImageList.Add(_dicomFilePathList[iCurrentIndex]);
                        }

                        _efilmFullPath = Printers.Instance.PrintObjectStoragePath + "/" + DateTime.Now.Millisecond + ".dcm";

                        _dpi = Printers.Instance.GetMaxDensityOf(_printerAE);
                        _size = ConvertFilmSizeFrom(FilmSize, _dpi);
                        if (_staThread == null)
                        {
                            _staThread = new Thread(new ThreadStart(new Action(()=>SaveEFilm(_proxy,_ifSaveEFilm))));
                            _staThread.SetApartmentState(ApartmentState.STA);
                            _staThread.Start();
                        }

                        windowLockEvent.Set();
                        //Console.WriteLine("window lock set");

                        //Console.WriteLine("job lock: wait image burnt");
                        Logger.LogWarning("Waiting for film burning (milliseconds): " + _timeOfWaitingForFilmBurning);
                        lockEvent.WaitOne(millisecondsTimeout:_timeOfWaitingForFilmBurning);    //first time: 7000, waiting for dll loaded
                        _timeOfWaitingForFilmBurning = MiscellaneousConfigureReader.Instance.TimeOfWaitingForFilmBurning; //later: 3000
                        lockEvent.Reset();
                        //t.Abort();
                        //Console.WriteLine("Job lock reset");

                        //Console.WriteLine("begin print");

                        filmBoxBuilder.SetLayout("STANDARD\\1,1");
                        filmBoxBuilder.SetOrientation((FilmingPrintJob.Types.Orientation)Orientation);

                        FilmingPrintJob.Types.ImageBox.Builder imageBoxBuilder = new FilmingPrintJob.Types.ImageBox.Builder();
                        if(_efilmOriginalSopInstanceUID == string.Empty)
                            Logger.LogError("can't get sopinstanceUID of image to be print, may be print failed");
                        imageBoxBuilder.SetOriginSOPInstanceUID(_efilmOriginalSopInstanceUID);    
                        imageBoxBuilder.SetImagePath(_efilmFullPath);
                        filmBoxBuilder.AddImageBox(imageBoxBuilder);
                        _studyInstanceUid = _efilmOriginalStudyInstanceUid;
                        //----------------------------new code end------------------------------------ 
                    }
                    else
                    {
                        //------------------------------------old code begin---------------------------------
                        filmBoxBuilder.SetLayout(_layout);
                        filmBoxBuilder.SetLutFilePath(_lutFile);
                        filmBoxBuilder.SetOrientation((FilmingPrintJob.Types.Orientation)Orientation);

                        for (int j = 0; j < _sheetImageCount && iCurrentIndex < iTotalImage; j++, iCurrentIndex++)
                        {
                            FilmingPrintJob.Types.ImageBox.Builder imageBoxBuilder = new FilmingPrintJob.Types.ImageBox.Builder();
                            imageBoxBuilder.SetImagePath(_dicomFilePathList[iCurrentIndex]);
                            filmBoxBuilder.AddImageBox(imageBoxBuilder);
                        }
                        //------------------------------------old code end---------------------------------
                    }


                    //for update image print status

                    filmBoxBuilder.StudyInstanceUid = _studyInstanceUid;

                    foreach (var imageUID in _originalImageUIDList)
                    {
                        if (string.IsNullOrEmpty(imageUID)) continue;
                        var originalImageBox = new FilmingPrintJob.Types.ImageBox.Builder();
                        originalImageBox.OriginSOPInstanceUID = imageUID;

                        filmBoxBuilder.AddImageBox(originalImageBox);
                    }

                    filmingPrintJobBuilder.AddFilmBox(filmBoxBuilder);

                }

                FilmingJobStatus.Builder filmingJobStatusBuilder = new FilmingJobStatus.Builder();
                filmingJobStatusBuilder.SetJobStatus(JobStatus.PENDING);
                filmingPrintJobBuilder.SetJobStatus(filmingJobStatusBuilder);

                filmingPrintJob = filmingPrintJobBuilder.Build();

                serializedJob = filmingPrintJob.ToByteArray();

                Logger.LogFuncDown();
            }
            catch (System.Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                serializedJob = null;
                throw;
            }

            return serializedJob;
        }

        private void SaveEFilm(Size renderSize, double dpi)
        {
            try
            {
                Logger.LogFuncUp();

                ElectronicFilmInfo electronicFilmInfo = new ElectronicFilmInfo((int)_row, (int)_column, filmImageList);

                MedViewerWindow viewerWindow = new MedViewerWindow(_proxy);
                viewerWindow.SetRenderSize(renderSize);

                viewerWindow.FilmInfo = electronicFilmInfo;
                viewerWindow.ContentRendered += new EventHandler(viewerWindow_ContentRendered);
                //viewerWindow.ImageLoadedInViewerControl += viewerWindow_ImageLoaded;

                //Console.WriteLine("show image----begin");
                viewerWindow.ShowAllImages();

                viewerWindow.ShowDialog();
                //Console.WriteLine("show image----end");

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }

            ////for test
            //Logger.LogFuncUp();

            //var window = new Window();
            ////window.ContentRendered += new EventHandler(viewerWindow_ContentRendered);
            //window.ShowDialog();

            //Logger.LogFuncDown();
        }

        void viewerWindow_ImageLoaded(object sender)
        {
            var viewerWindow = sender as MedViewerWindow;
            if (viewerWindow != null)
            {
                viewerWindow.ShowDialog();
            }
        }

        void viewerWindow_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                MedViewerWindow viewerWindow = sender as MedViewerWindow;


                //Console.WriteLine("Render image----begin");
                viewerWindow.SaveEfilm(_efilmFullPath, ref _efilmOriginalSopInstanceUID,
                    ref _efilmOriginalStudyInstanceUid, _ifSaveEFilm);
                ;//MedViewerWindow.Instance().SaveEfilm(_efilmFullPath);
                //Console.WriteLine("Render image----end");

                viewerWindow.ContentRendered -= viewerWindow_ContentRendered;
                //Console.ReadKey();
                viewerWindow.Close();//MedViewerWindow.Instance().Close();

                Logger.LogFuncDown();


                //Logger.LogFuncUp();

                //var window = sender as Window;
                //_efilmOriginalSopInstanceUID = null;
                
                //window.Close();
                lockEvent.Set();

                Logger.LogFuncDown();

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
            finally
            {
                lockEvent.Set();
            }
            
        }

        private Size ConvertFilmSizeFrom(string filmSize, int DPI, Orientation orientation = Orientation.Portrait)
        // Config:  <FilmSizeID>8INX10IN\10INX12IN\10INX14IN\11INX14IN\14INX14IN\14INX17IN\24CMX24CM\24CMX30CM</FilmSizeID>
        {
            try
            {
                Logger.LogFuncUp();
                //split by 'X'
                string[] sParameters = filmSize.Split('X');
                if (sParameters.Length != 2) throw new Exception(filmSize); //log: wrong string

                //remove unit
                string sWidth = sParameters[0];
                string sHeight = sParameters[1];
                double width, height;
                if (sWidth.EndsWith("IN") && sHeight.EndsWith("IN"))
                {
                    width = Convert.ToInt32(sWidth.TrimEnd('I', 'N'));
                    height = Convert.ToInt32(sHeight.TrimEnd('I', 'N'));
                }
                else if (sWidth.EndsWith("CM") && sHeight.EndsWith("CM")) // convert unit from cm to inch
                {
                    width = (Convert.ToInt32(sWidth.TrimEnd('C', 'M')) * 0.3937); //1cm = 0.3937inch
                    height = (Convert.ToInt32(sHeight.TrimEnd('C', 'M')) * 0.3937);
                }
                else throw new Exception(filmSize);

                Logger.LogFuncDown();
                //multiple with DPI

                switch (orientation)
                {
                    case Orientation.Landscape:
                        return new Size(height * DPI, width * DPI);
                    default:
                        return new Size(width * DPI, height * DPI);
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException("Film Size format is wrong : (" + ex.StackTrace + ")");
                return ConvertFilmSizeFrom("14INX17IN", DPI);
            }
        }

        #endregion


        #region [--OneMedViewerWindow--]

        private Thread _staThread;
        private Size _size;
        private int _dpi;
        private static MedViewerWindow _viewerWindow;
        

        private void SaveEFilm(ICommunicationProxy proxy=null,bool ifSaveEFilm = false)
        {
            try
            {
                Logger.LogFuncUp();

                if (_viewerWindow == null)
                {
                    _viewerWindow = new MedViewerWindow(proxy);
                    //MedViewerWindow.ImageBurntEvent += MedViewerWindowOnImageBurntEvent;
                }

                while (true)
                {
                    try
                    {
                        //window.Hide();
                        //Console.WriteLine("Thread: wait a job");
                        windowLockEvent.WaitOne();
                        var electronicFilmInfo = new ElectronicFilmInfo((int)_row, (int)_column, filmImageList);
                        _viewerWindow.FilmInfo = electronicFilmInfo;
                        _viewerWindow.SetRenderSize(_size);
                        _viewerWindow.ShowAllImages();
                        _viewerWindow.Show();
                        _viewerWindow.SaveEfilm(_efilmFullPath, ref _efilmOriginalSopInstanceUID, ref _efilmOriginalStudyInstanceUid, ifSaveEFilm);
                        _viewerWindow.Hide();
                        MedViewerWindowOnImageBurntEvent(); //MedViewerWindow.RaiseImageBurntEvent();
                        windowLockEvent.Reset();
                        //Console.WriteLine("Thread: window lock reset");

                    }
                    catch (Exception ex)
                    {
                        Logger.LogFuncException(ex.Message+ex.StackTrace);
                        _efilmOriginalSopInstanceUID = string.Empty;
                        MedViewerWindowOnImageBurntEvent(); //MedViewerWindow.RaiseImageBurntEvent();
                        windowLockEvent.Reset();
                        //Console.WriteLine("Thread: Exception & window lock reset");
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }

        }

        private static void MedViewerWindowOnImageBurntEvent()
        {
            //Console.WriteLine("Job: Image Burnt");
            lockEvent.Set();
            //Console.WriteLine("Job: lock set");
        }

        private static ManualResetEvent windowLockEvent = new ManualResetEvent(false);


        #endregion [--OneMedViewerWindow--]

    }

    public enum LAYOUT_TYPE
    {
        STANDARD,
        ROW,
        COL
    }

    public enum PRINT_PRIORITY
    {
       HIGH = 1,
       MEDIUM = 2,
       LOW = 3
    }
}

