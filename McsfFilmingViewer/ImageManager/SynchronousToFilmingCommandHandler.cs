using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.Database;
using UIH.Mcsf.MainFrame;

namespace UIH.Mcsf.Filming.ImageManager
{
    class SynchronousToFilmingCommandHandler : ACommandHandler
    {
        public SynchronousToFilmingCommandHandler(byte[] interactionStudyInfo)
        {
            _commandContext = interactionStudyInfo;
        }

        #region Overrides of ACommandHandler

        public override void HandleCommand()
        {
            Logger.LogWarning(" Synchronous to film command executed");
            if (_commandContext == null) return;
            LoadStudyByInterationInfo(_commandContext);
        }
     
        #region Private Methods

        private void LoadStudyByInterationInfo(byte[] interationStudyInfo)
        {
            try
            {
                var infoWrapper = new InteractionInfoWrapper();
                infoWrapper.Deserialize(interationStudyInfo);
                List<string> studyInstanceUidList = (from study in infoWrapper.GetStudyList() select study.UID).ToList();
                FilmingViewerContainee.Main.StudyInstanceUID = studyInstanceUidList[0];
                LoadStudies(studyInstanceUidList);
                FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
            }
        }

        private int LoadStudyNotSame(List<string> studyInstanceUidList)
        {
            var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            if (filmingCard == null)
            {
                return 0;
            }
            var k = 0;
            for (int index = 0; index < studyInstanceUidList.Count; index++)
            {
                var studyUid = studyInstanceUidList[index];
                bool isLoaded =
                    filmingCard.studyTreeCtrl.seriesSelectionCtrl.StudyListViewModel.ContainsStudy(studyUid);
                if (!isLoaded)
                {
                    k++;
                }
                
            }
            return k;
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

                var studyInstanceUidListNotSame = LoadStudyNotSame(studyInstanceUidList);

                if (studyInstanceUidListNotSame == 0) return;

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
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }


        #endregion

        private byte[] _commandContext;

        #endregion



       
    }
}