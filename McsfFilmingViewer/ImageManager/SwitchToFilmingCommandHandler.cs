using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.Database;
using UIH.Mcsf.MainFrame;

namespace UIH.Mcsf.Filming.ImageManager
{
    class SwitchToFilmingCommandHandler : ACommandHandler
    {
        public SwitchToFilmingCommandHandler(InteractionInfoWrapper interactionStudyInfo)
        {
            infoWrapper = interactionStudyInfo;
            _paIdToHandlerMapper = new Dictionary<PaOperationId, Action<InteractionInfoWrapper>>
                                       {
                                           {PaOperationId.Study,  ReceiveStudyHandler},
                                           {PaOperationId.Series, ReceiveSeriesHandler},
                                           {PaOperationId.Images, ReceiveImagesHandler},
                                           {PaOperationId.SeriesInterleavePrintInfo, ReceiveInterleavePrintHandler},
                                           {PaOperationId.SeriesCompareInfo, ReceiveSeriesCompareHandler}
                                       };
        }



        #region Overrides of ACommandHandler

        public override void HandleCommand()
        {
            Logger.LogWarning("switch to film command executed");
            if (infoWrapper == null) return;
            LoadStudyByInterationInfo(infoWrapper);
        }

        public enum PaOperationId
        {
            Study = 111,
            Series = 121,
            Images = 131,
            SeriesInterleavePrintInfo = 122,
            SeriesCompareInfo = 123,
            AppendTab = 15145,
            OpenNewTab = 15146,
            UpdateStudyList = 15148
        }

        private readonly Dictionary<PaOperationId, Action<InteractionInfoWrapper>> _paIdToHandlerMapper;

        #region Private Methods      
        
        public void LoadStudyByInterationInfo(InteractionInfoWrapper infoWrapper)
        {
            try
            {
                //var infoWrapper = new InteractionInfoWrapper();
                //infoWrapper.Deserialize(interationStudyInfo);
                List<string> studyInstanceUidList = (from study in infoWrapper.GetStudyList() select study.UID).ToList();
                FilmingViewerContainee.Main.StudyInstanceUID = studyInstanceUidList[0];

                LoadStudies(studyInstanceUidList);
                var contextString = "[Switched to Filming Application]" + string.Join(";", studyInstanceUidList);
                Logger.Instance.LogSvcInfo(Logger.Source, FilmingSvcLogUid.LogUidSvcInfoSwitchToFilming, contextString);

                ProcessInfomationFromPa(infoWrapper);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
            }
        }

        private void ProcessInfomationFromPa(InteractionInfoWrapper infoWrapper)
        {
            if (FilmingViewerContainee.FilmingViewerWindow as FilmingCard == null) return;
            Action<InteractionInfoWrapper> action;
            var operateId = (PaOperationId) infoWrapper.GetOperationID();
            if (_paIdToHandlerMapper.TryGetValue(operateId, out action)) 
                action(infoWrapper);
            else
            {
                Logger.LogError("Wrong OperationId from PA : " + infoWrapper.GetOperationID());
                FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
            }
        }

        private void ReceiveStudyHandler(InteractionInfoWrapper infoWrapper)
        {
            var seriesList = (from series in infoWrapper.GetSeriesList() select series.UID).ToList();
            LoadSeriesIntoCard(seriesList);
        }

        private void ReceiveSeriesHandler(InteractionInfoWrapper infoWrapper)
        {
            var seriesList = infoWrapper.GetSeriesList();
            LoadSeriesIntoCard(seriesList.Select(seriesInfo => seriesInfo.UID).ToList());
        }

        private void LoadSeriesIntoCard(IEnumerable<string> seriesUidList)
        {
            try
            {
                Logger.LogFuncUp();

                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (null == filmingCard)
                {
                    Logger.LogError("Can't get filmingCard Instance");
                    return;
                }

                if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
                {
                    var dispatcher = Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread);
                    if (dispatcher != null)
                    {
                        dispatcher.Invoke((FilmingCard.MethodInvoker)(() => filmingCard.studyTreeCtrl.SwitchToFilmingWithSeries(seriesUidList)), null);
                    }
                }
                else
                {
                    filmingCard.studyTreeCtrl.SwitchToFilmingWithSeries(seriesUidList);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        private void ReceiveImagesHandler(InteractionInfoWrapper infoWrapper)
        {
            try
            {
                Logger.LogFuncUp();

                List<string> imageUiDs = infoWrapper.GetImageList().Select(image => image.UID).ToList();
                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (null == filmingCard)
                {
                    Logger.LogError("Can't get filmingCard Instance");
                    return;
                }
                if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
                {
                    var dispatcher = Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread);
                    if (dispatcher != null)
                    {
                        dispatcher.Invoke((FilmingCard.MethodInvoker)(() => filmingCard.studyTreeCtrl.SwitchFilmingWithImages(imageUiDs)), null);
                    }
                }
                else
                {
                    filmingCard.studyTreeCtrl.SwitchFilmingWithImages(imageUiDs);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        private void ReceiveInterleavePrintHandler(InteractionInfoWrapper infoWrapper)
        {
            try
            {
                Logger.LogFuncUp();

                var seriesUidList = infoWrapper.GetSeriesList().Select(seriesInfo => seriesInfo.UID).ToList();
                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (null == filmingCard)
                {
                    Logger.LogError("Can't get filmingCard Instance");
                    return;
                }
                if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
                {
                    var dispatcher = Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread);
                    if (dispatcher != null)
                    {
                        dispatcher.Invoke((FilmingCard.MethodInvoker)(() => filmingCard.SwitchToInterleavePrint(seriesUidList)), null);
                    }
                }
                else
                {
                    filmingCard.SwitchToInterleavePrint(seriesUidList);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        private void ReceiveSeriesCompareHandler(InteractionInfoWrapper infoWrapper)
        {
            try
            {
                Logger.LogFuncUp();

                var seriesUidList = infoWrapper.GetSeriesList().Select(seriesInfo => seriesInfo.UID).ToList();
                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (null == filmingCard)
                {
                    Logger.LogError("Can't get filmingCard Instance");
                    return;
                }
                if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
                {
                    var dispatcher = Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread);
                    if (dispatcher != null)
                    {
                        dispatcher.Invoke((FilmingCard.MethodInvoker)(() => filmingCard.studyTreeCtrl.SwitchToSeriesCompare(seriesUidList)), null);
                    }
                }
                else
                {
                    filmingCard.studyTreeCtrl.SwitchToSeriesCompare(seriesUidList);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
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
                    return ;
                }
           
                if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
                {
                    var dispatcher = Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread);
                    if (dispatcher != null)
                    {
                        dispatcher.Invoke((FilmingCard.MethodInvoker)(() => filmingCard.AddDataRepositry(studyInstanceUidList)), null);
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
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }


        #endregion

        private InteractionInfoWrapper infoWrapper;

        #endregion


       
    }
}