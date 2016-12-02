using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Mcsf;
using UIH.Mcsf.Core;
using UIH.Mcsf.Filming.Wrappers;
using UIH.Mcsf.MainFrame;
using UIH.Mcsf.Viewer;
using System.Text;


namespace UIH.Mcsf.Filming
{
    public class CommonCommand
    {
        private static string MCSF_FILMING_SERVICE_FE_NAME = "FilmingServiceFE";

        public static void SendFilmingDataToServiceFE(byte[] serializedObject)
        {
            string receiver = CommunicationNodeName.CreateCommunicationProxyName(MCSF_FILMING_SERVICE_FE_NAME);
            AsyncSendData(receiver, serializedObject);
        }

        private static void AsyncSendData(string receiver, byte[] serializedObject)
        {
            try
            {
                using (var context = new CLRSendDataContext())
                {
                    context.Buffer = serializedObject;
                    context.sReceiver = receiver;
                    int ret = FilmingViewerContainee.Main.AsyncSendData(context);
                    if (ret != 0)
                    {
                        Logger.Instance.LogDevError("Failed to async send big data. Error code is " + ret);
                    }
                    context.DestoryMem();
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogDevError("Exception in ReviewController.AsyncSendData: " + ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status">"-1" Indicates that there are images Displayed, Null indicates the contrary</param>
        public static void NotifyMainFrameAboutFilmingStatus(string status)
        {
            try
            {
                Logger.LogFuncUp();

                object[] param = new object[7];
                param[0] = (int)CommandID.NOTIFY_FILMING_STATUS;
                param[1] = CommandType.AsyncCommand;
                //param[2] = "FilmingFE";
                param[4] = null;
                param[5] = null;
                param[6] = status;

                param[3] = CommunicationNodeName.CreateCommunicationProxyName("MainFrame_FE");

                FilmingViewerContainee.SendCommand(param);

                Logger.LogFuncDown();
            }
            catch (System.Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public static void NotifyMainFrameAutoShutDownAfterFilming(string studyUID)
        {
            try
            {
                Logger.LogFuncUp();

                object[] param = new object[7];
                param[0] = (int)CommandID.NOTIFY_MAINFRAME_AUTO_SHUT_DOWN;
                param[1] = CommandType.AsyncCommand;
                param[2] = "FilmingFE";
                param[3] = CommunicationNodeName.CreateCommunicationProxyName("MainFrame_FE");

                param[4] = null;
                param[5] = null;
                param[6] = studyUID;
                

                FilmingViewerContainee.SendCommand(param);

                Logger.LogFuncDown();
            }
            catch (System.Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public static void SendReplaceStudyToMainFrame(string studyInstanceUid)
        {
            try
            {
                Logger.LogFuncUp();

                //Create interData
                var interData = new InteractionInfoWrapper();
                interData.SetSrcAppName("PA");
                interData.SetDestAppName("FilmingCard");
                interData.SetOperationID(131);//(int)PAToFilmingOperationID.SendImage);

                PrepareStudyInfo(studyInstanceUid, ref interData);

                object[] param = new object[7];
                param[0] = (int)CommandID.REPLACE_STUDY;    
                param[1] = CommandType.AsyncCommand;
                //param[2] = "FilmingFE";
                param[4] = null;
                param[5] = interData.Serialize(); 
                param[6] = null;

                param[3] = CommunicationNodeName.CreateCommunicationProxyName("MainFrame_FE");

                FilmingViewerContainee.SendCommand(param);

                Logger.LogFuncDown();
            }
            catch (System.Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private static void PrepareStudyInfo(string studyInstanceUid, ref InteractionInfoWrapper interData)
        {
            var db = FilmingDbOperation.Instance.FilmingDbWrapper;
            var study = db.GetStudyByStudyInstanceUID(studyInstanceUid);
            var patient = db.GetPatientByPatientUID(study.PatientUIDFk);
            
            //fill patient info
            var mfPatientBuilder = new PatientInfo.Builder();
            mfPatientBuilder.SetAge(study.PatientAge);
            string birthdate = null == patient.PatientBirthDate
                                   ? string.Empty
                                   : patient.PatientBirthDate.Value.ToString();
            if (!string.IsNullOrEmpty(birthdate))
            {
                birthdate = string.Format("{0:yyyy/MM/dd}", patient.PatientBirthDate);
            }
            mfPatientBuilder.SetDateOfBirth(birthdate);
            mfPatientBuilder.SetGender(patient.PatientSex ?? string.Empty);
            mfPatientBuilder.SetID(patient.PatientID ?? string.Empty);
            mfPatientBuilder.SetName(patient.PatientName ?? string.Empty);

            var interPatient = mfPatientBuilder.Build();

            var interPatientList = new List<PatientInfo> {interPatient};
            interData.SetPatientList(interPatientList);

            //fill study info
            var mfStudyBuilder = new StudyInfo.Builder();
            mfStudyBuilder.SetExamStatus(study.StudyFlag);
            mfStudyBuilder.SetModalityType(study.ModalitiesInStudy ?? String.Empty);
            mfStudyBuilder.SetID(study.StudyID ?? String.Empty);
            mfStudyBuilder.SetUID(study.StudyInstanceUID ?? String.Empty);

            mfStudyBuilder.SetParentPatient(interPatient);  

            var mfStudy = mfStudyBuilder.Build();

            var mfStudyList = new List<StudyInfo> {mfStudy};
            interData.SetStudyList(mfStudyList);

        }

        public static void SendFilmingCommandToServiceFE()
        {
            try
            {
                Logger.LogFuncUp();

                object[] param = new object[7];
                param[0] = (int)CommandID.CREATE_FILMING_JOB;
                param[1] = CommandType.AsyncCommand;
                //param[2] = "FilmingFE";
                param[4] = null;
                param[5] = "Hello";
                //param[6] = sImagePathList;

                param[3] = CommunicationNodeName.CreateCommunicationProxyName("FilmingServiceFE");

                FilmingViewerContainee.SendCommand(param);

                Logger.LogFuncDown();
            }
            catch (System.Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        #region UI Operation

        //public static void SetFilmingWindowToTop()
        //{
        //    try
        //    {
        //        //string title = FilmingViewerContainee.FilmingViewerWindow.Title;

        //        //move the window to screen center
        //        FilmingViewerContainee.FilmingViewerWindow.WindowStartupLocation
        //            = WindowStartupLocation.Manual;
        //        FilmingViewerContainee.FilmingViewerWindow.Left =
        //            (SystemParameters.PrimaryScreenWidth
        //            - FilmingViewerContainee.FilmingViewerWindow.Width) / 2;
        //        FilmingViewerContainee.FilmingViewerWindow.Top =
        //            (SystemParameters.PrimaryScreenHeight
        //            - FilmingViewerContainee.FilmingViewerWindow.Height) / 2;


        //        FilmingViewerContainee.FilmingViewerWindow.Topmost = true;
        //        FilmingViewerContainee.FilmingViewerWindow.Show();
        //        FilmingViewerContainee.FilmingViewerWindow.Topmost = false;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //    }
        //}

        //public static void DoHideWindow()
        //{
        //    if (null == FilmingViewerContainee.FilmingViewerWindow)
        //    {
        //        return;
        //    }

        //    //move the window out of screen wide
        //    FilmingViewerContainee.FilmingViewerWindow.WindowStartupLocation = WindowStartupLocation.Manual;
        //    FilmingViewerContainee.FilmingViewerWindow.Left = -1500;
        //    FilmingViewerContainee.FilmingViewerWindow.Top = -1500;
        //}

        #endregion  //UI Operation

        #region Image Command

        public static void SaveEFilmsCommand(string sImagePathList)
        {
            try
            {
	            Logger.LogFuncUp();

	            object[] param = new object[7];
                param[0] = (int)CommandID.SAVE_EFILMS_COMMAND;
	            param[1] = CommandType.AsyncCommand;
	            //param[2] = "FilmingFE";
                param[4] = null;
				var encoding = Encoding.UTF8.GetBytes(sImagePathList);
                param[5] = encoding;
                //param[6] = sImagePathList;

	            param[3] = CommunicationNodeName.CreateCommunicationProxyName("FilmingService");
	
	            FilmingViewerContainee.SendCommand(param);
	
	            Logger.LogFuncDown();
            }
            catch (System.Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        //public static void SaveFilmBucketCommand(FilmsInfo.Builder filmsInfo, JobCreator jobCreator)
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();

        //        object[] param = new object[6];
        //        param[0] = (int)CommandID.SAVE_FILMS_COMMAND;
        //        param[1] = CommandType.AsyncCommand;
        //        //param[2] = "FilmingFE";
        //        param[3] = CommunicationNodeName.GetPeerCommunicationProxyName(FilmingViewerContainee.Main.GetName(), "BE");
        //        param[4] = new SaveFilmBucketCallBackHandler(jobCreator);
        //        param[5] = filmsInfo.Build().ToByteArray();

        //        FilmingViewerContainee.SendCommand(param);

        //        Logger.LogFuncDown();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //    }
        //}

        //public static void SaveFilmsCommand(FilmsInfo.Builder filmsInfo, JobCreator jobCreator)
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();
	
        //        object[] param = new object[6];
        //        param[0] = (int)CommandID.SAVE_FILMS_COMMAND;
        //        param[1] = CommandType.AsyncCommand;
        //        //param[2] = "FilmingFE";
        //        param[3] = CommunicationNodeName.GetPeerCommunicationProxyName(FilmingViewerContainee.Main.GetName(), "BE");
        //        param[4] = new SaveFilmsCallBackHandler(jobCreator);
        //        param[5] = filmsInfo.Build().ToByteArray();
	
        //        FilmingViewerContainee.SendCommand(param);
	
        //        Logger.LogFuncDown();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //    }
        //}

        //public static void LoadImageCommand(string loadImageParam)
        //{
        //    FilmingViewerContainee.SendCommand(
        //            (int)CommandID.LOAD_IMAGE_COMMAND,
        //            CommandType.AsyncCommand,
        //            "",
        //            CommunicationNodeName.GetPeerCommunicationProxyName(FilmingViewerContainee.Main.GetName(), "BE"),
        //            //new LoadImagesCallBackHandler(FilmingViewerContainee.FilmingViewerWindow),
        //            null,
        //            null,
        //            loadImageParam);
        //}

        //public static void DoClearSheetCommand(bool bUseCallback)
        //{
        //    object[] param = new object[7];
        //    param[0] = (int)CommandID.REMOVE_ALL_COMMAND;
        //    param[1] = CommandType.SyncCommand;
        //    //param[2] = "FilmingFE";
        //    param[3] = CommunicationNodeName.GetPeerCommunicationProxyName(FilmingViewerContainee.Main.GetName(), "BE");
        //    param[4] = bUseCallback ? new CleerSheetForAutoFilmingCallBackHandler(FilmingViewerContainee.FilmingViewerWindow) : null;
        //    param[5] = null;
        //    param[6] = "1";

        //    FilmingViewerContainee.SendCommand(param);
        //}

        public static void DoCreateNewViewerController(string index)
        {
            try
            {

                Logger.LogFuncUp();

                object[] param = new object[7];
                param[0] = (int)CommandID.CREATE_NEW_VIEWER_CONTROLLER;
                param[1] = CommandType.SyncCommand;
                //param[2] = "FilmingFE";

                param[4] = null;
                param[5] = null;
                param[6] = index;

                param[3] = CommunicationNodeName.GetPeerCommunicationProxyName(FilmingViewerContainee.Main.GetName(), "BE");

                FilmingViewerContainee.SendCommand(param);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        //public static void SaveImagesCommand(String sSaveImageParam)
        //{
        //    object[] param = new object[7];
        //    param[0] = (int)CommandID.SAVE_IMAGE_COMMAND;
        //    param[1] = CommandType.AsyncCommand;
        //    //param[2] = "FilmingFE";
        //    param[3] = CommunicationNodeName.GetPeerCommunicationProxyName(FilmingViewerContainee.Main.GetName(), "BE");
        //    ICommandCallbackHandler saveImageHandler = new SaveImagesCallBackHandler(FilmingViewerContainee.FilmingViewerWindow);
        //    param[4] = saveImageHandler;/*handler*/;
        //    param[5] = null;
        //    param[6] = sSaveImageParam;

        //    FilmingViewerContainee.SendCommand(param);
        //}

        #endregion  //Image Command

        #region Filming Job Command

        //public static void DoPrintCommand(FilmingPrintJob filmingPrintJob )
        //{
        //    Logger.LogFuncUp();

        //    object[] param = new object[6];
        //    param[0] = (int)CommandID.ADD_PRINT_JOB_COMMAND;
        //    param[1] = CommandType.SyncCommand;
        //    //param[2] = "FilmingFE";
        //    param[3] = CommunicationNodeName.CreateCommunicationProxyName("FilmingService");
        //    param[4] = null;//new DoPrintCallBackHandler(this);/*handler*/;
        //    param[5] = filmingPrintJob.ToByteArray();

        //    FilmingViewerContainee.SendCommand(param);

        //    DoClearSheetCommand(false);

        //    Logger.LogFuncDown();
        //}

        //public static ObservableCollection<FilmingPrintJob> DoGetCurrentJobsCommand()
        //{
        //    try
        //    {
        //        object[] param = new object[7];
        //        param[0] = CommandID.QUERY_CURRENT_PRINT_JOBS_COMMAND;
        //        param[1] = CommandType.SyncCommand;
        //        //param[2] = "FilmingFE";
        //        param[3] = CommunicationNodeName.CreateCommunicationProxyName("FilmingService");
        //        param[4] = null/*handler*/;
        //        param[5] = null;
        //        param[6] = "";

        //        object syncResult = FilmingViewerContainee.SendCommand(param);

        //        //printJob_listView.ItemsSource = null;
        //        if (null != syncResult && null != ((ISyncResult)syncResult).GetSerializedObject())
        //        {
        //            FilmingPrintJobQueue currentFilmingJobQueue = FilmingPrintJobQueue.ParseFrom(((ISyncResult)syncResult).GetSerializedObject());
        //            Logger.LogInfo("study info list count = " + currentFilmingJobQueue.JobCount.ToString());
        //            //printJob_listView.ItemsSource = null;
        //            //printJob_listView.ItemsSource = currentFilmingJobQueue.JobList;

        //            return currentFilmingJobQueue.JobList as ObservableCollection<FilmingPrintJob>;
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //        return null;
        //    }
        //    return null;
        //}

        #endregion  //Filming Job Command

        #region Image Command CallBack Handler

        //class SaveEFilmsCallBackHandler : ICommandCallbackHandler
        //{

        //    public SaveEFilmsCallBackHandler()
        //    {

        //    }

        //    #region Override Interfaces

        //    public override int HandleReply(UIH.Mcsf.Core.IAsyncResult pAsyncResult)
        //    {
        //        try
        //        {
        //            Logger.LogFuncUp();

        //            String result = pAsyncResult.GetStringObject();
        //            if (result.Equals("0"))
        //            {
        //                Logger.LogInfo("Succeed to save e-films");
        //            }
        //            else
        //            {
        //                Logger.LogError(result);
        //            }

        //            Logger.LogFuncDown();
        //        }
        //        catch (System.Exception ex)
        //        {
        //            Logger.LogFuncException(ex.Message+ex.StackTrace);
        //        }
        //        return 0;
        //    }

        //    #endregion

        //}

        //class SaveFilmBucketCallBackHandler : ICommandCallbackHandler
        //{
        //    public SaveFilmBucketCallBackHandler(JobCreator jobCreator)
        //    {
        //        _jobCreator = jobCreator;
        //    }

        //    public override int HandleReply(UIH.Mcsf.Core.IAsyncResult pAsyncResult)
        //    {
        //        try
        //        {
        //            Logger.LogFuncUp();
        //            string commandExecutingResult = pAsyncResult.GetStringObject();
        //            if (!commandExecutingResult.Equals("0"))
        //            {
        //                Logger.LogError(commandExecutingResult);
        //            }
        //            else
        //            {
        //                if (ShowNextFilmBoard())
        //                {
        //                    SaveNextFilmBucket();
        //                }
        //                else
        //                {
        //                    //do print
        //                    _jobCreator.SendFilmingJobCommand(FilmingViewerContainee.Main.GetCommunicationProxy());
        //                    //Clear Images
        //                    Clear();
        //                    EnableFilmingCardUI();
        //                }
        //            }
        //            return 0;
        //        }
        //        catch (System.Exception ex)
        //        {
        //            Logger.LogFuncException(ex.Message+ex.StackTrace);
        //            EnableFilmingCardUI();
        //            return -1;
        //        }
        //        finally
        //        {
        //            Logger.LogFuncDown();
        //        }
        //    }

        //    private bool ShowNextFilmBoard()
        //    {
        //        bool hasNextPage = true;

        //        if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
        //        {
        //            System.Windows.Threading.Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread).Invoke((FilmingCard.MethodInvoker)
        //            delegate
        //            {
        //                hasNextPage = (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).GotoNextFilmBoard();
        //            }
        //            , null);
        //        }
        //        else
        //        {
        //            hasNextPage = (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).GotoNextFilmBoard();
        //        }

        //        return hasNextPage;
        //    }

        //    private void Clear()
        //    {
        //        if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
        //        {
        //            System.Windows.Threading.Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread).Invoke((FilmingCard.MethodInvoker)
        //            delegate
        //            {
        //                (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).DeleteAllFilmPage();
        //            }
        //            , null);
        //        }
        //        else
        //        {
        //            (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).DeleteAllFilmPage();
        //        }
        //    }

        //    private void EnableFilmingCardUI()
        //    {
        //        if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
        //        {
        //            System.Windows.Threading.Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread).Invoke((FilmingCard.MethodInvoker)
        //            delegate
        //            {
        //                (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).EnableUI();
        //            }
        //            , null);
        //        }
        //        else
        //        {
        //            (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).EnableUI();
        //        }
        //    }

        //    private void SaveNextFilmBucket()
        //    {
        //        if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
        //        {
        //            System.Windows.Threading.Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread).Invoke((FilmingCard.MethodInvoker)
        //            delegate
        //            {
        //                (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).SaveCurrentFilmPage();
        //            }
        //            , null);
        //        }
        //        else
        //        {
        //            (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).SaveCurrentFilmPage();
        //        }
        //    }

        //    private JobCreator _jobCreator;
        //}

        //class SaveFilmsCallBackHandler : ICommandCallbackHandler
        //{
        //    public SaveFilmsCallBackHandler(JobCreator jobCreator)
        //    {
        //        _jobCreator = jobCreator;
        //    }

        //    public override int HandleReply(UIH.Mcsf.Core.IAsyncResult pAsyncResult)
        //    {
        //        try
        //        {
        //            Logger.LogFuncUp();
        //            string commandExecutingResult = pAsyncResult.GetStringObject();
        //            if (!commandExecutingResult.Equals("0"))
        //            {
        //                Logger.LogError(commandExecutingResult);
        //            }
        //            else
        //            {
        //                _jobCreator.SendFilmingJobCommand(FilmingViewerContainee.Main.GetCommunicationProxy());
                        
        //                //Clear Images
        //                //Clear();

        //            }
        //            return 0;
        //        }
        //        catch (System.Exception ex)
        //        {
        //            Logger.LogFuncException(ex.Message+ex.StackTrace);
        //            return -1;
        //        }
        //        finally
        //        {
        //            EnableFilmingCardUI();
        //            Logger.LogFuncDown();
        //        }
        //    }

        //    private void Clear()
        //    {
        //        if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
        //        {
        //            System.Windows.Threading.Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread).Invoke((FilmingCard.MethodInvoker)
        //            delegate
        //            {
        //                (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).DeleteAllFilmPage();
        //            }
        //            , null);
        //        }
        //        else
        //        {
        //            (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).DeleteAllFilmPage();
        //        }
        //    }

        //    private void EnableFilmingCardUI()
        //    {
        //        if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
        //        {
        //            System.Windows.Threading.Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread).Invoke((FilmingCard.MethodInvoker)
        //            delegate
        //            {
        //                (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).EnableUI();
        //            }
        //            , null);
        //        }
        //        else
        //        {
        //            (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).EnableUI();
        //        }
        //    }

        //    private JobCreator _jobCreator;
        //}

        //class SaveImagesCallBackHandler : ICommandCallbackHandler
        //{
        //    private FilmingCard _window;

        //    //private bool _bLastSheetOfImage;

        //    public SaveImagesCallBackHandler(UserControl filmingWindow)
        //    {
        //        _window = filmingWindow as FilmingCard;
        //    }

        //    #region Override Interfaces

        //    public override int HandleReply(UIH.Mcsf.Core.IAsyncResult pAsyncResult)
        //    {
        //        Logger.LogFuncUp();

        //        //step 1: Enable Begin Print Button , for Saving images is done

        //        //if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
        //        //{
        //        //    System.Windows.Threading.Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread).Invoke((FilmingCard.MethodInvoker)
        //        //    delegate
        //        //    {
        //        //        (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).SetSavingImagesFlag(false);
        //        //    }
        //        //    , null);
        //        //}
        //        //else
        //        //{
        //        //    (FilmingViewerContainee.FilmingViewerWindow as FilmingCard).SetSavingImagesFlag(false);
        //        //}

        //        ////and allow new images to be loaded or deleted

        //        //if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
        //        //{
        //        //    System.Windows.Threading.Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread).Invoke((FilmingWindow.MethodInvoker)
        //        //    delegate
        //        //    {
        //        //        (FilmingViewerContainee.FilmingViewerWindow as FilmingWindow).EnableBeginPrintButton();
        //        //    }
        //        //    , null);
        //        //}
        //        //else
        //        //{
        //        //    (FilmingViewerContainee.FilmingViewerWindow as FilmingWindow).EnableBeginPrintButton();
        //        //}


        //        ////step 2: DoPrint

        //        //String result = pAsyncResult.GetStringObject();
        //        //if (result.Equals("0"))
        //        //{
        //        //    //_window.DoPrint();
        //        //    CommonCommand.DoPrintCommand(_window.CurrentFilmingPrintJob);
        //        //}
        //        //else
        //        //{
        //        //    Logger.LogError(result);
        //        //    System.Windows.MessageBox.Show(_window, result, "Filming");
        //        //}

        //        Logger.LogFuncDown();
        //        return 0;
        //    }

        //    #endregion

        //}

        //class CleerSheetForAutoFilmingCallBackHandler : ICommandCallbackHandler
        //{
        //    private FilmingCard _window;

        //    //private bool _bLastSheetOfImage;

        //    public CleerSheetForAutoFilmingCallBackHandler(UserControl filmingWindow)
        //    {
        //        _window = filmingWindow as FilmingCard;
        //    }

        //    #region Override Interfaces

        //    public override int HandleReply(UIH.Mcsf.Core.IAsyncResult pAsyncResult)
        //    {
        //        Logger.LogFuncUp();

        //        //try
        //        //{
        //        //    String result = pAsyncResult.GetStringObject();
        //        //    if (result.Equals("0"))
        //        //    {
        //        //        _window.LoadImages();
        //        //    }
        //        //    else
        //        //    {
        //        //        Logger.LogError(result);
        //        //        System.Windows.MessageBox.Show(_window, result, "AutoFilming-Remove-Old-Images");
        //        //        _window.SetSavingImagesFlag(false);
        //        //        _window.BeginPrintButton.IsEnabled = true;
        //        //    }
        //        //}
        //        //catch (System.Exception ex)
        //        //{
        //        //    Logger.LogFuncException(ex.ToString());
        //        //    System.Windows.MessageBox.Show(_window, ex.ToString(), "AutoFilming-Loading-Images");
        //        //    _window.SetSavingImagesFlag(false);
        //        //    _window.BeginPrintButton.IsEnabled = true;
        //        //}

        //        Logger.LogFuncDown();
        //        return 0;
        //    }

        //    #endregion

        //}

        //class LoadImagesCallBackHandler : ICommandCallbackHandler
        //{
        //    private FilmingCard _window;

        //    //private bool _bLastSheetOfImage;

        //    public LoadImagesCallBackHandler(UserControl filmingWindow)
        //    {
        //        _window = filmingWindow as FilmingCard;
        //    }

        //    #region Override Interfaces

        //    public override int HandleReply(UIH.Mcsf.Core.IAsyncResult pAsyncResult)
        //    {
        //        Logger.LogFuncUp();

        //        //try
        //        //{
        //        //    //step 2: Get SopInstace UID

        //        //    String result = pAsyncResult.GetStringObject();
        //        //    if (result.Equals("0"))
        //        //    {
        //        //        _window.SaveImages();
        //        //    }
        //        //    else
        //        //    {
        //        //        Logger.LogError(result);
        //        //        System.Windows.MessageBox.Show(_window, result, "AutoFilming-Load-Images");
                        
        //        //    }
        //        //}
        //        //catch (System.Exception ex)
        //        //{
        //        //    Logger.LogFuncException(ex.ToString());
        //        //    System.Windows.MessageBox.Show(_window, ex.ToString(), "AutoFilming-Saving-Images");
                    
        //        //}

        //        Logger.LogFuncDown();
        //        return 0;
        //    }

        //    #endregion

        //}

        //class AutoFilmingSaveImagesCallBackHandler : ICommandCallbackHandler
        //{
        //    private FilmingCard _window;

        //    //private bool _bLastSheetOfImage;

        //    public AutoFilmingSaveImagesCallBackHandler(FilmingCard filmingWindow)
        //    {
        //        _window = filmingWindow;
        //    }

        //    #region Override Interfaces

        //    public override int HandleReply(UIH.Mcsf.Core.IAsyncResult pAsyncResult)
        //    {
        //        Logger.LogFuncUp();

        //        try
        //        {
        //            //step 2: Get SopInstace UID

        //            String result = pAsyncResult.GetStringObject();
        //            if (result.Equals("0"))
        //            {
        //                //_window.DoClearSheetCommand();
        //                object[] param = new object[7];
        //                param[0] = (int)CommandID.REMOVE_ALL_COMMAND;
        //                param[1] = CommandType.AsyncCommand;
        //                //param[2] = "FilmingFE";
        //                param[3] = CommunicationNodeName.GetPeerCommunicationProxyName(FilmingViewerContainee.Main.GetName(), "BE");
        //                param[4] = new SaveImagesCallBackHandler(_window);//null/*handler*/;
        //                param[5] = null;
        //                param[6] = "1";
        //                //_window.DoPrint();
        //                FilmingViewerContainee.SendCommand(param);
        //            }
        //            else
        //            {
        //                Logger.LogError(result);
        //            }
        //        }
        //        catch (System.Exception ex)
        //        {
        //            Logger.LogFuncException(ex.ToString());
        //        }

        //        Logger.LogFuncDown();
        //        return 0;
        //    }

        //    #endregion

        //}

        #endregion  //Image Command CallBack Handler

    }
}
