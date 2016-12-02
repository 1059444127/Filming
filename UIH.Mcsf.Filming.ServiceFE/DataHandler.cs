using System;
using System.Text;
using UIH.Mcsf.Core;

namespace UIH.Mcsf.Filming.ServiceFE
{
    public class DataHandler : IDataHandler
    {
        public override int HandleDataTrans(byte[] buffer, int len)
        {
            try
            {
                Logger.LogFuncUp();
                
                JobDispatcher.Instance.AddJobToFilmCard(buffer);

                var ret = base.HandleDataTrans(buffer, len);

                Logger.LogFuncDown("Return: " + ret);

                return ret;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                return -1;
            }
        }
    }
}
