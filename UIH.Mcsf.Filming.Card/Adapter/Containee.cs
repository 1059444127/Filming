using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using UIH.Mcsf.Core;
using UIH.Mcsf.Filming.Card.View;
using UIH.Mcsf.MainFrame.ProcHosting;
using UIH.Mcsf.NLS;
using UIH.Mcsf.MHC;
using UIH.Mcsf.App.Common;

namespace UIH.Mcsf.Filming.Card.Adapter
{
    /// \brief  Filming Viewer FrontEnd Containee.
    public class Containee : CLRContaineeBase
    {

        #region [--Singleton As Main Controller--]

        //static Containee()
        //{
        //}

        /// <summary>
        /// FilmingViewingContainee static instantiate function.
        /// </summary>
        public static Containee Main { get; set; }

        public static string CommunicationProxyName
        {
            get
            {
                try
                {
                    return _communicationProxyName ??
                           (_communicationProxyName =
                            CommunicationNodeName.GetPeerCommunicationProxyName(Main.GetName(),
                                                                                "FE"));
                }
                catch (Exception)
                {
                    Logger.LogWarning("Can't Get proxy name");
                    return string.Empty;
                }
            }
        }

        //used when initialized a film page control
        private static string _communicationProxyName;
        #endregion [--Singleton As Main Controller--]

        #region [--Override Containee Interfaces--]

        public override void Startup()
        {
            ComProxyManager.SetCurrentProxy(GetCommunicationProxy());       //used by common user control update themselves
        }

        public override void DoWork()
        {
            Logger.LogFuncUp();

            //singleton
            Main = this;

            // Register big data cmd handler to receiver data header and image from external source
            RegisterDataHandler(new DataHandler());

            // Register the ExamContainee to SystemManager..
            RegisterCommandHandler((int)MainFrameCmdId.AttachWindow,
                               new AttachWindowCmdHandler());

            //register command handler
            var filmingViewerCmdHandler = new CommandHandler();
            //RegisterCommandHandler((int)CommandID.SHOW_ON_TOP_COMMAND, filmingViewerCmdHandler);
            //RegisterCommandHandler((int)CommandID.REMOVING_ALL_IMAGES_COMMAND, filmingViewerCmdHandler);
            //RegisterCommandHandler((int)CommandID.COUNT_OF_IMAGES_LOADING_COMMAND, filmingViewerCmdHandler);
            //RegisterCommandHandler((int)CommandID.IMAGES_TO_BE_LOADED_COMMAND, filmingViewerCmdHandler);
            //RegisterCommandHandler((int)CommandID.STUDY_TO_BE_LOADED_COMMAND, filmingViewerCmdHandler);
            //RegisterCommandHandler((int)CommandID.AUTO_FILMING_COMMAND, filmingViewerCmdHandler);
            //RegisterCommandHandler((int)CommandID.PA_MAINFRAME_COMMAND, filmingViewerCmdHandler);
            RegisterCommandHandler((int)CommandId.SwitchToApplication, filmingViewerCmdHandler);
            RegisterCommandHandler((int)CommandId.SaveEfilmCompleteCommand, filmingViewerCmdHandler);

            //start UI Thread
            Logger.LogInfo("++opening Filming UI thread++");
            var filmingViewerThread = new Thread(InitFilmingViewerApp);
            filmingViewerThread.SetApartmentState(ApartmentState.STA);
            filmingViewerThread.Start();
            Logger.LogInfo("++opened Filming UI thread++");

            Logger.LogFuncDown();
        }

