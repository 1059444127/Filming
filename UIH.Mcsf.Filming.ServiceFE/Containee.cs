using System;
using System.Windows.Threading;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.Core;
using UIH.Mcsf.Filming.Widgets;
using UIH.Mcsf.MainFrame.ProcHosting;

namespace UIH.Mcsf.Filming.ServiceFE
{
    public class Containee : CLRContaineeBase
    {
        #region  [---Static Properties---]
        public static Containee Main { get; private set; }
        #endregion


        #region [--Override Methods--]
        public override void Startup()
        {
            ComProxyManager.SetCurrentProxy(GetCommunicationProxy());
            Logger.LogUid = 001035003;
            Logger.Source = "UIH.MCSF.FilmingServiceFE";
        }

        public override void DoWork()
        {
            try
            {
                Logger.LogFuncUp();

                Main = this;
                GlobalData.CommunicationProxy = Main.GetCommunicationProxy();
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Dispatcher.CurrentDispatcher.UnhandledException += DispatcherOnUnhandledException;

                RegisterDataHandler(new DataHandler());


                //Event From Service about configure files changed
                //ImageProperty_Updated_Service_Config_Panel = 20005, 
                //ImageText_Updated_Service_Config_Panel = 20003,
                RegisterEventHandler(EventID.ChannelID, EventID.ImageProperty_Updated_Service_Config_Panel, JobDispatcher.Instance);
                RegisterEventHandler(EventID.ChannelID, EventID.ImageText_Updated_Service_Config_Panel,
                                     JobDispatcher.Instance);
                RegisterEventHandler(10, EventID.MCSF_SERVICE_EVENT_CONFIGURATION_XML_MODIFYED,
                                     JobDispatcher.Instance);

                if (0 != SendSystemEvent("", (int)CLRContaineeEventId.SYSTEM_COMMAND_EVENT_ID_COMPONENT_READY, GetCommunicationProxy().GetName()))
                {
                    Logger.LogWarning("The event send to System manager fail,Please restart the FilmingFEContainee");
                }

                //Logger.LogInfo("FilmingServiceFE has informed system manager about the status");

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public override bool Shutdown(bool bReboot)
        {
            return true;
        }
        #endregion

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.LogError("FilmingContaineeSeviceFE.CurrentDomain_UnhandledException.Crashed ");

            var exception = e.ExceptionObject;

            Logger.LogFuncException(string.Format("FilmingContaineeSeviceFE.CurrentDomain_UnhandledException {0}", exception.ToString()));

            foreach (var item in exception.ToString().Split('\n'))
            {
                Logger.LogError(string.Format("FilmingContaineeSeviceFE.CurrentDomain_UnhandledException {0}", item));
            }

        }

        private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            Logger.LogError("FilmingContaineeSeviceFE.CurrentDispatcher_UnhandledException.Crashed ");

            var exception = dispatcherUnhandledExceptionEventArgs.Exception;

            Logger.LogFuncException(string.Format("FilmingContaineeSeviceFE.CurrentDispatcher_UnhandledException {0}", exception.ToString()));

            foreach (var item in exception.ToString().Split('\n'))
            {
                Logger.LogError(string.Format("FilmingContaineeSeviceFE.CurrentDispatcher_UnhandledException {0}", item));
            }

            dispatcherUnhandledExceptionEventArgs.Handled = true;
        }

    }
}
