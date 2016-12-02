using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIH.Mcsf.Core;
using UIH.Mcsf.MHC;
using UIH.Mcsf.MainFrame.ProcHosting;

namespace UIH.Mcsf.Filming
{
    public static class ContaineeExtension
    {
        public static void ShowStatusInfo(this CLRContaineeBase containee, string key, params object[] args)
        {
            try
            {
                Logger.LogFuncUp();

                ShowStatus("Info", StatusBarInfoType.Info, key, args);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        public static void ShowStatusWarning(this CLRContaineeBase containee, string key, params object[] args)
        {
            try
            {
                Logger.LogFuncUp();

                ShowStatus("Warning", StatusBarInfoType.Warning, key, args);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        public static void ShowStatusError(this CLRContaineeBase containee, string key, params object[] args)
        {
            ShowStatus("Error", StatusBarInfoType.Error, key, args);
        }

        private static void ShowStatus(string iconName, StatusBarInfoType infoType, string key, params object[] args)
        {
            try
            {
                Logger.LogFuncUp();

                StatusBarProxy statusBar = new StatusBarProxy(Nls.Instance.ResourceDictionary, GlobalData.CommunicationProxy);
                statusBar.ShowStatusAndIcon(infoType, iconName, key, args);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }
    }
}
