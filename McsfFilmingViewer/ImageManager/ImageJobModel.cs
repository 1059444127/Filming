//======================================================================
//
//        Copyright (C) 2013 Shanghai United Imaging Healthcare Inc.    
//        All rights reserved
//
//        filename :ImageJobModel
//        description : Image job model implement
//
//        created by MU Pengxuan at 2013-5-7 16:41:11
//        pengxuan.mu@united-imaging.com
//
//======================================================================

using UIH.Mcsf.Pipeline.Data;

namespace UIH.Mcsf.Filming.ImageManager
{
    using Viewer;

    public class ImageJobModel : ACommandHandler
    {
        public MedViewerControlCell ImageCell { get; set; }

        public string Modality { get; set; }

        public string SeriesInstanceUid { get; set; }

        public DicomAttributeCollection DataHeader { get; set; }

        //ImageTextFileContext+分隔符char 1+TextItemFileContext+分割符 char1+序列化后的四角信息字符串+分隔符char 1 + FilmingIdentifier+分隔符char1+胶片index+分隔符 char 1+胶片内的CellIndex
        public string ImageTextFileContext = string.Empty;
        public string TextItemFileContext = string.Empty;
        public string SerializedImageText = string.Empty;
        public string FilmingIdentifier = string.Empty;
        public int FilmIndex = -1;
        public int CellIndex = -1;
        public string UserInfo = string.Empty;

        #region Implementation of IACommandHandler

        public override void HandleCommand()
        {
            Logger.LogFuncUp();

            Logger.LogWarning("DataHeader Job Handling, FilmingIdentifier is " + FilmingIdentifier + ", CellIndex is " + CellIndex);

            var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            if (filmingCard == null)
            {
                //FilmingViewerContainee.ShowStatusWarning("UID_Filming_Receive_Image_From_Other_Module_Fail");

                //FilmingViewerContainee.Main.ShowStatusWarning("UID_Filming_Warning_Unsupport_Images", unsupportImageCount);
                return;
            }

            //if (null != FilmingViewerContainee.FilmingViewerWindow.Dispatcher)
            //{
            //    var dispatcher = Dispatcher.FromThread(FilmingViewerContainee.FilmingViewerWindow.Dispatcher.Thread);
            //    if (dispatcher != null)
            //    {
            //        dispatcher.Invoke((FilmingCard.MethodInvoker)(() => filmingCard.AddImagesToFilmCard(imageJobModel)), null);
            //    }
            //}
            //else
            {
                filmingCard.studyTreeCtrl.AddImagesToFilmCard(this);
            }

            Logger.LogFuncDown();
        }

        #endregion
    }
}
