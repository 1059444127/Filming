using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using McsfCommonSave;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.Database;
using UIH.Mcsf.DicomConvertor;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.NLS;
using UIH.Mcsf.Pipeline.Data;
using UIH.Mcsf.Pipeline.Dictionary;
using UIH.Mcsf.Viewer;
using UIH.Mcsf.Core;
using System.ComponentModel;
//using UIH.Mcsf.MedDataManagement;
//using UIH.Mcsf.App.Common;

namespace UIH.Mcsf.Filming.Auto
{
    /// <summary>
    /// Interaction logic for MedViewerWindow.xaml
    /// </summary>
    public partial class MedViewerWindow
    {
       #region [--Fields--]

        private Size _renderSize;

        private ICommunicationProxy _proxy;

        private ElectronicFilmInfo _electronicFilmInfo = null;


        private DicomAttributeCollection _dicomDataHeader = null;
        McsfDicomConvertorProxyFactory _dicomConvertorProxyFactory = new McsfDicomConvertorProxyFactory();
        IDicomConvertorProxy _dicomConvertorProxy = null;

        //private IDataLoader _dataLoader;

        #endregion [--Fields--]

        #region [--Properties--]

        public ElectronicFilmInfo FilmInfo
        {
            set { _electronicFilmInfo = value; }
        }

        public bool ImageLoadedFlag { get; set; }

        #endregion [--Properties--]

        #region [--Events--]

        public delegate void ImageLoadedInViewerControlHandler(object sender);

        public event ImageLoadedInViewerControlHandler ImageLoadedInViewerControl;

        private void HandleImageLoadedInViewerControl()
        {
            var handler = ImageLoadedInViewerControl;
            if (handler != null)
            {
                handler(this);
            }
        }

        #endregion [--Events--]


        #region [--Constructor--]

        //static private MedViewerWindow _medViewerWindow;
        //static public MedViewerWindow Instance(ICommunicationProxy proxy=null)
        //{
        //    _proxy = null == proxy ? _proxy : proxy;
        //    if(_medViewerWindow == null)
        //    {
        //        _medViewerWindow = new MedViewerWindow();
        //    }

        //    return _medViewerWindow;
        //}

        

        public MedViewerWindow(ICommunicationProxy proxy = null)
        {

            try
            {
                Logger.LogFuncUp();

                //_renderSize = renderSize;

                _proxy = null == proxy ? _proxy : proxy;

                InitializeComponent();

               // String sEntryPath = mcsf_clr_systemenvironment_config.GetApplicationPath("FilmingConfigPath");
                //String sReviewEntryPath = mcsf_clr_systemenvironment_config.GetApplicationPath("ReviewConfigPath");
                medViewerControl.InitializeWithoutCommProxy(@"config/review/");

                Closing += OnMedViewerWindowClosing;

                ImageLoadedFlag = false;

                ShowInTaskbar = false;
                WindowStartupLocation = WindowStartupLocation.Manual;
                //SetRenderSize(renderSize);


                //this.Focusable = false;

                //DataManagement for separation UI with data
                //var studyTree = new StudyTree();
                //_dataLoader = DataLoaderFactory.Instance().CreateLoader(studyTree, DBWrapperHelper.DBWrapper);
                //_dataLoader.SopLoadedHandler += OnImageDataLoaded;

                //ResourceMgr nls = ResourceMgr.Instance();
                //var resDic = nls.Init("Filming");
                //Resources.MergedDictionaries.Add(resDic);
                
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }

        }

        public void SetRenderSize(Size renderSize)
        {
            _renderSize = renderSize;
            Top = -renderSize.Height;
            Left = -renderSize.Width;
            if (renderSize.Width < renderSize.Height)
            {
                medViewerControl.Height = 1022; //DR review viewer control height
                medViewerControl.Width = 1022 * renderSize.Width / renderSize.Height;
            }
            else
            {
                medViewerControl.Width = 1022;  //DR review viewer control width
                medViewerControl.Height = 1022 * renderSize.Height / renderSize.Width;
            }
        }
        #endregion [--Constructor--]


        #region [--Event Subscribed--]

