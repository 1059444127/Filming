using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Windows.Controls;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Filming.Model;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.MHC;
using UIH.Mcsf.Service;
using UIH.Mcsf.Viewer;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.AppControls.Viewer;

namespace UIH.Mcsf.Filming.Views
{
    /// <summary>
    /// Interaction logic for FilmingCard_BtnEdit.xaml
    /// </summary>
    public partial class FilmingCardBtnEdit : INotifyPropertyChanged
    {
        private FilmingCard Card { get; set; }
        private InterleavedDeleteWindow _interleavedDeleteWindow;
        private Dictionary<string, ImageSource> EnhanceIconDic = new Dictionary<string, ImageSource>();

        public DecimalUpDown scaleTextBox;

        
        public string NavTabName
        {
            get { return (string)GetValue(NavTabNameyProperty); }
            set { SetValue(NavTabNameyProperty, value); }
        }
        
        public static readonly DependencyProperty NavTabNameyProperty =
            DependencyProperty.Register("NavTabName", typeof(string), typeof(FilmingCardBtnEdit), new PropertyMetadata(""));

        public FilmingCardBtnEdit(FilmingCard card)
        {
            Card = card;
            _interleavedDeleteWindow = new InterleavedDeleteWindow();
            InitializeComponent();
            InitCtrl();
            InitEnhancementIcon();
            this.filmingButtonGrid2.DataContext = this;
            this.Loaded += new RoutedEventHandler(FilmingCardBtnEdit_Loaded);
        }

        void FilmingCardBtnEdit_Loaded(object sender, RoutedEventArgs e)
        {
            NavTabName = Card.TryFindResource("UID_Filming_ChangeNavTabPrint") as string;
        }

        private void InitCtrl()
        {

            scaleTextBox = new DecimalUpDown();
            scaleTextBox.Height = 35;
            scaleTextBox.Width = 105;
            scaleTextBox.Minimum = new decimal(0.1);
            scaleTextBox.Maximum = 15;
            scaleTextBox.Increment = new decimal(0.1);
            scaleTextBox.GotFocus += scaleTextBox_GotFocus;
            scaleTextBox.LostFocus += scaleTextBox_LostFocus;
            // scaleTextBox.PreviewMouseMove += scaleTextBox_PreviewMouseDown;
            scaleTextBox.KeyDown += scaleTextBox_KeyDown;

            var style = Card.TryFindResource("Style_DecimalUpDown_Common_CSW_Default") as Style;
            scaleTextBox.Style = style;
            AutomationProperties.SetAutomationId(scaleTextBox, "ID_TXT_FILMING_ZOOM");

            editButtonGrid1.Children.Add(scaleTextBox);
           // Grid.SetRow(scaleTextBox, 3);
            Grid.SetColumn(scaleTextBox, 6);
            scaleTextBox.ValueChanged += ScaleTextBoxOnValueChanged;
        }

        #region Enhancement
        private void InitEnhancementIcon()
        {
            for (int i = 5; i >= -5; i--)
            {
                var iconKey = GetEnhanceIconKey(i.ToString());
                var iconKeyNotWhite = GetEnhanceIconKey(i.ToString(), false);
                var img = Card.FindResource(iconKey) as ImageSource;
                var imgNotWhite = Card.FindResource(iconKeyNotWhite) as ImageSource;
                EnhanceIconDic.Add(iconKey, img);
                EnhanceIconDic.Add(iconKeyNotWhite, imgNotWhite);

                var itemImg = new System.Windows.Controls.Image();
                itemImg.Source = img;
                itemImg.Tag = i.ToString();
                itemImg.ToolTip = i.ToString();
                itemImg.HorizontalAlignment = HorizontalAlignment.Left;
                itemImg.VerticalAlignment = VerticalAlignment.Top;
                itemImg.Margin = new Thickness(-7, 0, 0, 0);
                itemImg.Width = 35;
                itemImg.Height = 35;

                enhancementCombox.Items.Add(itemImg);
            }
        }

        private string GetEnhanceIconKey(string tagName, bool white = true)
        {
            string iconKey = string.Empty;
            switch (tagName)
            {
                case "5":
                    iconKey = "Sharp_5";
                    break;
                case "4":
                    iconKey = "Sharp_4";
                    break;
                case "3":
                    iconKey = "Sharp_3";
                    break;
                case "2":
                    iconKey = "Sharp_2";
                    break;
                case "1":
                    iconKey = "Sharp_1";
                    break;
                case "0":
                    iconKey = "Sharp_Smooth_0";
                    break;
                case "-1":
                    iconKey = "Smooth_1";
                    break;
                case "-2":
                    iconKey = "Smooth_2";
                    break;
                case "-3":
                    iconKey = "Smooth_3";
                    break;
                case "-4":
                    iconKey = "Smooth_4";
                    break;
                case "-5":
                    iconKey = "Smooth_5";
                    break;
            }
            if (white)
            {
                iconKey += "_White";
            }
            return iconKey;
        }

