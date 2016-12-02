using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using UIH.Mcsf.Core;
using UIH.Mcsf.Filming.ImageManager;
using UIH.Mcsf.MainFrame;
using Mcsf;
using UIH.Mcsf.Database;
using System.Linq;
using System.Text;

namespace UIH.Mcsf.Filming
{
    /// \Command handler for set the review window to top
    public class FilmingViewerCmdHandler : ICLRCommandHandler
    {
        public FilmingViewerCmdHandler()
        {

        }

        /// \brief  Handle command
        ///         
        /// \param[in] cc       command context
        /// \param[in] marObj   marshal object
        /// 
        /// \return 0 for success and -1 for failure
        public override int HandleCommand(CommandContext pContext, ISyncResult pSyncResult)
        {
            try
            {
                CommandID id = (CommandID)pContext.iCommandId;
                Logger.LogFuncUp("Command: " + id.ToString());
                switch (id)
                {
                    case CommandID.SWITCH_TO_APPLICATION:
                        {
                            Logger.LogInfo("Get Mainframe's SWITCH_TO_APPLICATION information!");

                            if (!FilmingViewerContainee.IsInitialized)
                                FilmingViewerContainee.InitializeFilmingCard();
                            
                            //var switchHandler=new SwitchToFilmingCommandHandler(pContext.sSerializeObject);
                            //switchHandler.LoadStudyByInterationInfo(pContext.sSerializeObject);
                            var infoWrapper = new InteractionInfoWrapper();
                            infoWrapper.Deserialize(pContext.sSerializeObject);

                            if (infoWrapper.GetSrcAppName().ToUpper() == "PA") //只有从pa进入的切换才进入JobManager队列处理
                            {
                                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                                if (filmingCard != null&&filmingCard.IsModalityDBT())
                                {
                                    break;
                                }
                                List<string> studyInstanceUidList = (from study in infoWrapper.GetStudyList() select study.UID).ToList();
                                if (studyInstanceUidList.Count > 0)
                                {
                                    FilmingViewerContainee.DataHeaderJobManagerInstance.PushProcessedJob(
                                        new SwitchToFilmingCommandHandler(infoWrapper));
                                }
                            }
                        }
                        break;
                    case CommandID.SynchronousStudyList:
                        {
                            Logger.LogInfo("Get Mainframe's Update_StudyList information!");

                            if (!FilmingViewerContainee.IsInitialized)
                                FilmingViewerContainee.InitializeFilmingCard();
                            SynchronousLoadStudyByInterationInfo(pContext.sSerializeObject);
                          //  FilmingViewerContainee.DataHeaderJobManagerInstance.PushProcessedJob(new SynchronousToFilmingCommandHandler(pContext.sSerializeObject));
                        }
                        break;
                    case CommandID.SAVE_EFILM_COMPLETE_COMMAND:
                        var encoding = new UTF8Encoding( );  
                        string constructedString = encoding.GetString(pContext.sSerializeObject);   
                        string[] msg = constructedString.Split('#');   //msg = "patientName" + ["#ErrorInfo"]
                        if (msg.Length > 1)
                        {
                            //FilmingViewerContainee.ShowStatusWarning("UID_Filming_Fail_To_Save_EFilm", msg[0]);
                            FilmingViewerContainee.Main.ShowStatusWarning("UID_Filming_Fail_To_Save_EFilm", msg[0]);
                        }
                        else
                        {
                            //FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Complete_To_Save_EFilm", msg[0]);
                            FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Complete_To_Save_EFilm", msg[0]);
                        }

                        
                        break;
                    case CommandID.AutoLoadSeries:  //auto load series from review
                        //var seriesUids = pContext.sStringObject.Split(';');
                        //LoadSeries(seriesUids);

                        //FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Request_Of_Loading_Series_From_Review");
                        FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Request_Of_Loading_Series_From_Review");

                        FilmingViewerContainee.DataHeaderJobManagerInstance.PushProcessedJob(new ReviewSeriesCommandHandler(pContext.sStringObject));
                        break;
                    case CommandID.SET_LAYOUT_COMMAND:

                        if (!FilmingViewerContainee.IsInitialized)
                            FilmingViewerContainee.InitializeFilmingCard();

                        FilmingViewerContainee.DataHeaderJobManagerInstance.PushProcessedJob(new SetLayoutCommandHandler(new LayoutCommandInfo(pContext.sStringObject)));
                        break;
                    default:
                        break;
                }

                Logger.LogFuncDown();

            }
            catch (Exception exp)
            {
                //MedViewerLogger.Instance.LOG_DEV_ERROR(
                //    MedViewerLogger.Source,
                //    MedViewerLogger.LogUID,
                //    "Exception: " + exp.Message);
                Logger.LogFuncException(exp.Message);
                return -1;
            }

            return 0;
        }

        private void LoadStudyByInterationInfo(byte[] interationStudyInfo)
        {
            try
            {
                var infoWrapper = new InteractionInfoWrapper();
                infoWrapper.Deserialize(interationStudyInfo);
                List<string> studyInstanceUidList = (from study in infoWrapper.GetStudyList() select study.UID).ToList();
                FilmingViewerContainee.Main.StudyInstanceUID = studyInstanceUidList[0];
                LoadStudies(studyInstanceUidList);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
            }
        }

        private void SynchronousLoadStudyByInterationInfo(byte[] interationStudyInfo)
        {
            try
            {
                var infoWrapper = new InteractionInfoWrapper();
                infoWrapper.Deserialize(interationStudyInfo);
                List<string> studyInstanceUidList = (from study in infoWrapper.GetStudyList() select study.UID).ToList();
                LoadStudies(studyInstanceUidList);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
            }
        }

        private void LoadStudies(List<string> studyInstanceUidList)
        {
            try
            {
                Logger.LogFuncUp();
                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (filmingCard == null)
                {
                    return;
                }

                if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
                {
                    var dispatcher = Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread);
                    if (dispatcher != null)
                    {
                        dispatcher.BeginInvoke((FilmingCard.MethodInvoker)(() => filmingCard.AddDataRepositry(studyInstanceUidList)), null);
                    }
                }
                else
                {
                    filmingCard.AddDataRepositry(studyInstanceUidList);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }



    }
}