        private void OnMedViewerWindowClosing(object sender, CancelEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                Closing -= OnMedViewerWindowClosing;

                medViewerControl.RemoveCell(-1);

                var layoutCell = medViewerControl.LayoutManager.RootCell;
                layoutCell.RemoveAll();
                layoutCell.Refresh();
                //var layoutCellImpl = layoutCell.Control as MedViewerLayoutCellImpl;
                //if (layoutCellImpl != null) layoutCellImpl.UnRegisterEvent();
                
                medViewerControl.RemoveAll();

                Dispatcher.InvokeShutdown();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        #endregion [--Event Subscribed--]


        #region [--Interface For Filming job--]

        public void ShowAllImages()
        {
            try
            {
                Logger.LogFuncUp();

                var layoutCell = medViewerControl.LayoutManager.RootCell;
                layoutCell.RemoveAll();
                layoutCell.Refresh();
                medViewerControl.RemoveAll();

                medViewerControl.LayoutManager.Rows = _electronicFilmInfo.Rows;
                medViewerControl.LayoutManager.Columns = _electronicFilmInfo.Columns;

                foreach (var imageFilePath in _electronicFilmInfo.FilePathList)
                {
                    ShowImage(imageFilePath); //changed to use MedDataManagement to load image
                    //LoadImage(imageFilePath);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        //public void SaveEfilm(string efilmFullPath, ref string efilmOriginalSopInstanceUID)
        //{
        //    efilmOriginalSopInstanceUID = string.Empty;

        //}

        public void SaveEfilm(string efilmFullPath, ref string efilmOriginalSopInstanceUID, ref string efilmOriginalStudyInstanceUid,bool ifSaveEFilm)
        {
            efilmOriginalSopInstanceUID = string.Empty;

            try
            {
                Logger.LogFuncUp();

                if (_dicomDataHeader == null)
                {
                    return;
                }

                var wrtBmp = RenderControlWriteableBitmap();

                if (wrtBmp == null)
                {
                    throw new ApplicationException("ViewerControl Writeable Bitmap Render failed.");
                }

               

                byte[] data = ProcessImage(wrtBmp);
                Console.WriteLine(Convert.ToString(wrtBmp.PixelWidth) + " ,,,,," + Convert.ToString(wrtBmp.PixelHeight));

                ////MedViewerLayoutCell layoutCell = medViewerControl.LayoutManager.RootCell;
                ////MedViewerControlCell cell = GetControlCell(layoutCell);


                var pixelHeightTag = new DicomAttributeUS(new DicomTag(Pipeline.Dictionary.Tag.Rows));
                pixelHeightTag.SetUInt16(0, Convert.ToUInt16(wrtBmp.PixelHeight));
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.Rows);
                _dicomDataHeader.AddDicomAttribute(pixelHeightTag);

                var pixelWidthTag = new DicomAttributeUS(new DicomTag(Pipeline.Dictionary.Tag.Columns));
                pixelWidthTag.SetUInt16(0, Convert.ToUInt16(wrtBmp.PixelWidth));
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.Columns);
                _dicomDataHeader.AddDicomAttribute(pixelWidthTag);

                var bitStoredTag = new DicomAttributeUS(new DicomTag(Pipeline.Dictionary.Tag.BitsStored));
                bitStoredTag.SetUInt16(0, 8);
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.BitsStored);
                _dicomDataHeader.AddDicomAttribute(bitStoredTag);

                var bitAllocatedTag = new DicomAttributeUS(new DicomTag(Pipeline.Dictionary.Tag.BitsAllocated));
                bitAllocatedTag.SetUInt16(0, 8);
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.BitsAllocated);
                _dicomDataHeader.AddDicomAttribute(bitAllocatedTag);

                var hightBitTag = new DicomAttributeUS(new DicomTag(Pipeline.Dictionary.Tag.HighBit));
                hightBitTag.SetUInt16(0, 7);
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.HighBit);
                _dicomDataHeader.AddDicomAttribute(hightBitTag);

                var pixelRepresentationTag = new DicomAttributeUS(new DicomTag(Pipeline.Dictionary.Tag.PixelRepresentation));
                pixelRepresentationTag.SetUInt16(0, 0);
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.PixelRepresentation);
                _dicomDataHeader.AddDicomAttribute(pixelRepresentationTag);

                var rescaleSlopeTag = new DicomAttributeDS(new DicomTag(Pipeline.Dictionary.Tag.RescaleSlope));
                rescaleSlopeTag.SetString(0, "1");
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.RescaleSlope);
                _dicomDataHeader.AddDicomAttribute(rescaleSlopeTag);