        private void OnEnhanceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                var enhanceCombobox = sender as ComboBox;
                if (enhanceCombobox != null)
                {
                    var item = enhanceCombobox.SelectedItem as System.Windows.Controls.Image;
                    if (item != null)
                    {
                        string tagName = item.Tag.ToString();
                        SetEnhancementOnSelectedCells(tagName);

                        var iconKey = GetEnhanceIconKey(tagName, false);
                        imgEnhance.Source = EnhanceIconDic[iconKey];
                        Card.UpdateButtonStatus();
                    }
                }
                else
                {
                    imgEnhance.Source = null;
                }
                e.Handled = true;
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void SetEnhancementOnSelectedCells(string tagName)
        {
            if (null == tagName || string.IsNullOrEmpty(tagName))
            {
                return;
            }

            foreach (var cell in Card.ActiveFilmingPageList.Where(p => p.Visibility == Visibility.Visible).SelectMany(page => page.SelectedCells()).Where(c => c != null && !c.IsEmpty))
            {
                FilmingActionDeal.SetEnhancementCell(cell, tagName, true);
            }
            foreach (var cell in Card.ActiveFilmingPageList.Where(p => p.Visibility != Visibility.Visible).SelectMany(page => page.SelectedCells()).Where(c => c != null && !c.IsEmpty))
            {
                FilmingActionDeal.SetEnhancementCell(cell, tagName, false);
            }
            if (Card.IfZoomWindowShowState)
            {
                FilmingActionDeal.SetEnhancementCell(Card.ZoomViewer.DisplayedZoomCell, tagName, true);
            }
        }

        public bool IsEnableEnhancement
        {
            get
            {
                var isCtorMgImages =
                    Card.ActiveFilmingPageList.Any(page => page.SelectedCells().Any(cell =>
                            cell != null && cell.Image != null
                            && cell.Image.CurrentPage != null
                            && (cell.Image.CurrentPage.Modality == Modality.CT
                            || cell.Image.CurrentPage.Modality == Modality.MG)));

                return isCtorMgImages;
            }
        }

        public void UpdateEnhanceSelectedStatus()
        {
            var cell = Card.LastSelectedCell;
            if (null != cell
                && cell.IsSelected
                && null != cell.Image
                && null != cell.Image.CurrentPage
                && (Modality.CT == cell.Image.CurrentPage.Modality
                || Modality.MG == cell.Image.CurrentPage.Modality))
            {
                double enhancePara = cell.Image.CurrentPage.PState.EnhancePara;
                foreach (var item in enhancementCombox.Items)
                {
                    var comboBoxItem = item as System.Windows.Controls.Image;
                    if (comboBoxItem != null && comboBoxItem.Tag.ToString() == enhancePara.ToString())
                    {
                        enhancementCombox.SelectedItem = comboBoxItem;
                    }
                }
            }
        }
        #endregion Enhancement
        #region [--zoom editor--]

        // ReSharper disable InconsistentNaming
        private void scaleTextBox_GotFocus(object sender, RoutedEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            //scaleTextBox.SelectAll();
            scaleTextBox.PreviewMouseDown -= scaleTextBox_PreviewMouseDown;
            Card.ClearPtInputBindings();
        }

        private void scaleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            scaleTextBox.PreviewMouseDown += scaleTextBox_PreviewMouseDown;

            if (Card.CurrentActionType != ActionType.AnnotationText && Card.CurrentActionType != ActionType.AnnotationArrow)
            {
                Card.RecoverPtInputBindings();
            }
        }


