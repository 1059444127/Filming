using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using UIH.Mcsf.Database;
using UIH.Mcsf.DicomConvertor;
using UIH.Mcsf.Filming.Widgets;
using McsfCommonSave;

namespace UIH.Mcsf.Filming.Models
{
    public class CardModel : IDisposable
    {

        #region [---Constructors----]

        public CardModel()
        {
            this.PrinterSettingInfoModel = new PrinterSettingInfoModel();
            this.PageTitleConfigInfoModel = new PageTitleConfigInfoModel();
           // this.PageModels = new List<PageModel>();

        }

        #endregion



        #region  [---Properties---]

        public PrinterSettingInfoModel PrinterSettingInfoModel { get; private set; }
        public PageTitleConfigInfoModel PageTitleConfigInfoModel { get; private set; }
        public PageModel FilmPageModel { get; private set; }
        public PeerNode PeerNode { get; private set; }
        public bool IsPrint { get; private set; }
        public Size AllFilmSize { get; set; }
        public double DisplayedFilmPageHeight;
        public double DisplayedFilmPageWidth;
        public double DisplayedFilmPageViewerHeight;

        public static List<FilmingPrintJob.Types.FilmBox.Builder> filmBoxList = new List<FilmingPrintJob.Types.FilmBox.Builder>(); //打印胶片信息
        public static PrintSetting PrintSetting = new PrintSetting(); //打印设置信息
        public static Patient CurrentPatient = new Patient(); //当前胶片patient
        public static PeerNode CurrentPeer = new PeerNode(); //当前打印端口
        #endregion



        #region [---Public Methods---]