                var rescaleInterceptTag = new DicomAttributeDS(new DicomTag(Pipeline.Dictionary.Tag.RescaleIntercept));
                rescaleInterceptTag.SetString(0, "0"); ;
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.RescaleIntercept);
                _dicomDataHeader.AddDicomAttribute(rescaleInterceptTag);


                ////insert Photometric Interpretation //maybe can fix inverse problem
                //var photometricInterpretationTag = new DicomAttributeUS(new DicomTag(Pipeline.Dictionary.Tag.PhotometricInterpretation));
                //rescaleInterceptTag.SetString(0, "MONOCHROME2");
                ////_dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.PhotometricInterpretation);
                //_dicomDataHeader.AddDicomAttribute(photometricInterpretationTag);
                //insert Photometric Interpretation
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.PhotometricInterpretation);
                var photometricInterpretationTag = DicomAttribute.CreateAttribute(Pipeline.Dictionary.Tag.PhotometricInterpretation);
                if (!photometricInterpretationTag.SetString(0, "MONOCHROME2"))
                {
                    throw new Exception("Failed to Insert Columns to Data header");
                }
                _dicomDataHeader.AddDicomAttribute(photometricInterpretationTag);

                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.WindowWidth);
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.WindowCenter);
                var windowWidthTag = new DicomAttributeDS(new DicomTag(Pipeline.Dictionary.Tag.WindowWidth));
                //var windowWidthTag = DicomAttribute.CreateAttribute(Pipeline.Dictionary.Tag.WindowWidth);
                windowWidthTag.SetString(0, "256");
                _dicomDataHeader.AddDicomAttribute(windowWidthTag);
                var windowCenterTag = new DicomAttributeDS(new DicomTag(Pipeline.Dictionary.Tag.WindowCenter));
                //var windowCenterTag = DicomAttribute.CreateAttribute(Pipeline.Dictionary.Tag.WindowCenter);
                windowCenterTag.SetString(0, "127");
                _dicomDataHeader.AddDicomAttribute(windowCenterTag);

                if (_dicomDataHeader.Contains(Pipeline.Dictionary.Tag.PixelData))
                {
                    _dicomDataHeader[Pipeline.Dictionary.Tag.PixelData].SetBytes(0, data);
                }
                else
                {
                    var pixelTag = new DicomAttributeOW(new DicomTag(Pipeline.Dictionary.Tag.PixelData));
                    pixelTag.SetBytes(0, data);
                    _dicomDataHeader.AddDicomAttribute(pixelTag);
                }
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.ShutterShape);
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.ShutterLowerHorizontalEdge);
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.ShutterRightVerticalEdge);
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.ShutterLeftVerticalEdge);
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.ShutterUpperHorizontalEdge);
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.ImageHorizontalFlip);
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.ImageRotation);
                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.SeriesNumber);
                //_dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.);
                //AssembleTag();

                var uidManager = McsfDatabaseDicomUIDManagerFactory.Instance().CreateUIDManager();
                var seriesInstanceUid = uidManager.CreateSeriesUID("1", "2", ""); //seriesUID; //
                var imageUid = uidManager.CreateImageUID("");

                //_dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.SeriesInstanceUid);
                //var seriesInstanceUidTag = new DicomAttributeUI(new DicomTag(Pipeline.Dictionary.Tag.SeriesInstanceUid));
                //seriesInstanceUidTag.SetString(0, seriesInstanceUid);
                //_dicomDataHeader.AddDicomAttribute(seriesInstanceUidTag);

                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.SOPInstanceUID);
                var sopInstanceUidTag = new DicomAttributeUI(new DicomTag(Pipeline.Dictionary.Tag.SOPInstanceUID));
                sopInstanceUidTag.SetString(0, imageUid);
                _dicomDataHeader.AddDicomAttribute(sopInstanceUidTag);

                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.ImageType);
                var imageTypeCollection = ("DERIVED\\SECONDARY\\DISPLAY\\FILMING\\EFILM").Split('\\');
                InsertStringArrayDicomElement(_dicomDataHeader, Pipeline.Dictionary.Tag.ImageType, imageTypeCollection);

                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.ConversionType);
                var cTypeTag = new DicomAttributeCS(new DicomTag(Pipeline.Dictionary.Tag.ConversionType));
                cTypeTag.SetString(0, "WSD");
                _dicomDataHeader.AddDicomAttribute(cTypeTag);

                _dicomDataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.SeriesDescription);
                var seriesDescriptionTag = new DicomAttributeLO(new DicomTag(Pipeline.Dictionary.Tag.SeriesDescription));
                seriesDescriptionTag.SetString(0, "Electronic film_"+DateTime.Now.ToString());
                _dicomDataHeader.AddDicomAttribute(seriesDescriptionTag);
                
                _dicomConvertorProxy.SaveFile(_dicomDataHeader, efilmFullPath, _proxy);

                var medViewerControlCell = medViewerControl.LayoutManager.ControlCells.FirstOrDefault();
                if (medViewerControlCell != null)
                {
                    var currentPage = medViewerControlCell.Image.CurrentPage;
                    efilmOriginalSopInstanceUID = currentPage.SOPInstanceUID;
                    var dicomHeader = currentPage.ImageHeader.DicomHeader;
                    if (dicomHeader.ContainsKey(ServiceTagName.StudyInstanceUID)   )
                    {
                        efilmOriginalStudyInstanceUid = dicomHeader[ServiceTagName.StudyInstanceUID];
                    }
                }

                if (!ifSaveEFilm) return;
                CreateSeries(efilmOriginalStudyInstanceUid,seriesInstanceUid);
                SaveEFilmInCommonSave(seriesInstanceUid);
                
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
            finally
            {
                medViewerControl.RemoveAll();
            }
        }
        
        #endregion [--Interface For Filming job--]


        #region [--Private Image Handle Method--]


        private void SaveEFilmInCommonSave(string seriesInstanceUid)
        {

            //系列化DataHeader
            byte[] serializedInfo;
            _dicomDataHeader.Serialize(out serializedInfo);


            //设置commonsave参数，todo 主干PS接口有变更
            SaveFilmingCommandContext.Builder builder = SaveFilmingCommandContext.CreateBuilder();
            builder.SetCellIndex(0);
            builder.SetSaveImageType(SavingType.SecondaryCapture);
            builder.SetOperationType(SaveFilmingMode.Save);
            builder.SetStrategy(SaveFilmingStrategy.SaveImages);
            
            //builder.
            builder.SetKeepSameSeries(true);
            builder.SetSeriesUID(seriesInstanceUid);
            ImageAuxiliary.Builder auxiliaryBuilder = ImageAuxiliary.CreateBuilder();
            //auxiliaryBuilder.ActivePS = string.Empty;
            //auxiliaryBuilder.BurnedPS = string.Empty;
            auxiliaryBuilder.PS = string.Empty;
            auxiliaryBuilder.CellIndex = -1;//区别于 SaveSeries
            auxiliaryBuilder.SaveAsDisplay = true;
            auxiliaryBuilder.DataHeaderBytes = Google.ProtocolBuffers.ByteString.CopyFrom(serializedInfo);
            builder.AddImageAuxiliaries(auxiliaryBuilder);
            byte[] btInfo = builder.Build().ToByteArray();



            //SAVE_EFILM_COMMAND
            CommandContext cs = new CommandContext();
            cs.iCommandId = 16000; //7088
            cs.sReceiver = CommunicationNodeName.CreateCommunicationProxyName("FilmingService");
            cs.sSerializeObject = btInfo;
            //var result = Containee.Main.GetCommunicationProxy().SyncSendCommand(cs);
            int errorCode = _proxy.AsyncSendCommand(cs);
            if (0 != errorCode)
            {
                throw new Exception("send filming job command failure, error code: " + errorCode);
            }
        }

        private void CreateSeries(string studyInstanceUid ,string seriesInstanceUid)
        {
            Series series = DBWrapperHelper.DBWrapper.CreateSeries();
            series.SeriesInstanceUID = seriesInstanceUid;
            series.StudyInstanceUIDFk = studyInstanceUid;
            series.Modality = Modality.DX.ToString();
            // new seriesNumber equals the max seriesNumber of exist series add one
            series.SeriesNumber = Convert.ToInt32(GetSerieNumber(studyInstanceUid)) + 1;
            series.Save();
            //var target = new SystemResManagerProxy(pCommProxy);
            //if (!target.HaveEnoughSpace())
            //{
            //    Logger.LogWarning("No enough disk space, so Electronic Image Series will not be created");
            //    Containee.Main.ShowStatusWarning("UID_Filming_No_Enough_Disk_Space_To_Create_Electronic_Image_Series");
            //    return string.Empty;
            //}
        }

        public static string GetSerieNumber(string studyInstanceUid)
        {
            int serieNumber = 0;
            try
            {
                var db = DBWrapperHelper.DBWrapper;
                var serieses = db.GetSeriesListByStudyInstanceUID(studyInstanceUid);
                serieNumber = serieses.Max((series) => series.SeriesNumber);
            }
            catch
            {
                serieNumber = 0;
            }
            if (serieNumber < 8000)
            {
                serieNumber = 8000;
            }
            return Convert.ToString(serieNumber);
        }
        //private void LoadImage(string imageFilePath)
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();

        //        //need to add Reference to UIH.MCSF.Utility to get DBWrapper, and UIH.MCSF.MedDataManagement
        //        //first add debug version,  then release version
        //        _dataLoader.LoadSopByPath(imageFilePath);

        //        Logger.LogFuncDown();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //        throw;
        //    }
        //}
        private void InsertStringArrayDicomElement(DicomAttributeCollection dataHeader, Tag stringTagName, string[] tagValues)
        {
            var element = DicomAttribute.CreateAttribute(stringTagName);
            for (int i = 0; i < tagValues.Length; i++)
            {
                if (!element.SetString(i, tagValues[i]))
                    Logger.LogWarning("Failed to Insert NULL " + stringTagName.ToString() + " to Data header");
            }
            dataHeader.AddDicomAttribute(element);
        }
        /// <summary>
        /// use dicom convertor to load image
        /// </summary>
        /// <param name="fileFullPath"></param>
        private void ShowImage(string fileFullPath)
        {
            try
            {
                Logger.LogFuncUp();

                if (_dicomConvertorProxy == null)
                {
                    _dicomConvertorProxy = _dicomConvertorProxyFactory.CreateDicomConvertorProxy();
                }
            
                _dicomDataHeader = _dicomConvertorProxy.LoadFile(fileFullPath, _proxy);

                if (null == _dicomDataHeader)
                {
                    string msg = "Can't Load Dicom File " + fileFullPath;
                    Logger.LogError(msg);
                    throw new Exception(msg);
                }

                var sopInstanceUidTag = _dicomDataHeader[Pipeline.Dictionary.Tag.SOPInstanceUID];
                string sopInstancUid = null;
                sopInstanceUidTag.GetString(0, out sopInstancUid);

                if (null == sopInstancUid)
                {
                    string msg = "Dicom File " + fileFullPath + " is not in DB";
                    Logger.LogWarning(msg);
                }
                //Dispatcher.BeginInvoke(new Action(() =>_dataLoader.LoadSopByUid(sopInstancUid)));

                var db = DBWrapperHelper.DBWrapper;
                var imageBase = db.GetImageBaseBySOPInstanceUID(sopInstancUid);
                string psPath = string.Empty;
                List<GSPS> listGsps = db.GetGSPSListByImageUID(imageBase.SOPInstanceUID);
                if (null != listGsps && listGsps.Count == 1)
                {
                    psPath = listGsps[0].GSPSFilePath;
                }
                Logger.LogInfo("ps file path = " + psPath);

                //var psHeader = _dicomConvertorProxy.LoadFile(psPath, _proxy);
                string ps = string.Empty;
                //if (psHeader.Contains(0x00613100))
                //{
                //DicomAttribute psTag = psHeader[0x00613100];
                //    psTag.GetString(0, out ps);
                //    Logger.LogInfo("ps info = " + ps);
                //}

                if (File.Exists(psPath))
                {
                    var psFileStream = new FileStream(psPath, FileMode.Open);
                    var psFileStreamReader = new StreamReader(psFileStream);
                    var psFileString = psFileStreamReader.ReadToEnd();
                    psFileStreamReader.Close();
                    psFileStream.Dispose();
                    psFileStreamReader.Dispose();
                    int psStart = psFileString.IndexOf("<PresentationState>");
                    int psEnd = psFileString.IndexOf("</PresentationState>");
                    int psLength = psEnd + "</PresentationState>".Length - psStart;
                    ps = psFileString.Substring(psStart, psLength);
                }
                Logger.LogInfo("ps info = " + ps);

                //medViewerControl.Controller.LoadImageFE(_dicomDataHeader, -1, -1);
                var sop = SopInstanceFactory.Create(_dicomDataHeader, fileFullPath);

                AppendSop(sop, ps);

                //if(!File.Exists(fileFullPath))
                //{
                //    return;
                //}

                //var dataLoader = new DataAccessor();
                //var displayData = dataLoader.CreateImageDataByFilePath(fileFullPath);

                //dataLoader.AddOverlays(displayData);

                //if(displayData != null)
                //{
                //    var newCell = new MedViewerControlCell();
                //    newCell.Image.AddPage(displayData);
                //    displayData.DeserializePSInfo();
                //    newCell.Refresh();

                //    medViewerControl.AddCell(newCell);
                //}

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        private void AppendSop(Sop sop, string ps="")
        {
            try
            {
                Logger.LogFuncUp();

                //new a cell
                var cell = new MedViewerControlCell();


                //add display data to cell
                var accessor = new DataAccessor(medViewerControl.Configuration);  //IViewerConfiguration config = null?
                var imgSop = sop as ImageSop;
                byte[] pixelData = null;
                //string ps = string.Empty;
                if (imgSop != null)
                {
                    pixelData = imgSop.GetNormalizedPixelData();
                    //ps = imgSop.PresentationState;
                }
                var displayData = accessor.CreateImageDataForFilming(sop.DicomSource, pixelData, ps);
                cell.Image.AddPage(displayData);

                //add cell to viewcontrol
                medViewerControl.LayoutManager.AddControlCell(cell);

                //2014-04-15 针对旋转后的图片做适合窗口处理
                if (displayData != null && displayData.PState != null)
                {
                    displayData.PState.SetRenderSize((int)medViewerControl.Width, (int)medViewerControl.Height);
                    displayData.FitWindow();
                }
                //2014-04-15 针对旋转后的图片做适合窗口处理

                //displayData.DeserializePSInfo();
                //_viewerControl.OnRaiseImageLoading(displayData);
                medViewerControl.Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        if (medViewerControl.CellCount > 0)
                        {
                            medViewerControl.LayoutManager.Refresh();
                        }
                        else
                        {
                            cell.Refresh();
                        }

                        //OnNewCellAdded

                    }
                    catch (Exception exp)
                    {
                        Logger.LogError(exp.Message);
                    }
                }));

                //cell.DeserializeGraphics();
                //_viewerControl.OnRaiseImageLoaded(displayData);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        //private MedViewerControlCell GetControlCell(MedViewerLayoutCell layoutCell)
        //{
        //    MedViewerControlCell cell = null;
        //    foreach (MedViewerControlCell tmp in layoutCell.ControlCells)
        //    {
        //        if (tmp != null
        //            && tmp.Image != null
        //            && tmp.Image.CurrentPage != null)
        //        {
        //            cell = tmp;
        //            break;
        //        }
        //    }

        //    if (cell == null)
        //    {
        //        MedViewerLogger.Instance.LogDevError(MedViewerLogger.Source, "No image displayed, cannot save image as DICOM.");
        //        return null;
        //    }


        //    return cell;
        //}

        private WriteableBitmap RenderControlWriteableBitmap()
        {
            try
            {
                Logger.LogFuncUp();

                var screenSaver = new MedViewerScreenSaver(medViewerControl);
                //BitmapSource bmp = screenSaver.RenderViewerControlToBitmap(new Size(800, 1000));    //8 inch * 10 inch
                //BitmapSource bmp = screenSaver.RenderViewerControlToBitmap(medViewerControl.DesiredSize);
                BitmapSource bmp = screenSaver.RenderViewerControlToBitmap(_renderSize, false, true);
                Console.WriteLine(_renderSize);
                if (bmp == null)
                {
                    return null;
                }

                var wtb = new WriteableBitmap(new FormatConvertedBitmap(bmp, PixelFormats.Gray8, null, 0));
                Console.WriteLine(wtb.Width);
                Console.WriteLine(wtb.Height);

                Logger.LogFuncDown();

                return wtb;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        private byte[] ProcessImage(WriteableBitmap wrtBmp)
        {
            byte[] data = null;
            try
            {
                Logger.LogFuncUp();

                int stride = wrtBmp.PixelWidth;
                int height = wrtBmp.PixelHeight;

                if (wrtBmp.Format == PixelFormats.Gray8)
                {
                    stride = wrtBmp.PixelWidth;
                }
                else if (wrtBmp.Format == PixelFormats.Rgb24)
                {
                    stride = wrtBmp.PixelWidth * 3;
                }
                else if(wrtBmp.Format == PixelFormats.Gray16)
                {
                    stride = wrtBmp.PixelWidth*2;
                }

                data = new byte[stride * height];
                if (wrtBmp.PixelWidth % 4 != 0)
                {
                    Int32Rect rect = new Int32Rect(0, 0, wrtBmp.PixelWidth, height);
                    wrtBmp.CopyPixels(rect, data, stride, 0);
                }
                else
                {
                    wrtBmp.CopyPixels(data, stride, 0);
                }

                Logger.LogFuncDown();
                
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }

            return data;
        }

        #endregion [--Private Image Handle Method--]


        #region [--OneMedviewerWindow--]



        #endregion //[--OneMedviewerWindow--]
    }
}
