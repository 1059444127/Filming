using System;
using System.Windows.Threading;

namespace UIH.Mcsf.Filming.ImageManager
{
    class DataHeaderBatchJob : BatchJob
    {
        public DataHeaderBatchJob(int dataHeaderCount) : base("", dataHeaderCount + 1)
        {
            _dataHeaderCount = dataHeaderCount;
            //Add(this);
        }

        private readonly int _dataHeaderCount;

        #region Implementation of IACommandHandler

        public override void HandleCommand()
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
                        dispatcher.Invoke((FilmingCard.MethodInvoker)(() => filmingCard.ImagesLoadBeginning((uint)_dataHeaderCount)), null);
                    }
                }
                else
                {
                    filmingCard.ImagesLoadBeginning((uint)_dataHeaderCount);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
            finally
            {
                FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
            }
        }

        #endregion
    }
}