        public bool DeserializedFromXml(byte[] contentsByte, XmlNode filmPageNode)
        {
            try
            {
                Logger.LogFuncUp();

                string contentsString = Encoding.UTF8.GetString(contentsByte);
                var xDoc = new XmlDocument();
                xDoc.LoadXml(contentsString);

                var rootNode = xDoc.DocumentElement;
                if (null == rootNode)
                {
                    Logger.LogWarning("[FilmingSerivceFE]No Card info for print");
                    return false;
                }

                this.IsPrint = bool.Parse(rootNode.Attributes[OffScreenRenderXmlHelper.OPERATION].Value);

                var printerSettingNode = rootNode.SelectSingleNode(OffScreenRenderXmlHelper.PRINTER_SETTING_INFO);
                this.PrinterSettingInfoModel.DeserializedFromXml(printerSettingNode);

                var filmingPageTitleSettingNode = rootNode.SelectSingleNode(OffScreenRenderXmlHelper.FILMING_PAGE_TITLE_SETTING);
                this.PageTitleConfigInfoModel.DeserializedFromXml(filmingPageTitleSettingNode);

                //PageControl大小
                DisplayedFilmPageHeight = FilmingUtility.DisplayedFilmPageHeight;
                DisplayedFilmPageWidth = FilmingUtility.DisplayedFilmPageWidth;
                var filmingPageSizeNode = rootNode.SelectSingleNode(OffScreenRenderXmlHelper.FILMING_PAGE_SIZE);
                Double.TryParse(OffScreenRenderXmlHelper.GetChildNodeValue(filmingPageSizeNode, OffScreenRenderXmlHelper.FILMING_PAGE_HEIGHT),out DisplayedFilmPageHeight);
                Double.TryParse(OffScreenRenderXmlHelper.GetChildNodeValue(filmingPageSizeNode, OffScreenRenderXmlHelper.FILMING_PAGE_WIDTH), out DisplayedFilmPageWidth);
                Double.TryParse(OffScreenRenderXmlHelper.GetChildNodeValue(filmingPageSizeNode, OffScreenRenderXmlHelper.FILMING_PAGE_VIEWER_HEIGHT), out DisplayedFilmPageViewerHeight);
                
                var allFilmPagesNode = rootNode.SelectSingleNode(OffScreenRenderXmlHelper.ALL_FILMING_PAGE_INFO);
                if (allFilmPagesNode == null)
                {
                    Logger.LogWarning("[FilmingSerivceFE]No Common Page info for print");
                    return false;
                }

                //全局PeerNode
                var ae = this.PrinterSettingInfoModel.CurrPrinterAE;
                var peer = new PeerNode();
                
                if (-1==Printers.Instance.GetPacsNodeParametersByAE(ae, ref peer))
                {
                    Printers.Instance.ReloadPrintersConfig();
                    if (-1 == Printers.Instance.GetPacsNodeParametersByAE(ae, ref peer))
                    {
                        Logger.LogWarning("[FilmingSerivceFE]Can Not Find Printer:" + ae + " in Config File.");
                        if(IsPrint)
                            return false;
                    }
                }
                this.PeerNode = peer;
     
                //全局SeriesUid
                var uidManager = McsfDatabaseDicomUIDManagerFactory.Instance().CreateUIDManager();
                var allEFilmSeriesUid = uidManager.CreateSeriesUID("");

                //全局FilmSize
                AllFilmSize = Widget.ConvertFilmSizeFrom(this.PrinterSettingInfoModel.CurrFilmSize,
                                                                                      int.Parse(this.PrinterSettingInfoModel.CurrPrinterDPI),
                                                                                      this.PrinterSettingInfoModel.CurrOrientation);
                var lowPrecisionEFilmSize = Widget.ConvertFilmSizeFrom(this.PrinterSettingInfoModel.CurrFilmSize,
                                                                                                               96,
                                                                                                               this.PrinterSettingInfoModel.CurrOrientation);
                //全局操作方式
                var ifPrint = IsPrint;
                var ifSave = !IsPrint || bool.Parse(PrinterSettingInfoModel.IfSave);


              //  foreach (XmlNode filmPageNode in allFilmPagesNode)
                {
                var filmPage = new PageModel(this.PrinterSettingInfoModel);

                var eFilmModel = new EFilmModel();
                filmPage.EFilmModel = eFilmModel;

                FilmPageModel = filmPage;
                if (!filmPage.DeserializedFromXml(filmPageNode))
                {
                    Logger.LogWarning("[FilmingSerivceFE]Fail to get page info of a film page");
                    return false;
                }

                    eFilmModel.EFilmSeriesUid = allEFilmSeriesUid;
                    eFilmModel.FilmSize = AllFilmSize;
                    eFilmModel.LowPrecisionEFilmSize = lowPrecisionEFilmSize;
                    eFilmModel.PeerNode = this.PeerNode;
                    eFilmModel.PrintSettings = this.PrinterSettingInfoModel;
                eFilmModel.PageTitlePosition = filmPage.PageTitleInfoModel.DisplayPosition;//  PageTitleConfigInfoModel.DisplayPosition;
                eFilmModel.IfSaveEFilm = ifSave;
                eFilmModel.IfPrint = ifPrint;
            }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                return false;
            }

            return true;
        }

