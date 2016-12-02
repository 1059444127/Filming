using System;
using System.Windows.Threading;
using UIH.Mcsf.Core;
using UIH.Mcsf.MainFrame.ProcHosting;

namespace UIH.Mcsf.Filming
{
    public class FilmingViewerEventHandler : IEventHandler
    {
        public override int HandleEvent(string sender, int channelId, int eventId, string serialzedObject)
        {

            Logger.LogWarning("chanelID: "+channelId+", eventID: "+eventId+", serialzedObject: "+serialzedObject);

            if (eventId == (int)CommandID.FilmingPage_Updated_Service_Config_Panel && serialzedObject == "FilmingPage_Updated_Service_Config_Panel")
                UpdateFilmingPageTitleDisplay();
            else if (eventId == (int)CommandID.ImageText_Updated_Service_Config_Panel && serialzedObject == "ImageText_Updated_Service_Config_Panel")
                UpdateFilmingImageText();
            else if (eventId == (int)CommandID.ImageProperty_Updated_Service_Config_Panel && serialzedObject == "ImageProperty_Updated_Service_Config_Panel")
                UpdateFilmingImageProperty();
            else if(eventId ==(int)CommandID.Update_Protocal_Content)
            {
                UpdateProtocal();
            }
            else if (eventId == (int)CommandID.MCSF_SERVICE_EVENT_CONFIGURATION_XML_MODIFYED)
            {
                UpdatePrinterConfig();
            }

            return 0;
        }

        private static void UpdatePrinterConfig()
        {
            try
            {
                Logger.LogFuncUp();

                if(!FilmingViewerContainee.IsInitialized) return;

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
                        dispatcher.Invoke((FilmingCard.MethodInvoker)(filmingCard.UpdatePrintersConfig), null);
                    }
                }
                else
                {
                    filmingCard.UpdatePrintersConfig();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private static void UpdateProtocal()
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
                        dispatcher.Invoke((FilmingCard.MethodInvoker)(filmingCard.UpdateFilmingProtocol), null);
                    }
                }
                else
                {
                    filmingCard.UpdateFilmingProtocol();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private static void UpdateFilmingImageProperty()
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
                        dispatcher.Invoke((FilmingCard.MethodInvoker)(filmingCard.UpdateFilmingImageProperty), null);
                    }
                }
                else
                {
                    filmingCard.UpdateFilmingImageProperty();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        private static void UpdateFilmingImageText()
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
                        dispatcher.Invoke((FilmingCard.MethodInvoker)(filmingCard.UpdateFilmingImageText), null);
                    }
                }
                else
                {
                    filmingCard.UpdateFilmingImageText();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        private static void UpdateFilmingPageTitleDisplay()
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
                        dispatcher.Invoke((FilmingCard.MethodInvoker) (filmingCard.UpdateFilmingPageTitleDisplay), null);
                        dispatcher.Invoke((FilmingCard.MethodInvoker)(filmingCard.mgMethod.UpdateFilmPageSize), null);
                    }
                }
                else
                {
                    filmingCard.UpdateFilmingPageTitleDisplay();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

    }
   
}