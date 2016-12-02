using System;
using System.Windows.Threading;

namespace UIH.Mcsf.Filming.ImageManager
{
    class SetLayoutCommandHandler : BatchJob
    {

        public SetLayoutCommandHandler(LayoutCommandInfo layoutInfo) : base(layoutInfo.FilmingIdentifier, (int)layoutInfo.ImageCount+1)
        {
            _layoutInfo = layoutInfo;
        }

        #region Implementation of IACommandHandler

        public override void HandleCommand()
        {
            try
            {
                Logger.LogFuncUp();

                Logger.LogWarning("SetLayoutCommand Handling:  FilmingIdentifier is : " + Id);

                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (filmingCard == null) return;

                if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
                {
                    var dispatcher = Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread);
                    if (dispatcher != null) dispatcher.Invoke((FilmingCard.MethodInvoker)(() => filmingCard.layoutCtrl.SetLayoutForCommonFilming(_layoutInfo)), null);
                }
                else
                {
                    filmingCard.layoutCtrl.SetLayoutForCommonFilming(_layoutInfo);
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

        private readonly LayoutCommandInfo _layoutInfo;
    }
}