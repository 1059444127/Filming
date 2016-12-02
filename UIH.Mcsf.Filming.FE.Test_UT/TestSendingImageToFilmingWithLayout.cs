using System;
using System.IO;
using System.Text;
using UIH.Mcsf.Core;
using UIH.Mcsf.DicomConvertor;
using UIH.Mcsf.Pipeline.Data;

namespace UIH.Mcsf.Filming.CardFE_UT
{
    public class TestSendingImageToFilmingWithLayout : CLRContaineeBase
    {
        public override void DoWork()
        {
            while (true)
            {
                Logger.Instance.LogDevError("Begin to send a Layout Command to Filming, Enter a key to continue...");
                Console.ReadKey();
                //SendLayout("Amy");//SendLayoutCommand();
                //SendLayout("Charlie");
                //Console.WriteLine("Begin to send a series of DataHeader to Filming");
                //SendImages("Amy");
                //SendImages("Charlie");
                SendLayout("Amy");
                SendImage(0,"Amy");
                SendLayout("Charlie");
                SendImage(2, "Amy");
                SendImage(1, "Charlie");
                SendImage(2, "Charlie");
                SendImage(4, "Amy");
                SendImage(3, "Charlie");
            }
        }

        private void SendLayout(string filmingIdentifier = FilmingIdentifier)
        {
            AsyncSendData(CommunicationNodeName.CreateCommunicationProxyName(McsfFilmingName, FrontEnd), Encoding.UTF8.GetBytes(CreateLayoutInfo(filmingIdentifier)));
        }

        private void SendLayoutCommand()
        {
            try
            {
                Logger.LogFuncUp();

                var cs = new CommandContext
                             {
                                 iCommandId = LayoutCommand,
                                 sReceiver = CommunicationNodeName.CreateCommunicationProxyName(McsfFilmingName, FrontEnd),
                                 sStringObject = CreateLayoutInfo()
                                 //bServiceAsyncDispatch = true
                             };
                //cs.sSender = "";

                //byte[] serializedJob = new byte[10]; 
                //cs.sSerializeObject = serializedJob;

                //layoutInfo Splitter
                SplitInfoString(cs.sStringObject);
               

                if (-1 == AsyncSendCommand(cs))
                {
                    throw new Exception("failed to send auto filming command to filming module!");
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
            }
        }

        /// <summary>
        /// Composed Info check
        /// </summary>
        /// <param name="layoutInfoString"></param>
        private void SplitInfoString(string layoutInfoString)
        {
            var strings = layoutInfoString.Split(ComponentModel.Chars._1);
            //foreach (var s in strings)
            //{
            //    Console.WriteLine(s);
            //}
        }

        //layoutInfo + FilmingImageNumbers + FilmingType(obsolete) + FilmingIdentifier + 分隔符
        private string CreateLayoutInfo(string filmingIdentifier = FilmingIdentifier)
        {
            string layoutInfo;

                using (var sr = new StreamReader(LayoutFilePath))
                {
                    layoutInfo = sr.ReadToEnd();
                }
            
            layoutInfo = string.Join(_separator, layoutInfo, FilmingImageNumbers, FilmingType, filmingIdentifier);

            return layoutInfo;
        }

        private void SendImages(string filmingIdentifier = FilmingIdentifier)
        {
            //1. Create Dataheader
            for (int i=0; i<FilmingImageNumbers; i++)
            {
                SendImage(i*2, filmingIdentifier);
            }
        }

        private void SendImage(int cellIndex, string filmingIdentifier = FilmingIdentifier)
        {
            var dicomConvertorProxyFactory = new McsfDicomConvertorProxyFactory();
            var dicomConvertorProxy = dicomConvertorProxyFactory.CreateDicomConvertorProxy();
            var dicomDataHeader = dicomConvertorProxy.LoadFile(DicomFilePath, GetCommunicationProxy());

            //2. Insert private tag
            var privateTag0X00613102String = CreateprivateTag0X00613102String(filmingIdentifier,cellIndex);
            SplitInfoString(privateTag0X00613102String);
            var privateTag0X00613102Value = System.Text.Encoding.UTF8.GetBytes(privateTag0X00613102String);

            //createPrivateTag
            var element = DicomAttribute.CreateAttribute(0x00613102);
            if (!element.SetBytes(0, privateTag0X00613102Value))
            {
                Logger.Instance.LogDevError("Failed to Insert NULL " + privateTag0X00613102String + " to Data header");
            }
            dicomDataHeader.AddDicomAttribute(element);

            //3.Send dataHeader
            byte[] imageData;
            dicomDataHeader.Serialize(out imageData);
            AsyncSendData(CommunicationNodeName.CreateCommunicationProxyName(McsfFilmingName, FrontEnd), imageData);
        }

        private string CreateprivateTag0X00613102String(string filmingIdentifier = FilmingIdentifier, int cellIndex = -1, int filmIndex = -1)
        {
            string imageTextConfig;
            using (var sr = new StreamReader(ImageTextFilePath))
            {
                imageTextConfig = sr.ReadToEnd();
            }
            string textItemConfig;
            using (var sr = new StreamReader(TextItemFilePath))
            {
                textItemConfig = sr.ReadToEnd();
            }

            return string.Join(_separator, imageTextConfig, textItemConfig, PsInfo, filmingIdentifier, filmIndex, cellIndex);
        }

        /// <summary>
        ///  Send big data command to other process.
        /// </summary>
        /// <param name="receiver">the proxy name of receiver process</param>
        /// <param name="serializedObject">context need to send</param>
        /// <returns></returns>
        private void AsyncSendData(string receiver, byte[] serializedObject)
        {
            try
            {
                using (var context = new CLRSendDataContext())
                {
                    context.Buffer = serializedObject;
                    context.sReceiver = receiver;
                    int ret = AsyncSendData(context);
                    if (ret != 0)
                    {
                       Logger.Instance.LogDevError("Failed to async send big data. Error code is " + ret);
                    }
                    context.DestoryMem();
                    context.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogDevError("Exception in ReviewController.AsyncSendData: " + ex);
            }
        }

        #region Send Layout Info Constant

        private const int LayoutCommand = 7101;
        private const string McsfFilmingName = "Card";
        private const string FrontEnd = "FE";
        private readonly string _separator = string.Empty + ComponentModel.Chars._1;

        private const int FilmingImageNumbers = 3;

        private const string FilmingIdentifier = "FilmingIdentifier";

        private const int FilmingType = 1; //Orientation//create new Film;

        private const string LayoutFilePath =
            @"D:\UIH\appdata\filming\config\McsfMedViewerConfig\MedViewerLayouts\mcsf_med_viewer_layout_type_18_1x2(2x1,3x1).xml";

        #endregion //Send Layout Info Constant




        #region Send DataHeader(Image) Constant

        private const string DicomFilePath = @"E:\1.dcm";

        private const string ImageTextFilePath =
            @"D:\UIH\appdata\filming\config\McsfMedViewerConfig\MedViewerImageText\mcsf_med_viewer_image_text_mr.xml";

        private const string TextItemFilePath =
            @"D:\UIH\appdata\filming\config\McsfMedViewerConfig\MedViewerImageText\mcsf_med_viewer_text_item_mr.xml";

        private const string PsInfo =
            @"<PresentationState><DisplayData><WindowCenter>600</WindowCenter><WindowWidth>600</WindowWidth></DisplayData></PresentationState>";

        #endregion  //Send DataHeader(Image) Constant


    }


}