        public override bool Shutdown(bool bReboot)
        {
            try
            {
                Logger.LogInfo("On Filming Card unloaded, bReboot = " + bReboot);
                Logger.Instance.LogTraceInfo("########Filming is unlocking When shut down########");
                DbWrapper.Instance.UnLock();
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

        public static ResourceDictionary FilmingResourceDict
        {
            get 
            {
                try
                {
                    return _filmingResDict ?? (_filmingResDict = ResourceMgr.Instance().Init("Filming"));
                }
                catch (Exception e)
                {
                    Logger.LogFuncException(e.Message);
                    return null;
                }
            }
        }

        public static void ShowStatusInfo(string key, params object[] args)
        {
            try
            {
                Logger.LogFuncUp();

                ShowStatus("Info", StatusBarInfoType.Info, key, args);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        public static void ShowStatusWarning(string key, params object[] args)
        {
            try
            {
                Logger.LogFuncUp();

                ShowStatus("Warning", StatusBarInfoType.Warning, key, args);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        private static void ShowStatus(string iconName, StatusBarInfoType infoType, string key, params object[] args)
        {
            try
            {
                Logger.LogFuncUp();

                var statusBar = new StatusBarProxy(FilmingResourceDict, GlobalData.CommunicationProxy);
                statusBar.ShowStatusAndIcon(infoType, iconName, key, args);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        private static ResourceDictionary _filmingResDict;
        #endregion

        #region Interfaces

        /// <summary>
        /// Send Command.
        /// </summary>
        /// <param name="param"></param>
        /// object[] param = new object[5];
        ///param[0] = 6888;
        ///param[1] = "FilmingFE";
        ///param[2] = "FilmingBE";
        ///param[3] = null/*handler*/;
        ///param[4] = CreatePrintJobSerialize();
        ///
        public static object SendCommand(params object[] param)
        {
            try
            {
                Logger.LogFuncUp(param.GetValue(0).ToString());

                var cs = new CommandContext();
                cs.iCommandId = (int)param.GetValue(0);//6888
                var commandType = (CommandType)param.GetValue(1);
                //cs.sSender = param.GetValue(2).ToString();// "FilmingFE";
                cs.sReceiver = param.GetValue(3).ToString();// "FilmingBE";
                //cs.sReceiver = CommunicationNodeName.GetPeerCommunicationProxyName(Containee.Main.GetName(), "BE");
                cs.pCommandCallback = (ICommandCallbackHandler)param.GetValue(4);
              //  cs.bServiceAsyncDispatch = true;
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
                        var syncResult = Main.SyncSendCommand(cs);
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

        #endregion

        #region UI Thread

        //private static UserControl _filmingViewerWindow;

        //public static UserControl FilmingViewerWindow
        //{
        //    get { return _filmingViewerWindow; }
        //    set { _filmingViewerWindow = value; }
        //}

        //public static Thread filmingViewerThread;

        private void InitFilmingViewerApp()
        {
            try
            {
                Logger.LogFuncUp();
                Logger.LogInfo("UI showing up");
                Main = this;

                var filmingWindow = new View.Card();

                GlobalData.MainDispatcher = Dispatcher.CurrentDispatcher;
                GlobalData.MainAddInControl = filmingWindow;
                GlobalData.CommunicationProxy = Main.GetCommunicationProxy();

                //inform system manager Filming Viewer is up
                if (0 != SendSystemEvent("", (int)CLRContaineeEventId.SYSTEM_COMMAND_EVENT_ID_COMPONENT_READY, GetCommunicationProxy().GetName()))
                {
                    Logger.LogWarning("The event send to System manager fail,Please restart the FilmingFEContainee");
                }
                Logger.LogInfo("has informed system manager that FilmingViewer is up");

                //NOTE: these three lines code are used for developer! 
                //you must remove macro 'DEVELOPER' in debug version project build setting
#if DEVELOPER
                RunStandAlone();

#else
                if (Printers.Instance.IfStandAlone)
                {
                    RunStandAlone(filmingWindow);
                }
                else
                {
                    Dispatcher.Run();
                }
#endif

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
                //MessageBox.Show(ex.StackTrace);
            }
            //finally
            //{
            //    //TO DO .. need confirm is neccery after REVIEW team refine BL
            //    //filmingWindow.Dispose();
            //}


        }

        private void RunStandAlone(UserControl control)
        {

            try
            {
                Logger.LogFuncUp();

                var app = new Application();
                var window = new Window { Content = control, Title = "FilmingTestWindow"};
                app.Run(window);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
            
            //TestFilmingWindow testFilmWindow = new TestFilmingWindow();
            //testFilmWindow.MainGrid.Children.Add(control);
            //testFilmWindow.ShowDialog();
            //throw new Exception("exit test filming windows!");
        }
        #endregion
    }

    public enum CommandId
    {
        //send to PA
        //FILMING_COMPLETE_COMMAND = 3997,

        //Listen to PA
        //IMAGES_TO_BE_LOADED_COMMAND = 7072,
        //STUDY_TO_BE_LOADED_COMMAND = 7011,

        //Listen to AutoFilming task
        //AUTO_FILMING_COMMAND = 7076,

        //Listen to MainFrame about PA
        //PA_MAINFRAME_COMMAND = 15004,

        //Image Command ID
        //LOAD_STUDY_COMMAND = 7071,
        //LOAD_IMAGE_COMMAND = 7070,
        //SAVE_IMAGE_COMMAND = 7073,
        //REMOVE_ALL_COMMAND = 7074,

        //REMOVE_CELL_COMMAND = 7075,

        SaveEfilmsCommand = 7078,

        //SAVE_FILMS_COMMAND = 7079,

        //Filming Command ID
        //ADJUST_PRIORITY_OF_PRINT_JOB_COMMAND = 7081,
        //DELETE_PRINT_JOB_COMMAND = 7082,//
        //GET_PRINTER_CONFIG_COMMAND = 7083,
        //QUERY_HISTORY_PRINT_JOB_COMMAND = 7084,
        //REPRINT_COMMAND = 7085,//
        //PAUSE_PRINT_JOB_COMMAND = 7086,
        //RESUME_PRINT_JOB_COMMAND = 7087,
        //ADD_PRINT_JOB_COMMAND = 7088,//
        //QUERY_CURRENT_PRINT_JOBS_COMMAND = 7089,
        //PAUSE_PRINT_COMMAND = 7090,
        //RESUME_PRINT_COMMAND = 7091,

        //Listen to BE Command ID
        //COUNT_OF_IMAGES_LOADING_COMMAND = 7097,
        //REMOVING_ALL_IMAGES_COMMAND = 7098,
        //SHOW_ON_TOP_COMMAND = 7099,

        SaveEfilmCompleteCommand = 7299,


        //FilmingPage Command ID
        CreateNewViewerController = 7100,

        //Listen to MainFrame
        SwitchToApplication = 1416
    }

    public enum CommandType
    {
        SyncCommand,
        AsyncCommand
    }
}