        private void scaleTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //scaleTextBox.Focus();
            //e.Handled = true;
        }

        // ReSharper disable InconsistentNaming
        private void scaleTextBox_KeyDown(object sender, KeyEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            if (e.Key != Key.Enter) return;
            double scale;
            if (double.TryParse(scaleTextBox.Text, out scale))
            {
                //key ssfs: 452651
                if (scale < 0.1) scale = 0.1;
                if (scale > 15) scale = 15;
                scaleTextBox.Text = scale.ToString("0.00");
                ZoomSelectedCells(scale);
            }

        }

        private void ZoomSelectedCells(double scale)
        {
            //todo: performance optimization Refresh begin
            foreach (var cell in Card.ActiveFilmingPageList.Where(p => p.Visibility == Visibility.Visible).SelectMany(page => page.SelectedCells()).Where(c => c != null && !c.IsEmpty))
            {
                FilmingActionDeal.ZoomCell(cell, scale, scale, true);
                //cell.Refresh(CellRefreshType.ImageText);
            }
            foreach (var cell in Card.ActiveFilmingPageList.Where(p => p.Visibility != Visibility.Visible).SelectMany(page => page.SelectedCells()).Where(c => c != null && !c.IsEmpty))
            {
                FilmingActionDeal.ZoomCell(cell, scale, scale, false);
            }
            if (Card.IfZoomWindowShowState)
            {
                FilmingActionDeal.ZoomCell(Card.ZoomViewer.DisplayedZoomCell, scale, scale, true);  
            }
            //todo: performance optimization end
        }

        public void GetScaleOfSelectedCells()
        {
            //Logger.LogTimeStamp("开始GetScaleOfSelectedCells");
            if (scaleTextBox == null) return;
            string scale = string.Empty;

            var selectedCells = Card.ActiveFilmingPageList.SelectMany(page => page.SelectedCells()).ToList();
            if (selectedCells.Any(c => c != null && !c.IsEmpty))
            {
                var scales =
                    selectedCells.Where(c => c != null && !c.IsEmpty && c.Image.CurrentPage != null).
                        Select(c => Math.Ceiling(c.Image.CurrentPage.PState.ScaleX * 1000000) / 1000000).Distinct().ToList();
                scale = (scales.Count() != 1) ? string.Empty : scales.ElementAt(0).ToString("0.00");
            }

            if (scaleTextBox.Text != scale)
            {
                _byUpdateUI = true;
                scaleTextBox.Text = scale;
                _byUpdateUI = false;
            }

            scaleTextBox.AllowSpin = !string.IsNullOrWhiteSpace(scale);

            //Logger.LogTimeStamp("结束GetScaleOfSelectedCells");
        }

        private bool _byUpdateUI;
        private void ScaleTextBoxOnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            double scale;
            if (double.TryParse(scaleTextBox.Text, out scale) && !_byUpdateUI)
            {
                if (scale < 0.1) scale = 0.1;
                if (scale > 15) scale = 15;
                ZoomSelectedCells(scale);
                scaleTextBox.AllowSpin = !string.IsNullOrWhiteSpace(scaleTextBox.Text);
            }

        }

        #endregion //[--zoom editor--]

        private void OnHideAnnotationButtonClick(object sender, RoutedEventArgs e)
        {
            var imageTextDisplayMode = Card.ImageTextDisplayMode == ImgTxtDisplayState.Customization ? ImgTxtDisplayState.None : ImgTxtDisplayState.Customization;
            UpdateCornerText(imageTextDisplayMode);
            Card.ImageTextDisplayMode = imageTextDisplayMode;
        }


        public void UpdateCornerText(ImgTxtDisplayState type)
        {
            if (Card.ImageTextDisplayMode == type)
            {
                return;
            }

            Card.ImageTextDisplayMode = type;

            foreach (var filmingPageControl in Card.EntityFilmingPageList)
            {
                filmingPageControl.IsBeenRendered = filmingPageControl.IsVisible;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                                              new Action<object>((object para) =>
                                                              {
                                                                  var f = para as FilmingPageControl;
                                                                  if (null != f)
                                                                  {
                                                                      f.UpdateCornerText(type, f.IsVisible);
                                                                  }
                                                              }), filmingPageControl);

            }
            if (Card.IfZoomWindowShowState)
            {
                if(Card.ZoomViewer.DisplayedZoomCell != null)
                    FilmingHelper.UpdateCornerTextDisplayData(Card.ZoomViewer.DisplayedZoomCell.Image.CurrentPage, type);
            }
            this.Dispatcher.Invoke(new Action(() => { }));
        }

        public void UpdateCornerTextForSelectedCells(ImgTxtDisplayState type)
        {
            foreach (var filmingPageControl in Card.EntityFilmingPageList)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Input,
                                                              new Action<object>((object para) =>
                                                            {
                                                                var f = para as FilmingPageControl;
                                                                if (null != f)
                                                                {
                                                                    f.UpdateCornerTextOfSelectedCells(type, f.IsVisible);
                                                                }
                                                            }), filmingPageControl);
            }
            if (Card.IfZoomWindowShowState)
            {
                if (Card.ZoomViewer.DisplayedZoomCell != null)
                    FilmingHelper.UpdateCornerTextDisplayData(Card.ZoomViewer.DisplayedZoomCell.Image.CurrentPage, type);
            }
            this.Dispatcher.Invoke(new Action(() => { }));
        }

        public bool IsEnableSelectSeries
        {
            get
            {
                if (Card.IsCellModalitySC) return false;
                if (Card.ActiveFilmingPageList.Count == 0) return false;
                bool isEnabled =
                    Card.ActiveFilmingPageList.Any(film => film.Cells.Any(cell => cell.IsSelected && !cell.IsEmpty));
                return isEnabled;
            }
        }

        public void OnSelectSeries(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag);
                if (Card.IfZoomWindowShowState)
                {
                    Card.ZoomViewer.CloseDialog();
                }
                if (Card.CurrentActionType != ActionType.Pan && Card.CurrentActionType != ActionType.Zoom)  //zoom和pan状态forceendaction非常慢
                {
                    foreach (var film in Card.ActiveFilmingPageList)
                    {
                        foreach (var cell in film.SelectedCells())
                        {
                            cell.ForceEndAction();
                        }
                    }
                }

                var seiresUIDList = FilmPageUtil.GetSelectedSeriesUid(Card.ActiveFilmingPageList);
                SelectSeries(seiresUIDList);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        #region series selection

        public void SelectSeries(List<string> seriesUIDList)
        {
            try
            {
                Logger.LogFuncUp();

                if (seriesUIDList == null || !seriesUIDList.Any())
                {
                    Logger.LogError("Can not select series. SeriesUID is null!");
                    return;
                }

                var selectedCells = Card.CollectSelectedCells();

                Card.ActiveFilmingPageList.UnSelectAllCells();

                foreach (var page in Card.EntityFilmingPageList)
                {
                    foreach (var cell in page.Cells)
                    {
                        if (cell.Image == null)
                            continue;

                        var seriesUID = FilmPageUtil.GetSeriesUidFromImage(cell.Image);
                        if (!string.IsNullOrEmpty(seriesUID) && seriesUIDList.Contains(seriesUID))
                        {
                            if (selectedCells.Any(selectedCell =>
                                FilmPageUtil.GetSeriesUidFromImage(selectedCell.Image) == seriesUID &&
                                selectedCell.Image.CurrentPage.UserSpecialInfo == cell.Image.CurrentPage.UserSpecialInfo))
                            {
                                cell.IsSelected = true;

                                var viewport = page.ViewportOfCell(cell);
                                if (viewport != null && !viewport.IsSelected)
                                {
                                    viewport.IsSelected = true;
                                }

                                if (!page.IsSelected)
                                {
                                    page.IsSelected = true;
                                }

                                Card.LastSelectedCell = cell;
                                Card.LastSelectedViewport = viewport;
                                Card.LastSelectedFilmingPage = page;
                            }
                        }
                    }
                }

                Card.UpdateUIStatus();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        #endregion

        public void OnSelectAllFilmPages(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Card.IfZoomWindowShowState)
                {
                    Card.ZoomViewer.CloseDialog();
                }
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag);
                if (FilmingCard._miniCellsList != null && FilmingCard._miniCellsList.Count > 0)
                    FilmingCard.GroupUnselectedForSingleUseCell(FilmingCard._miniCellsList[0]);
                foreach (var filmingPage in Card.EntityFilmingPageList)
                {
                    FilmPageUtil.SelectAllOfFilmingPage(filmingPage, true);
                }

                Card.LastSelectedFilmingPage = Card.ActiveFilmingPageList.LastOrDefault();
                Card.SelectLastCellOrFirstViewport(Card.LastSelectedFilmingPage);

                Card.UpdateUIStatus();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void OnDeleteActiveFilmPages(object sender, RoutedEventArgs e)
        {

            var cellCount = Card.ActiveFilmingPageList.Aggregate<FilmingPageControl, uint>(0, (current, page) => current + (uint)page.Cells.Count(c => c != null && !c.IsEmpty));
            if (cellCount == 0)
                Card.commands.DeleteActiveFilmPages();
            else
                Card.commands.WarnWhenDeleteImages(Card.commands.DeleteActiveFilmPages);


        }

        /// <summary>
        /// obey SSFS key: 103282
        /// </summary>
        public void OnDeleteAllFilmPages(object sender, RoutedEventArgs e)
        {
            var cellCount = (uint)Card.GetTotalImageCount();

            Card.filmingViewerControl_ResetGraphicStatisticMenu();

            if (cellCount == 0)
                Card.commands.DeleteAllFilmPages();
            else
                Card.commands.WarnWhenDeleteImages(Card.commands.DeleteAllFilmPages);

        }

        private void CopyExecuted(object sender, RoutedEventArgs e)
        {
            Card.commands.CopyImages();
            e.Handled = true;
        }
        private void CutExecuted(object sender, RoutedEventArgs e)
        {
            Card.commands.CutImages();
            e.Handled = true;
        }
        private void PasteExecuted(object sender, RoutedEventArgs e)
        {
            Card.commands.PasteImages();
            e.Handled = true;
        }

        private void InsertRefImageExecuted(object sender, RoutedEventArgs e)
        {
            InsertRefImage();
        }


        public ImgTxtPosEnum GetLocalizedImageLastAddedPosition()
        {
            var config = Printers.Instance.LocalizedImagePosition;
            if (config == 2) return ImgTxtPosEnum.BottomRight;
            if (config == 1) return ImgTxtPosEnum.TopRight;
            if (config == 3) return ImgTxtPosEnum.BottomLeft;
            if (config == 0) return ImgTxtPosEnum.TopLeft;
            return ImgTxtPosEnum.BottomRight;
        }       

        public void AddLocalImage(DisplayData refDisplayData, Sop sop)
        {
            try
            {
                var refStudyUid = refDisplayData.ImageHeader.DicomHeader[ServiceTagName.StudyInstanceUID];
                var refSeriesUid = refDisplayData.ImageHeader.DicomHeader[ServiceTagName.SeriesInstanceUID];

                var cellDisplayData = Card._dropViewCell.Image.CurrentPage;
                var cellStudyUid = cellDisplayData.ImageHeader.DicomHeader[ServiceTagName.StudyInstanceUID];
                var cellSeriesUid = cellDisplayData.ImageHeader.DicomHeader[ServiceTagName.SeriesInstanceUID];
                var canCalculateReferenceLine = false;
                if (refStudyUid == cellStudyUid && refSeriesUid != cellSeriesUid)
                {
                    canCalculateReferenceLine = OverlayLocalizedImageHelper.Instance.CanCalculateReferenceLine(cellDisplayData, refDisplayData);
                    OverlayLocalizedImageHelper.Instance.AddedPosition = GetLocalizedImageLastAddedPosition();
                    if (canCalculateReferenceLine)
                    {
                        foreach (var page in Card.ActiveFilmingPageList)
                        {
                            foreach (var cell in page.Cells)
                            {
                                if (!cell.IsEmpty && cell.IsSelected)
                                {
                                    var displayData = cell.Image.CurrentPage;
                                    var refDisplayDataTemp = Card.CreateDisplayDataBy(sop);
                                    if (refDisplayDataTemp != null)
                                    {
                                        ImagePatientInfo ipi = new ImagePatientInfo();
                                        ipi.AppendPatientInfo(refDisplayDataTemp);
                                    }

                                    refDisplayDataTemp.DicomHeader = sop.DicomSource.Clone();

                                    var studyUidTemp = displayData.ImageHeader.DicomHeader[ServiceTagName.StudyInstanceUID];
                                    var seriesUidTemp = displayData.ImageHeader.DicomHeader[ServiceTagName.SeriesInstanceUID];
                                    var canCalculateReferenceLineTemp = OverlayLocalizedImageHelper.Instance.CanCalculateReferenceLine(displayData, refDisplayDataTemp);
                                  //  if (refStudyUid == studyUidTemp && cellSeriesUid == seriesUidTemp && refSeriesUid != seriesUidTemp && canCalculateReferenceLineTemp)
                                    if (refStudyUid == studyUidTemp && refSeriesUid != seriesUidTemp && canCalculateReferenceLineTemp)
                                    {

                                        var dispacherPriority = page.IsVisible ? DispatcherPriority.Render : DispatcherPriority.Background;

                                        this.Dispatcher.BeginInvoke(dispacherPriority,
                                                                                new Action<DisplayData, DisplayData>(
                                                                                    (data, dataTemp) => OverlayLocalizedImageHelper.Instance.ChangeRefImage(data, dataTemp)),
                                                                                displayData, refDisplayDataTemp);
                                    }

                                }
                            }
                        }
                    }
                }

                if (!canCalculateReferenceLine)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        MessageBoxHandler.Instance.ShowWarning("UID_Filming_Warning_FailedReplaceRefImage");
                    }));
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException("AddLocalImage::" + ex.Message + ex.StackTrace);
            }
        }

        //public bool IsCheckInsertRefImage(List<DisplayData> displayDataList)
        //{
        //    bool isDifferentSeries = false;
        //    if (displayDataList.Count > 1)
        //    {
                
        //        var seriesUID = displayDataList[0].ImageHeader.DicomHeader[ServiceTagName.SeriesInstanceUID];
        //        foreach (DisplayData data in displayDataList)
        //        {
        //            var seriesUIDTemp = data.ImageHeader.DicomHeader[ServiceTagName.SeriesInstanceUID];
        //            if (seriesUIDTemp != seriesUID)
        //            {
        //                isDifferentSeries = true;
        //                break;
        //            }

        //        }                
        //    }

        //    return isDifferentSeries;
        //}

        public void InsertRefImage(string type="auto",DisplayData refDisplayData=null)
        {
            Logger.Instance.LogPerformanceRecord("[Begin][InsertRefImage]");
            if (Card.IfZoomWindowShowState)
                Card.ZoomViewer.CloseDialog();
            var displayDataList = (from cell in Card.CollectSelectedCells() where cell != null && cell.Image != null && cell.Image.CurrentPage != null select cell.Image.CurrentPage).ToList();
           
            FilmingPageControl destPage;
            MedViewerControlCell destCell;
            if (!Card.CollectFirstSectedCell(out destPage, out destCell))
            {
                return;
            }
           // var isDifferentSeries = IsCheckInsertRefImage(displayDataList);
            if (!IsEnableInsertRefImage)
            {
                MessageBoxHandler.Instance.ShowWarning("UID_Filming_Warning_FailedManualAddRefImage");
                return;
            }
            string imgTxtConfigContent = "";
            if (null != displayDataList && displayDataList.Count > 0 && Printers.Instance.Modality2FilmingImageTextConfigContent.ContainsKey(displayDataList[0].Modality.ToString()))
            {
                imgTxtConfigContent =
                    Printers.Instance.Modality2FilmingImageTextConfigContent[displayDataList[0].Modality.ToString()];
            }

            var displayData = new DisplayData();

            if(type=="auto")
            {
                displayData = OverlayLocalizedImageHelper.Instance.GenerateImageLocalizedImage(displayDataList, imgTxtConfigContent);
            }
            else
            {
                displayData = OverlayLocalizedImageHelper.Instance.ManualGenerateImageLocalizedImage(displayDataList, refDisplayData, imgTxtConfigContent);
            }

            if (displayData == null)
            {
                MessageBoxHandler.Instance.ShowWarning("UID_Filming_Warning_FailedInsertRefImage");
                Keyboard.Focus(Card);
                destPage.Focus();
                Card.LastSelectedFilmingPage = destPage;
                Card.LastSelectedCell = destCell;
                return;
            }
            //添加filming需要特殊值
            ImagePatientInfo ipi = new ImagePatientInfo();
            ipi.AppendPatientInfo(displayData);
            displayData.UserSpecialInfo = DateTime.Now.ToString();
            var overlayFilmingF1ProcessText = displayData.GetOverlay(OverlayType.FilmingF1ProcessText) as OverlayFilmingF1ProcessText;
            if (null != overlayFilmingF1ProcessText)
            {
                overlayFilmingF1ProcessText.SetRulerDisplayMode(Card.commands.IfShowImageRuler);
            }

            var dpList = new List<DisplayData> { displayData };
            Card.ActiveFilmingPageList.UnSelectAllCells();
            Card.InsertCells(Card.CreateCellsByDisplayData(dpList), destPage, destCell);
            Card.LastSelectedFilmingPage = destPage;
            Card.LastSelectedCell = destCell;
            if (Card.IsEnableRepack)
            {
                Card.contextMenu.Repack(RepackMode.RepackPaste);
            }
            else
            {
                Card.RefreshAnnotationDisplayMode();
            }
            Card.UpdateFilmCardScrollBar();
            Card.ReOrderCurrentFilmPageBoard();
            Card.UpdateUIStatus();
            Card.UpdateImageCount();

            if (!Card.IsEnableRepack)
                Card.EntityFilmingPageList.ForEach((film) => film.RefereshPageTitle());
            Logger.Instance.LogPerformanceRecord("[End][InsertRefImage]");
        }

       

        private void InsertPageBreakClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                if (Card.IfZoomWindowShowState)
                {
                    Card.ZoomViewer.CloseDialog();
                }
                Card.commands.InsertPageBreak();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        #region Reverse Button

        private void OnReverseClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                if (Card.IfZoomWindowShowState)
                {
                    Card.ZoomViewer.CloseDialog();
                }
                //1. 备份选中cell
                var backupDisplayData = new List<DisplayData>();
                List<MedViewerControlCell> selectedCells = new List<MedViewerControlCell>();
                List<FilmingPageControl> sortedActivePageList =
                    Card.ActiveFilmingPageList.OrderBy(a => a.FilmPageIndex).ToList();
                foreach (var filmingPage in sortedActivePageList)
                {
                    selectedCells.AddRange(filmingPage.SelectedCells().OrderBy(a => a.CellIndex).ToList());
                }

                foreach (var cell in selectedCells)
                {
                    if (!cell.IsEmpty)
                        backupDisplayData.Add(cell.Image.CurrentPage);
                    else
                    {
                        backupDisplayData.Add(null);
                    }
                    // Make the cut cell empty
                    cell.Image.Clear();
                    cell.Refresh(CellRefreshType.ImageText);
                }

                //3. 逆序
                backupDisplayData.Reverse();

                //4.refresh cell
                //var film = EntityFilmingPageList.FirstOrDefault();
                for (int i = 0; i < selectedCells.Count; i++)
                {
                    var cell = selectedCells[i];
                    var displayData = backupDisplayData[i];
                    if (displayData != null)
                        cell.Image.AddPage(displayData);


                    cell.Refresh();
                    //if (displayData != null)
                    //    FilmingHelper.RefereshDisplayMode(displayData);
                }

                Card.ActiveFilmingPageList.UpdateAllPageTitle();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            finally
            {
                Card.actiontoolCtrl.RefreshAction(); //reset cursor 
            }
        }

        #endregion

        public void OnCompareSeriesFilmClick(object sender, RoutedEventArgs e)
        {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "Compare Series");
            Card.studyTreeCtrl.CompareFilm();
        }

        private void OnInterleavedDeleteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                var count = Card.filmingCard.ActiveFilmingPageList.Aggregate<FilmingPageControl, uint>(0,
                   (current, page) => current + (uint)page.SelectedCells().Count(c => c != null && !c.IsEmpty));

                _interleavedDeleteWindow.ViewModel.TotalImages = count;
                _interleavedDeleteWindow.CheckNullParameter();
                var messageWindow = new MessageWindow
                {
                    WindowTitle = Card.Resources["UID_Filming_InterleavedDelete"] as string,
                    WindowChild = _interleavedDeleteWindow,
                    WindowDisplayMode = WindowDisplayMode.Default
                };
                Card.HostAdornerCount++;
                messageWindow.ShowModelDialog();
                Card.HostAdornerCount = 0;
                if (_interleavedDeleteWindow.IsQuit == true)
                    FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message);
            }
        }


        private void OnMultiFormatBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                //not allow multi format for different people.
                string studyIDOfCell = string.Empty;
                foreach (var filmingPageControl in Card.ActiveFilmingPageList)
                {
                    foreach (var cell in filmingPageControl.SelectedCells())
                    {
                        if (studyIDOfCell == string.Empty)
                        {
                            studyIDOfCell = cell.StudyIdOfCell();
                        }
                        else
                        {
                            if (cell.StudyIdOfCell() != string.Empty && !studyIDOfCell.Equals(cell.StudyIdOfCell()))
                            {
                                MessageBoxHandler.Instance.ShowWarning("UID_Filming_Warning_ForbidMultiFormat");
                                return;
                            }
                        }
                    }
                }
                WindowHostManagerWrapper.ShowSecondaryWindow(Card.MultiFormatLayoutWindow,
                                                             Card.Resources["UID_Filming_MultiFormat_Title"] as string);

                Logger.LogFuncDown();

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void UpdateBtnState()
        {
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableInsertPageBreak"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableHideAnnotation"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableCutImage"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableCopyImage"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnablePasteImage"));

            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableCompareFilm"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableSelectAllFilm"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableSelectSeries"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableDeleteAllFilm"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableDeleteActiveFilm"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableReverseArrangement"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableInterleavedDelete"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableMultiFormat"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableInsertRefImage"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableEnhancement"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsLocalizedImageReferenceLineBtnEnabled"));
            OnPropertyChanged(new PropertyChangedEventArgs("IsEnableCombinePrint"));
            //scaleTextBox.IsEnabled =
            //    ActiveFilmingPageList.SelectMany(page => page.SelectedCells()).Any(c => c != null && !c.IsEmpty);
        }


        public bool IsEnableCompareFilm
        {
            get { return Card.studyTreeCtrl.IsEnableCompareFilm; }
        }


        public bool IsEnableCutImage
        {
            get { return Card.commands.IsEnableCutImage; }
        }

        public bool IsEnableCopyImage
        {
            get { return Card.commands.IsEnableCopyImage; }
        }

        public bool IsEnablePasteImage
        {
            get { return Card.commands.IsEnablePasteImage; }
        }

        public bool IsEnableHideAnnotation
        {
            get { return !Card.IsCellModalitySC; }
        }

        public bool IsEnableMultiFormat
        {
            get
            {
                if (Card.IsCellModalitySC) return false;
                if (Card.ActiveFilmingPageList.Any((page) => page.IsInSeriesCompareMode && page.SelectedCells().Any()))
                {
                    return false;
                }

                if (Card.ActiveFilmingPageList.All(page => page.SelectedCells().All(cell => cell.IsEmpty)))
                {
                    return false;
                }

                //can't apply MultiFormat operation to a multi format cell.
                if (
                    Card.ActiveFilmingPageList.Any(
                        page => page.SelectedCells().Any(cell => FilmPageUtil.IsMultiFormatCell(cell))))
                {
                    return false;
                }

                if (Card.ActiveFilmingPageList.Any(page => page.ViewportList.Count > 1))
                {
                    return false;
                }


                //bug ID:117769 allow multi format for single selection
                if (Card.ActiveFilmingPageList.Sum(filmPage => filmPage.SelectedCells().Count) == 1)
                {
                    return true;
                }

                var filterFilmPageList =
                    Card.ActiveFilmingPageList.Where(page => !page.IsEmpty() && page.SelectedCells().Count != 0).ToList();

                return FilmPageUtil.IsSelectedCellsSuccessional(filterFilmPageList);
            }
        }



        public bool IsEnableInterleavedDelete
        {
            get
            {
                if (Card.IsCellModalitySC) return false;
                //don't allow for compare film page
                if (Card.ActiveFilmingPageList.Any((page) => page.IsInSeriesCompareMode && page.SelectedCells().Any()))
                {
                    return false;
                }

                //bug ID: 119907: don't allow reverse cross viewport.
                if (Card.ActiveFilmingPageList.Any(page => page.ViewportLayout.LayoutType == LayoutTypeEnum.DefinedLayout))//page.ViewportList.Count > 1))
                {
                    return false;
                }
                List<MedViewerControlCell> selectedCells = new List<MedViewerControlCell>();
                List<FilmingPageControl> sortedActivePageList = Card.ActiveFilmingPageList.OrderBy(a => a.FilmPageIndex).ToList();
                foreach (var filmingPage in sortedActivePageList)
                {
                    selectedCells.AddRange(filmingPage.SelectedCells().OrderBy(a => a.CellIndex).ToList());
                }
                if (selectedCells.Count < 2) return false;
                if (selectedCells.FirstOrDefault().IsEmpty) return false;
                if (selectedCells.LastOrDefault().IsEmpty) return false;

                return FilmPageUtil.IsSelectedCellsSuccessional(Card.ActiveFilmingPageList);
            }
        }


        public bool IsEnableReverseArrangement
        {
            get
            {
                if (Card.IsCellModalitySC) return false;
                //don't allow for compare film page
                if (Card.ActiveFilmingPageList.Any((page) => page.IsInSeriesCompareMode && page.SelectedCells().Any()))
                {
                    return false;
                }

                //bug ID: 119907: don't allow reverse cross viewport.
                //if (Card.ActiveFilmingPageList.Any(page => page.ViewportLayout.LayoutType == LayoutTypeEnum.DefinedLayout))//page.ViewportList.Count > 1))
                //{
                //    return false;
                //}
                List<MedViewerControlCell> selectedCells = new List<MedViewerControlCell>();
                List<FilmingPageControl> sortedActivePageList = Card.ActiveFilmingPageList.OrderBy(a => a.FilmPageIndex).ToList();
                foreach (var filmingPage in sortedActivePageList)
                {
                    selectedCells.AddRange(filmingPage.SelectedCells().OrderBy(a => a.CellIndex).ToList());
                }
                if (selectedCells.Count == 0) return false;
                if (selectedCells.FirstOrDefault().IsEmpty) return false;
                if (selectedCells.LastOrDefault().IsEmpty) return false;

                //if pages from different region
                var pageBreak = sortedActivePageList.FindLast(film => film.FilmPageType == FilmPageType.BreakFilmPage);

                if (pageBreak != null && pageBreak != sortedActivePageList.First())
                {
                    return false;
                }

                return FilmPageUtil.IsSelectedCellsSuccessional(Card.ActiveFilmingPageList);
            }
        }

        public bool IsEnableDeleteActiveFilm
        {
            get { return Card.ActiveFilmingPageList.Any(); } // && ActiveFilmingPageList.Any(page => page.ViewportList.Any(viewport => viewport.IsSelected)); }
        }

        public bool IsEnableDeleteAllFilm
        {
            get { return Card.EntityFilmingPageList.Any(); }
        }

        public bool IsEnableSelectAllFilm
        {
            get
            {
                if (Card.IsCellModalitySC) return false;
                return Card.EntityFilmingPageList.Any();
            }
        }

        public bool IsEnableInsertRefImage
        {
            get
            {
                if (Card.IsCellModalitySC) return false;
                var uidandInfoList = new List<string>();
                foreach (var cell in Card.CollectSelectedCells())
                {
                    if (cell != null && cell.Image != null && cell.Image.CurrentPage != null)
                    {
                        var uid = cell.Image.CurrentPage.ImageHeader.DicomHeader[ServiceTagName.SeriesInstanceUID];
                        var userInfo = cell.Image.CurrentPage.UserSpecialInfo;
                        if (!uidandInfoList.Contains(uid + userInfo))
                        {
                            uidandInfoList.Add(uid + userInfo);
                            if (uidandInfoList.Count > 1) return false;
                        }
                    }
                }
                if (uidandInfoList.Count < 1) return false;              
                return true;
            }

        }

        /// <summary>
        /// obey SSFS key:101993
        /// </summary>
        public bool IsEnableInsertPageBreak
        {
            get
            {
                if (Card.IsCellModalitySC) return false;
                if (Card.ActiveFilmingPageList.Count != 1)
                {
                    return false;
                }

                var page = Card.ActiveFilmingPageList.First();
                if (page == null || /*page.ViewportList.Count > 1*/ page.ViewportLayout.LayoutType == LayoutTypeEnum.DefinedLayout || page.SelectedCells().Count != 1)
                {
                    return false;
                }

                if (page.IsInSeriesCompareMode)
                {
                    return false;
                }

                var lastCell = page.SelectedCells().LastOrDefault();
                int nextPageBreakIndex = Card.GetLinkedPageEndIndex(page);
                bool isLastPageOfRegion = Card.EntityFilmingPageList.IndexOf(page) == nextPageBreakIndex - 1;
                bool isLastCell = lastCell == page.Cells.LastOrDefault(cell => !cell.IsEmpty);

                if (lastCell == null || isLastCell && isLastPageOfRegion)
                {
                    return false;
                }
                int lastNonEmpty = page.Cells.ToList().FindLastIndex(cell => !cell.IsEmpty);
                if (lastNonEmpty < 0) return false;
                if (lastCell.CellIndex > lastNonEmpty) return false; //last empty cells disable insert break

                return true;
            }
        }

        public bool IsEnableCombinePrint
        {
            get { return Card.studyTreeCtrl.seriesSelectionCtrl.SelectedSeriesItems.Count == 1; }
        }

        public bool IsLocalizedImageReferenceLineBtnEnabled
        { 
            get
            {
                return Card.contextMenu.IsEnableAddLocalizedImageReferenceLine || Card.contextMenu.IsEnableDeleteLocalizedImageReferenceLine;
            }
        }

        private void referenceLineSnapButton_Click(object sender, RoutedEventArgs e)
        {
            var menu=Card.contextMenu;
            if (menu.IsEnableAddLocalizedImageReferenceLine)
            {
                menu.OnLocalizedImageReferenceLineChecked(null, null);
            }
            else{
                if (menu.IsEnableDeleteLocalizedImageReferenceLine)
                    menu.OnLocalizedImageReferenceLineUnChecked(null, null);
            }
        }

        private void combinePrintButton_Click(object sender, RoutedEventArgs e)
        {
            var item = Card.studyTreeCtrl.seriesSelectionCtrl.SelectedSeriesItems.FirstOrDefault();
            
            //int totalCount = item.ImageCount;
            if(item == null) return;
            var totalCount = item.ImageCount;

            Card.CombinePrint.ViewModel.TotalImages = (uint)totalCount;
            Card.CombinePrint.CheckNullParameter();
            Card.CombinePrint.CreateMemento();

            var messageWindow = new MessageWindow
            {
                WindowTitle = Card.Resources["UID_Filming_CombinePrint_Title"] as string,
                WindowChild = Card.CombinePrint,
                WindowDisplayMode = WindowDisplayMode.Default
            };
            Card.HostAdornerCount++;
            messageWindow.ShowModelDialog();
            Card.HostAdornerCount--;
            if (Card.CombinePrint.IsQuit == true)
            {
                FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
                Card.CombinePrint.RestoreMemento();
            }
        }

        private void changeToolTabButton_Click(object sender, RoutedEventArgs e)
        {
            if(!Card.PrintSetCtrl.IsVisible)
            {
                Card.PrintSetCtrl.Visibility = Visibility.Visible;
                Card.actiontoolCtrl.Visibility = Visibility.Collapsed;
                NavTabName = Card.TryFindResource("UID_Filming_ChangeNavTabAction") as string;
                Grid.SetRow(Card.PrintSetCtrl, 3);
                Grid.SetRow(Card.actiontoolCtrl, 5);
            }
            else
            {
                Card.PrintSetCtrl.Visibility = Visibility.Collapsed;
                Card.actiontoolCtrl.Visibility = Visibility.Visible;
                NavTabName = Card.TryFindResource("UID_Filming_ChangeNavTabPrint") as string;
                Grid.SetRow(Card.PrintSetCtrl, 5);
                Grid.SetRow(Card.actiontoolCtrl, 3);
            }
        }




    }
}
