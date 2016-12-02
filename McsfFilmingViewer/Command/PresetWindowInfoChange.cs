using System;
using System.Windows.Threading;
using UIH.Mcsf.Core;

namespace UIH.Mcsf.Filming
{
    public class PresetWindowInfoChange : IEventHandler
    {
        public override int HandleEvent(string sender, int channelId, int eventId, string serialzedObject)
        {
            Printers.Instance.ReloadPresetWindowConfig();
            UpdatePresetWindowInfo();
            return 0;
        }

        private static void UpdatePresetWindowInfo()
        {
            try
            {
                Logger.LogFuncUp();

                var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                if (filmingCard == null)
                {
                    throw new Exception("filming card is not available");
                }

                if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
                {
                    var dispatcher = Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread);
                    if (dispatcher != null)
                    {
                        dispatcher.Invoke((FilmingCard.MethodInvoker)(UpdatePresetWindowInfoDetail), null);
                    }
                }
                else
                {
                    UpdatePresetWindowInfoDetail();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private static void UpdatePresetWindowInfoDetail()
        {
            var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            filmingCard.contextMenu.InitializePresetWindowInfo();
            filmingCard.contextMenu.RefreshPtWindowInfo();
        }


    }

   
}