        public  void CreateFilmBoxList(PeerNode peer, Core.ICommunicationProxy proxy, List<CardModel> cardList)
        {
            try
            {
                Logger.LogFuncUp();
                var patient = new Patient();
                //Job Info Title
                CurrentPeer = peer;
                var cellModels = FilmPageModel.CellModels.ToList();
                patient.PatientAge = string.Join(";", cellModels.Select(c => c.PatientAge).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
                patient.PatientID = string.Join(";", cellModels.Select(c => c.PatientId).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
                patient.PatientName = string.Join(";", cellModels.Select(c => c.PatientsName).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
                patient.PatientSex = string.Join(";", cellModels.Select(c => c.PatientsSex).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
                patient.StudyID = string.Join(";", cellModels.Select(c => c.StudyId).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
                CurrentPatient = patient;

              //  var printSetting = new PrintSetting();

                PrintSetting.Copies = uint.Parse(PrinterSettingInfoModel.CurrCopyCount);
                PrintSetting.FilmDestination = this.PrinterSettingInfoModel.CurrFilmDestination;
                PrintSetting.FilmingDateTime = DateTime.Now;
                PrintSetting.IfSaveElectronicFilm = bool.Parse(this.PrinterSettingInfoModel.IfSave);
                PrintSetting.MediaType = this.PrinterSettingInfoModel.CurrMediumType;
                PrintSetting.Priority = PRINT_PRIORITY.MEDIUM;
                PrintSetting.IfColorPrint = bool.Parse(this.PrinterSettingInfoModel.IfColorPrint);
                var filmPageModel = FilmPageModel;
              
                var filmBox = new FilmingPrintJob.Types.FilmBox.Builder();

                var dicomFilePath = Widget.CreatePrintImageFullPath(0, 0);
                var dicomConvertorProxy = McsfDicomConvertorProxyFactory.Instance().CreateDicomConvertorProxy();
                    
                dicomConvertorProxy.SaveFile(filmPageModel.EFilmModel.DataHeaderForPrint, dicomFilePath, proxy);

                var imageBoxForPrint = new FilmingPrintJob.Types.ImageBox.Builder { ImagePath = dicomFilePath };//{ ImagePath = filmPageModel.EFilmModel.EFilmFilePath };
                filmBox.AddImageBox(imageBoxForPrint);
                filmBox.FilmSize = this.PrinterSettingInfoModel.CurrFilmSize;
                filmBox.Orientation = this.PrinterSettingInfoModel.CurrOrientation == "0" ? FilmingPrintJob.Types.Orientation.PORTRAIT : FilmingPrintJob.Types.Orientation.LANDSCAPE;
                filmBox.Layout = "STANDARD\\1,1";
                filmBox.PatientName = filmPageModel.PageTitleInfoModel.PatientName;
                filmBox.StudyInstanceUid = string.Join(";", filmPageModel.CellModels.Select(c => c.StudyInstanceUid).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());

                //fill sopinstance list  for print status updating
                var sopInstanceUids = filmPageModel.CellModels.Select(c => c.SopUid).Distinct();

                foreach (var uid in sopInstanceUids)
                {
                    var imageBoxForPrintStatusUpdating = new FilmingPrintJob.Types.ImageBox.Builder { OriginSOPInstanceUID = uid };
                    filmBox.AddImageBox(imageBoxForPrintStatusUpdating);
                }

                //fill seriesUid list for print status updating 
                var seriesUids = filmPageModel.CellModels.Select(c => c.SeriesUid).Distinct().Where(s => !string.IsNullOrWhiteSpace(s));
                filmBox.AddRangeSeriesInstanceUidList(seriesUids);
               
                filmBoxList.Add(filmBox);
                
                Logger.LogFuncDown();

               
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
               
            }
        }
        public static JobCreator CreateJobCreator()
        {
            try
            {
                Logger.LogFuncUp();

                var jobCreator = new JobCreator();
             
                jobCreator.Patient = CurrentPatient;
               
                //打印节点
                jobCreator.Peer = CurrentPeer;

                //打印设置
                jobCreator.PrintSetting = PrintSetting;
               
                //填充Film box
                jobCreator.FilmBoxList = filmBoxList;
                Logger.LogFuncDown();

                return jobCreator;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                return null;
            }
        }
        #endregion



        #region [---Private Methods---]

        #endregion



        #region [---Dispose---]

        private bool isDisposed = false;

        ~CardModel()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                var pageModel = FilmPageModel;
                //foreach (var pageModel in this.PageModels)
                {
                    if (null != pageModel.EFilmModel.DataHeaderForPrint)
                    {
                        pageModel.EFilmModel.DataHeaderForPrint.Dispose();
                    }

                    if (null != pageModel.EFilmModel.DataHeaderForSave)
                    {
                        pageModel.EFilmModel.DataHeaderForSave.Dispose();
                    }

                    foreach (var cell in pageModel.CellModels)
                    {
                        var sop = cell.Sop;
                        if (sop != null)
                        {
                            if (sop.DicomSource != null)
                            {
                                sop.DicomSource.Dispose();
                            }
                            sop.Dispose();
                        }
                    }
                }
                pageModel.EFilmModel = null;
                pageModel.CellModels.Clear();
            }

            this.isDisposed = true;
        }

        #endregion

    }
}
