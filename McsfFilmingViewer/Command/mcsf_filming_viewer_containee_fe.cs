using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using UIH.Mcsf.Core;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Filming.Wrappers;
using UIH.Mcsf.MainFrame.ProcHosting;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.NLS;
using UIH.Mcsf.MHC;

using UIH.Mcsf.App.Common;
using UIH.Mcsf.Filming.ImageManager;


namespace UIH.Mcsf.Filming
{
    public enum CommandID
    {
        //send to PA
        FILMING_COMPLETE_COMMAND = 3997,
        
        //Listen to PA
        IMAGES_TO_BE_LOADED_COMMAND = 7072,
        STUDY_TO_BE_LOADED_COMMAND = 7011,

        //Listen to AutoFilming task
        AUTO_FILMING_COMMAND = 7076,

        //Listen to MainFrame about PA
        PA_MAINFRAME_COMMAND = 15004,

        //Image Command ID
        LOAD_STUDY_COMMAND = 7071,
        LOAD_IMAGE_COMMAND = 7070,
        SAVE_IMAGE_COMMAND = 7073,
        REMOVE_ALL_COMMAND = 7074,

        REMOVE_CELL_COMMAND = 7075,

        SAVE_EFILMS_COMMAND = 7078,

        SAVE_FILMS_COMMAND = 7079,

        //Filming Command ID
        ADJUST_PRIORITY_OF_PRINT_JOB_COMMAND = 7081,
        DELETE_PRINT_JOB_COMMAND = 7082,//
        GET_PRINTER_CONFIG_COMMAND = 7083,
        QUERY_HISTORY_PRINT_JOB_COMMAND = 7084,
        REPRINT_COMMAND = 7085,//
        PAUSE_PRINT_JOB_COMMAND = 7086,
        RESUME_PRINT_JOB_COMMAND = 7087,
        ADD_PRINT_JOB_COMMAND = 7088,//
        QUERY_CURRENT_PRINT_JOBS_COMMAND = 7089,
        PAUSE_PRINT_COMMAND = 7090,
        RESUME_PRINT_COMMAND = 7091,

        //Listen to BE Command ID
        COUNT_OF_IMAGES_LOADING_COMMAND = 7097,
        REMOVING_ALL_IMAGES_COMMAND = 7098,
        SHOW_ON_TOP_COMMAND = 7099,

        SAVE_EFILM_COMPLETE_COMMAND = 7299,


        //FilmingPage Command ID
        CREATE_NEW_VIEWER_CONTROLLER = 7100,

        //Common Save Command ID
        SET_LAYOUT_COMMAND = 7101,

        //Send Filming meta info to FilmingServiceFE
        CREATE_FILMING_JOB = 7102,

        //Listen to MainFrame
        SWITCH_TO_APPLICATION = 1416,
        PopMiniPA_MainFrame = 15158,

        SynchronousStudyList = 15148,

        //Notify MainFrame About Filming Status
        NOTIFY_FILMING_STATUS = 15133,
        NOTIFY_MAINFRAME_AUTO_SHUT_DOWN = 15138,

        //Replace Study   i.e.  PAToFilmByMainFrameCmd
        REPLACE_STUDY = 15004,

        //Listen to Review
        AutoLoadSeries = 15042, 


        //Event From Service about configure files changed
        ImageProperty_Updated_Service_Config_Panel = 20005, 

        FilmingPage_Updated_Service_Config_Panel = 20004,

        ImageText_Updated_Service_Config_Panel = 20003,             
        Preset_Window_Service_Config_Panel = 20007,

        Update_Protocal_Content=30001,
        MCSF_SERVICE_EVENT_CONFIGURATION_XML_MODIFYED = 140011,

        DELETE_LOADED_IMAGES_BY_SERIES_UID = 20161
    }

    public enum CommandType
    {
        SyncCommand,
        AsyncCommand
    }

    /// \brief  Filming Viewer FrontEnd Containee.
    public class FilmingViewerContainee : CLRContaineeBase
    {

        #region Singleton

        static FilmingViewerContainee()
        {
        }

        private static FilmingViewerContainee _main;

        /// <summary>
        /// FilmingViewingContainee static instantiate function.
        /// </summary>
        public static FilmingViewerContainee Main
        {
            get
            {
                return _main;
            }
            set
            {
                _main = value;
            }
        }

        private static string _communicationProxyName;
        public static string CommunicationProxyName
        {
            get
            {
                try
                {
                    return _communicationProxyName ??
                           (_communicationProxyName =
                            CommunicationNodeName.GetPeerCommunicationProxyName(FilmingViewerContainee.Main.GetName(),
                                                                                "FE"));
                }
                catch (Exception)
                {
                    Logger.LogWarning("Can't Get proxy name");
                    return string.Empty;
                }
            }
        }

