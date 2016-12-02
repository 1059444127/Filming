using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace UIH.Mcsf.Filming.ImageManager
{
    class ReviewSeriesCommandHandler : ACommandHandler
    {

        public ReviewSeriesCommandHandler(string interactionStudyInfo)
        {
            _commandContext = interactionStudyInfo;
        }

        #region Overrides of ACommandHandler

        public override void HandleCommand()
        {
            if (_commandContext == null) return;
            LoadSeries(_commandContext.Split(';'));
            Logger.LogWarning("review series command executed");
        }

        #endregion

        private void LoadSeries(IList<string> seriesUids)
        {
            try
            {
                Logger.LogFuncUp();

                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (filmingCard == null)return;

                if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
                {
                    var dispatcher = Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread);
                    if (dispatcher != null) dispatcher.Invoke((FilmingCard.MethodInvoker)(() => filmingCard.LoadSeriesBy(seriesUids)), null);
                }
                else
                {
                    filmingCard.LoadSeriesBy(seriesUids);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }


        private string _commandContext;
    }
}