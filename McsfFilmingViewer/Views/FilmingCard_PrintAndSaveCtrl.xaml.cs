using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;

using UIH.Mcsf.App.Common;
using UIH.Mcsf.Core;
using UIH.Mcsf.Database;
using UIH.Mcsf.DicomConvertor;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Filming.Configure;
using UIH.Mcsf.Filming.Model;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Filming.Wrappers;
using UIH.Mcsf.MHC;
using UIH.Mcsf.AppControls.Viewer;
using UIH.Mcsf.Pipeline.Data;
using McsfCommonSave;
using UIH.Mcsf.Pipeline.Dictionary;
using UIH.Mcsf.Viewer;
using ImageAuxiliary = McsfCommonSave.ImageAuxiliary;
using SaveFilmingCommandContext = McsfCommonSave.SaveFilmingCommandContext;
using SaveFilmingMode = McsfCommonSave.SaveFilmingMode;
using SaveFilmingStrategy = McsfCommonSave.SaveFilmingStrategy;
using SavingType = McsfCommonSave.SavingType;

namespace UIH.Mcsf.Filming.Views
{
    /// <summary>
    /// Interaction logic for FilmingCard_PrintAndSaveCtrl.xaml
    /// </summary>
    public partial class FilmingCardPrintAndSaveCtrl : UserControl
    {
        private FilmingCard Card { set; get; }
        private JobCreator _jobCreator;

        private bool isPrintCurrentFilm = false; //是否是单打印当前页
        private bool isPrintShowPageNoFilm = true; //是否是单打印当前页号

        public bool IsEnableSaveElectronicFilm
        {
            get { return Card.IsAnyImageLoaded && !HasMixedFilm() && !Card.IsCellModalitySC; }
        }

        public bool IsEnablePrintAllFilm
        {
            get { return Card.IsAnyImageLoaded; }
        }


        public FilmingCardPrintAndSaveCtrl(FilmingCard _card)
        {
            Card = _card;                
            InitializeComponent();
        }


        #region [--Printer Setting UI Event Handler--]


        private string _currentFilmSize;
        public string CurrentFilmSize
        {
            get { return _currentFilmSize; }
            set
            {
                if (value != _currentFilmSize)
                {
                    _currentFilmSize = value;
                    if (Card.IsModalityForMammoImage())
                    {
                        this.SetFilmSize();
                    }
                }
            }
        }

        public double CorrectedRatio { get; set; }


        public void SetFilmSize()
        {
            try
            {
                Logger.LogFuncUp();

                var pages = new List<FilmingPageControl>();
                pages.AddRange(Card.EntityFilmingPageList);
                pages.AddRange(Card.DeletedFilmingPageList);
                foreach (var filmPage in pages)
                {
                    Card.SetFilmPageSize(filmPage);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }


        public FilmOrientationEnum _currentFilmOrientation = FilmOrientationEnum.Portrait;

        public FilmOrientationEnum CurrentFilmOrientation
        {
            get { return _currentFilmOrientation; }
            set
            {
                if (value != _currentFilmOrientation)
                {
                    _currentFilmOrientation = value;
                    this.SetFilmSize();
                }
            }
        }

        private FilmingCardPrintSetCtrl _printerSetting;

        public FilmingCardPrintSetCtrl PrinterSetting
        {
            get
            {
                if (_printerSetting == null)
                {
                    _printerSetting = Card.PrintSetCtrl;
                    //WindowHostManager.HostAdorner.OnClosing += _printerSettingDialog.OnClosingWindow;
                }
               
                return _printerSetting;
            }
           
        }

        public string OldCurrentPrinterAE
        {
            set;
            get;
        }

        public void UpdatePrintDataViewModal()
        {
            OldCurrentPrinterAE = _printerSetting.DataViewModal.CurrentPrinterAE;
            _printerSetting.DataViewModal.ReloadPrinterSetting();
            PrinterSetting.CloneViewModel();

            if (_printerSetting.DataViewModal.RegisterPrinterAEList.Contains(Printers.Instance.DefaultAE))
                _printerSetting.DataViewModal.CurrentPrinterAE = Printers.Instance.DefaultAE;
            else
            {
                if (_printerSetting.DataViewModal.RegisterPrinterAEList.Contains(OldCurrentPrinterAE))
                    _printerSetting.DataViewModal.CurrentPrinterAE = OldCurrentPrinterAE;
            }
            _printerSetting.DataViewModal.CopyTo(_printerSetting._oldDataViewModal);

        }

        //public void ShowPrinterSettingDlgBtnClick(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();
        //        var messageWindow = new MessageWindow
        //        {
        //            WindowTitle = Card.Resources["UID_Filming_PrinterSetting_Title"] as string,
        //            WindowChild =PrinterSetting,
        //            WindowDisplayMode = WindowDisplayMode.Default
        //        };
        //        Card.HostAdornerCount++;
        //        messageWindow.Closing -=PrinterSetting.OnClosingWindow;
        //        messageWindow.Closing += new CancelEventHandler(PrinterSettingDialog.OnClosingWindow);
        //        messageWindow.ShowModelDialog();
        //        Card.HostAdornerCount--;
        //        Logger.LogFuncDown();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message + ex.StackTrace);
        //    }
        //}

        #endregion [--Printer Setting UI Event Handler--]

        #region [--Print UI Event Handler--]


        private void printAllButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private bool AnyUnsupportedImage()
        {
            try
            {
                foreach (var film in Card.EntityFilmingPageList)
                {
                    foreach (var cell in film.filmingViewerControl.Cells)
                    {
                        if (null == cell.Image || null == cell.Image.CurrentPage)
                        {
                            continue;
                        }
                        Modality modality = cell.Image.CurrentPage.Modality;
                        if (modality == Modality.CR || modality == Modality.DX)// || modality == Modality.MG)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                return true;
            }

        }



        private void PrintAllAndSaveEFilmHandler(MessageBoxResponse response = MessageBoxResponse.OK)
        {
            int previousDisplaymode = Card.SelectedFilmCardDisplayMode;
            try
            {
                Logger.LogFuncUp();

                //disable all UI controls and show a progress bar
                Card.DisableUI();
                Card.EntityFilmingPageList.RemoveEmptyPages();
                Card.layoutCtrl.SelectedFilmCardDisplayMode = 1;
                //0.remove selected status of filming object, for filming now is done by copy screen
                Card.ActiveFilmingPageList.ClearTextAnnotationEditableStatus();
                //Card.ActiveFilmingPageList.UnSelectAllCells();
                //  UnselectActiveFilmingPages();

                //1.update annotation according to setting
                //UpdateCornerText((ImgTxtDisplayState)PrinterSettingDialog.DataViewModal.CurrentAnnotationType);

                //2. Save Image and Print Films, SaveFilmsCallBackHandler will EnableUI
                PrintAll();
                //if there is no exception
                if (PrinterSetting.DataViewModal.IfClearAfterAddFilmingJob)
                {
                    Card.DeleteAllFilmPage();
                    Card.OnAddFilmPageAfterClearFilmingCard(null, null);
                }
                Card.UpdateUIStatus();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);

#if DEVELOPER
                MessageBox.Show("打印失败");
#else
                MessageBoxHandler.Instance.ShowError("UID_Filming_Info_PrintAll_Fail");
#endif
            }
            finally
            {
                Card.layoutCtrl.SelectedFilmCardDisplayMode = previousDisplaymode;
                //EnableUI(FilmingCardModality == FilmingUtility.EFilmModality);
                Card.UpdateUIStatus();
            }
        }

        private void RemoveEmptyPages()
        {
            Card.EntityFilmingPageList.RemoveAll(film => film.IsEmpty());
            Card.EntityFilmingPageList.ForEach((film) => film.FilmPageIndex = Card.EntityFilmingPageList.IndexOf(film));
            Card.EntityFilmingPageList.UpdatePageLabel();
            //FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Remove_Empty_Films_Before_Print");
            FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Remove_Empty_Films_Before_Print");
        }

        private void PrintAllHandler(MessageBoxResponse response = MessageBoxResponse.YES)
        {
            if (response != MessageBoxResponse.YES)
            {
                return;
            }

            //bool saveEFilmsWhenPrinting = CanSaveFilm;
            //if (saveEFilmsWhenPrinting && HasMixedFilm())
            //{
            //    MessageBoxHandler.Instance.ShowInfo("UID_Filming_Not_Support_Save_EFilm_For_Mixed_Images");
            //    saveEFilmsWhenPrinting = false;
            //}
            //else
            //{
            try
            {
                SetFilmSize();
                SetPageTitleStatusBeforePrint();
                Card.Dispatcher.Invoke(new Action(() => PrintAllAndSaveEFilmHandler()), DispatcherPriority.Background);
                Card.Dispatcher.BeginInvoke(new Action(() => Card.EnableUI(Card.FilmingCardModality == FilmingUtility.EFilmModality)),
                                      DispatcherPriority.Background);
            }
            finally
            {
                SetPageTitleStatusAfterPrint();
            }

            //}
        }

        #region [jinyang.li Performance]

        private readonly object _printtingTaskLock = new object();

        private void PrintAllBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Instance.LogPerformanceRecord("[Begin][Print]");
                //  DisableUI();
                Logger.LogFuncUp();

                if (0 == Printers.Instance.PeerNodes.Count)
                {
                    MessageBoxHandler.Instance.ShowInfo("UID_Filming_Info_Not_Config_Printer");
                    return;
                }
                isPrintCurrentFilm = false;
                if (Printers.Instance.IfSaveAsNewSeries)
                {
                    CellSaveHelper.SeriesInstanceUid = string.Empty;
                    if (HasMixedFilm())
                    {
                        MessageBoxHandler.Instance.ShowWarning("UID_Filming_Not_Support_Save_New_Series_For_Mixed_Images");
                    }
                    else
                    {
                        SaveSeries();
                    }
                }


                var peer = new PeerNode();
                Printers.Instance.GetPacsNodeParametersByAE(
                    PrinterSetting.DataViewModal.CurrentPrinterAE.ToString(), ref peer);

                var dpi = PrinterSetting.DataViewModal.GeneralPrinterDPI;
                if (dpi > 800)
                {
                    Logger.Instance.LogSvcWarning(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcWarnResolutionHigh,
                                                  "[Resolution is over high( > 800) !]" + dpi);
                }
                else if(dpi < 100)
                {
                    Logger.Instance.LogSvcWarning(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcWarnResolutionHigh,
                                "[Resolution is over low( < 100) !]" + dpi);
                }

                if (!Printers.Instance.IfUseFilmingServiceFE
                    || peer.NodeType == PeerNodeType.FILM_PRINTER && Card._filmingCardModality == FilmingUtility.EFilmModality)
                {
                    _toBePrintFilmingPageList = FilmsNotEmpty;
                    PrintFilms();
                }
                else
                {
                    
                    if (HasMixedFilm())
                    {
                        MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Warning_Mixed_Film_Print", PrintAllBtnClickOperation);
                    }
                    else
                    {
                        Logger.LogInfo("[Begin To Print](Maybe saving Electronic films at the same time)");
                        PrintAllBtnClickOperation(MessageBoxResponse.YES);
                        Logger.LogInfo("[End To Print](Maybe saving Electronic films at the same time)");
                    }
                    
                }
               
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message);
            }
            finally
            {
                Logger.Instance.LogPerformanceRecord("[End][Print]");
            }

        }
        private void NotifyMFShutDownCurrentPatient()
        {
            lock (_printtingTaskLock)
            {
                CommonCommand.NotifyMainFrameAutoShutDownAfterFilming(FilmingViewerContainee.Main.StudyInstanceUID);
            }
        }

