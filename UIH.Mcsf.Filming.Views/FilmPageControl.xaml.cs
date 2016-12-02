using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using McsfCommonSave;
using UIH.Mcsf.Core;
using UIH.Mcsf.Filming.Models;
using UIH.Mcsf.Filming.ViewModels;
using UIH.Mcsf.Filming.Widgets;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.Pipeline.Data;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Views
{
    /// <summary>
    /// Interaction logic for FilmPageControl.xaml
    /// </summary>
    public partial class FilmPageControl
    {
        #region [---Fields---]

        public static DataAccessor DataAccessorInstance;
        private MedViewerControl _filmingViewerControl;
        private FilmTitleViewModel _filmTitleViewModel;

        private List<FrameworkElement> _firstColumnTitleItems;
        private List<FrameworkElement> _secondColumnTitleItems;

        #endregion


        #region [---Properties---]

        public FilmTitleViewModel FilmTitleViewModel
        {
            get { return _filmTitleViewModel; }
        }

        public MedViewerControl FilmingViewerControl { get { return this._filmingViewerControl; } }

        #endregion


        #region [---Constructors---]

        public FilmPageControl()
        {
            InitializeComponent();

            this._filmTitleViewModel = new FilmTitleViewModel();

            DataContext = this._filmTitleViewModel;

            this._filmingViewerControl = new MedViewerControl();
            this._filmingViewerControl.InitializeWithoutCommProxy(Configure.Environment.Instance.FilmingUserConfigPath);
            //todo: wait for medviewer merge from 61
            //this._filmingViewerControl.Configuration.IppThreadCount = 1;
            this._filmingViewerControl.FilmingViewMode = true;
            this._filmingViewerControl.Controller.SetLutMode(LutDisplayMode.Print);
            filmingViewerControlGrid.Children.Add(this._filmingViewerControl);

            this._firstColumnTitleItems = new List<FrameworkElement>();
            _firstColumnTitleItems.Add(patientNameLabel);
            _firstColumnTitleItems.Add(patientIDLabel);
            _firstColumnTitleItems.Add(patientSexTextBlock);
            _firstColumnTitleItems.Add(accessionNoTextBlock);

            this._secondColumnTitleItems = new List<FrameworkElement>();
            _secondColumnTitleItems.Add(InstitutionNameLabel);
            _secondColumnTitleItems.Add(AcquisitionDateTimeLabel);
            _secondColumnTitleItems.Add(txtComment);

            FilmPageControl.DataAccessorInstance = new DataAccessor(this._filmingViewerControl.Configuration);
        }

        #endregion


        /// <summary>
        /// 只针对PT图像，在页眉处显示药物信息
        /// </summary>
        private void PageTitle_AddDrugForPT()
        {
            try
            {
                this.DrugTextBlock.Inlines.Clear();

                var firstPTCell = this._filmingViewerControl.Cells.FirstOrDefault(n => null != n.Image && null != n.Image.CurrentPage && n.Image.CurrentPage.Modality == Modality.PT);
                if (null == firstPTCell)
                {
                    return;
                }

                var displaydata = firstPTCell.Image.CurrentPage;
                string sourceIso;
                displaydata.ImageHeader.DicomHeader.TryGetValue(ServiceTagName.SourceIsotopeName, out sourceIso);
                if (null == sourceIso || string.IsNullOrEmpty(sourceIso))
                {
                    return;
                }

                string[] sourceIsoArray = sourceIso.Split('-');
                if (sourceIsoArray.Count() != 2)
                {
                    return;
                }

                string radiopha;
                uint Radiopharmaceutical = 0x00180031;
                displaydata.ImageHeader.DicomHeader.TryGetValue(Radiopharmaceutical, out radiopha);
                if (null == radiopha || string.IsNullOrEmpty(radiopha))
                {
                    return;
                }

                //var radiophaText = new Run("  " + radiopha + " " + sourceIso[0])
                //{
                //    Foreground = new SolidColorBrush(Colors.White),
                //    FontFamily = new FontFamily("Arial"),
                //    FontSize = this.FilmTitleViewModel.DisplayFont - 2
                //};
                //this.DrugTextBlock.Inlines.Add(radiophaText);
                //var sourceIsoText = new Run(sourceIsoArray[1])
                //{
                //    BaselineAlignment = BaselineAlignment.Superscript,
                //    Foreground = new SolidColorBrush(Colors.White),
                //    FontSize = this.FilmTitleViewModel.DisplayFont - 4,
                //    FontFamily = new FontFamily("Arial")
                //};
                //this.DrugTextBlock.Inlines.Add(sourceIsoText);

                var text = new Run(" " + sourceIsoArray[1] + sourceIsoArray[0] + "-" + radiopha)
                               {
                                   Foreground = new SolidColorBrush(Colors.White),
                                   FontFamily = new FontFamily("Arial"),
                                   FontSize = this.FilmTitleViewModel.DisplayFont
                               };

                this.DrugTextBlock.Inlines.Add(text);
            }
            catch (Exception e)
            {
                Logger.LogFuncException("Add Drug Info to Page Title Failed"
                                                           + "[Exception:Message]" + e.Message
                                                           + "[Exception:Source]" + e.StackTrace);
            }
        }



        public void AddFilmPage(PageModel pageModel,bool ifPageNo)
        {

            try
            {
                Logger.LogFuncUp();

                this.FilmTitleViewModel.ResetModel(pageModel.PageTitleInfoModel);

                var rootCell = this._filmingViewerControl.LayoutManager.RootCell;
                rootCell.RemoveAll();
                //rootCell.BorderThickness = -1D;
                rootCell.SetBorderThicknessWithoutRefresh(-1D);
                this._filmingViewerControl.LayoutManager.SetLayoutByXML(pageModel.Layout, new Func<MedViewerLayoutCell>(() => { return new MedViewerLayoutCell() { BorderThickness = -1D, CellControlMargin = 0 }; }));

                this._filmingViewerControl.GraphicContextMenu.GraphicsStatisticItemsMode =
                    pageModel.PageGraphicOperationModel.GraphicsStatisticItemsMode;

                List<MedViewerControlCell> controlCells = new List<MedViewerControlCell>();
                foreach (var cellModel in pageModel.CellModels)
                {
                    var cell = new MedViewerControlCell();
                    controlCells.Add(cell);
                    var sop = cellModel.Sop as ImageSop;
                    if (null != sop)
                    {
                        var displayData = FilmPageControl.DataAccessorInstance.CreateImageDataForFilming(sop.DicomSource, sop.GetNormalizedPixelData(), cellModel.Ps,cellModel.PtUnit);
                        cell.Image.AddPage(displayData);
                        //图元颜色修改
                        if (pageModel.EFilmModel.IfSaveImageAsGrayScale)
                        {
                            var overlayGraphics = displayData.GetOverlay(OverlayType.Graphics) as OverlayGraphics;
                            if (null != overlayGraphics)
                            {
                                foreach (var graphic in overlayGraphics.Graphics)
                                {
                                    var parent = graphic as IComposed;
                                    if (parent != null)
                                    {
                                        foreach (var obj in parent.Components)
                                        {
                                            var graphicBase = obj as DynamicGraphicBase;
                                            if (graphicBase != null)
                                            {
                                                graphicBase.GeometryProperty.NormalColor = Colors.White; //设置打印图元线颜色
                                                if (!(graphicBase is GraphicText))
                                                    graphicBase.GeometryProperty.Thickness = 2D;  //设置打印图元线宽
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var graphicBase = graphic as DynamicGraphicBase;
                                        if (null != graphicBase)
                                        {
                                            graphicBase.GeometryProperty.NormalColor = Colors.White; //设置打印图元线颜色
                                            if (!(graphicBase is GraphicText))
                                                graphicBase.GeometryProperty.Thickness = 2D; //设置打印图元线宽
                                        }

                                    }
                                }
                            }
                        }
                        //四角信息配置（出厂and高级应用）
                        var overlayFilmTxt = displayData.GetOverlay(OverlayType.FilmingText) as OverlayFilmingText;
                        if (null == overlayFilmTxt)
                        {
                            return;
                        }
                        overlayFilmTxt.GraphicFilmingImageText.LeftMargin = 0;
                        overlayFilmTxt.GraphicFilmingImageText.RightMargin = 0;
                        overlayFilmTxt.GraphicFilmingImageText.BottomMargin = 0;
                        overlayFilmTxt.GraphicFilmingImageText.TopMargin = 0;
                        overlayFilmTxt.GraphicFilmingImageText.FontStyle = "Arial";
                        overlayFilmTxt.GraphicFilmingImageText.ImgTxtMgPostion =  cellModel.ImgTxTPosition;
                        overlayFilmTxt.GraphicFilmingImageText.IsShowOrientation = (cellModel.MgOrientationIsShow=="1"?true:false);
                        overlayFilmTxt.GraphicFilmingImageText.IsImgTxtShowFromF1 = (cellModel.IsShowImgTxt == "1" ? true : false);
                        overlayFilmTxt.GraphicFilmingImageText.IsRulerShowFromF1 = (cellModel.IsShowRuler == "1" ? true : false);
                     
                        if (ImgTxtDisplayState.All == displayData.PState.DisplayMode || ImgTxtDisplayState.Customization == displayData.PState.DisplayMode)
                        {
                            string modalities;
                            mcsf_clr_systemenvironment_config.GetModalityName(out modalities);

                            if (modalities == "DBT")
                            {
                                if (null != overlayFilmTxt.Page &&
                                     null != overlayFilmTxt.Page.Image &&
                                     null != overlayFilmTxt.Page.Image.Cell)
                                {
                                    var config = overlayFilmTxt.Page.Image.Cell.ViewerControlSetting.Configuration;
                                    config.EntryConfig.ImageTextMap =
                                        Printers.Instance.Modality2FilmingImageTextConfigPath;
                                }

                            }

                            //overlayFilmTxt.ConfigStandardImageText(displayData.ImageHeader.DicomHeader,
                            //                                                                    cellModel.ImgTxtFilePathOrContent,
                            //                                                                    cellModel.ImgTxtItemPathOrContent);
                            overlayFilmTxt.GraphicFilmingImageText.GraphicImageText.SerializedContent =
                                cellModel.ImgTxtFilePathOrContent;


                        }
                        else if (ImgTxtDisplayState.FromApplication == displayData.PState.DisplayMode)
                        {
                            //ImageTextConfigContent imageTextConfigContent = new ImageTextConfigContent()
                            //                                                    {
                            //                                                        ImageTextFileContent = cellModel.ImgTxtFilePathOrContent,
                            //                                                        //TextItemFileContent = cellModel.ImgTxtItemPathOrContent
                            //                                                    };
                            //overlayFilmTxt.ConfigApplicationConfigReader(displayData.ImageHeader.DicomHeader, imageTextConfigContent);
                            overlayFilmTxt.GraphicFilmingImageText.GraphicImageText.SerializedContent =
                              cellModel.ImgTxtFilePathOrContent;
                            overlayFilmTxt.GraphicFilmingImageText.GraphicImageText.SerializedDataHeader = cellModel.ImgTxTContent;
                        }

                        //定位像参考线
                        if (!string.IsNullOrEmpty(cellModel.LocalizedImageUid) && null != cellModel.LocalizedImageSop)
                        {
                            DisplayData smallDisplayData = DataAccessorInstance.CreateImageData(cellModel.LocalizedImageSop.DicomSource, (cellModel.LocalizedImageSop as ImageSop).GetNormalizedPixelData());
                            ImgTxtPosEnum pos = ImgTxtPosEnum.Center;
                            if (!string.IsNullOrEmpty(cellModel.LocalizedImagePSInfo))
                            {
                                smallDisplayData.PSXml = cellModel.LocalizedImagePSInfo;
                                smallDisplayData.DeserializePSInfo();
                            }
                            if (!string.IsNullOrEmpty(cellModel.LocalizedImagePosion))
                            {
                                Enum.TryParse(cellModel.LocalizedImagePosion, out pos);
                                overlayFilmTxt.GraphicFilmingImageText.ReferenceLinePos = pos;
                                overlayFilmTxt.GraphicFilmingImageText.HasReferenceLine = true;
                            }
                            DataAccessorInstance.AddOverlayLocalizedImage(displayData, smallDisplayData, pos);
                            

                            if (pageModel.EFilmModel.IfSaveImageAsGrayScale)
                            {
                                var overlayReferenceLine = smallDisplayData.Overlays.FirstOrDefault(n => n is OverlayReferenceLine2Filming) as OverlayReferenceLine2Filming;
                                if (overlayReferenceLine != null)
                                {
                                    overlayReferenceLine.LineColor = Colors.White; //设置打印参考线颜色
                                    //overlayReferenceLine.LineThickness = 0.3D; //设置打印参考线线宽
                                }
                            }
                        }

                        //比例尺
                        var overlayRuler = displayData.GetOverlay(OverlayType.Ruler) as OverlayRuler;
                        if (null != overlayRuler)
                        {
                            overlayRuler.IsVisible = cellModel.RulerVisible;
                            overlayRuler.GraphicRuler.MinimalCellSize = 0D;
                        }


                        var tagName2Value = new Dictionary<uint, string>();
                        tagName2Value.Add(ServiceTagName.PatientAge, string.Empty);
                        tagName2Value.Add(ServiceTagName.PatientID, string.Empty);
                        tagName2Value.Add(ServiceTagName.PatientName, string.Empty);
                        tagName2Value.Add(ServiceTagName.PatientSex, string.Empty);
                        tagName2Value.Add(ServiceTagName.StudyID, string.Empty);
                        tagName2Value.Add(ServiceTagName.SeriesInstanceUID, string.Empty);
                        tagName2Value.Add(ServiceTagName.StudyInstanceUID, string.Empty);
                        tagName2Value.Add(ServiceTagName.Modality, string.Empty);

                        Widget.GetTagValue(cell, tagName2Value);

                        cellModel.PatientAge = tagName2Value[ServiceTagName.PatientAge];
                        cellModel.PatientId = tagName2Value[ServiceTagName.PatientID];
                        cellModel.PatientsName = tagName2Value[ServiceTagName.PatientName];
                        cellModel.PatientsSex = tagName2Value[ServiceTagName.PatientSex];
                        cellModel.StudyId = tagName2Value[ServiceTagName.StudyID];
                        cellModel.SeriesUid = tagName2Value[ServiceTagName.SeriesInstanceUID];
                        cellModel.StudyInstanceUid = tagName2Value[ServiceTagName.StudyInstanceUID];
                        cellModel.Modality = tagName2Value[ServiceTagName.Modality];
                    }
                }
                this._filmingViewerControl.AddCells(controlCells);

                foreach (var controlCell in controlCells)
                {
                    controlCell.Refresh();
                }

                //only for PT
                this.PageTitle_AddDrugForPT();


                var eFilmModel = pageModel.EFilmModel;

                if (eFilmModel.IfSaveEFilm)
                {
                    var sampleCell =
                        controlCells.FirstOrDefault(c => c != null && c.Image != null && c.Image.CurrentPage != null);

                    if (!eFilmModel.IsMixed)
                    {
                        eFilmModel.FillTagsFrom(sampleCell);
                        string modalities;
                        mcsf_clr_systemenvironment_config.GetModalityName(out modalities);
                        eFilmModel.EFilmModality = modalities;
                    }
                }

                if (pageModel.EFilmModel.PeerNode.NodeType == PeerNodeType.GENERAL_PRINTER)
                {
                    #region [--纸质打印--]
                    this.PrintByGeneralPrinter(eFilmModel, ifPageNo);
                    #endregion
                }
                else
                {
                    #region [--胶片打印--]

                    this.GeneralFilms(eFilmModel, ifPageNo);

                    #endregion
                }

                ClearViewerControl(_filmingViewerControl);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void ClearViewerControl(MedViewerControl viewerControl)
        {
            foreach (var cell in viewerControl.Cells)
            {
                if (cell != null && cell.Image != null && cell.Image.CurrentPage != null)
                {
                    cell.Image.CurrentPage.ImageHeader.Dispose();
                }
            }

            var rootCell = viewerControl.LayoutManager.RootCell;
            rootCell.RemoveAll();
            rootCell.Refresh();
            GC.Collect();
        }

        private void GeneralFilms(EFilmModel eFilmModel, bool ifPageNo)
        {
            try
            {
                Logger.LogFuncUp();

                var dataHeader = GenerateDataHeader(eFilmModel, eFilmModel.FilmSize, ifPageNo);

                eFilmModel.DataHeaderForPrint = dataHeader;
                eFilmModel.DataHeaderForSave = dataHeader;


                var printSettings = eFilmModel.PrintSettings;
                if (eFilmModel.IfSaveEFilm && !bool.Parse(printSettings.IfSaveHighPrecisionEFilm))
                {
                    eFilmModel.DataHeaderForSave = GenerateDataHeader(eFilmModel, eFilmModel.LowPrecisionEFilmSize, ifPageNo);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private DicomAttributeCollection GenerateDataHeader(EFilmModel eFilmModel, Size size, bool ifPageNo)
        {
            try
            {
                Logger.LogFuncUp();

                bool ifGrayScalePrint = eFilmModel.IfSaveImageAsGrayScale;
                
                var bitmap = this.RenderToBitmap(size,
                                                                         eFilmModel.PageTitlePosition,
                                                                         this,
                                                                         _filmingViewerControl,
                                                                         filmPageBarGrid,filmPageBarGridSimple,
                                                                         ifGrayScalePrint, ifPageNo);

                byte[] pixelData = Widget.ProcessImage(bitmap);
                //if (ifGrayScalePrint)
                //{
                //    //pixelData = ApplyLUT(pixelData);
                //    pixelData = PerformGammaCorrected(pixelData);
                //}
                eFilmModel.Rows = (ushort)bitmap.PixelHeight;
                eFilmModel.Columns = (ushort)bitmap.PixelWidth;

                var dicomElements = new DicomElementWidget();
                var dataHeader = dicomElements.AssembleSendData(pixelData, eFilmModel);

                Logger.LogFuncDown();

                return dataHeader;


            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                return null;
            }
        }

        private int[] _lookUpTable = new int[256] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 8, 8, 9, 9, 9, 10, 10, 10, 10, 11, 11, 11, 12, 12, 12, 13, 13, 13, 14, 14, 15, 15, 15, 16, 16, 17, 17, 17, 18, 18, 19, 19, 20, 20, 21, 21, 22, 22, 23, 23, 24, 24, 25, 25, 26, 26, 27, 28, 28, 29, 29, 30, 31, 31, 32, 32, 33, 34, 34, 35, 36, 37, 37, 38, 39, 39, 40, 41, 42, 43, 43, 44, 45, 46, 47, 47, 48, 49, 50, 51, 52, 53, 54, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 70, 71, 72, 73, 74, 75, 76, 77, 79, 80, 81, 82, 83, 85, 86, 87, 88, 90, 91, 92, 94, 95, 96, 98, 99, 100, 102, 103, 105, 106, 108, 109, 110, 112, 113, 115, 116, 118, 120, 121, 123, 124, 126, 128, 129, 131, 132, 134, 136, 138, 139, 141, 143, 145, 146, 148, 150, 152, 154, 155, 157, 159, 161, 163, 165, 167, 169, 171, 173, 175, 177, 179, 181, 183, 185, 187, 189, 191, 193, 196, 198, 200, 202, 204, 207, 209, 211, 214, 216, 218, 220, 223, 225, 228, 230, 232, 237, 240, 242, 245, 247, 250, 252, 255 };
        private byte[] ApplyLUT(byte[] pixelData)
        {
            Read();
            var pixels = new byte[pixelData.Length];
            for (int i = 0; i < pixelData.Length; i++)
            {
                pixels[i] = (byte)(_lookUpTable[pixelData[i]] / 4);
            }
            return pixels;
        }

        private double _gamma = 1;
        private void ReadGamma()
        {
            try
            {
                StreamReader sr = new StreamReader("C:\\Gamma.txt", Encoding.Default);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Double.TryParse(line, out _gamma);
                }
                sr.Dispose();
            }
            catch (Exception e)
            {
                Logger.LogFuncException(e.Message + e.StackTrace);
            }
        }

        private byte[] PerformGammaCorrected(byte[] pixelData)
        {
            ReadGamma();
            var pixels = new byte[pixelData.Length];
            for (int i = 0; i < pixelData.Length; i++)
            {
                pixels[i] = (byte)(int)(Math.Pow((pixelData[i] + 0.5) / 256, 1 / _gamma) * 256);
            }
            return pixels;
        }
        private void Read()
        {
            try
            {
                StreamReader sr = new StreamReader("C:\\lut1.txt", Encoding.Default);
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    var a = line.Split('\\');
                    for (int i = 0; i < a.Count(); i++)
                    {
                        int c = 0;
                        if (int.TryParse(a[i], out c))
                            _lookUpTable[i] = c;
                    }
                }
                sr.Dispose();
            }
            catch (IOException e)
            {
                Logger.LogFuncException(e.Message + e.StackTrace);
            }
        }
        #region 截屏

        public WriteableBitmap RenderToBitmap(Size filmSize,
                                              string pageTitlePosition,
                                              FilmPageControl filmingPagePageControl,
                                              MedViewerControl medViewerControl,
                                              Grid filmingPageBarGrid,
                                              Grid filmingPageBarGridSimple,
                                              bool ifSaveImageAsGreyScale = true,bool ifPageNo=true)
        {
            try
            {
                Logger.LogFuncUp();
                double headerScale = FilmingUtility.HEADER_PERCENTAGE_OF_FILMPAGE;
                bool ifHeaderSimple = (pageTitlePosition == "0" && ifPageNo);
                if (ifHeaderSimple)
                {
                    filmPageBarGrid.Visibility = Visibility.Collapsed;
                    Grid.SetRow(filmPageBarGrid, 1);
                    Grid.SetRow(filmingPageBarGridSimple, 0);
                    headerScale = 0.03;
                }
                else
                {
                    filmPageBarGrid.Visibility = Visibility.Visible;
                    Grid.SetRow(filmPageBarGrid, 0);
                    Grid.SetRow(filmingPageBarGridSimple, 1);
                }

                //计算viewcontrol尺寸，title尺寸的比例
                double scale = filmSize.Height > filmSize.Width
                                   ? headerScale
                                   : headerScale * filmSize.Width / filmSize.Height;
                

                //生成viewcontrol的Size并截屏
                var viewerControlSize = pageTitlePosition != "0"
                                             ? new Size(filmSize.Width, filmSize.Height * (1 - scale))
                                             : (ifPageNo ? new Size(filmSize.Width, filmSize.Height * (1 - headerScale)) : filmSize);
                var viewerControlBitmap = this.RenderViewerControlToBitmap(viewerControlSize,
                                                                           filmingPagePageControl,
                                                                           medViewerControl,
                                                                           ifSaveImageAsGreyScale);

                //判断title位置，决定是否截屏title，拼接两张图片生成最终胶片
                var headerSize = new Size(viewerControlBitmap.PixelWidth, filmSize.Height * (ifHeaderSimple?headerScale:scale));

                WriteableBitmap headerBitmap = null;
                WriteableBitmap filmpageBitmap = null;
                if (pageTitlePosition == "0"&& !ifPageNo) //no film page bar
                {
                    filmpageBitmap = viewerControlBitmap;
                    Logger.LogFuncDown();
                    return filmpageBitmap;
                }

                headerBitmap = this.RenderHeaderToBitmap(headerSize, ifHeaderSimple?filmingPageBarGridSimple:filmingPageBarGrid, ifSaveImageAsGreyScale);
                if (pageTitlePosition == "2")
                {
                    filmpageBitmap = StitchBitmapsHorizontally(ifSaveImageAsGreyScale, viewerControlBitmap, headerBitmap);
                }
                else
                {
                    filmpageBitmap = StitchBitmapsHorizontally(ifSaveImageAsGreyScale, headerBitmap, viewerControlBitmap);
                }


                Logger.LogFuncDown();

                return filmpageBitmap;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }


        /// <summary>
        /// 胶片截屏
        /// </summary>
        /// <param name="viewerControlSize"></param>
        /// <param name="filmingPagePageControl"></param>
        /// <param name="filmingViewerControl"></param>
        /// <param name="ifSaveImageAsGreyScale"></param>
        /// <returns></returns>
        private WriteableBitmap RenderViewerControlToBitmap(Size viewerControlSize,
                                                            FilmPageControl filmingPagePageControl,
                                                            MedViewerControl filmingViewerControl,
                                                            bool ifSaveImageAsGreyScale = true)
        {
            try
            {
                Logger.LogFuncUp();

                MedViewerScreenSaver viewerScreenSaver = new MedViewerScreenSaver(filmingViewerControl);

                Logger.LogFuncUp("Start RenderViewerControlToBitmap");
                BitmapSource viewerControlBitmap = viewerScreenSaver.RenderViewerControlToBitmap(viewerControlSize,
                                                                                                 Printers.Instance.IfPrintSplitterLine, true);
                Logger.LogFuncDown("End RenderViewerControlToBitmap");

                WriteableBitmap writableViewerControlBitmap = ifSaveImageAsGreyScale
                                                                  ? new WriteableBitmap(
                                                                        new FormatConvertedBitmap(
                                                                            viewerControlBitmap, PixelFormats.Gray8,
                                                                            null, 0))
                                                                  : new WriteableBitmap(
                                                                        new FormatConvertedBitmap(viewerControlBitmap,
                                                                                                  PixelFormats.Rgb24,
                                                                                                  null, 0));
                Logger.LogFuncDown();

                return writableViewerControlBitmap;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Title截屏
        /// </summary>
        /// <param name="headerSize"></param>
        /// <param name="filmPageBarGrid"></param>
        /// <returns></returns>
        private WriteableBitmap RenderHeaderToBitmap(Size headerSize,
                                                     Grid filmPageBarGrid,
                                                     bool ifSaveImageAsGreyScale = true)
        {
            try
            {
                Logger.LogFuncUp();

                var headerBitmap = CaptureScreen(filmPageBarGrid, (int)headerSize.Width, (int)headerSize.Height);
                Logger.LogFuncDown();
                var writeableHeaderBitmap = ifSaveImageAsGreyScale
                                                ? new WriteableBitmap(new FormatConvertedBitmap(headerBitmap,
                                                                                                PixelFormats.Gray8, null,
                                                                                                0))
                                                : new WriteableBitmap(new FormatConvertedBitmap(headerBitmap,
                                                                                                PixelFormats.Rgb24, null,
                                                                                                0));

                return writeableHeaderBitmap;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 拼接Title 与 胶片
        /// </summary>
        /// <param name="ifSaveImageAsGreyScale"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private WriteableBitmap StitchBitmapsHorizontally(bool ifSaveImageAsGreyScale, params WriteableBitmap[] args)
        {
            try
            {
                Logger.LogFuncUp();
                if (!args.Any())
                {
                    throw new Exception("no bitmap to be stitched");
                }

                var width = args[0].PixelWidth;
                var height = args.Sum((bitmap) => bitmap.PixelHeight);

                var dpi = Printers.Instance.DefaultPaperPrintDPI;
                var stitchedBitmap = ifSaveImageAsGreyScale
                                         ? new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null)
                                         : new WriteableBitmap(width, height, dpi, dpi, PixelFormats.Rgb24, null);

                unsafe
                {
                    var stitchedBytes = stitchedBitmap.BackBuffer;
                    int stitchIndex = 0;
                    foreach (var bitmap in args)
                    {
                        var bytes = bitmap.BackBuffer;
                        int length = bitmap.BackBufferStride * bitmap.PixelHeight;
                        NativeMethods.CopyMemory(stitchedBytes + stitchIndex, bytes, length);
                        stitchIndex += length;
                    }
                }
                Logger.LogFuncDown();
                return stitchedBitmap;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }


        private BitmapSource CaptureScreen(Visual target, int pixelWidth, int pixelHeight)
        {
            try
            {
                Logger.LogFuncUp();

                if (target == null || pixelWidth <= 0 || pixelHeight <= 0)
                {
                    return null;
                }

                double dpiX = 96.0 * pixelWidth / (target as FrameworkElement).ActualWidth;
                double dpiY = 96.0 * pixelHeight / (target as FrameworkElement).ActualHeight;

                RenderTargetBitmap rtb = new RenderTargetBitmap(pixelWidth, pixelHeight, dpiX, dpiY,
                                                                PixelFormats.Pbgra32);

                rtb.Render(target);

                Logger.LogFuncDown();

                return rtb;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            return null;
        }

        private void CreateThumbnail(string filename, BitmapSource image)
        {
            try
            {
                Logger.LogFuncUp();

                if (string.Empty != filename)
                    using (FileStream stream = new FileStream(filename, FileMode.Create))
                    {
                        var encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(image));
                        encoder.Save(stream);
                        stream.Close();
                    }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region [---纸打---]

        private void PrintByGeneralPrinter(EFilmModel eFilmModel,bool ifPageNo)
        {
            try
            {
                Logger.LogFuncUp();
                var setting = eFilmModel.PrintSettings;
                var dpi = int.Parse(setting.CurrPrinterDPI);
                var isExistPrinter = true; //检测是否存在打印机

                // 设置页边距
                //const double margin = 30;
                double widthMargin = 30 * dpi / 96;
                double heightMargin = 45 * dpi / 96;
                PrintDialog printDlg = new PrintDialog();

                // 打印方向
                printDlg.PrintTicket.PageOrientation = setting.CurrOrientation == "0"
                                                           ? PageOrientation.Portrait
                                                           : PageOrientation.Landscape;

                //打印份数
                printDlg.PrintTicket.CopyCount = int.Parse(setting.CurrCopyCount);

                //选择纸张尺寸 //todo: FilmSize convertor
                var pageMediaSizeName = (PageMediaSizeName)Enum.Parse(typeof(PageMediaSizeName), setting.CurrFilmSize);
                printDlg.PrintTicket.PageMediaSize = new PageMediaSize(pageMediaSizeName);

                //选择一个打印机;
                if (eFilmModel.IfPrint)
                {
                    PrintQueue selectedPrinter;
                    try
                    {
                        selectedPrinter =
                            Printers.Instance.GeneralPrinters.FirstOrDefault(p => p.Name == setting.CurrPrinterAE);
                    }
                    catch (Exception)
                    {
                        var printers = new LocalPrintServer().GetPrintQueues();
                        selectedPrinter = printers.FirstOrDefault(p => p.Name == setting.CurrPrinterAE);
                    }
                    if (selectedPrinter == null)
                    {
                        Logger.LogError("没有找到" + setting.CurrPrinterAE);
                        isExistPrinter = false;
                        // return;
                    }
                    else
                    {
                        //设置打印;
                        printDlg.PrintQueue = selectedPrinter;
                    }

                }

                Size filmSize = eFilmModel.FilmSize;
                bool ifGrayPrint = eFilmModel.IfSaveImageAsGrayScale;
                var bitmap = this.RenderToBitmap(filmSize,
                                                 eFilmModel.PageTitlePosition,
                                                 this,
                                                 _filmingViewerControl,
                                                 filmPageBarGrid, filmPageBarGridSimple, ifGrayPrint);

                if (isExistPrinter&&eFilmModel.IfPrint)
                {
                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext context = visual.RenderOpen())
                    {
                       // VisualBrush brush = new VisualBrush(this);

                        if (printDlg.PrintTicket.PageOrientation == PageOrientation.Portrait)
                        {
                            //context.DrawRectangle(brush, null, new Rect(new Point(30, 120),
                            //                                            new Size(this.ActualWidth, this.ActualHeight)));
                            context.DrawImage(bitmap,
                                              new Rect(new Point(widthMargin, heightMargin),
                                                       new Size(bitmap.PixelWidth, bitmap.PixelHeight)));
                        }
                        else if (printDlg.PrintTicket.PageOrientation == PageOrientation.Landscape)
                        {
                            //context.DrawRectangle(brush, null, new Rect(new Point(120, 30),
                            //                                            new Size(this.ActualWidth, this.ActualHeight)));
                            context.DrawImage(bitmap,
                                              new Rect(new Point(heightMargin, widthMargin),
                                                       new Size(bitmap.PixelWidth, bitmap.PixelHeight)));
                        }


                    }
                    double scale = Math.Min(printDlg.PrintableAreaWidth / (bitmap.PixelWidth + (widthMargin * 2)),
                                            printDlg.PrintableAreaHeight / (bitmap.PixelHeight + (widthMargin * 2)));
                    visual.Transform = new ScaleTransform(scale, scale);
                    printDlg.PrintVisual(visual, string.Empty);
                }


                if (!eFilmModel.IfSaveEFilm) return;
                if (bool.Parse(eFilmModel.PrintSettings.IfSaveHighPrecisionEFilm))
                {
                    byte[] pixelData = Widget.ProcessImage(bitmap);

                    eFilmModel.Rows = (ushort)bitmap.PixelHeight;
                    eFilmModel.Columns = (ushort)bitmap.PixelWidth;

                    var dicomElements = new DicomElementWidget();
                    eFilmModel.DataHeaderForSave = dicomElements.AssembleSendData(pixelData, eFilmModel);
                }
                else
                {
                    eFilmModel.DataHeaderForSave = GenerateDataHeader(eFilmModel, eFilmModel.LowPrecisionEFilmSize, ifPageNo);
                }


                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }

        }

        #endregion

        public void SetPageTitleElementPosition()
        {
            int rowscount = GetVisibleRowCount();
            var displayFont = (int)FilmTitleViewModel.DisplayFont;
            InitFilmingTileGrid(displayFont, rowscount);
            int firstVisibleCount = 0;
            int secondVisibleCount = 0;

            for (int i = 0; i < _firstColumnTitleItems.Count; i++)
            {
                if (_firstColumnTitleItems[i].Visibility == Visibility.Visible)
                {
                    Grid.SetRow(_firstColumnTitleItems[i], firstVisibleCount);
                    Grid.SetColumn(_firstColumnTitleItems[i], 0);

                    firstVisibleCount++;
                }
            }
            for (int i = 0; i < _secondColumnTitleItems.Count; i++)
            {
                if (_secondColumnTitleItems[i].Visibility == Visibility.Visible)
                {
                    Grid.SetRow(_secondColumnTitleItems[i], secondVisibleCount);
                    Grid.SetColumn(_secondColumnTitleItems[i], 1);
                    secondVisibleCount++;
                }
            }
            if (firstVisibleCount <= 2 && secondVisibleCount <= 2)
            {
                Grid.SetRowSpan(UIHLogo, 2);
                if (filmPageBarGrid.RowDefinitions.Count == 3)
                    filmPageBarGrid.RowDefinitions.RemoveAt(2);
            }
            else
            {
                if (firstVisibleCount == 3 || secondVisibleCount == 3)
                {
                    Grid.SetRowSpan(UIHLogo, 3);

                }
            }
        }

        private int GetVisibleRowCount()
        {
            int rowCount1 = _firstColumnTitleItems.Count(t => t.Visibility == Visibility.Visible);
            int rowCount2 = _secondColumnTitleItems.Count(t => t.Visibility == Visibility.Visible);
            if (Math.Max(rowCount1, rowCount2) < 2)
            {
                return 2;
            }
            return 3;
        }

        private void InitFilmingTileGrid(int fontsize, int rowCount)
        {
            int rowheight = 12;
            if (fontsize == 10)
                rowheight = 18;
            else if (fontsize == 15)
                rowheight = 23;
            filmPageBarGrid.RowDefinitions.Clear();
            for (int i = 0; i < rowCount; i++)
            {
                RowDefinition row1 = new RowDefinition();
                row1.Height = new GridLength(rowheight);
                filmPageBarGrid.RowDefinitions.Add(row1);
            }
        }

        public void UpdateFilmingImageProperty()
        {
            try
            {
                Logger.LogFuncUp();

                _filmingViewerControl.Controller.LoadConfigReader();
                foreach (var cell in _filmingViewerControl.Cells)
                {
                    cell.Refresh();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void UpdateFilmingImageText()
        {
            try
            {
                Logger.LogFuncUp();

                foreach (var cell in _filmingViewerControl.Cells)
                {
                    cell.Refresh();
                    cell.ReloadImageTextConfig();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void UpdateFilmingPrintConfig()
        {
            try
            {
                Logger.LogFuncUp();

                Printers.Instance.ReloadPrintersConfig();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }
    }


    internal class NativeMethods
    {
        [DllImport("msvcrt.dll", EntryPoint = "memcpy")]
        public static extern unsafe void CopyMemory(
            IntPtr pDest,
            IntPtr pSrc,
            int length
            );
    }

}