        private static bool _isInitialized = false;
		public static bool IsInitialized
        {
            get { return _isInitialized; }
            set { _isInitialized = value; }
        }


        public static bool IsBusy { get; set; }
        public static bool IsLoading { get; set; }

        private static ImageJobManager _dataheaderJobManagerInstance;
        public static ImageJobManager DataHeaderJobManagerInstance
        {
            get
            {
                try
                {
                    if(_dataheaderJobManagerInstance == null)
                    {
                        _dataheaderJobManagerInstance = new ImageJobManager();
                    }

                    return _dataheaderJobManagerInstance;
                }
                catch (Exception ex)
                {
                    Logger.LogFuncException(ex.Message+ex.StackTrace);
                    return null;
                }
            }
        }


        public IDataLoader ImageDataLoader { get; private set; }

        #endregion

        #region Override Containee Interfaces

        public override void Startup()
        {
            try
            {
                Logger.LogFuncUp();

                ComProxyManager.SetCurrentProxy(GetCommunicationProxy());
                // start up image job manager
                DataHeaderJobManagerInstance.JobWorkFlow = new DataHeaderJobWorker();
                DataHeaderJobManagerInstance.StartWork();

                //ImageJobManagerInstance.JobWorkFlow = new DataHeaderJobWorker();
                //ImageJobManagerInstance.StartWork();
                //FilmingHelper.CheckFilmingConfig();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                if(DataHeaderJobManagerInstance != null)
                    DataHeaderJobManagerInstance.JobWorkFlow = null;
            }
        }

        public override void DoWork()
        {
            try
            {
                Logger.LogFuncUp();

                //singleton
                Main = this;

                // Register big data cmd handler to receiver data header and image from external source
                RegisterDataHandler(new FilmingBigDataCmdHandler());

                // Register the ExamContainee to SystemManager..
                RegisterCommandHandler((int)MainFrameCmdId.AttachWindow, new AttachWindowCmdHandler());

                //register command handler
                var filmingViewerCmdHandler = new FilmingViewerCmdHandler();
                RegisterCommandHandler((int)CommandID.SHOW_ON_TOP_COMMAND, filmingViewerCmdHandler);
                RegisterCommandHandler((int)CommandID.REMOVING_ALL_IMAGES_COMMAND, filmingViewerCmdHandler);
                RegisterCommandHandler((int)CommandID.COUNT_OF_IMAGES_LOADING_COMMAND, filmingViewerCmdHandler);
                RegisterCommandHandler((int)CommandID.IMAGES_TO_BE_LOADED_COMMAND, filmingViewerCmdHandler);
                RegisterCommandHandler((int)CommandID.STUDY_TO_BE_LOADED_COMMAND, filmingViewerCmdHandler);
                RegisterCommandHandler((int)CommandID.AUTO_FILMING_COMMAND, filmingViewerCmdHandler);
                RegisterCommandHandler((int)CommandID.PA_MAINFRAME_COMMAND, filmingViewerCmdHandler);
                RegisterCommandHandler((int)CommandID.SWITCH_TO_APPLICATION, filmingViewerCmdHandler);
                RegisterCommandHandler((int)CommandID.SAVE_EFILM_COMPLETE_COMMAND, filmingViewerCmdHandler);
                RegisterCommandHandler((int)CommandID.AutoLoadSeries, filmingViewerCmdHandler);
                RegisterCommandHandler((int) CommandID.SET_LAYOUT_COMMAND, filmingViewerCmdHandler);
                

                RegisterCommandHandler((int)CommandID.SynchronousStudyList, filmingViewerCmdHandler);

                var filmingViewerEventHandler = new FilmingViewerEventHandler();
                RegisterEventHandler(11, (int)CommandID.FilmingPage_Updated_Service_Config_Panel, filmingViewerEventHandler);
                RegisterEventHandler(11, (int)CommandID.ImageProperty_Updated_Service_Config_Panel, filmingViewerEventHandler);
                RegisterEventHandler(11, (int) CommandID.ImageText_Updated_Service_Config_Panel,
                                     filmingViewerEventHandler);
                var presetWindowInfoEventHandler = new PresetWindowInfoChange();
                RegisterEventHandler(11, (int)CommandID.Preset_Window_Service_Config_Panel,presetWindowInfoEventHandler);

                RegisterEventHandler(11, (int)CommandID.Update_Protocal_Content, filmingViewerEventHandler);

                RegisterEventHandler(10, (int)CommandID.MCSF_SERVICE_EVENT_CONFIGURATION_XML_MODIFYED, filmingViewerEventHandler);

                //for MG Reject删除图片
                var deleteLoadedImagesHandler = new DeleteLoadedImagesBySeriesUids();
                RegisterCommandHandler((int)CommandID.DELETE_LOADED_IMAGES_BY_SERIES_UID, deleteLoadedImagesHandler);
                //DataLoader
                //var studyTree = new StudyTree();
                //ImageDataLoader = DataLoaderFactory.Instance().CreateLoader(studyTree, DBWrapperHelper.DBWrapper);

                //start UI Thread
                Logger.LogInfo("++opening Filming UI thread++");
                FilmingViewerThread = new Thread(InitFilmingViewerApp);
                FilmingViewerThread.SetApartmentState(ApartmentState.STA);
                FilmingViewerThread.Start();
                Logger.LogInfo("++opened Filming UI thread++");

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }

        }