        /// <summary>
        /// 是否打印当前胶片
        /// </summary>
        /// <param name="isCurrentFilmPrint"></param>
        /// <returns></returns>
        private bool IsExistCellEmpty(bool isCurrentFilmPrint)
        {
            var filmList = !isCurrentFilmPrint ? Card.EntityFilmingPageList : Card.ActiveFilmingPageList;
            if (filmList.Any(page => page.filmingViewerControl.Cells.Any(cell => cell.IsEmpty)))
            {
                return true;
            }
            return false;
        }

        private void PrintAllBtnClickOperation(MessageBoxResponse response = MessageBoxResponse.YES)
        {
            if (response != MessageBoxResponse.YES)
            {
                return;
            }
            _toBePrintFilmingPageList = FilmsNotEmpty;
            if (_toBePrintFilmingPageList.Count < Card.EntityFilmingPageList.Count)
            {
                MessageBoxHandler.Instance.ShowInfo("UID_Filming_Auto_Delete_Empty_Film");
                Card.EntityFilmingPageList.RemoveEmptyPages();
                Card.ReOrderCurrentFilmPageBoard();
            }

            if (IsExistCellEmpty(false))
            {
                MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Print_CellEmptyHint", OffScreenOperationHandler);

            }
            else
            {
                OffScreenOperation();
            }
        }

        private void OffScreenOperationHandler(MessageBoxResponse response = MessageBoxResponse.YES)
        {
            if (response != MessageBoxResponse.YES)
            {
                return;
            }
            OffScreenOperation();
        }

        private void OffScreenOperation(bool isPrint = true)
        {
            try
            {
                
                Logger.LogFuncUp();
                
                Card.ActiveFilmingPageList.ClearTextAnnotationEditableStatus();

                FilmingViewerContainee.Main.ShowStatusInfo(isPrint
                                                               ? "UID_Filming_Begin_To_Print"
                                                               : "UID_Filming_Begin_To_Save_EFilm");

                XmlDocument xDoc = new XmlDocument();
                var root = xDoc.CreateElement(OffScreenRenderXmlHelper.PRINT_JOB_INFO);
                root.SetAttribute(OffScreenRenderXmlHelper.OPERATION, isPrint.ToString());
                xDoc.AppendChild(root);

                //1.打印机配置信息
                PrinterSetting.DataViewModal.SerializedToXml(root);

                //2.胶片标题配置信息
                PageTitleConfigure.Instance.SerializeToXml(root, isPrintShowPageNoFilm);

                //3.所有的胶片信息
                Dictionary<string, byte[]> allFilePath2DataHeader = null;
                List<string> originalUidsInCard;
                if (!this.SerializeToXml(root, out allFilePath2DataHeader, out originalUidsInCard))
                {
                    Logger.Instance.LogDevError("Film Card Serialized Failed！");
                    return;
                }

                //4.发送大数据
                if (null != allFilePath2DataHeader && allFilePath2DataHeader.Count > 0)
                {
                    if (!Directory.Exists(OffScreenRenderXmlHelper.DataHeaderFilePath))
                    {
                        Directory.CreateDirectory(OffScreenRenderXmlHelper.DataHeaderFilePath);
                    }
                    var thread = new Thread(() =>
                    {
                        lock (_printtingTaskLock)
                        {
                            foreach (KeyValuePair<string, byte[]> entry in allFilePath2DataHeader)
                            {
                                using (var fs = new FileStream(entry.Key, FileMode.Create))
                                {
                                    fs.Write(entry.Value, 0, entry.Value.Length);
                                    fs.Flush();
                                    fs.Close();
                                }
                            }
                            CommonCommand.SendFilmingDataToServiceFE(Encoding.UTF8.GetBytes(xDoc.WriteToString()));
                        }
                    });
                    thread.Start();
                }
                else
                {
                    CommonCommand.SendFilmingDataToServiceFE(Encoding.UTF8.GetBytes(xDoc.WriteToString()));
                }

                if (isPrint && originalUidsInCard != null && originalUidsInCard.Count > 0)
                {
                    originalUidsInCard = originalUidsInCard.Distinct().ToList();    //当前存储的是studyInstanceUid

                    Logger.Instance.LogSvcInfo(Logger.Source, FilmingSvcLogUid.LogUidSvcInfoPrintButtonClicked,
                            "[Print Button or Menu Clicked]" +
                            (originalUidsInCard.Count > 1 ? "mixed" : originalUidsInCard.FirstOrDefault()));

                    FilmingUtility.UpdatePrintStatus(originalUidsInCard, FilmingDbOperation.Instance.FilmingDbWrapper, FilmingViewerContainee.Main.GetCommunicationProxy());
                }

                if ((isPrint && PrinterSetting.DataViewModal.IfClearAfterAddFilmingJob)||(!isPrint && PrinterSetting.DataViewModal.IfClearAfterSaveEFilm))
                {
                    _toBePrintFilmingPageList.ToList().ForEach(p => p.IsSelected = true);

                    Card.BtnEditCtrl.OnDeleteActiveFilmPages(null, null);
                }

                if (isPrint)
                {
                    if (PrinterSetting.DataViewModal.IfShutDownAfterPrint && !isPrintCurrentFilm)
                    {
                        NotifyMFShutDownCurrentPatient();
                    }
                    FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_OffScreen_Begin_To_Print");
                }
                else
                {
                    Logger.LogInfo("[Begin To Print]Has been sent to the off-screen render queue");//对医生来说不知道离屏渲染状态，该界面提示不要，只做日志
                }

                Logger.LogFuncDown();
                // EnableUI();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }

        }

        #endregion


        private bool PrintFilms()
        {
            if (0 == Card.EntityFilmingPageList.Count)
            {
                return true;
            }
            if (AnyUnsupportedImage())
            {
                MessageBoxHandler.Instance.ShowInfo("UID_Filming_Info_Not_Support_Comman_Filming_For_XR_Images");
                return true;
            }
            if (HasMixedFilm())
            {
                MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Warning_Mixed_Film_Print", PrintAllHandler);
            }
            else
            {
                Logger.LogInfo("[Begin To Print](Maybe saving Electronic films at the same time)");
                PrintAllHandler(MessageBoxResponse.YES);
                Logger.LogInfo("[End To Print](Maybe saving Electronic films at the same time)");
            }
            return false;
        }

        #region [Jinyang.li Performance]

        public bool SerializeToXml(XmlNode parentNode, out  Dictionary<string, byte[]> allFilePath2DataHeader, out List<string> originalUidsInCard)
        {
            try
            {
                Logger.LogFuncUp();

                allFilePath2DataHeader = new Dictionary<string, byte[]>();
                originalUidsInCard = new List<string>();

                //胶片尺寸信息
                if (parentNode.OwnerDocument == null) return false;

                var filmingPageSizeNode =
                    parentNode.OwnerDocument.CreateElement(OffScreenRenderXmlHelper.FILMING_PAGE_SIZE);
                OffScreenRenderXmlHelper.AppendChildNode(filmingPageSizeNode, OffScreenRenderXmlHelper.FILMING_PAGE_HEIGHT, FilmingUtility.DisplayedFilmPageHeight.ToString());
                OffScreenRenderXmlHelper.AppendChildNode(filmingPageSizeNode, OffScreenRenderXmlHelper.FILMING_PAGE_WIDTH, FilmingUtility.DisplayedFilmPageWidth.ToString());
                OffScreenRenderXmlHelper.AppendChildNode(filmingPageSizeNode, OffScreenRenderXmlHelper.FILMING_PAGE_VIEWER_HEIGHT, FilmingUtility.DisplayedFilmPageViewerHeight.ToString());
                parentNode.AppendChild(filmingPageSizeNode);

                var allFilmingPageInfo = parentNode.OwnerDocument.CreateElement(OffScreenRenderXmlHelper.ALL_FILMING_PAGE_INFO);
                parentNode.AppendChild(allFilmingPageInfo);

                foreach (var filmingPageControl in _toBePrintFilmingPageList)
                {
                    Dictionary<string, byte[]> partFilePath2DataHeader = null;
                    List<string> originalSopInstanceUidsInPage;
                    if (!filmingPageControl.SerializeToXml(allFilmingPageInfo, out partFilePath2DataHeader, out originalSopInstanceUidsInPage))
                    {
                        Logger.Instance.LogDevError("Film Page Serialized Failed!");
                        allFilePath2DataHeader = null;
                        originalUidsInCard = null;
                        return false;
                    }

                    foreach (KeyValuePair<string, byte[]> entry in partFilePath2DataHeader)
                    {
                        allFilePath2DataHeader.Add(entry.Key, entry.Value);
                    }

                    originalUidsInCard.AddRange(originalSopInstanceUidsInPage);
                }

                Logger.LogFuncDown();

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                allFilePath2DataHeader = null;
                originalUidsInCard = null;
                return false;
            }
        }

        #endregion

        //Now Printing One Film, TODO: printing a sheet
        public void PrintDisplayedSelectedFilm(object sender, RoutedEventArgs e)
        {
            try
            {
                //  DisableUI();
                Logger.LogFuncUp();

                if (0 == Card.EntityFilmingPageList.Count)
                {
                    return;
                }

                //if (AnyUnsupportedImage())
                //{
                //    MessageBoxHandler.Instance.ShowInfo("UID_Filming_Info_Not_Support_Comman_Filming_For_XR_Images");
                //    return;
                //}

                MessageBoxResponse response = MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Print_CellPageNo", PrintPageNoHandler);

                if (response==MessageBoxResponse.NO)
                {
                    isPrintShowPageNoFilm = false;
                }

                if (IsExistCellEmpty(true))
                {
                    MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Print_CellEmptyHint", PrintDisplayedSelectedFilmHandler);
                }
                else
                {
                    if (HasMixedFilmInDisplayedSelectedFilm())
                    {
                        MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Warning_Mixed_Film_Print",
                                                                PrintDisplayedSelectedFilmHandler);
                    }
                    else
                    {
                        PrintDisplayedSelectedFilmHandler(MessageBoxResponse.YES);
                    }
                }

                Logger.LogFuncDown();
                // EnableUI();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }

        }

        private void PrintPageNoHandler(MessageBoxResponse response = MessageBoxResponse.YES)
        {
            if (response != MessageBoxResponse.YES)
            {
                return;
            }

            isPrintShowPageNoFilm = true;
           
        }

        private bool HasMixedFilmInDisplayedSelectedFilm()
        {
            var studyIds =
                DisplayedSelectedFilmPage.Where(film => film.IsAnyImageLoaded()).Select(
                    film => film.PageTitle.StudyInstanceUid).Distinct().ToList();
            studyIds.Remove(string.Empty);
            return (studyIds.Count > 1 || studyIds.Contains(FilmingHelper.StarsString));
        }

        private void PrintDisplayedSelectedFilmHandler(MessageBoxResponse response = MessageBoxResponse.YES)
        {
            if (response != MessageBoxResponse.YES)
            {
                return;
            }
            Logger.LogInfo("[Begin To Customization Print](Maybe saving Electronic films at the same time)");
            if (0 == Printers.Instance.PeerNodes.Count)
            {
                MessageBoxHandler.Instance.ShowInfo("UID_Filming_Info_Not_Config_Printer");
                return;
            }
            isPrintCurrentFilm = true;
            var peer = new PeerNode();
            Printers.Instance.GetPacsNodeParametersByAE(
                PrinterSetting.DataViewModal.CurrentPrinterAE.ToString(), ref peer);

            _toBePrintFilmingPageList = DisplayedSelectedFilmPage.ToList();

            if (Printers.Instance.IfUseFilmingServiceFE && !(peer.NodeType == PeerNodeType.FILM_PRINTER && Card._filmingCardModality == FilmingUtility.EFilmModality))
                OffScreenOperation();
            else 
                PrintAllHandler(MessageBoxResponse.YES);
            Logger.LogInfo("[End To Customization Print](Maybe saving Electronic films at the same time)");
        }

        public IEnumerable<FilmingPageControl> DisplayedSelectedFilmPage
        {
            get
            {
                return Card.ActiveFilmingPageList.ToList();
                //var filmCount = EntityFilmingPageList.Count;
                //var start = CurrentFilmPageBoardIndex * SelectedFilmCardDisplayMode;
                //var end = start + SelectedFilmCardDisplayMode;
                //if (end > filmCount) end = filmCount;
                //for (int i = start; i < end; i++)
                //{
                //    var page = EntityFilmingPageList[i];
                //    if (page != null && page.IsSelected && !page.IsEmpty()) yield return page;

                //}
            }

        }

        private void SaveEFilmHandler(MessageBoxResponse response)
        {
            int previouseDisplayMode = Card.SelectedFilmCardDisplayMode;
            try
            {
                Logger.LogFuncUp();

                //disable all UI controls and show a progress bar
                Card.DisableUI();

                Card.layoutCtrl.SelectedFilmCardDisplayMode = 1;
                Card.ActiveFilmingPageList.UnSelectAllCells();

                //FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Begin_To_Save_EFilm");
                FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Begin_To_Save_EFilm");
                //TryToCopyScreenCount = 0;
                SaveEFilms();
                //TryToCopyScreenCount = 0;
                //FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_End_To_Save_EFilm");

                Logger.LogFuncDown();

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
#if DEVELOPER
                MessageBox.Show("保存电子胶片失败");
#else
                MessageBoxHandler.Instance.ShowError("UID_Filming_Info_SaveEFilm_Fail");
#endif
                //MessageBox.Show("Exception when Saving films as dicom image files :" + ex.StackTrace);
            }
            finally
            {
                Card.layoutCtrl.SelectedFilmCardDisplayMode = previouseDisplayMode;
                Card.actiontoolCtrl.RefreshAction();
                printAllButton.IsEnabled = true;
                saveEFilmButton.IsEnabled = true;
                Card._maskBorder.Visibility = Visibility.Hidden;
                Card._maskBorder.Cursor = null;
                Keyboard.Focus(Card);
                // HostAdornerCount = 0;
                // ReOrderCurrentFilmPageBoard();

                //todo: performance optimization begin pageTitle
                Card.EntityFilmingPageList.ForEach((film) => film.RefereshPageTitle());
                //todo: performance optimization end

                //RefreshAnnotationDisplayMode(); //TODO: Annotation Display WorkAround

                FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();

                Card.UpdateUIStatus();

            }
        }
        private void SetPageTitleStatusBeforePrint()
        {
            printAllButton.Focus();

            foreach (FilmingPageControl filmingPageControl in Card.EntityFilmingPageList)
            {
                filmingPageControl.SetPageTitleBeforePrint();
            }
        }
        private void SetPageTitleStatusAfterPrint()
        {
            foreach (FilmingPageControl filmingPageControl in Card.EntityFilmingPageList)
            {
                filmingPageControl.SetPageTitleAfterPrint();
            }
        }
        private void SaveEFilmBtnClick(object sender, RoutedEventArgs e)
        {
            Logger.Instance.LogPerformanceRecord("[Begin][SaveEFilm]");
            if (!Printers.Instance.IfUseFilmingServiceFE)
            {
                SaveEFilmEventHandler();
            }
            else
            {
                _toBePrintFilmingPageList = FilmsNotEmpty;
                if (_toBePrintFilmingPageList.Count < Card.EntityFilmingPageList.Count)
                {
                    MessageBoxHandler.Instance.ShowInfo("UID_Filming_Auto_Delete_Empty_Film");
                    Card.EntityFilmingPageList.RemoveEmptyPages();
                    Card.ReOrderCurrentFilmPageBoard();
                }
                OffScreenOperation(false);
            }
            Logger.Instance.LogPerformanceRecord("[End][SaveEFilm]");
        }

        private void SaveEFilmEventHandler()
        {
            try
            {
                Logger.LogFuncUp();

                if (0 == Card.EntityFilmingPageList.Count)
                {
                    return;
                }

                if (AnyUnsupportedImage())
                {
                    MessageBoxHandler.Instance.ShowInfo("UID_Filming_Info_Not_Support_Comman_Filming_For_XR_Images");
                    return;
                }

                if (HasMixedFilm())
                {
                    MessageBoxHandler.Instance.ShowInfo("UID_Filming_Not_Support_Save_EFilm_For_Mixed_Images");
                    //CommandManager.InvalidateRequerySuggested();
                    return;
                }

                Logger.LogInfo("[Begin To save Electronic films]");
                SetPageTitleStatusBeforePrint();
                Card.Dispatcher.Invoke(new Action(() => SaveEFilmHandler(MessageBoxResponse.OK)),
                                 DispatcherPriority.Normal);
                Card.Dispatcher.BeginInvoke(new Action(() => Card.EnableUI(Card.FilmingCardModality == FilmingUtility.EFilmModality)),
                                      DispatcherPriority.Normal);

                Logger.LogInfo("[End To save Electronic films]");

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);

            }
            finally
            {
                SetPageTitleStatusAfterPrint();
            }


        }

