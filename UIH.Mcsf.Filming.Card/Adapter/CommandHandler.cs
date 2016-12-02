using System;
using System.Collections.Generic;
using UIH.Mcsf.Core;
using UIH.Mcsf.MainFrame;
using System.Linq;
using System.Text;
//using System.Windows.Threading;

namespace UIH.Mcsf.Filming.Card.Adapter
{

    #region [--Override Interface--]

    /// \Command handler for set the review window to top
    public class CommandHandler : ICLRCommandHandler
    {
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
                var id = (CommandId)pContext.iCommandId;
                Logger.LogFuncUp("Command: " + id.ToString());
                switch (id)
                {
                    case CommandId.SwitchToApplication:
                        {
                            Logger.LogInfo("Get Mainframe's information!");
                            Logger.Instance.LogSvcInfo(Logger.Source, FilmingSvcLogUid.LogUidSvcInfoSwitchToFilming,
                           "Switched to Filming Application");
                            LoadStudyByInterationInfo(pContext.sSerializeObject);
                        }
                        break;
                    case CommandId.SaveEfilmCompleteCommand:
                        var encoding = new UTF8Encoding();
                        string constructedString = encoding.GetString(pContext.sSerializeObject);
                        string[] msg = constructedString.Split('#');   //msg = "patientName" + ["#ErrorInfo"]
                        if (msg.Length > 1)
                        {
                            Containee.ShowStatusWarning("UID_Filming_Fail_To_Save_EFilm", msg[0]);
                        }
                        else
                        {
                            Containee.ShowStatusInfo("UID_Filming_Complete_To_Save_EFilm", msg[0]);
                        }
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

        #endregion [--Override Interface--]


    #region [--Private Methods--]


        //1.2.156.112605.75006877305224.20121123025201.9.4420.2
        private void LoadStudyByInterationInfo(byte[] interationStudyInfo)
        {
            try
            {
                Logger.LogFuncUp();

                var infoWrapper = new InteractionInfoWrapper();
                infoWrapper.Deserialize(interationStudyInfo);

                var studyUidList = (from study in infoWrapper.GetStudyList()
                                              select study.UID).ToList();
                var seriesUidList = (from series in infoWrapper.GetSeriesList()
                                               select series.UID).ToList();

                LoadStudies(studyUidList);

                if (seriesUidList.Count == 1 && infoWrapper.GetSrcAppName() == "PA")//Efilm series from PA
                {
                    AddEFilmSeriesToFilmPage(seriesUidList.FirstOrDefault());
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
            }
        }

        private void AddEFilmSeriesToFilmPage(string seriesUid)
        {
            try
            {
                Logger.LogFuncUp("Parameters: [seriesUid: " + seriesUid + "]");

                //////////////////////////////////////////////////////////////////////////
                //// Load e film series to Filming Display area & Study Tree

                
                
                //var filmingCard = Containee.FilmingViewerWindow as Card;
                //if (filmingCard == null)
                //{
                //    return;
                //}

                //if (null != Containee.FilmingViewerWindow.Dispatcher)
                //{
                //    var dispatcher = Dispatcher.FromThread(Containee.FilmingViewerWindow.Dispatcher.Thread);
                //    if (dispatcher != null)
                //    {
                //        dispatcher.Invoke((Card.MethodInvoker)(() => filmingCard.LoadEFilmSeries(seriesUid)), null);
                //    }
                //}
                //else
                //{
                //    filmingCard.LoadEFilmSeries(seriesUid);
                //}

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }

        }

        private void LoadStudies(IList<string> studyInstanceUidList)
        {
            try
            {
                Logger.LogFuncUp();

                //Console.WriteLine(studyInstanceUidList.ToString());

                ////////////////////////////////////////////////////////////////////////////////

                //var filmingCard = Containee.FilmingViewerWindow as Card;
                //if (filmingCard == null)
                //{
                //    return;
                //}

                //if (null != Containee.FilmingViewerWindow.Dispatcher)
                //{
                //    var dispatcher = Dispatcher.FromThread(Containee.FilmingViewerWindow.Dispatcher.Thread);
                //    if (dispatcher != null)
                //    {
                //        dispatcher.Invoke((Card.MethodInvoker)(() => filmingCard.AddDataRepositry(studyInstanceUidList)), null);
                //    }
                //}
                //else
                //{
                //    filmingCard.AddDataRepositry(studyInstanceUidList);
                //}

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        #endregion [--Private Methods--]

    }
}