        public override bool Shutdown(bool bReboot)
        {
            try
            {
                Logger.LogInfo("On Filming Card unloaded, bReboot = " + bReboot);
                Logger.Instance.LogTraceInfo("########Filming is unlocking When shut down########");
                DataHeaderJobManagerInstance.StopWork();
                //ImageJobManagerInstance.StopWork();
                FilmingDbOperation.Instance.UnLock();
                Logger.Instance.LogTraceInfo("$$$$$$$$Filming has unlocked When shut down$$$$$$$$");
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
            return true;
        }

        #endregion
        

        #region NLS support

        private static ResourceDictionary _filmingResDict = null;
        public static ResourceDictionary FilmingResourceDict
        {
            get
            {
                if(_filmingResDict == null)
                {
                    ResourceMgr nls = ResourceMgr.Instance();
                    _filmingResDict = nls.Init("Filming");
                }

                return _filmingResDict;
            }
            
        }
        
        #endregion

        #region Interfaces

        /// <summary>
        /// Send Command.
        /// </summary>
        /// <param name="param"></param>
        /// object[] param = new object[5];
        ///param[0] = 6888   iCommandId;
        ///param[1] = CommandType.AsyncCommand;    commandType
        ///param[2] = "FilmingFE";               sSender
        ///param[3] = "FilmingBE";               sReceiver
        ///param[4] = null/*handler*/;           pCommandCallback
        ///param[5] = CreatePrintJobSerialize();        sSerializeObject
        ///param[6] =                       ;             sStringObject
        ///
        public static object SendCommand(params object[] param)
        {
            try
            {
                Logger.LogFuncUp(param.GetValue(0).ToString());

                CommandContext cs = new CommandContext();
                cs.iCommandId = (int)param.GetValue(0);
                CommandType commandType = (CommandType)param.GetValue(1);
                
                if(null != param.GetValue(2))
                    cs.sSender = param.GetValue(2).ToString();

                cs.sReceiver = param.GetValue(3).ToString();
                
                cs.pCommandCallback = (ICommandCallbackHandler)param.GetValue(4);
               // cs.bServiceAsyncDispatch = true;
                if (null != param.GetValue(5))
                {
                    cs.sSerializeObject = (byte[])param.GetValue(5);
                }
                else
                {
                    cs.sStringObject = (string)param.GetValue(6);
                }

                switch ((int)commandType)
                {
                    case (int)CommandType.SyncCommand:
                        ISyncResult syncResult = Main.SyncSendCommand(cs);
                        Logger.LogFuncDown(param.GetValue(0).ToString());
                        return syncResult;
                    case (int)CommandType.AsyncCommand:
                        int iRet = Main.AsyncSendCommand(cs);
                        Logger.LogFuncDown(param.GetValue(0).ToString());
                        return iRet;
                }

                Logger.LogFuncDown();
            }
            catch (Exception exp)
            {
                Logger.LogFuncException(exp.Message);
            }

            return null;

        }

        public static void InitializeFilmingCard()
        {
            try
            {
                Logger.LogFuncUp();

                var filmingCard = FilmingViewerWindow as FilmingCard;
                if (filmingCard == null)
                {
                    return;
                }
                bool RepackStatus = Printers.Instance.IfRepack;//Utility.FilmingHelper.GetRepackStatusFromConfigFile();
                if (null != FilmingViewerWindow.Dispatcher)
                {
                    var dispatcher = Dispatcher.FromThread(FilmingViewerWindow.Dispatcher.Thread);
                    if (dispatcher != null)
                    {
                        dispatcher.Invoke((FilmingCard.MethodInvoker)(() =>
                        {
                            filmingCard.InitializeDefaultFilmingPage();
                            Keyboard.Focus(filmingCard);
                            filmingCard.InitialRepackStatus = RepackStatus;
                        }), null);
                    }
                }
                else
                {
                    filmingCard.InitializeDefaultFilmingPage();
                    Keyboard.Focus(filmingCard);
                    filmingCard.InitialRepackStatus = RepackStatus;
                }
                
                
                
                //Utility.FilmingHelper.
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }

        }
        
        #endregion

        #region UI Thread

        private static UserControl _filmingViewerWindow;

        public static UserControl FilmingViewerWindow
        {
            get { return _filmingViewerWindow; }
            set { _filmingViewerWindow = value; }
        }


        public void OnEnterSecondaryUI()
        {
            var filmingCard = _filmingViewerWindow as FilmingCard;
            Logger.LogInfo("Entering secondary window, level {0}", ++filmingCard.HostAdornerCount);
            filmingCard._maskBorder.Cursor = Cursors.Arrow;
        }

        public void OnExitSecondaryUI(bool? isQuit = null)
        {
            var filmingCard = _filmingViewerWindow as FilmingCard;
            Logger.LogInfo("Exiting secondary window, level {0}", --filmingCard.HostAdornerCount);
            filmingCard._maskBorder.Cursor = Cursors.Wait;
            if(isQuit == true)
                DataHeaderJobManagerInstance.JobFinished();
        }

        public void ResetHostAdornerCount()
        {
            (_filmingViewerWindow as FilmingCard).HostAdornerCount = 0;
            Logger.LogInfo("Get back to main window");
        }

        public void FocusOnFilmingCard()
        {
            _filmingViewerWindow.Focus();
        }

        public static Thread FilmingViewerThread;
        public string StudyInstanceUID="";

        private void InitFilmingViewerApp()
        {
            try
            {
                Logger.LogFuncUp();
                Logger.LogInfo("UI showing up");
                Main = this;

                FilmingCard filmingWindow = new FilmingCard();

                _filmingViewerWindow = filmingWindow;
                ++filmingWindow.HostAdornerCount;

                GlobalData.MainDispatcher = Dispatcher.CurrentDispatcher;


                GlobalData.MainAddInControl = filmingWindow;
                GlobalData.CommunicationProxy = Main.GetCommunicationProxy();

                GlobalData.MainDispatcher.UnhandledException += DispatcherOnUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                //inform system manager Filming Viewer is up
                if (0 != SendSystemEvent("", (int)CLRContaineeEventId.SYSTEM_COMMAND_EVENT_ID_COMPONENT_READY, GetCommunicationProxy().GetName()))
                {
                    Logger.LogWarning("The event send to System manager fail,Please restart the FilmingFEContainee");
                }
                Logger.LogInfo("has informed system manager that FilmingViewer is up");

                Dispatcher.Run();               
                
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                //MessageBox.Show(ex.StackTrace);
            }
            finally
            {
                //TO DO .. need confirm is neccery after REVIEW team refine BL
                //filmingWindow.Dispose();
                GlobalData.MainDispatcher.UnhandledException -= DispatcherOnUnhandledException;
                AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;

            }
            
            
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Instance.LogDevError("FilmingContaineeFE.CurrentDomain_UnhandledException.Crashed ");

            var exception = e.ExceptionObject;

            Logger.Instance.LogDevError(string.Format("FilmingContaineeFE.CurrentDomain_UnhandledException {0}", exception.ToString()));

            foreach (var item in exception.ToString().Split('\n'))
            {
                Logger.Instance.LogDevError(string.Format("FilmingContaineeFE.CurrentDomain_UnhandledException {0}", item));
            }
        }

        private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            Logger.Instance.LogDevError("FilmingContaineeFE.CurrentDispatcher_UnhandledException.Crashed ");

            var exception = dispatcherUnhandledExceptionEventArgs.Exception;

            Logger.Instance.LogDevError(string.Format("FilmingContaineeFE.CurrentDispatcher_UnhandledException {0}", exception.ToString()));

            foreach (var item in exception.ToString().Split('\n'))
            {
                Logger.Instance.LogDevError(string.Format("FilmingContaineeFE.CurrentDispatcher_UnhandledException {0}", item));
            }

            dispatcherUnhandledExceptionEventArgs.Handled = true;
        }

        //private void RunStandAlone()
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();

        //        var testFilmWindow = new TestFilmingWindow();
        //        var filmingCard = _filmingViewerWindow as FilmingCard;
        //        if (filmingCard != null)
        //        {
        //            var studyCtrl = filmingCard.studyTreeCtrl.seriesSelectionCtrl;
        //            filmingCard.studyTreeCtrl.PreviewDrop += new DragEventHandler(studyCtrl.FilmingStudyTreePreviewDrop);
        //            studyCtrl.InitCompareBtn();
        //            filmingCard.InitializeDefaultFilmingPage();
        //            testFilmWindow.MainGrid.Children.Add(_filmingViewerWindow);
        //        }
        //        testFilmWindow.ShowDialog();

        //        Logger.LogFuncDown();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //    }
        //    throw new Exception("exit test filming windows!");
        //}
        #endregion
    }
}