        private bool HasMixedFilm()
        {
            //foreach (var filmingPage in EntityFilmingPageList)
            //{
            //    if (filmingPage.PageTitle.IsMixed)
            //    {
            //        return true;
            //    }
            //}
            var studyIds =
                 Card.EntityFilmingPageList.Where(film => film.IsAnyImageLoaded()).Select(
                    film => film.PageTitle.StudyInstanceUid).Distinct().ToList();
            studyIds.Remove(string.Empty);
            return (/*studyIds.Count > 1 ||*/ studyIds.Contains(FilmingHelper.StarsString));

        }


        private void SaveEFilms()
        {
            try
            {
                Logger.LogFuncUp();

                //for once save action, stored all the film page as one new series.

                IEnumerable<FilmingPageControl> filmsToBeSaved = from film in Card.EntityFilmingPageList
                                                                 where
                                                                     !film.PageTitle.IsMixed && film.IsAnyImageLoaded()
                                                                 select film;

                var studyIDList = (from film in filmsToBeSaved
                                   select film.PageTitle.StudyInstanceUid)
                    .Distinct();
                if (studyIDList == null || studyIDList.Count() == 0)
                {
                    return;
                }

                //McsfDatabaseDicomUIDManager uidManager = McsfDatabaseDicomUIDManagerFactory.Instance().CreateUIDManager();
                var studySeriesIDMap = new Dictionary<string, Dictionary<string, string>>();
                foreach (var study in studyIDList)
                {
                    var modalityListWithSameStudyUid =
                        (from film in filmsToBeSaved
                         where film.PageTitle.StudyInstanceUid == study
                         select film.PageTitle.FilmingPageModality).Distinct();
                    var modalityToSeriesUidDic = new Dictionary<string, string>();
                    foreach (var modality in modalityListWithSameStudyUid)
                    {
                        string seriesUID = string.Empty;
                        CreateSeries(study, modality, out seriesUID, true);
                        modalityToSeriesUidDic.Add(modality, seriesUID);
                    }

                    studySeriesIDMap[study] = modalityToSeriesUidDic;//uidManager.CreateSeriesUID("1", "2", ""); // //
                }

                //Save films as dicom images files
                TraverseFilmPages();

                // string sDicomFileList = string.Empty;
                string currentFileSize;
                if (PrinterSetting.DataViewModal.CurrentFilmSize == null)
                {
                    currentFileSize = string.Empty;
                }
                else
                {
                    currentFileSize = PrinterSetting.DataViewModal.CurrentFilmSize.ToString();
                }

                var dpi = Printers.Instance.IfSaveHighPrecisionEFilm
                              ? Printers.Instance.GetMaxDensityOf(PrinterSetting.DataViewModal.CurrentPrinterAE)
                              : FilmingUtility.ScreenDPI;
                Size size = ConvertFilmSizeFrom(currentFileSize, dpi);
                bool DoingForceRender = true;
                if (filmsToBeSaved.Count() == 1)
                    DoingForceRender = false;
                foreach (var film in filmsToBeSaved)
                {
                    SaveFilmingCommandContext.Builder builder = SaveFilmingCommandContext.CreateBuilder();
                    builder.SetCellIndex(0);
                    builder.SetSaveImageType(SavingType.SecondaryCapture);
                    builder.SetOperationType(SaveFilmingMode.Save);
                    builder.SetStrategy(SaveFilmingStrategy.SaveImages);
                    builder.SetKeepSameSeries(true);

                    //string sampleSeriesUid = string.Empty;
                    //try
                    //{
                    //    var samplePage = film; // filmsToBeSaved.First();
                    //    var sampleCell = samplePage.Cells.First(c => c != null && !c.IsEmpty);
                    //    var dicomHeader = sampleCell.Image.CurrentPage.ImageHeader.DicomHeader;
                    //    sampleSeriesUid = dicomHeader["SeriesInstanceUID"];
                    //}
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine(e);
                    //}

                    //builder.SetSeriesUID(sampleSeriesUid);



                    //begin

                    var studyInstanceUid = film.PageTitle.StudyInstanceUid;
                    var modalityToSeriesUidDic = studySeriesIDMap[studyInstanceUid];
                    film.PageTitle.EFilmSeriesUid = modalityToSeriesUidDic[film.PageTitle.FilmingPageModality];

                    builder.SetSeriesUID(film.PageTitle.EFilmSeriesUid);

                    //sDicomFileList += film.SaveViewerControlAsDicom(size);
                    //sDicomFileList += "#";

                    //sDicomFileList += film.SaveViewerControlAsDicom(size);
                    //sDicomFileList += "#";
                    bool ifSaveImageAsGreyScale = !Printers.Instance.IfColorPrint;
                    WriteableBitmap bmp = RenderBitmapHelper.RenderToBitmap(size,
                                                               film.PageTitle,
                                                               Card,
                                                               film,
                                                               film.filmingViewerControl,
                                                               film.filmPageBarGrid, DoingForceRender, ifSaveImageAsGreyScale);

                    //string dicomFilePath = CreatePrintImageFullPath(0, 0);
                    WriteableBitmap wtb = ifSaveImageAsGreyScale
                        ? new WriteableBitmap(new FormatConvertedBitmap(bmp, PixelFormats.Gray8, null, 0))
                        : new WriteableBitmap(new FormatConvertedBitmap(bmp, PixelFormats.Rgb24,
                                                                                              null, 0));
                    byte[] data = RenderBitmapHelper.ProcessImage(wtb);
                    var dataHeader = film.PageTitle.AssembleSendData(data, wtb.PixelWidth, wtb.PixelHeight, film.FilmPageIndex, ifSaveImageAsGreyScale);

                    Thread thread = new Thread(() => { AssembleMyData(dataHeader, builder); });
                    thread.Start();
                    //bool b;
                    //byte[] serializedInfo;
                    //if (dataHeader != null)
                    //{
                    //    b = dataHeader.Serialize(out serializedInfo);
                    //}
                    //else
                    //{
                    //    serializedInfo = new byte[0];
                    //}

                    //ImageAuxiliary.Builder auxiliaryBuilder = ImageAuxiliary.CreateBuilder();
                    //auxiliaryBuilder.PS = string.Empty;
                    ////auxiliaryBuilder.ActivePS = string.Empty;
                    ////auxiliaryBuilder.BurnedPS = string.Empty;
                    //auxiliaryBuilder.CellIndex = -1;//区别于 SaveSeries
                    //auxiliaryBuilder.SaveAsDisplay = true;

                    ////string sInfo = serializedInfo;
                    ////string str = System.Text.Encoding.UTF8.GetString(serializedInfo);
                    //auxiliaryBuilder.DataHeaderBytes = Google.ProtocolBuffers.ByteString.CopyFrom(serializedInfo);
                    //builder.AddImageAuxiliaries(auxiliaryBuilder);

                    ////end

                    //byte[] btInfo = builder.Build().ToByteArray();

                    //CommandContext cs = new CommandContext();
                    //cs.iCommandId = 16000; //7088
                    //cs.sReceiver = CommunicationNodeName.CreateCommunicationProxyName("FilmingService");
                    ////byte[] serializedJob = CreateFilmingJobInstance();
                    //cs.sSerializeObject = btInfo;

                    ////cs.bServiceAsyncDispatch = true;

                    //int errorCode = FilmingViewerContainee.Main.GetCommunicationProxy().AsyncSendCommand(cs);
                    //if (0 != errorCode)
                    //{
                    //    throw new Exception("send filming job command failure, error code: " + errorCode);
                    //}
                }
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {

                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }
        private void AssembleMyData(DicomAttributeCollection dataHeader, SaveFilmingCommandContext.Builder builder)
        {


            bool b;
            byte[] serializedInfo;
            if (dataHeader != null)
            {

                b = dataHeader.Serialize(out serializedInfo);
            }
            else
            {
                serializedInfo = new byte[0];
            }

            ImageAuxiliary.Builder auxiliaryBuilder = ImageAuxiliary.CreateBuilder();
            auxiliaryBuilder.PS = string.Empty;

            auxiliaryBuilder.CellIndex = -1;//区别于 SaveSeries
            auxiliaryBuilder.SaveAsDisplay = true;


            auxiliaryBuilder.DataHeaderBytes = Google.ProtocolBuffers.ByteString.CopyFrom(serializedInfo);
            builder.AddImageAuxiliaries(auxiliaryBuilder);

            //end

            byte[] btInfo = builder.Build().ToByteArray();

            CommandContext cs = new CommandContext();
            cs.iCommandId = 16000; //7088
            cs.sReceiver = CommunicationNodeName.CreateCommunicationProxyName("FilmingService");

            cs.sSerializeObject = btInfo;

            //cs.bServiceAsyncDispatch = true;

            int errorCode = FilmingViewerContainee.Main.GetCommunicationProxy().AsyncSendCommand(cs);
            if (0 != errorCode)
            {
                throw new Exception("send filming job command failure, error code: " + errorCode);
            }
        }

        private void CreateSeries(string study, string modality, out string seriesUID, bool bSaveEFilm)
        {
            //seriesUID = string.Empty;

            //FilmingPageControl page = pageList.First(n => n.PageTitle.StudyInstanceUid == studyUID);
            //if (null == page)
            //{
            //    return;
            //}

            //Dictionary<string, string> dicomHeader = null;
            //if (page.Cells.Count() > 0)
            //{
            //    MedViewerControlCell cell = page.Cells.First(n => n.Image != null && n.Image.CurrentPage != null && n.Image.CurrentPage.ImageHeader != null);
            //    dicomHeader = cell.Image.CurrentPage.ImageHeader.DicomHeader;

            //    if (dicomHeader == null)
            //    {
            //        return;
            //    }
            //}
            //else
            //{
            //    return;
            //}

            //string rawSeriesIntanceUID = string.Empty;
            //if (dicomHeader.ContainsKey("SeriesInstanceUID"))
            //{
            //    rawSeriesIntanceUID = dicomHeader["SeriesInstanceUID"];
            //    if (string.IsNullOrEmpty(rawSeriesIntanceUID))
            //    {
            //        return;
            //    }
            //}
            //else
            //{
            //    return;
            //}

            //var oldSeries = DBWrapperHelper.DBWrapper.GetSeriesBaseBySeriesInstanceUID(rawSeriesIntanceUID);
            //if (oldSeries == null)
            //{
            //    return;
            //}

            var studyInstanceUid = study;

            Series series = DBWrapperHelper.DBWrapper.CreateSeries();
            seriesUID = series.SeriesInstanceUID;
            series.StudyInstanceUIDFk = studyInstanceUid;
            series.Modality = modality;
            // new seriesNumber equals the max seriesNumber of exist series add one
            series.SeriesNumber = Convert.ToInt32(FilmingHelper.GetSerieNumber(studyInstanceUid)) + 1;

            if (studyInstanceUid == FilmingHelper.StarsString) return;

            // Check if we need to save the created series
            if (bSaveEFilm)
            {
                //check whether disk space is enough
                ICommunicationProxy pCommProxy = FilmingViewerContainee.Main.GetCommunicationProxy();
                var target = new SystemResManagerProxy(pCommProxy);
                if (!target.HaveEnoughSpace())
                {
                    Logger.LogWarning("No enough disk space, so Electronic Image Series will not be created");
                    FilmingViewerContainee.Main.ShowStatusWarning("UID_Filming_No_Enough_Disk_Space_To_Create_Electronic_Image_Series");
                    return;
                }

                series.Save();
            }
            //else
            //{
            //    if (CanSaveFilm)
            //    {
            //        series.Save();
            //    }
            //}
        }

        #endregion [--Print UI Event Handler--]

        #region [--For XiongKe Hospital--]

        private void SaveSeries()
        {

            try
            {
                Logger.LogFuncUp();

                var cells = new List<MedViewerControlCell>();
                foreach (var film in Card.EntityFilmingPageList)
                {
                    cells.AddRange(film.Cells.Where(cell => cell != null && cell.Image != null && cell.Image.CurrentPage != null
                        ));
                }

                var uidManager = McsfDatabaseDicomUIDManagerFactory.Instance().CreateUIDManager();
                var dicomConvertorProxyFactory = new McsfDicomConvertorProxyFactory();
                var dicomConvertorProxy = dicomConvertorProxyFactory.CreateDicomConvertorProxy();

                //1.Record all sopInstance uids
                string studyInstanceUid = string.Empty;
                List<string> savedImageUids = new List<string>();
                foreach (var cell in cells)
                {
                    try
                    {

                        var displayData = cell.Image.CurrentPage;
                        if (displayData.ImageHeader == null)
                        {
                            Logger.LogWarning("No ImageHeader");
                            continue;
                        }
                        var dicomHeader = displayData.ImageHeader.DicomHeader;
                        if (dicomHeader == null)
                        {
                            Logger.LogWarning("No dicomHeader");
                            continue;
                        }

                        if (!dicomHeader.ContainsKey(ServiceTagName.SOPInstanceUID)) continue;
                        var sopInstanceUid = dicomHeader[ServiceTagName.SOPInstanceUID];
                        if (string.IsNullOrWhiteSpace(studyInstanceUid) && dicomHeader.ContainsKey(ServiceTagName.StudyInstanceUID))
                        {
                            studyInstanceUid = dicomHeader[ServiceTagName.StudyInstanceUID];
                            string modality = string.Empty;
                            if (dicomHeader.ContainsKey(ServiceTagName.Modality)) modality = dicomHeader[ServiceTagName.Modality];
                            CellSaveHelper.RefreshSeries(studyInstanceUid, modality);
                            savedImageUids = CellSaveHelper.SavedImageUids.ToList();
                        }

                        if (savedImageUids.Contains(sopInstanceUid))
                        {
                            savedImageUids.Remove(sopInstanceUid);
                            continue;
                        }

                        //var sop = _studyTree.GetSop(sopInstanceUid);

                        //Debug.Assert(sop != null);

                        var dataHeader = displayData.DicomHeader.Clone();
                        FilmingHelper.ModifyStringDicomElement(dataHeader, Pipeline.Dictionary.Tag.SeriesInstanceUid, CellSaveHelper.SeriesInstanceUid);
                        FilmingHelper.ModifyStringDicomElement(dataHeader, Pipeline.Dictionary.Tag.SOPInstanceUID, uidManager.CreateImageUID(""));

                        var pixelData = displayData.PixelData;
                        if (pixelData == null)
                        {
                            pixelData = new byte[displayData.ImageDataLength];
                            Marshal.Copy(displayData.ImageDataPtr, pixelData, 0, displayData.ImageDataLength);
                        }

                        var element = DicomAttribute.CreateAttribute(Pipeline.Dictionary.Tag.PixelData, VR.OB);
                        if (!element.SetBytes(0, pixelData))
                        {
                            Logger.LogWarning("Failed to Insert NULL  image Data to Data header");
                        }
                        dataHeader.AddDicomAttribute(element);


                        dicomConvertorProxy.SaveImage(dataHeader, FilmingViewerContainee.Main.GetCommunicationProxy());

                        dataHeader.Dispose();
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.LogDevError(e.Message+e.StackTrace);
                    }
                }

                //1.1 remove old images
                var db = FilmingDbOperation.Instance.FilmingDbWrapper;
                foreach (var savedImageUid in savedImageUids)
                {
                    var image = db.GetImageBaseBySOPInstanceUID(savedImageUid);
                    image.Erase();
                }


                //2.Create a new series

                //update series thumbnail

                db.UpdateSeriesThumbnail(CellSaveHelper.SeriesInstanceUid);

                ////3. Get all Images from studyTree


                ////4. AutoArchiving new series
                ////暂时交给了后端?
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }

        }

        //private void SaveSeriesOld()
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();

        //        SaveFilmingCommandContext.Builder builder = SaveFilmingCommandContext.CreateBuilder();
        //        builder.SetCellIndex(0);
        //        builder.SetSaveImageType(SavingType.Original);
        //        builder.SetOperationType(SaveFilmingMode.Save);

        //        //!!!save all the cells as a series, for a test
        //        var cells = new List<MedViewerControlCell>();
        //        foreach (var film in EntityFilmingPageList)
        //        {
        //            cells.AddRange(film.Cells.Where(cell => cell != null && cell.Image != null && cell.Image.CurrentPage != null
        //                && cell.Image.CurrentPage.PixelData != null));
        //        }

        //        string sampleSeriesUid = string.Empty;
        //        try
        //        {
        //            var sampleCell = cells.First();
        //            var dicomHeader = sampleCell.Image.CurrentPage.ImageHeader.DicomHeader;
        //            sampleSeriesUid = dicomHeader["SeriesInstanceUID"];
        //        }
        //        catch (Exception e)

        //        {
        //            Console.WriteLine(e);
        //        }

        //        builder.SetSeriesUID(sampleSeriesUid);

        //        int i = 0;
        //        foreach (var cell in cells)
        //        {

        //            //var film = EntityFilmingPageList.FirstOrDefault();
        //            //var displayData = cell.Image.CurrentPage;
        //            //var dicomHeader = displayData.ImageHeader.DicomHeader;

        //            //if (i == 0) CellSaveHelper.CreateSeries(dicomHeader);

        //            ////here: 需要重写
        //            //var dataHeader = film.PageTitle.AssembleSendData(
        //            //    displayData.PixelData, Convert.ToDouble(dicomHeader["Rows"]), Convert.ToDouble(dicomHeader["Columns"]), i);
        //            //dataHeader.RemoveDicomAttribute(Pipeline.Dictionary.Tag.SeriesInstanceUid);
        //            //FilmingHelper.InsertStringDicomElement(dataHeader, Pipeline.Dictionary.Tag.SeriesInstanceUid,
        //            //    CellSaveHelper.SeriesInstanceUid);

        //            var dataHeader = CellSaveHelper.SaveCellAsDataHeader(cell, i);

        //            bool b;
        //            byte[] serializedInfo;
        //            if (dataHeader != null)
        //            {
        //                b = dataHeader.Serialize(out serializedInfo);
        //            }
        //            else
        //            {
        //                serializedInfo = new byte[0];
        //            }

        //            ImageAuxiliary.Builder auxiliaryBuilder = ImageAuxiliary.CreateBuilder();
        //            auxiliaryBuilder.ActivePS = string.Empty;
        //            auxiliaryBuilder.BurnedPS = string.Empty;
        //            int iSamplesPerPixel = 1;
        //            try
        //            {
        //                iSamplesPerPixel = Convert.ToInt32(FilmingHelper.GetTagValueFrom(dataHeader, 0x00280002));
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine(e);
        //            }
        //            auxiliaryBuilder.CellIndex = iSamplesPerPixel;
        //            auxiliaryBuilder.SaveAsDisplay = false;

        //            //string sInfo = serializedInfo;
        //            //string str = System.Text.Encoding.UTF8.GetString(serializedInfo);
        //            auxiliaryBuilder.DataHeaderBytes = Google.ProtocolBuffers.ByteString.CopyFrom(serializedInfo);
        //            builder.AddImageAuxiliaries(auxiliaryBuilder);

        //            i++;
        //        }

        //        byte[] btInfo = builder.Build().ToByteArray();

        //        CommandContext cs = new CommandContext();
        //        cs.iCommandId = 16000; //7088
        //        cs.sReceiver = CommunicationNodeName.CreateCommunicationProxyName("FilmingService");
        //        //byte[] serializedJob = CreateFilmingJobInstance();
        //        cs.sSerializeObject = btInfo;

        //        //cs.bServiceAsyncDispatch = true;

        //        int errorCode = FilmingViewerContainee.Main.GetCommunicationProxy().AsyncSendCommand(cs);
        //        if (0 != errorCode)
        //        {
        //            throw new Exception("send filming job command failure, error code: " + errorCode);
        //        }

        //        Logger.LogFuncDown();
        //    }
        //    catch (Exception ex)
        //    {

        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //        throw;
        //    }
        //}

        #endregion


        #region [--Private Printing Helper Methods--]

        /// <key>\n 
        /// PRA:Yes \n
        /// Traced from: SSFS_PRA_Filming_ImageScale \n
        /// Tests: N/A \n
        /// Description: print film based WYSWYG UI principle \n
        /// Short Description: WYSWYG \n
        /// Component: Filming \n
        /// </key> \n
        private void PrintAll()
        {
            try
            {
                Logger.LogFuncUp();

                Printers printers = Printers.Instance;
                PeerNode peer = new PeerNode();
                if (printers.GetPacsNodeParametersByAE(PrinterSetting.DataViewModal.CurrentPrinterAE, ref peer) ==
                    0)
                {
                    switch (peer.NodeType)
                    {
                        case PeerNodeType.FILM_PRINTER:

                            PrintAllByFilmPrinter();
                            break;
                        case PeerNodeType.GENERAL_PRINTER:
                            //if (CanSaveFilm)
                            //{
                            //    SaveEFilmHandler(MessageBoxResponse.YES);
                            //}
                            PrintAllByGeneralPrinter();
                            break;
                    }
                }
                else
                {
                    if (Printers.Instance.IfSaveEFilmWhenFilming && Card.FilmingCardModality != FilmingUtility.EFilmModality)
                    {
                        SaveEFilmHandler(MessageBoxResponse.YES);
                    }
                    MessageBoxHandler.Instance.ShowError("UID_Filming_Info_GetPacsNodeByAE_Fail");
                }

                Logger.LogFuncDown();

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        private bool CanSaveFilm
        {
            get
            {
                return !HasMixedFilm() && Printers.Instance.IfSaveEFilmsAvailable &&
                       Printers.Instance.IfSaveEFilmWhenFilming && Card.FilmingCardModality != FilmingUtility.EFilmModality;
            }
        }

        private void PrintAllByGeneralPrinter()
        {
            try
            {
                Logger.LogFuncUp();

                Card.EntityFilmingPageList.RemoveEmptyPages();
                var filmsToPrint = FilmsNotEmpty;
                if (filmsToPrint == null || filmsToPrint.Count == 0)
                {
                    return;
                }

                FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Begin_To_Print");

                bool canSaveFilm = CanSaveFilm;

                var studyIDList = (from film in filmsToPrint
                                   select film.PageTitle.StudyInstanceUid
                                  ).Distinct();
                if (!studyIDList.Any())
                {
                    return;
                }

                //McsfDatabaseDicomUIDManager uidManager = McsfDatabaseDicomUIDManagerFactory.Instance().CreateUIDManager();
                var studySeriesIDMap = new Dictionary<string, Dictionary<string, string>>();
                foreach (var study in studyIDList)
                {
                    var modalityListWithSameStudyUid =
                        (from film in filmsToPrint
                         where film.PageTitle.StudyInstanceUid == study && !film.PageTitle.IsMixed
                         select film.PageTitle.FilmingPageModality).Distinct();
                    var modalityToSeriesUidDic = new Dictionary<string, string>();
                    foreach (var modality in modalityListWithSameStudyUid)
                    {
                        string seriesUID = string.Empty;
                        CreateSeries(study, modality, out seriesUID, CanSaveFilm);
                        modalityToSeriesUidDic.Add(modality, seriesUID);
                    }

                    studySeriesIDMap[study] = modalityToSeriesUidDic;//uidManager.CreateSeriesUID("1", "2", ""); // //
                }

                TraverseFilmPages();


                //////////////////////////////纸质配置
                PrinterSettingDataViewModal setting = PrinterSetting.DataViewModal;
                var dpi = setting.GeneralPrinterDPI;

                // 设置页边距
                //const double margin = 30;
                double widthMargin = 30 * dpi / 96;
                double heightMargin = 60 * dpi / 96;
                PrintDialog printDlg = new PrintDialog();
                // 打印方向
                printDlg.PrintTicket.PageOrientation = setting.CurrentPaperPrintOrientation;
                //打印份数
                printDlg.PrintTicket.CopyCount = (int)setting.CurrentCopyCount;
                //选择纸张尺寸
                printDlg.PrintTicket.PageMediaSize = new PageMediaSize((PageMediaSizeName)setting.CurrentFilmSize);

                //从本地计算机中获取所有打印机对象(PrintQueue);

                //选择一个打印机;
                //var printers = new LocalPrintServer().GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Connections });
                //var printers = new LocalPrintServer().GetPrintQueues();

                var selectedPrinter = Printers.Instance.GeneralPrinters.FirstOrDefault(p => p.Name == setting.CurrentPrinterAE);
                if (selectedPrinter == null)
                {
                    MessageBox.Show("没有找到" + setting.CurrentPrinterAE);
                    return;
                }
                //设置打印;
                printDlg.PrintQueue = selectedPrinter;

                Card.ActiveFilmingPageList.UnSelectAllCells();

                //   if (printDlg.ShowDialog() != true) return;

                //var film = EntityFilmingPageList.FirstOrDefault();
                //var size = new Size(film.ActualWidth, film.ActualHeight);
                Size filmSize = ConvertSizeFromPaperSize(setting.CurrentFilmSize.ToString(), dpi);
                Size efilmSize = ConvertSizeFromPaperSize(setting.CurrentFilmSize.ToString(),
                                     Printers.Instance.IfSaveHighPrecisionEFilm
                                         ? dpi
                                         : FilmingUtility.ScreenDPI);


                //double scale = Math.Min(printDlg.PrintableAreaWidth / (film.ActualWidth + (margin * 2)),
                //        printDlg.PrintableAreaHeight / (film.ActualHeight + (margin * 2)));
                double scale = Math.Min(printDlg.PrintableAreaWidth / (filmSize.Width + (widthMargin * 2)),
                            printDlg.PrintableAreaHeight / (filmSize.Height + (widthMargin * 2)));
                //double scale = 1/Printers.Instance.DefaultPaperPrintDPI;

                /////////////////////////////纸质打印配置


                // 根据纸张尺寸设置缩放比例
                bool DoingForceRender = true;
                if (filmsToPrint.Count == 1) DoingForceRender = false;

                foreach (var filmPage in filmsToPrint)
                {
                    //if (!ele.IsVisible)
                    //{
                    //    DisplayFilmPage(ele);
                    //    filmPageGrid.UpdateLayout();
                    //}

                    WriteableBitmap printBitmap;

                    if (Card._filmingCardModality == FilmingUtility.EFilmModality)
                    {
                        printBitmap = new WriteableBitmap((int)filmSize.Width, (int)filmSize.Height, 96, 96, PixelFormats.Gray8, null);
                        printBitmap = RenderBitmapHelper.ConvertDisplayDataToBitmap(printBitmap,
                                                                               filmPage.Cells.FirstOrDefault().Image.
                                                                                   CurrentPage);
                        //printBitmap = RenderBitmapHelper.RenderViewerControlToBitmap(filmSize, this, filmPage,
                        //                                                             filmPage.filmingViewerControl, false);
                    }
                    else
                    {
                        printBitmap = RenderBitmapHelper.RenderToBitmap(filmSize, filmPage.PageTitle, Card, filmPage,
                                                                   filmPage.filmingViewerControl, filmPage.filmPageBarGrid, DoingForceRender,
                                                                   false);
                    }

                    //////////////////////////////纸质打印部分

                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext context = visual.RenderOpen())
                    {
                        ////VisualBrush brush = new VisualBrush(ele);

                        Logger.LogWarning("begin to draw Image");

                        if (printDlg.PrintTicket.PageOrientation == PageOrientation.Portrait)
                        {
                            //context.DrawRectangle(brush, null, new Rect(new Point(margin, 120),
                            //                                            new Size(ele.ActualWidth, ele.ActualHeight)));
                            context.DrawImage(printBitmap, new Rect(new Point(widthMargin, heightMargin), filmSize));
                        }
                        else if (printDlg.PrintTicket.PageOrientation == PageOrientation.Landscape)
                        {
                            //context.DrawRectangle(brush, null, new Rect(new Point(120, margin),
                            //                                            new Size(ele.ActualWidth, ele.ActualHeight)));
                            context.DrawImage(printBitmap, new Rect(new Point(heightMargin, widthMargin), filmSize));
                        }

                        Logger.LogWarning("end to draw image");
                    }

                    Logger.LogWarning("begin to Print visual");
                    visual.Transform = new ScaleTransform(scale, scale);
                    printDlg.PrintVisual(visual, string.Empty);

                    Logger.LogWarning("end to print visual");

                    //////////////////////////////纸质打印部分


                    SaveFilmingCommandContext.Builder builder = SaveFilmingCommandContext.CreateBuilder();
                    builder.SetCellIndex(0);
                    builder.SetSaveImageType(SavingType.SecondaryCapture);
                    builder.SetStrategy(SaveFilmingStrategy.SaveImages);
                    builder.SetKeepSameSeries(true);

                    builder.SetOperationType(SaveFilmingMode.Save);

                    //begin
                    ////skip empty film
                    //if (filmPage.IsAnyImageLoaded())
                    //{
                    var studyInstanceUid = filmPage.PageTitle.StudyInstanceUid;
                    var modality = filmPage.PageTitle.FilmingPageModality;
                    var modalityToSeriesUidDic = studySeriesIDMap[studyInstanceUid];
                    if (modalityToSeriesUidDic.ContainsKey(modality))
                        filmPage.PageTitle.EFilmSeriesUid = modalityToSeriesUidDic[modality];
                    else
                        filmPage.PageTitle.EFilmSeriesUid = string.Empty;
                    builder.SetSeriesUID(filmPage.PageTitle.EFilmSeriesUid);
                    DicomAttributeCollection dataHeader;
                    //}

                    if (!canSaveFilm || filmPage.PageTitle.IsMixed) continue;

                    WriteableBitmap eFilmBitmap;
                    //if (!Printers.Instance.IfSaveHighPrecisionEFilm || printBitmap == null)
                    //{
                    //    Logger.LogWarning("begin to render bitmap for low precisionEfilm");
                    eFilmBitmap = RenderBitmapHelper.RenderToBitmap(efilmSize,
                                                               filmPage.PageTitle,
                                                               Card,
                                                               filmPage,
                                                               filmPage.filmingViewerControl,
                                                               filmPage.filmPageBarGrid,
                                                               DoingForceRender);
                    //    Logger.LogWarning("end to render bitmap for low precisionEfilm");
                    //}
                    //else
                    //{
                    //    eFilmBitmap = printBitmap;
                    //}
                    //string dicomFilePath = CreatePrintImageFullPath(0, 0);
                    WriteableBitmap wtb =
                        new WriteableBitmap(new FormatConvertedBitmap(eFilmBitmap, PixelFormats.Gray8, null, 0));
                    byte[] data = RenderBitmapHelper.ProcessImage(wtb);
                    dataHeader = filmPage.PageTitle.AssembleSendData(data, wtb.Width, wtb.Height,
                                                                     filmPage.FilmPageIndex);

                    bool b;
                    byte[] serializedInfo;
                    if (dataHeader != null)
                    {
                        b = dataHeader.Serialize(out serializedInfo);
                    }
                    else
                    {
                        serializedInfo = new byte[0];
                    }

                    ImageAuxiliary.Builder auxiliaryBuilder = ImageAuxiliary.CreateBuilder();
                    auxiliaryBuilder.PS = string.Empty;
                    //auxiliaryBuilder.ActivePS = string.Empty;
                    //auxiliaryBuilder.BurnedPS = string.Empty;
                    auxiliaryBuilder.CellIndex = -1;//区别于 SaveSeries
                    auxiliaryBuilder.SaveAsDisplay = true;

                    //string sInfo = serializedInfo;
                    //string str = System.Text.Encoding.UTF8.GetString(serializedInfo);
                    auxiliaryBuilder.DataHeaderBytes = Google.ProtocolBuffers.ByteString.CopyFrom(serializedInfo);
                    builder.AddImageAuxiliaries(auxiliaryBuilder);

                    //end



                    byte[] btInfo = builder.Build().ToByteArray();

                    CommandContext cs = new CommandContext();
                    cs.iCommandId = 16000; //7088
                    cs.sReceiver = CommunicationNodeName.CreateCommunicationProxyName("FilmingService");
                    //byte[] serializedJob = CreateFilmingJobInstance();
                    cs.sSerializeObject = btInfo;

                    //cs.bServiceAsyncDispatch = true;

                    int errorCode = FilmingViewerContainee.Main.GetCommunicationProxy().AsyncSendCommand(cs);
                    if (0 != errorCode)
                    {
                        throw new Exception("send filming job command failure, error code: " + errorCode);
                    }

                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }

        }

        //private void PrintAllByGeneralPrinter()
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();

        //        EntityFilmingPageList.RemoveEmptyPages();


        //        PrinterSettingDataViewModal setting =PrinterSetting.DataViewModal;

        //        // 设置页边距
        //        const double margin = 30;
        //        PrintDialog printDlg = new PrintDialog();
        //        // 打印方向
        //        printDlg.PrintTicket.PageOrientation = (PageOrientation)setting.CurrentPaperPrintOrientation;
        //        //打印份数
        //        printDlg.PrintTicket.CopyCount = (int)setting.CurrentCopyCount;
        //        //选择纸张尺寸
        //        printDlg.PrintTicket.PageMediaSize = new PageMediaSize((PageMediaSizeName)setting.CurrentFilmSize);

        //        //从本地计算机中获取所有打印机对象(PrintQueue);

        //        //选择一个打印机;
        //        var printers = new LocalPrintServer().GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Connections });
        //        //var printers = new LocalPrintServer().GetPrintQueues();

        //        var selectedPrinter = printers.FirstOrDefault(p => p.Name == setting.CurrentPrinterAE);
        //        if (selectedPrinter == null)
        //        {
        //            MessageBox.Show("没有找到" + setting.CurrentPrinterAE);
        //            return;
        //        }
        //        //设置打印;
        //        printDlg.PrintQueue = selectedPrinter;

        //        ActiveFilmingPageList.UnSelectAllCells();

        //        //   if (printDlg.ShowDialog() != true) return;
        //        // 根据纸张尺寸设置缩放比例
        //        foreach (var ele in EntityFilmingPageList)
        //        {
        //            if (!ele.IsVisible)
        //            {
        //                DisplayFilmPage(ele);
        //                filmPageGrid.UpdateLayout();
        //            }
        //            double scale = Math.Min(printDlg.PrintableAreaWidth / (ele.ActualWidth + (margin * 2)),
        //                                    printDlg.PrintableAreaHeight / (ele.ActualHeight + (margin * 2)));
        //            DrawingVisual visual = new DrawingVisual();
        //            using (DrawingContext context = visual.RenderOpen())
        //            {
        //                VisualBrush brush = new VisualBrush(ele);
        //                if (printDlg.PrintTicket.PageOrientation == PageOrientation.Portrait)
        //                {
        //                    context.DrawRectangle(brush, null, new Rect(new Point(margin, 120),
        //                                                                new Size(ele.ActualWidth, ele.ActualHeight)));
        //                }
        //                else if (printDlg.PrintTicket.PageOrientation == PageOrientation.Landscape)
        //                {
        //                    context.DrawRectangle(brush, null, new Rect(new Point(120, margin),
        //                                                                new Size(ele.ActualWidth, ele.ActualHeight)));
        //                }
        //            }
        //            visual.Transform = new ScaleTransform(scale, scale);
        //            printDlg.PrintVisual(visual, string.Empty);
        //        }

        //        Logger.LogFuncDown();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //        throw;
        //    }

        //}

        //private void PrintAllByFilmPrinter()
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();

        //        if (FilmsNotEmpty == null || FilmsNotEmpty.Count == 0)
        //        {
        //            return;
        //        }

        //        FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Begin_To_Print");

        //        InitialAndSetFilmSetting();

        //        var studyIDList = (from film in FilmsNotEmpty
        //                          select film.PageTitle.StudyInstanceUid).Distinct();
        //        if (studyIDList == null || studyIDList.Count() == 0)
        //        {
        //            return;
        //        }
        //        McsfDatabaseDicomUIDManager uidManager = McsfDatabaseDicomUIDManagerFactory.Instance().CreateUIDManager();
        //        Dictionary<string, string> studySeriesIDMap = new Dictionary<string, string>();
        //        foreach (var study in studyIDList)
        //        {
        //            studySeriesIDMap[study] = uidManager.CreateSeriesUID("1", "2", "");
        //        }
        //        TraverseFilmPages();
        //        foreach (FilmingPageControl filmPage in EntityFilmingPageList)
        //        {
        //            try
        //            {
        //                if (filmPage.IsAnyImageLoaded())
        //                {
        //                    filmPage.PageTitle.EFilmSeriesUid = studySeriesIDMap[filmPage.PageTitle.StudyInstanceUid];
        //                    AddFilmInfo(filmPage);
        //                    ////show film page, when begin to copy screen
        //                }//skip empty film
        //            }
        //            catch (Exception ex)
        //            {
        //                Logger.LogFuncException(ex.Message+ex.StackTrace);
        //                throw;
        //            }
        //        }

        //        _jobCreator.SendFilmingJobCommand(FilmingViewerContainee.Main.GetCommunicationProxy());

        //        Logger.LogFuncDown();

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //        throw;
        //    }
        //}

        private void PrintAllByFilmPrinter()
        {
            try
            {
                Logger.LogFuncUp();
                var filmsToPrint = this._toBePrintFilmingPageList;
                if (filmsToPrint == null || filmsToPrint.Count == 0)
                {
                    return;
                }

                FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Begin_To_Print");

                bool canSaveFilm = CanSaveFilm;

                InitialAndSetFilmSetting();

                var studyIDList = (from film in filmsToPrint
                                   select film.PageTitle.StudyInstanceUid
                                  ).Distinct();
                if (!studyIDList.Any())
                {
                    return;
                }

                //  McsfDatabaseDicomUIDManager uidManager = McsfDatabaseDicomUIDManagerFactory.Instance().CreateUIDManager();
                var studySeriesIDMap = new Dictionary<string, Dictionary<string, string>>();
                foreach (var study in studyIDList)
                {
                    var modalityListWithSameStudyUid =
                        (from film in filmsToPrint
                         where film.PageTitle.StudyInstanceUid == study && !film.PageTitle.IsMixed
                         select film.PageTitle.FilmingPageModality).Distinct();
                    var modalityToSeriesUidDic = new Dictionary<string, string>();
                    foreach (var modality in modalityListWithSameStudyUid)
                    {
                        string seriesUID = string.Empty;
                        CreateSeries(study, modality, out seriesUID, CanSaveFilm);
                        modalityToSeriesUidDic.Add(modality, seriesUID);
                    }

                    studySeriesIDMap[study] = modalityToSeriesUidDic;//uidManager.CreateSeriesUID("1", "2", ""); // //
                }

                TraverseFilmPages();

                //string sDicomFileList = string.Empty;
                string currentFileSize;
                if (PrinterSetting.DataViewModal.CurrentFilmSize == null)
                {
                    currentFileSize = string.Empty;
                }
                else
                {
                    currentFileSize = PrinterSetting.DataViewModal.CurrentFilmSize.ToString();
                }
                var dpi = Printers.Instance.IfSaveHighPrecisionEFilm
                              ? Printers.Instance.GetMaxDensityOf(PrinterSetting.DataViewModal.CurrentPrinterAE)
                              : FilmingUtility.ScreenDPI;

                Size size = ConvertFilmSizeFrom(currentFileSize, dpi);

                bool DoingForceRender = true;
                if (filmsToPrint.Count == 1)
                    DoingForceRender = false;
                foreach (FilmingPageControl filmPage in filmsToPrint)
                {
                    SaveFilmingCommandContext.Builder builder = SaveFilmingCommandContext.CreateBuilder();
                    builder.SetCellIndex(0);
                    builder.SetSaveImageType(SavingType.SecondaryCapture);
                    builder.SetStrategy(SaveFilmingStrategy.SaveImages);
                    builder.SetKeepSameSeries(true);

                    builder.SetOperationType(SaveFilmingMode.Save);


                    var studyInstanceUid = filmPage.PageTitle.StudyInstanceUid;
                    var modality = filmPage.PageTitle.FilmingPageModality;
                    var modalityToSeriesUidDic = studySeriesIDMap[studyInstanceUid];
                    if (modalityToSeriesUidDic.ContainsKey(modality))
                        filmPage.PageTitle.EFilmSeriesUid = modalityToSeriesUidDic[modality];
                    else
                        filmPage.PageTitle.EFilmSeriesUid = string.Empty;
                    builder.SetSeriesUID(filmPage.PageTitle.EFilmSeriesUid);
                    DicomAttributeCollection dataHeader = AddFilmInfo(filmPage);


                    if (!canSaveFilm || filmPage.PageTitle.IsMixed) continue;
                    bool ifSaveImageAsGreyScale = !Printers.Instance.IfColorPrint;
                    if (!Printers.Instance.IfSaveHighPrecisionEFilm)
                    {
                        WriteableBitmap bmp = RenderBitmapHelper.RenderToBitmap(size,
                                                                   filmPage.PageTitle,
                                                                   Card,
                                                                   filmPage,
                                                                   filmPage.filmingViewerControl,
                                                                   filmPage.filmPageBarGrid,
                                                                   DoingForceRender, ifSaveImageAsGreyScale);

                        var wtb = ifSaveImageAsGreyScale
                                    ? new WriteableBitmap(new FormatConvertedBitmap(bmp, PixelFormats.Gray8, null, 0))
                                    : new WriteableBitmap(new FormatConvertedBitmap(bmp, PixelFormats.Rgb24, null, 0));

                        byte[] data = RenderBitmapHelper.ProcessImage(wtb);
                        dataHeader = filmPage.PageTitle.AssembleSendData(data, wtb.PixelWidth, wtb.PixelHeight,
                                                                         filmPage.FilmPageIndex, ifSaveImageAsGreyScale);
                    }
                    bool b;
                    byte[] serializedInfo;
                    if (dataHeader != null)
                    {
                        b = dataHeader.Serialize(out serializedInfo);
                    }
                    else
                    {
                        serializedInfo = new byte[0];
                    }

                    ImageAuxiliary.Builder auxiliaryBuilder = ImageAuxiliary.CreateBuilder();
                    auxiliaryBuilder.PS = string.Empty;

                    auxiliaryBuilder.CellIndex = -1;//区别于 SaveSeries
                    auxiliaryBuilder.SaveAsDisplay = true;

                    auxiliaryBuilder.DataHeaderBytes = Google.ProtocolBuffers.ByteString.CopyFrom(serializedInfo);
                    builder.AddImageAuxiliaries(auxiliaryBuilder);


                    byte[] btInfo = builder.Build().ToByteArray();

                    CommandContext cs = new CommandContext();
                    cs.iCommandId = 16000; //7088
                    cs.sReceiver = CommunicationNodeName.CreateCommunicationProxyName("FilmingService");

                    cs.sSerializeObject = btInfo;

                    int errorCode = FilmingViewerContainee.Main.GetCommunicationProxy().AsyncSendCommand(cs);
                    if (0 != errorCode)
                    {
                        throw new Exception("send filming job command failure, error code: " + errorCode);
                    }
                }

                _jobCreator.SendFilmingJobCommand(FilmingViewerContainee.Main.GetCommunicationProxy());

                Logger.LogFuncDown();

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        private IList<FilmingPageControl> _toBePrintFilmingPageList;

        public IList<FilmingPageControl> FilmsNotEmpty
        {
            get
            {
                return (from film in Card.EntityFilmingPageList
                        where film.IsAnyImageLoaded()
                        select film).ToList();
            }

        }

        public Size ConvertFilmSizeFrom(string filmSize, int DPI)
        // Config:  <FilmSizeID>8INX10IN\10INX12IN\10INX14IN\11INX14IN\14INX14IN\14INX17IN\24CMX24CM\24CMX30CM</FilmSizeID>
        {
            try
            {
                Logger.LogFuncUp();
                //split by 'X'
                string[] sParameters = filmSize.ToUpper().Split('X');
                if (sParameters.Length != 2) throw new Exception(filmSize); //log: wrong string

                //remove unit
                string sWidth = sParameters[0];
                string sHeight = sParameters[1];
                double width, height;
                if (sWidth.EndsWith("IN") && sHeight.EndsWith("IN"))
                {
                    width = Convert.ToInt32(sWidth.TrimEnd('I', 'N'));
                    height = Convert.ToInt32(sHeight.TrimEnd('I', 'N'));
                    return ConvertFilmSizeFromInchSize(width, height, DPI);
                }
                if (sWidth.EndsWith("CM") && sHeight.EndsWith("CM")) // convert unit from cm to inch
                {
                    width = (Convert.ToInt32(sWidth.TrimEnd('C', 'M')) * 0.3937); //1cm = 0.3937inch
                    height = (Convert.ToInt32(sHeight.TrimEnd('C', 'M')) * 0.3937);
                    return ConvertFilmSizeFromInchSize(width, height, DPI);
                }

                Logger.LogFuncDown();

                return ConvertSizeFromPaperSize(filmSize, DPI);
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Film Size format is wrong : (" + ex.StackTrace + ")");
                return ConvertSizeFromPaperSize(filmSize, DPI);
            }
        }

        private Size ConvertFilmSizeFromInchSize(double width, double height, int DPI)
        {
            //multiple with DPI
            FilmOrientationEnum filmOrientation = _currentFilmOrientation;
            //(FilmOrientationEnum)(PrinterSettingDialog.DataViewModal.CurrentFilmOrientation);
            switch (filmOrientation)
            {
                case FilmOrientationEnum.Landscape:
                    return new Size(height * DPI, width * DPI);
                default:
                    return new Size(width * DPI, height * DPI);
            }
        }

        private Size ConvertFilmSizeFromCmSize(double width, double height, int DPI)
        {
            return ConvertFilmSizeFromInchSize(width * 0.3937, height * 0.3937, DPI);//1cm = 0.3937inch
        }

        private Size ConvertSizeFromPaperSize(string paperSize, int DPI)
        {
            switch (paperSize.ToUpper())
            {
                case "ISOA3":
                    return ConvertFilmSizeFromCmSize(29.7, 42, DPI);
                default:    // "ISOA4"
                    return ConvertFilmSizeFromCmSize(21, 29.7, DPI);
            }
        }

        private void GetPatientInfo()
        {

            try
            {
                Logger.LogFuncUp();

                IList<FilmingPageControl> filmsNotEmpty = FilmsNotEmpty;
                IList<ImageInfo> imageInfoList = new List<ImageInfo>();
                foreach (var film in filmsNotEmpty)
                {
                    foreach (var cell in film.Cells)
                    {
                        if (cell != null && cell.Image != null && cell.Image.CurrentPage != null &&
                            cell.Image.CurrentPage.ImageHeader != null &&
                            cell.Image.CurrentPage.ImageHeader.DicomHeader != null)
                        {
                            imageInfoList.Add(new ImageInfo(cell));
                        }
                    }
                }

                //distinct image info from same study
                IList<ImageInfo> studyImageInfos = imageInfoList.Distinct(new StudyInstanceUIDComparer()).ToList();
                if (studyImageInfos == null) return;

                //distinct image info from same patient
                IList<ImageInfo> patientImageInfos = studyImageInfos.Distinct(new PatientComparer()).ToList();
                if (patientImageInfos == null) return;


                var patient = _jobCreator.Patient;

                patient.PatientID = string.Join(";", patientImageInfos.Select((image) => image.PatientID));
                patient.PatientName = string.Join(";", patientImageInfos.Select((image) => image.PatientName));
                patient.StudyID = string.Join(";", studyImageInfos.Select((image) => image.StudyID));
                //study id need not be distincted

                patient.PatientAge = string.Join(";", studyImageInfos.Select((image) => image.Age));
                patient.PatientSex = string.Join(";", patientImageInfos.Select((image) => image.Sex));

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }


        }

        ///<summary>
        ///Traverse all film pages for screen copy
        ///</summary>
        private void TraverseFilmPages()
        {

            //int filmingPageCount = EntityFilmingPageList.Count / SelectedFilmCardDisplayMode;
            //for (int index = 0; index < filmingPageCount; index++)
            //{
            //    GotoFilmBoardWithIndex(index);
            //    filmPageGrid.UpdateLayout();
            //}


        }

        private void InitialAndSetFilmSetting()
        {
            try
            {
                Logger.LogFuncUp();

                _jobCreator = new JobCreator();
                _jobCreator.ArchivedSeriesInstanceUid = CellSaveHelper.SeriesInstanceUid;

                //Get setting fromPrinterSetting
                PrinterSettingDataViewModal setting =PrinterSetting.DataViewModal;
                //set patient
                GetPatientInfo();
                //set printer
                Printers printers = Printers.Instance;
                PeerNode peer = _jobCreator.Peer;
                printers.GetPacsNodeParametersByAE(setting.CurrentPrinterAE, ref peer);
                _jobCreator.Peer = peer;

                //set printSetting
                PrintSetting printSetting = _jobCreator.PrintSetting;
                printSetting.Copies = setting.CurrentCopyCount;
                printSetting.Priority = PRINT_PRIORITY.MEDIUM;
                printSetting.FilmingDateTime = DateTime.Now;
                var filmingCardModality = Card.FilmingCardModality;
                printSetting.IfSaveElectronicFilm = Printers.Instance.IfSaveEFilmWhenFilming &&
                                                    filmingCardModality != FilmingUtility.EFilmModality;
                printSetting.MediaType = (string)setting.CurrentMediumType;
                printSetting.FilmDestination = (string)setting.CurrentFilmDestination;
                printSetting.IfColorPrint = setting.IfColorPrint;
                if (filmingCardModality == FilmingUtility.EFilmModality)
                {
                    var film = Card.EntityFilmingPageList.First();
                    var cell = film.Cells.First();
                    var displayData = cell.Image.CurrentPage;

                    printSetting.IfColorPrint = displayData.SamplesPerPixel == 3;  //彩色电子胶片
                }
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }

        }

        private DicomAttributeCollection AddFilmInfo(FilmingPageControl filmPage)
        {
            DicomAttributeCollection dataHeader = null;
            try
            {
                Logger.LogFuncUp();

                //Create FilmInfo For film page
                var filmBox = new FilmingPrintJob.Types.FilmBox.Builder();
                string filmSize;
                if (PrinterSetting.DataViewModal.CurrentFilmSize == null)
                {
                    filmSize = string.Empty;
                }
                else
                {
                    filmSize =PrinterSetting.DataViewModal.CurrentFilmSize.ToString();
                }
                filmBox.SetFilmSize(filmSize); //add to filmbox common properties?
                filmBox.SetOrientation(
                    (FilmingPrintJob.Types.Orientation)PrinterSetting.DataViewModal.CurrentFilmOrientation);

                //bool ifSaveEFilm = Printers.Instance.IfSaveEFilmsAvailable && Printers.Instance.IfSaveEFilmWhenFilming && FilmingCardModality != FilmingUtility.EFilmModality;
                var dpi = Printers.Instance.GetMaxDensityOf(PrinterSetting.DataViewModal.CurrentPrinterAE);
                dataHeader = filmPage.CreateFilmInfo(ref filmBox, CanSaveFilm, ConvertFilmSizeFrom(filmSize, dpi),
                                        ConvertFilmSizeFrom(filmSize, FilmingUtility.ScreenDPI), Card._filmingCardModality == FilmingUtility.EFilmModality);
                Logger.LogInfo("create film succeed");
                //Add filmBox to FilmingPrintJob
                _jobCreator.FilmBoxList.Add(filmBox);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
            return dataHeader;
        }

        #endregion [--Private Printing Helper Methods--]

        public void SetWindowLevelsForSingleSeriesComparePrint()
        {
            try
            {
                Logger.LogFuncUp();

                //get film
                int filmCount = Card.cellWindowLevelsViewModel.FilmCount;

                WindowLevel wl1 = new WindowLevel(Card.cellWindowLevelsViewModel.Window1Center,
                                                  Card.cellWindowLevelsViewModel.Window1Width);
                WindowLevel wl2 = new WindowLevel(Card.cellWindowLevelsViewModel.Window2Center,
                                                  Card.cellWindowLevelsViewModel.Window2Width);
                DateTime t1 = DateTime.Now;
                DateTime t2 = t1.AddSeconds(1);
                for (int i = 0; i < filmCount; i++)
                {
                    var film = Card.EntityFilmingPageList[Card.EntityFilmingPageList.Count - filmCount + i];
                    IList<MedViewerControlCell> cells =
                        new List<MedViewerControlCell>(
                            film.filmingViewerControl.Cells.ToList().Where(c => !c.IsEmpty).ToList());
                    int cellCount = cells.Count;
                    switch ((CompareStyleEnum)Card.cellWindowLevelsViewModel.CurrentCompareStyle)
                    {
                        case CompareStyleEnum.Horizontal:
                            int step = (int)Card.cellWindowLevelsViewModel.CurrentColumn;
                            for (int cellIndex = 0; cellIndex < cellCount; )
                            {
                                int cellRange = cellCount - cellIndex >= 2 * step ? step : (cellCount - cellIndex) / 2;
                                //if there has 2 entire row of cells contains image
                                int columnIndex = 0;
                                for (columnIndex = 0; columnIndex < cellRange; columnIndex++)
                                {
                                    Card.SetWindowLevel(cells[cellIndex], wl1);
                                    SetDisplayDataTimeStamp(cells[cellIndex], t1);
                                    cellIndex++;
                                }
                                for (columnIndex = 0; columnIndex < cellRange; columnIndex++)
                                {
                                    Card.SetWindowLevel(cells[cellIndex], wl2);
                                    SetDisplayDataTimeStamp(cells[cellIndex], t2);
                                    cellIndex++;
                                }
                                //insert empty cells For last comparing row
                                if (cellRange < step)
                                {
                                    int emptycellIndex = cellIndex - cellRange;
                                    //for (columnIndex = cellRange; columnIndex < step; columnIndex++)
                                    //{
                                    //    insertEmptyCell(film, emptycellIndex++);
                                    //}
                                    if (Card.studyTreeCtrl.InsertEmptyCells(film, emptycellIndex, step - cellRange))
                                        film.Cells.ToList().GetRange(emptycellIndex + step - cellRange, cellRange).ForEach(c => c.IsSelected = true);
                                }
                            }
                            break;
                        case CompareStyleEnum.Vertical:
                            for (int cellIndex = 0; cellIndex < cellCount; )
                            {
                                Card.SetWindowLevel(cells[cellIndex], wl1);
                                SetDisplayDataTimeStamp(cells[cellIndex], t1);
                                cellIndex++;
                                Card.SetWindowLevel(cells[cellIndex], wl2);
                                SetDisplayDataTimeStamp(cells[cellIndex], t2);
                                cellIndex++;
                            }
                            break;
                        default:
                            throw new Exception("non-supported compare style of single series compare print");
                    }
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
            finally
            {
                Card.cellWindowLevelsViewModel = null;
            }
        }

        private void SetDisplayDataTimeStamp(MedViewerControlCell cell, DateTime t)
        {
            try
            {
                //RefreshImage
                var cellImage = cell.Image;
                if (cellImage == null)
                {
                    return;
                }
                var currentPage = cellImage.CurrentPage;
                if (currentPage == null)
                {
                    return;
                }
                currentPage.UserSpecialInfo = t.ToString("yyyy-MM-dd-HH-mm-ss-ffffff");
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        public void UpdateBtnState()
        {
            printAllButton.IsEnabled = IsEnablePrintAllFilm;
            saveEFilmButton.IsEnabled = IsEnableSaveElectronicFilm;
        }


    }
}
