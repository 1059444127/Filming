using UIH.Mcsf.Core;

namespace UIH.Mcsf.Filming.Card.Adapter
{
    public class DataHandler : IDataHandler
    {
        public override int HandleDataTrans(byte[] buffer, int len)
        {
        //    try
        //    {
        //        Logger.LogFuncUp();

        //        var filmingCard = Containee.FilmingViewerWindow as Card;
        //        if (filmingCard == null)
        //        {
        //            Containee.ShowStatusWarning("UID_Filming_Receive_Image_From_Other_Module_Fail");
        //            return -1;
        //        }

        //        DicomAttributeCollection dataHeader = DicomAttributeCollection.Deserialize(buffer);

        //        if (null != Containee.FilmingViewerWindow.Dispatcher)
        //        {
        //            var dispatcher = Dispatcher.FromThread(Containee.FilmingViewerWindow.Dispatcher.Thread);
        //            if (dispatcher != null)
        //            {
        //                dispatcher.Invoke((Card.MethodInvoker)(() => filmingCard.AddImagesToFilmCard(dataHeader)), null);
        //            }
        //        }
        //        else
        //        {
        //            filmingCard.AddImagesToFilmCard(dataHeader);
        //        }

        //        var ret = base.HandleDataTrans(buffer, len);

        //        Logger.LogFuncDown("Return: " + ret);

        //        return ret;
        //    }
        //    catch (Exception ex)
        //    {
        //        Containee.ShowStatusWarning("UID_Filming_Receive_Image_From_Other_Module_Fail");
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //        return -1;
        //    }
            return -1;
        }
    }
}
