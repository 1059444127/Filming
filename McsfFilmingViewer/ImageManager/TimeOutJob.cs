using System;
using System.Windows.Threading;

namespace UIH.Mcsf.Filming.ImageManager
{
    public class TimeOutJob : ACommandHandler
    {
        #region Overrides of ACommandHandler

        public override void HandleCommand()
        {
            try
            {
                Logger.LogFuncUp();

                FilmingViewerContainee.Main.ShowStatusError("UID_Filming_Time_Out_For_Receiving_Images");

                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (filmingCard == null)return;

                if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
                {
                    var dispatcher = Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread);
                    if (dispatcher != null) dispatcher.Invoke((FilmingCard.MethodInvoker)(filmingCard.ResetUI), null);
                }
                else
                {
                    filmingCard.ResetUI();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }


        #endregion
    }
}