using System;
using UIH.Mcsf.Core;
using UIH.Mcsf.Filming.ImageManager;

namespace UIH.Mcsf.Filming.Command
{
    public class FilmingBigDataCmdHandler : IDataHandler
    {
        public override int HandleDataTrans(byte[] buffer, int len)
        {
            try
            {
                if (!FilmingViewerContainee.IsInitialized)
                    FilmingViewerContainee.InitializeFilmingCard();
                
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "buffer length:"+buffer.Length);
                
                FilmingViewerContainee.DataHeaderJobManagerInstance.PushOriginalJob(buffer);

                var ret = base.HandleDataTrans(buffer, len);
                
                return ret;
            }
            catch (Exception ex)
            {
                //FilmingViewerContainee.ShowStatusWarning("UID_Filming_Receive_Image_From_Other_Module_Fail");
                FilmingViewerContainee.Main.ShowStatusWarning("UID_Filming_Receive_Image_From_Other_Module_Fail");
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                return -1;
            }
        }
    }
}
