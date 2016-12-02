using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using McsfCommonSave;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.DicomConvertor;
using UIH.Mcsf.Filming.Model;
using UIH.Mcsf.Filming.Wrappers;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.Pipeline.Data;
using UIH.Mcsf.Core;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Viewer;
using Mcsf;
using Color = System.Windows.Media.Color;
using System.Diagnostics;
using System.Windows.Automation;
using UIH.Mcsf.AppControls.Viewer;
using UIH.Mcsf.Filming.ImageManager;
using UIH.Mcsf.Filming.Utility;
using DICOM = UIH.Mcsf.Pipeline.Dictionary;
using WindowLevel = UIH.Mcsf.Viewer.WindowLevel;

namespace UIH.Mcsf.Filming
{

    /// <summary>
    /// Interaction logic for FilmingControl.xaml
    /// </summary>
    public partial class FilmingPageControl : UserControl
    {

        #region [--Need to be classified--]

        public delegate void CellDelegate(FilmingPageControl page, McsfFilmViewport viewport, MedViewerControlCell cell);
        public delegate void ViewportDelegate(FilmingPageControl page, McsfFilmViewport viewport);
        public delegate void PageDelegate(FilmingPageControl page);

        public event CellDelegate CellLeftButtonUpHandler;
        public event CellDelegate CellLeftButtonDownHandler;
        public event CellDelegate CellRightButtonDownHandler;
        public event CellDelegate CellRightButtonUpHandler;
        //public event CellDelegate CellMiddleButtonDownHandler;
        //public event CellDelegate CellMiddleButtonUpHandler;
        public event CellDelegate CellLeftButtonDoubleClickHandler;
        public event ViewportDelegate ViewportLeftClickHandler;
        public event ViewportDelegate ViewportRightClickHandler;
        public event PageDelegate PageTitleBarLeftClickHandler;
        public event PageDelegate PageTitleBarRightClickHandler;

        public event PageDelegate PageActiveStatusChangedHander;

        private MedViewerLayoutCell _rootCell2D;

        private readonly List<McsfFilmViewport> _viewportList = new List<McsfFilmViewport>();

        private ActionType _actionType;

        private int ImageLoadedCount { get; set; }

        public void UpdateImageCount()
        {
            ImageLoadedCount = ImageCount;
        }


        #endregion [--Need to be classified--]

        #region Viewer Controller Wrapper




        public void AddCell(MedViewerControlCell cell)
        {
            try
            {
                Logger.LogFuncUp();

                //todo: performance optimization begin
                //   filmingViewerControl.AddCell(cell);
                filmingViewerControl.LayoutManager.AddControlCell(cell);
                //var e = new MedViewerEventArgs(cell);
                //OnNewCellAdded(null, e);
                //todo: performance optimization end

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        #region  [edit by jinyang.li]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetCell"></param>
        /// <param name="sourceCell"></param>
        public void AddControlCellToLayoutCell(MedViewerLayoutCell targetCell, MedViewerControlCell sourceCell)
        {
            try
            {
                Logger.LogFuncUp();

                targetCell.AddCell(sourceCell);
                var e = new MedViewerEventArgs(sourceCell);
                OnNewCellAdded(null, e);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message);
                throw;
            }
        }
        #endregion

        private void ReplaceCell(MedViewerControlCell dest, MedViewerControlCell src)
        {
            if (dest.IsEmpty && src.IsEmpty) return;

            if (dest.Image.CurrentPage == src.Image.CurrentPage) return;

            if (dest.IsEmpty)
            {
                dest.Image.AddPage(src.Image.CurrentPage);
            }
            else if (src.IsEmpty)
            {
                dest.Image.Clear();
            }
            else
            {
                dest.Image.Clear();
                dest.Image.AddPage(src.Image.CurrentPage);
            }
        }

        public void ReplaceCells(IEnumerable<MedViewerControlCell> cells)
        {
            try
            {
                Logger.LogFuncUp();
                IEnumerable<MedViewerControlCell> cellsInControl = filmingViewerControl.Cells;
                int cnt = Math.Min(cellsInControl.Count(), cells.Count());
                for (int i = 0; i < cnt; i++)
                {
                    ReplaceCell(cellsInControl.ElementAt(i), cells.ElementAt(i));
                }
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        //public void AddCells(IEnumerable<MedViewerControlCell> cells)
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();
        //        filmingViewerControl.AddCells(cells);
        //        PageTitle.PatientInfoChanged();
        //        if (IsVisible) Refresh();
        //        Logger.LogFuncDown();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //        throw;
        //    }
        //}

        public int GetSelectedCellIndex(MedViewerLayoutCell layoutCell)
        {
            try
            {

                MedViewerControlCell lastSelectCell = SelectedCells().LastOrDefault();
                if (layoutCell != null)
                {
                    return lastSelectCell != null ? layoutCell.Children.ToList().IndexOf(lastSelectCell) : 0;
                }

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }

            return 0;
        }

        public bool InsertCell(MedViewerControlCell cell, MedViewerLayoutCell layoutCell)
        {
            if (!HasEmptyCell())
            {
                return false;
            }

            MedViewerControlCell lastSelectCell = SelectedCells().LastOrDefault();
            //insert to special position
            if (layoutCell != null)
            {
                int index = lastSelectCell != null ? layoutCell.Children.ToList().IndexOf(lastSelectCell) : 0;

                layoutCell.InsertCell(cell, lastSelectCell == null ? 0 : index + 1);

                var e = new MedViewerEventArgs(cell);
                OnNewCellAdded(null, e);
                cell.IsSelected = false;

                filmingViewerControl.LayoutManager.ResetLayout();

                ReBuildViewportList();

                return true;
            }

            return false;
        }

        public bool InsertCell(MedViewerControlCell cell, MedViewerLayoutCell layoutCell, int index)
        {
            if (!HasEmptyCell())
            {
                return false;
            }

            if (layoutCell != null)
            {
                layoutCell.InsertCell(cell, (index >= layoutCell.Count ? layoutCell.Count : index + 1));

                var e = new MedViewerEventArgs(cell);
                OnNewCellAdded(null, e);
                cell.IsSelected = true;
                IsSelected = true;

                filmingViewerControl.SelectedCells.Add(cell);
                filmingViewerControl.LayoutManager.ResetLayout();

                ReBuildViewportList();

                return true;
            }

            return false;
        }

        #endregion

        public FilmingPageTitle PageTitle { get; set; }
        #region [--Need to be classified--]

        /// <summary>
        /// NOTE:you must use Initialize() after you new the FilmingPageControl!!
        /// </summary>
        public FilmingPageControl()
        {
            InitializeComponent();
            //#region [edit by jinyang.li][通过内存位图方式绘制四角信息]
            //Logger.Instance.LogDevInfo("CreateBitmapHelperManager Register [MedViewerControl:" + filmingViewerControl.GetHashCode() + "]Begin!");
            CreateBitmapHelperManager.Instance.RegisterCreateBitmapHelperToMedViewer(filmingViewerControl);
            //Logger.Instance.LogDevInfo("CreateBitmapHelperManager Register [MedViewerControl:" + filmingViewerControl.GetHashCode() + "]End!");
            //#endregion
            FirstColumnTitleItems = new List<FrameworkElement>();
            SecondColumnTitleItems = new List<FrameworkElement>();
            //this control can accept drop pop message and get drop DataObject
            AllowDrop = true;
            // Disable inner mouse left button down events.
            filmingViewerControl.CanSelectCellByLeftClick = false;
            filmingViewerControl.CanSwitchPageByMouseWheelWhileNoneCellSelected = false;

            RegisterEventHandler();

            PageTitle = new FilmingPageTitle(filmingViewerControl);
            DataContext = PageTitle;
            PageTitle.TitleVisibleChanged += new FilmingPageTitle.UpdatePageTitleLayoutHandler(PageTitleVisibleChanged);

            //only for PT,页面显示药物信息
            PageTitle.DrugInfoChangedEvent -= new EventHandler(PageTitle_AddDrugForPT);
            PageTitle.DrugInfoChangedEvent += new EventHandler(PageTitle_AddDrugForPT);

            _actionType = ActionType.Pointer;
            //todo: performance optimization begin
            MedViewerControl.HideScrollBar = true;
            //todo: performance optimization end

            filmingViewerControl.CurrentApplicationMode = MedViewerControl.ApplicationMode.Filming;
            filmingViewerControl.FilmingViewMode = true;
            filmingViewerControl.Focusable = true;
            filmingViewerControl.CanShiftSelectCell = false;
            filmingViewerControl.Focus();

            IsBeenRendered = false;

            //PageTitle.ParseFilmingPageConfig();

            SetPageTitleDisplay();
            FirstColumnTitleItems.Add(patientNameLabel);
            FirstColumnTitleItems.Add(patientIDLabel);
            FirstColumnTitleItems.Add(accessionNo);
            FirstColumnTitleItems.Add(patientSexTextBlock);

            SecondColumnTitleItems.Add(InstitutionNameLabel);
            SecondColumnTitleItems.Add(AcquisitionDateTimeLabel);
            SecondColumnTitleItems.Add(txtComment);
            PageTitleVisibleChanged();
            txtComment.ContextMenu = null;
            

        }

        private void PageTitle_AddDrugForPT(object sender, EventArgs e)
        {
            try
            {
                this.DrugTextBlock.Inlines.Clear();

                var firstPTCell = this.filmingViewerControl.Cells.FirstOrDefault(n => null != n.Image && null != n.Image.CurrentPage && n.Image.CurrentPage.Modality == Modality.PT);
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
                uint tagRadiopharmaceutical = 0x00180031;
                displaydata.ImageHeader.DicomHeader.TryGetValue(tagRadiopharmaceutical, out radiopha);
                if (null == radiopha || string.IsNullOrEmpty(radiopha))
                {
                    return;
                }

                //var radiophaText = new Run("  " + radiopha + " " + sourceIso[0])
                //{
                //    Foreground = new SolidColorBrush(Colors.White),
                //    FontFamily = new FontFamily(this.TryFindResource("UID_Filming_Font_Family").ToString()),
                //    FontSize = this.PageTitle.DisplayFont - 2
                //};
                //this.DrugTextBlock.Inlines.Add(radiophaText);

                //var sourceIsoText = new Run(sourceIsoArray[1])
                //{
                //    BaselineAlignment = BaselineAlignment.Superscript,
                //    Foreground = new SolidColorBrush(Colors.White),
                //    FontSize = this.PageTitle.DisplayFont - 4,
                //    FontFamily = new FontFamily(this.TryFindResource("UID_Filming_Font_Family").ToString())
                //};
                //this.DrugTextBlock.Inlines.Add(sourceIsoText);

                var text = new Run(" " + sourceIsoArray[1] + sourceIsoArray[0] + "-" + radiopha)
                {
                    Foreground = new SolidColorBrush(Colors.White),
                    FontFamily = new FontFamily("Arial"),
                    FontSize = this.PageTitle.DisplayFont
                };

                this.DrugTextBlock.Inlines.Add(text);
                this.DrugTextBlock.ToolTip = text.Text;

            }
            catch (Exception exception)
            {
                Logger.LogFuncException("Add Drug Info to Page Title Failed"
                                                         + "[Exception:Message]" + exception.Message
                                                         + "[Exception:Source]" + exception.StackTrace);
            }
        }


        private List<FrameworkElement> FirstColumnTitleItems;
        private List<FrameworkElement> SecondColumnTitleItems;
        void PageTitleVisibleChanged()
        {
            int rowscount = getVisibleRowCount();
            InitFilmingTileGrid((int)PageTitle.DisplayFont, rowscount);
            int firstVisibleCount = 0;
            int secondVisibleCount = 0;

            for (int i = 0; i < FirstColumnTitleItems.Count; i++)
            {
                if (FirstColumnTitleItems[i].Visibility == Visibility.Visible)
                {
                    Grid.SetRow(FirstColumnTitleItems[i], firstVisibleCount);
                    Grid.SetColumn(FirstColumnTitleItems[i], 0);

                    firstVisibleCount++;
                }
            }
            for (int i = 0; i < SecondColumnTitleItems.Count; i++)
            {
                if (SecondColumnTitleItems[i].Visibility == Visibility.Visible)
                {
                    Grid.SetRow(SecondColumnTitleItems[i], secondVisibleCount);
                    Grid.SetColumn(SecondColumnTitleItems[i], 1);
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
            filmPageNumberTextBlock.Visibility = PageTitle.PageNoVisibility;
        }
        private int getVisibleRowCount()
        {
            int rowCount1 = FirstColumnTitleItems.Count(t => t.Visibility == Visibility.Visible);
            int rowCount2 = SecondColumnTitleItems.Count(t => t.Visibility == Visibility.Visible);
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
        public void UpdatePageTitleDisplay()
        {
            PageTitle.SetElementDisplay();
            SetPageTitleDisplay();
        }

        private void SetPageTitleDisplay()
        {
            if (PageTitle.DisplayPosition == "0") //no film page bar
            {
                SetPagingBarLayout();
                filmPageBarGrid.Visibility = Visibility.Collapsed;
            }
            else if (PageTitle.DisplayPosition == "1") //film page bar on top
            {
                SetPagingBarLayout();
            }
            else if (PageTitle.DisplayPosition == "2") //film page bar at bottom
            {
                SetPagingBarAsFooter();
            }

            filmPageBarGridSimple.Visibility = (PageTitle.DisplayPosition == "0" && PageTitle.PageNoVisibility == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
        }

        public void SetPagingBarAsFooter()
        {
            mainGrid.RowDefinitions.Clear();

            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = new GridLength(1, GridUnitType.Star);
            mainGrid.RowDefinitions.Add(rowDef);

            rowDef = new RowDefinition();
            rowDef.Height = GridLength.Auto;
            mainGrid.RowDefinitions.Add(rowDef);

            rowDef = new RowDefinition();
            rowDef.Height = GridLength.Auto;
            mainGrid.RowDefinitions.Add(rowDef);

            mainGrid.Children.Clear();
            mainGrid.Children.Add(filmPageBarGrid);
            mainGrid.Children.Add(filmingViewerControlGrid);

            Grid.SetRow(filmingViewerControlGrid, 0);
            Grid.SetRow(filmPageBarGrid, 1);
            Grid.SetRow(filmPageBarGridSimple, 2);

            filmPageBarGrid.Visibility = Visibility.Visible;
        }

        private Brush oldBrush = null;
        public void SetPageTitleBeforePrint()
        {
            // if(txtComment.)
            oldBrush = txtComment.BorderBrush;
            txtComment.Focusable = false;
            txtComment.BorderBrush = null;
        }
        public void SetPageTitleAfterPrint()
        {
            txtComment.Focusable = true;
            txtComment.BorderBrush = oldBrush;
        }
        public void SetPagingBarLayout()
        {
            mainGrid.RowDefinitions.Clear();
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = GridLength.Auto;
            mainGrid.RowDefinitions.Add(rowDef);

            rowDef = new RowDefinition();
            rowDef.Height = GridLength.Auto;
            mainGrid.RowDefinitions.Add(rowDef);

            rowDef = new RowDefinition();
            rowDef.Height = new GridLength(1, GridUnitType.Star);
            mainGrid.RowDefinitions.Add(rowDef);

            mainGrid.Children.Clear();
            mainGrid.Children.Add(filmPageBarGrid);
            mainGrid.Children.Add(filmPageBarGridSimple);
            mainGrid.Children.Add(filmingViewerControlGrid);
            Grid.SetRow(filmPageBarGrid, 0);
            Grid.SetRow(filmPageBarGridSimple, 1);
            Grid.SetRow(filmingViewerControlGrid, 2);

            filmPageBarGrid.Visibility = Visibility.Visible;
        }

        //todo: performance optimization begin (remove group action transmit)
        //void OnGroupMouseMoveInvoking(object sender, MedViewerEventArgs e)
        //{
        ////    //var actionArguments = e.Target as MouseMoveActionArgs;
        //    var filmingCard = GetFilmingCard();
        //    var target = e.Target as MouseMoveActionArgs;
        //    bool isRightButtonPressed = false;
        //    if (target != null)
        //    {
        //        var tag = target.Tag as MouseEventArgs;
        //        if (tag != null)
        //        {
        //            isRightButtonPressed = tag.RightButton == MouseButtonState.Pressed;
        //        }
        //    }
        //    if (filmingCard.MouseGestureButton != MouseButton.Left && isRightButtonPressed)
        //        IsGroupMouseMove = true;
        //    //if (filmingCard.MouseGestureButton != MouseButton.Left)// resolve 324430 [1/3/2014 shiquan.huang]
        //    //{
        //    //    filmingCard.EnableContextMenu(false);
        //    //}

        //    //if (null == actionArguments || null == filmingCard || !filmingCard.IsMultiImageActionMode() || filmingCard.CurrentActionType == ActionType.PixelLens)
        //    //{
        //    //    return;
        //    //}

        //    //foreach (var filmPage in filmingCard.EntityFilmingPageList)
        //    //{
        //    //    if (!Equals(this, filmPage))
        //    //    {
        //    //        var viewControl = filmPage.filmingViewerControl;
        //    //        viewControl.ProcessGroupMouseMove(viewControl.SelectedCells, actionArguments);
        //    //    }
        //    //}
        //}
        //todo: performance optimization end

        //跨胶片同步画图元
        public void OnSynGraphicToCellOperated(object sender, MedViewerSynGraphicEventArgs e)
        {
            var filmingCard = GetFilmingCard();

            var synGraphicXml = e.GraphicXml;
            var sourceGraphic = e.SourceGrahpic;
            var isDrawHideCell = e.IsDrawHideCell;
            var isDeleteLastGraphic = e.IsDeleteLastGraphic;

            if (string.IsNullOrEmpty(synGraphicXml)) return;

            var graphicOverlay = new OverlayGraphics();

            foreach (var page in filmingCard.ActiveFilmingPageList)
            {
                if (!page.Equals(this))
                {
                    var listCells = page.filmingViewerControl.SelectedCells.Cast<IViewerControlCell>().ToList();
                    graphicOverlay.DeserializeXmlToCells(sourceGraphic, synGraphicXml, listCells, null, isDrawHideCell, isDeleteLastGraphic);
                }
            }
            if (filmingCard.IfZoomWindowShowState)
            {
                filmingCard.ZoomViewer.RefreshDisplayCell();
            }
        }


        //todo: performance optimization begin
        //bool isNeedSynchronizeDifferentFilmingPage = false;

        public bool CanSynchronizeDifferntFilmingPage
        {
            get
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.LeftShift)
                    || Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    //    isNeedSynchronizeDifferentFilmingPage = true;
                    return false;
                }
                return true;
            }
        }

        //todo: performance optimization end


        //todo: performance optimization begin (remove group action transmit)
        internal bool IsGroupMouseMove = false;
        //void OnGroupMouseLeftButtonUpInvoking(object sender, MedViewerEventArgs e)
        //{
        //    //var actionArguments = e.Target as MouseActionArgs;
        //   // var filmingCard = GetFilmingCard();
        //    //if (null == actionArguments || null == filmingCard || !filmingCard.IsMultiImageActionMode()
        //    //    || (filmingCard.MouseGestureButton != MouseButton.Left && !IsGroupMouseMove))
        //    //{
        //    //    return;
        //    //}
        //    //foreach (var filmPage in filmingCard.EntityFilmingPageList)
        //    //{
        //    //    if (this != filmPage && !isNeedSynchronizeDifferentFilmingPage)
        //    //    {
        //    //        isNeedSynchronizeDifferentFilmingPage = false;
        //    //        var viewControl = filmPage.filmingViewerControl;
        //    //        viewControl.ProcessGroupMouseButtonUp(viewControl.SelectedCells, filmingCard.MouseGestureButton);
        //    //    }
        //    //}
        //   // filmingCard.RefreshAction();
        //}


        //void OnGroupMouseLeftButtonDownInvoking(object sender, MedViewerEventArgs e)
        //{
        //    //var actionArguments = e.Target as MouseActionArgs;
        //    //IsGroupMouseMove = false;
        //    //var filmingCard = GetFilmingCard();
        //    //if (null == actionArguments || null == filmingCard || !filmingCard.IsMultiImageActionMode())
        //    //{
        //    //    return;
        //    //}
        //    //foreach (var filmPage in filmingCard.EntityFilmingPageList)
        //    //{
        //    //    if (this != filmPage && CanSynchronizeDifferntFilmingPage)
        //    //    {
        //    //        //filmPage.filmingViewerControl.ProcessGroupMouseButtonDown(filmingCard.MouseGestureButton, actionArguments);
        //    //        var viewControl = filmPage.filmingViewerControl;
        //    //        viewControl.ProcessGroupMouseButtonDown(viewControl.SelectedCells, filmingCard.MouseGestureButton);
        //    //    }
        //    //}
        //}
        //todo: performance optimization end

        /// <summary>
        /// NOTE:you must use Initialize() after you new the FilmingPageControl!!
        /// </summary>
        public void Initialize()
        {
            try
            {
                Logger.LogFuncUp();

                if (FilmingViewerContainee.FilmingResourceDict != null)
                {
                    Resources.MergedDictionaries.Add(FilmingViewerContainee.FilmingResourceDict);
                }

                //Read configuration
                filmingViewerControl.IsGraphicContextMenuDisplayGestureText = false;

                filmingViewerControl.InitializeWithoutCommProxy(Configure.Environment.Instance.FilmingUserConfigPath);
                FilmingLayoutCell rootCell = new FilmingLayoutCell();
                FilmingLayoutCellImpl rootCellImpl = new FilmingLayoutCellImpl() { DataSource = rootCell };
                rootCellImpl.LayoutGrid.Margin = new Thickness(0);
                filmingViewerControl.LayoutManager.RootCell = rootCell;
                filmingViewerControl.LayoutManager.RootCellImpl = rootCellImpl;
                filmingViewerControl.ThresholdToShowImageTextPartially = 0;
                filmingViewerControl.SetAction(ActionType.Pointer);
                filmingViewerControl.LayoutManager.RootCellImpl.UseDeviceSpecific = true;
                filmingViewerControl.GraphicContextMenu.SetStatisticModeVisible(StatisticMode.Perimeter, true);
                InitializeSyncDrawGraphics();
                InitializeGraphicsOptions();

                _rootCell2D = filmingViewerControl.LayoutManager.RootCell;
                _rootCell2D.SplitterWidth = 1;
                _rootCell2D.IsScrollBarVisible = false;
                GetOriginalScaleTransform();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        //public void InitializeDataLoader()
        //{
        //    //adopt medDataManagement to load image at front end
        //    _studyTree = new StudyTree();
        //    _dataLoader = DataLoaderFactory.Instance().CreateLoader(_studyTree, DBWrapperHelper.DBWrapper);
        //    _dataLoader.SopLoadedHandler -= OnImageDataLoaded;
        //    _dataLoader.SopLoadedHandler += OnImageDataLoaded;
        //    //_dataLoader = FilmingViewerContainee.Main.ImageDataLoader;
        //}

        private void InitializeSyncDrawGraphics()
        {
            string modality;
            mcsf_clr_systemenvironment_config.GetModalityName(out modality);

            if (modality == "MR" || modality == "CT" || modality == "PT"|| modality =="PETMR")
            {
                this.filmingViewerControl.CanSyncDrawGraphics = true;
            }
        }

        private void InitializeGraphicsOptions()
        {
            this.filmingViewerControl.GraphicsOperateOpitions.EnableChangeOutImageRegionGraphicsColor = false;
        }

        public bool IsInSeriesCompareMode
        {
            get
            {
                return FilmPageType == FilmPageType.SingleSeriesCompareFilmPage
                || FilmPageType == FilmPageType.MultiSeriesCompareFilmPage;
            }
        }

        private void RegisterEventHandler()
        {
            filmingViewerControl.MouseRightButtonDown += OnMouseRightButtonDown;
            filmingViewerControl.MouseLeftButtonDown += OnMouseLeftButtonDown;
            filmingViewerControl.MouseRightButtonUp += OnMouseRightButtonUp;
            filmingViewerControl.NewCellAdded += OnNewCellAdded;
            filmingViewerControl.ImageLoaded += OnImageLoaded;
            filmingViewerControl.CellRemoved += OnCellRemoved;
            filmingViewerControl.ActionCleared += OnActionCleared;
            //filmingViewerControl.ImageLoading += OnImageLoading;
            filmingViewerControl.MedviewerControlIsInEditChanged += OnMedviewerControlIsInEditChanged;
            //todo: performance optimization begin (remove group action transmit)
            //filmingViewerControl.GroupMouseLeftButtonDownInvoking += OnGroupMouseLeftButtonDownInvoking;
            //filmingViewerControl.GroupMouseLeftButtonUpInvoking += OnGroupMouseLeftButtonUpInvoking;
            //filmingViewerControl.GroupMouseMoveInvoking += OnGroupMouseMoveInvoking;
            //todo: performance optimization end

            filmingViewerControl.SynGraphicToCellOperated += OnSynGraphicToCellOperated;
        }
       

        private void OnMedviewerControlIsInEditChanged(object sender, MedViewerEventArgs e)
        {
            try
            {
                bool isInEditing = (bool)e.Target;
                var filmingCard = this.GetFilmingCard();
                if (filmingCard != null)
                {
                    if (isInEditing)
                    {
                        filmingCard.ClearPtInputBindings();
                    }
                    else
                    {
                        filmingCard.RecoverPtInputBindings();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }

        }

        private FilmPageType _filmPageType = FilmPageType.NormalFilmPage;
        public FilmPageType FilmPageType
        {
            get { return _filmPageType; }
            set
            {
                if (_filmPageType != value)
                {
                    _filmPageType = value;
                    if (IsInSeriesCompareMode)
                    {
                        TreeViewMultipleDragDropHelper.SetIsDropTarget(mainGrid, false);
                    }
                    else
                    {
                        TreeViewMultipleDragDropHelper.SetIsDropTarget(mainGrid, true);
                    }
                }
            }
        }

        public MedViewerLayoutCell RootCell
        {
            get
            {
                return _rootCell2D;
            }
        }

        public List<McsfFilmViewport> ViewportList
        {
            get
            {
                if (!_viewportList.Any())
                {
                    GetViewports(filmingViewerControl.LayoutManager.RootCell);

                    //init the viewport index
                    foreach (var viewport in _viewportList)
                    {
                        viewport.IndexInFilm = _viewportList.IndexOf(viewport);
                    }
                }

                return _viewportList;
            }
        }

        private void GetViewports(MedViewerCellBase cellBase)
        {
            var layoutCell = cellBase as MedViewerLayoutCell;
            if (layoutCell != null)
            {
                if (layoutCell.Children == null || !layoutCell.Children.Any())// || ViewportLayout.LayoutType != LayoutTypeEnum.DefinedLayout)
                {
                    try
                    {
                        //if the list don't have this layoutCell, then add it.
                        if (_viewportList.First(region => region.RootLayoutCell.Equals(layoutCell)) == null)
                        {
                            _viewportList.Add(new McsfFilmViewport(layoutCell));
                        }
                    }
                    catch (Exception)
                    {
                        //we don't care the exception of First occurred.
                        _viewportList.Add(new McsfFilmViewport(layoutCell));
                    }
                }
                else
                {
                    foreach (var child in layoutCell.Children)
                    {
                        var subLayoutCell = child as MedViewerLayoutCell;
                        if (subLayoutCell != null && ViewportLayout.LayoutType == LayoutTypeEnum.DefinedLayout)
                        {
                            GetViewports(subLayoutCell);
                        }
                        else
                        {
                            _viewportList.Add(new McsfFilmViewport(layoutCell, ViewportLayout.LayoutType != LayoutTypeEnum.DefinedLayout));
                            return;
                        }
                    }
                }
            }
        }

        public ActionType ActionType
        {
            get
            {
                return _actionType;
            }
        }

        public FilmingCard GetFilmingCard()
        {
            return FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
        }

        void OnImageLoaded(object sender, MedViewerEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                var filmingCard = GetFilmingCard();
                if (filmingCard != null)
                {
                    DisplayData page = e.Target as DisplayData;
                    //todo: performance optimization begin pageTitle
                    // RefereshPageTitle();
                    //PageTitle.PatientInfoChanged();
                    //todo: performance optimization end
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            SetOverlayVisibility(page, OverlayType.FilmingF1ProcessText, filmingCard.commands.IfShowImageRuler);
                            filmingCard.ImageLoaded();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex.StackTrace);
                        }
                    }));
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        //private void OnImageDataLoaded(object sender, DataLoaderArgs e)
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();
        //        var sop = e.Target as Sop;

        //        string imageLoadingState = "[Image Data LoadingState] : "
        //                                   + " [Error Info] " + e.ErrorInfo
        //                                   + " [ImageSopInstanceUID] " + e.InstanceUID
        //                                   + " [LoadStatue] " + e.LoadStatue;

        //        //var imgSop = e.Target as ImageSop;
        //        //MessageBox.Show(imageLoadingState);
        //        Logger.LogInfo(imageLoadingState);

        //        Logger.LogTimeStamp("[Create Image Cell] ");
        //        AppendSop(sop);


        //        Logger.LogFuncDown();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message + ex.StackTrace);
        //        var filmingCard = GetFilmingCard();
        //        if (filmingCard != null) filmingCard.ImageLoaded();
        //    }
        //    //finally
        //    //{
        //    //    _dataLoader.SopLoadedHandler -= OnImageDataLoaded;
        //    //}
        //}

        public void Dispose()
        {
            Clear();

            filmingViewerControl.Controller.Dispose();
        }

        //private void On_AcceptSeriesList(object sender, SelectedSeriesListEventArgs e)
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();
        //        IsSelected = true;
        //        var filmingCard = GetFilmingCard();
        //        Debug.Assert(filmingCard != null);
        //        filmingCard.studyTreeCtrl.OnDrop(this);
        //        Logger.LogFuncDown();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message + ex.StackTrace);
        //    }

        //}
        protected override void OnDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(List<MedViewerControlCell>)))
            {
                base.OnDrop(e);
                return;
            }
            var filmingCard = GetFilmingCard();
            if (filmingCard != null)
                IsSelected = true;
            base.OnDrop(e);
        }

        public ActionType CurrentActionType { get; set; }

        internal void OnNewCellAdded(object sender, MedViewerEventArgs e)
        {
            Logger.LogFuncUp();
            try
            {
                var newCell = e.Target as MedViewerControlCell;
                if (newCell != null)    //cell now belongs to other container
                {
                    RegisterEventFromCell(newCell);

                    //todo: performance optimization begin pageTitle
                    //PageTitle.PatientInfoChanged();
                    // RefereshPageTitle();
                    //  UpdateFourCornerFontSizeForCell(newCell.CellControl as MedViewerControlCellImpl);
                    //todo: performance optimization end
                    var filmingCard = GetFilmingCard();
                    var page = newCell.Image.CurrentPage;

                    //fix DIM: 143564, state of copied cell should be consistent with original cell
                    if (filmingCard != null)
                    {
                        if (page != null)
                            SetOverlayVisibility(page, OverlayType.FilmingF1ProcessText, filmingCard.commands.IfShowImageRuler);
                        //  FilmingHelper.UpdateCornerTextForCell(newCell, filmingCard.ImageTextDisplayMode);
                    }

                    //set the current action to the new cell.
                    FilmPageUtil.SetAllActions(newCell, CurrentActionType);

                    //set our right button down context menu to this cell
                    if (newCell.CellControl != null)
                        newCell.CellControl.ContextMenu = null;

                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }

            Logger.LogFuncDown();
        }

        private void OnCellRemoved(object sender, MedViewerEventArgs e)
        {
            //todo: performance optimization pageTitle
            //PageTitle.PatientInfoChanged();
            // RefereshPageTitle();
            //todo: performance optimization end
        }

        #endregion [--Need to be classified--]

        #region Properties

        public ScaleTransform Scale
        {
            get { return _scale; }
        }

        public Visibility FilmPageTitleVisibility
        {
            set
            {
                filmPageBarGrid.Visibility = value;
            }
        }

        public string FilmPageTitle { get; set; }

        public string FilmPageTitileNumber
        {
            set
            {
                filmPageNumberTextBlock.Text = value;
                filmPageNumberTextBlockSimple.Text = value;

            }
        }

        private int _filmPageIndex;
        public int FilmPageIndex
        {
            get { return _filmPageIndex; }
            set
            {
                _filmPageIndex = value;
                AutomationProperties.SetAutomationId(this, "ID_EX_FILMING_PAGECONTROL_" + _filmPageIndex);
            }
        }

        public static int FilmPageCount { get; set; }

        public void OnPageActiveStatusChangedHander()
        {
            PageDelegate handler = PageActiveStatusChangedHander;
            if (handler != null)
                handler(this);
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;

                    //var layoutCell = filmingViewerControl.LayoutManager.RootCell;
                    //var layoutCellImpl = layoutCell.Control as FilmingLayoutCellImpl;
                    //if (layoutCellImpl != null)
                    //{
                    //    layoutCellImpl.Border.Visibility = _isSelected
                    //                                           ? Visibility.Visible
                    //                                           : Visibility.Collapsed;
                    //}

                    //if (_isSelected)
                    //{
                    //    SetTitleBarBackgroudForDisplay();
                    //}
                    //else
                    //{
                    //    SetTitleBarBackgroudForPrint();
                    //}

                    OnPageActiveStatusChangedHander();
                }
            }
        }

        public double FilmPageWidth
        {
            get { return filmingViewerControl.Width; }
            set
            {
                filmingViewerControl.Width = value;
                Width = value;
            }
        }

        public double FilmPageHeight
        {
            get { return filmingViewerControl.Height; }
            set
            {
                filmingViewerControl.Height = value;
                Height = value;
            }
        }

        public bool SetSelectedCellLayout(FilmLayout value)
        {

            try
            {
                Logger.LogFuncUp();


                var prevViewportList = SelectedViewports();
                if (prevViewportList.Any())
                {
                    bool changed = false;
                    foreach (McsfFilmViewport viewport in prevViewportList)
                    {
                        if (viewport.CellLayout.LayoutColumnsSize == value.LayoutColumnsSize &&
                            viewport.CellLayout.LayoutRowsSize == value.LayoutRowsSize)
                        {
                            continue;
                        }
                        changed = true;
                        //Limit of cell count of a viewport in a film
                        var rootCell = filmingViewerControl.LayoutManager.RootCell.Control;
                        if (rootCell.ActualHeight > 0)  //film page was shown(single series compare print problem)
                        {
                            var viewportCell = viewport.RootLayoutCell.Control;
                            uint maxRow = (uint)Math.Round(
                                (viewportCell.ActualHeight / rootCell.ActualHeight + 0.002)
                                * FilmLayout.MaxRowCount);
                            uint maxCol = (uint)Math.Round(
                                (viewportCell.ActualWidth / rootCell.ActualWidth)
                                * FilmLayout.MaxColCount);
                            if (maxRow < value.LayoutRowsSize || maxCol < value.LayoutColumnsSize)
                            {
                                MessageBoxHandler.Instance.ShowWarning("UID_Filming_Warning_ExceedLimitOfRowOrColumn");
                                return false;
                            }
                        }
                        //不规则布局
                        if (1 < _viewportList.Count)
                        {
                            viewport.IrregularCellLayout = value;
                        }
                        else
                        {
                            viewport.CellLayout = value;
                        }

                    }

                    if (!changed)
                    {
                        return false;
                    }

                    //update logic film page layout
                    string layoutString = filmingViewerControl.LayoutManager.SaveLayoutToXML();
                    if (!string.IsNullOrEmpty(layoutString))
                    {
                        ViewportLayout.LayoutXmlFileStream = layoutString;

                        filmingViewerControl.LayoutManager.SetLayoutByXML(ViewportLayout.LayoutXmlFileStream, CreateMedViewerLayoutCell);
                    }

                    ReBuildViewportList();
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

        public bool SetSelectedCellLayoutWithOutLimited(FilmLayout value)
        {
            try
            {
                var prevViewportList = SelectedViewports();
                if (prevViewportList.Any())
                {
                    bool changed = false;
                    foreach (McsfFilmViewport viewport in prevViewportList)
                    {
                        if (viewport.CellLayout.LayoutColumnsSize == value.LayoutColumnsSize &&
                            viewport.CellLayout.LayoutRowsSize == value.LayoutRowsSize)
                        {
                            continue;
                        }
                        changed = true;
                        //不规则布局
                        if (1 < _viewportList.Count)
                        {
                            viewport.IrregularCellLayout = value;
                        }
                        else
                        {
                            viewport.CellLayout = value;
                        }
                    }
                    if (!changed)
                    {
                        return false;
                    }
                    //update logic film page layout
                    string layoutString = filmingViewerControl.LayoutManager.SaveLayoutToXML();
                    if (!string.IsNullOrEmpty(layoutString))
                    {
                        ViewportLayout.LayoutXmlFileStream = layoutString;

                        filmingViewerControl.LayoutManager.SetLayoutByXML(ViewportLayout.LayoutXmlFileStream, CreateMedViewerLayoutCell);
                    }
                    ReBuildViewportList();
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }

        public bool IsEnableChangeViewportLayout(IEnumerable<McsfFilmViewport> viewportList, FilmLayout newFilmLayout)
        {
            foreach (var viewport in viewportList)
            {
                //because Region Layout should must be STANDARD layout, so we calculate it with row and column.
                int newLayoutMaxImageCount = newFilmLayout.LayoutColumnsSize * newFilmLayout.LayoutRowsSize;
                int oldLayoutMaxImageCount = viewport.GetCells().FindLastIndex(cell => !cell.IsEmpty) + 1;
                if (newLayoutMaxImageCount >= oldLayoutMaxImageCount)
                {
                    continue;
                }

                int imageCount = FilmPageUtil.GetImageCountOfViewport(viewport);
                if (newLayoutMaxImageCount >= imageCount)
                {
                    continue;
                }

                return false;
            }

            return true;
        }


        //todo: performance optimization begin layout change 
        private FilmLayout _viewportLayout;
        public FilmLayout ViewportLayout
        {
            get
            {
                if (_viewportLayout == null)
                {
                    _viewportLayout = new FilmLayout(filmingViewerControl.LayoutManager.SaveLayoutToXML());
                }
                return _viewportLayout;
            }
            set
            {
                try
                {
                    if (value == null)
                    {
                        value = new FilmLayout(5, 4);
                    }
                    _viewportLayout = value.Clone();
                    
                    if (string.IsNullOrWhiteSpace(_viewportLayout.LayoutXmlFileStream))
                    {
                        int newRows = _viewportLayout.LayoutRowsSize;
                        int newColumns = _viewportLayout.LayoutColumnsSize;
                        filmingViewerControl.LayoutManager.SetLayoutForFilming(newRows, newColumns, CreateMedViewerLayoutCell, CreateMedViewerControlCell);
                        _viewportLayout.LayoutType = LayoutTypeEnum.RegularLayout;
                    }
                    else
                    {
                        filmingViewerControl.LayoutManager.SetLayoutByXML(_viewportLayout.LayoutXmlFileStream, CreateMedViewerLayoutCell);
                    }

                    int cellCnt = filmingViewerControl.LayoutManager.RootCell.DisplayCapacity -
                                  filmingViewerControl.LayoutManager.RootCell.ControlCells.Count;

                    List<FilmingControlCell> filmingControlList = new List<FilmingControlCell>();
                    
                    for (int i = 0; i < cellCnt; i++)
                    {
                        filmingControlList.Add(new FilmingControlCell());
                    }

                    filmingViewerControl.AddCells(filmingControlList);

                    for (int i = 0; i < -cellCnt; i++)
                    {
                        filmingViewerControl.RemoveCell(filmingViewerControl.LayoutManager.RootCell.ControlCells.Count - 1);
                    }
                    ReBuildViewportList();
                    foreach (var cell in Cells)
                    {
                        RegisterEventFromCell(cell);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogFuncException(e.StackTrace);
                }
            }
        }
        //todo: performance optimization end		

        public void RemoveLayoutCellMargin(MedViewerLayoutCell layoutCell)
        {
            if (layoutCell == null) return;
            layoutCell.BorderThickness = -1D;
            var layoutCellImpl = layoutCell.Control as MedViewerLayoutCellImpl;
            if (null != layoutCellImpl)
            {
                //layoutCellImpl.SetDataSource(layoutCell);
                layoutCellImpl.LayoutGrid.Margin = new Thickness(layoutCell.BorderThickness);
            }
            layoutCell.Refresh();
            foreach (var cell in layoutCell.Children)
            {
                RemoveLayoutCellMargin(cell as MedViewerLayoutCell);
            }
        }

        public void SetViewportWithoutEmptyCell(FilmLayout viewportLayout)
        {
            if (viewportLayout == null)
            {
                viewportLayout = new FilmLayout(5, 4);
            }

            _viewportLayout = viewportLayout.Clone();

            if (string.IsNullOrWhiteSpace(_viewportLayout.LayoutXmlFileStream))
            {
                int newRows = _viewportLayout.LayoutRowsSize;
                int newColumns = _viewportLayout.LayoutColumnsSize;

                filmingViewerControl.LayoutManager.SetLayout(newRows, newColumns);
            }
            else
            {
                filmingViewerControl.LayoutManager.SetLayoutByXML(_viewportLayout.LayoutXmlFileStream, CreateMedViewerLayoutCell);
            }
            ReBuildViewportList();
        }

        public int MaxImagesCount
        {
            get
            {
                if (ViewportLayout.LayoutType == LayoutTypeEnum.StandardLayout)
                {
                    return ViewportLayout.LayoutColumnsSize * ViewportLayout.LayoutRowsSize;
                }
                else
                {
                    return _rootCell2D.DisplayCapacity;
                }
            }
        }

        public IEnumerable<MedViewerControlCell> Cells
        {
            get { return filmingViewerControl.Cells; }
        }

        public int NonEmptyImageCount
        {
            get
            {
                var imageCells = Cells.ToList();
                return imageCells.Count(cell => !cell.IsEmpty);
            }
        }

        public int ImageCount
        {
            get
            {
                var imageCells = Cells.ToList();
                return imageCells.FindLastIndex(cell => !cell.IsEmpty) + 1;
            }
        }

        public bool HasEmptyCell()
        {
            int maxImagesCount = MaxImagesCount;
            if (maxImagesCount != 0 && ImageCount < maxImagesCount)
            {
                return true;
            }
            return false;
        }

        public int EmptyCellCount()
        {
            if (MaxImagesCount >= ImageLoadedCount)
            {
                return MaxImagesCount - ImageLoadedCount;
            }
            else
            {
                return 0;
            }
        }

        public bool IfNeedRepack
        {
            get
            {
                var layoutCells = this.RootCell.Children.OfType<FilmingLayoutCell>().ToList();
                if (!layoutCells.Any()) layoutCells.Add(RootCell as FilmingLayoutCell);
                foreach (var layoutCell in layoutCells)
                {
                    if (layoutCell.IsMultiformatLayoutCell) continue;

                    if (layoutCell.Children.OfType<FilmingLayoutCell>()
                        .SelectMany(lCell => lCell.Children.OfType<FilmingControlCell>())
                        .Any(cCell => cCell.IsEmpty))
                        return true;

                    if (layoutCell.Children.OfType<FilmingControlCell>()
                        .Any(cCell => cCell.IsEmpty))
                        return true;
                }
                return false;
            }
        }

        public bool IfNeedRepackControlCell
        {
            get
            {
                foreach (var cell in this.Cells)
                {
                    if (cell.IsEmpty) return true;
                }
                return false;
            }
        }

        public int LastNonEmptyCellIndex
        {
            get
            {
                var imageCells = Cells.ToList();
                return imageCells.FindLastIndex(cell => !cell.IsEmpty);
            }
        }

        public bool IsEmpty()
        {
            //TO DO...
            if (Cells == null || !Cells.Any()) return true;

            foreach (var cell in Cells)
            {
                if (!cell.IsEmpty) return false;
            }

            return true;
        }

        public bool IsAnyImageLoaded()
        {
            if (IsEmpty())
            {
                return false;
            }

            return Cells.Any(cell => !cell.IsEmpty);
        }

        // public bool IsDisplay { get; set; }

        #endregion  //Properties

        #region Fields

        //private IDataLoader _dataLoader;
        //private StudyTree _studyTree;

        private bool _isSelected;

        #endregion  //Fields

        #region [--Need to be classified--]

        public MedViewerControlCell ShowImageInCell(FilmImageObject imageObject, int cellIndex)
        {
            if (imageObject == null)
            {
                Logger.LogWarning("[Parameter Check]The FilmImageObject is null!");
                return null;
            }

            var cell = ShowImageInCell(imageObject.ImageFilePath, cellIndex);
            if (cell != null)
            {
                imageObject.CellIndex = cell.CellIndex;
            }

            return cell;
        }
        public MedViewerControlCell ShowImageInCell(string imageFullPath, int cellIndex)
        {
            if (string.IsNullOrEmpty(imageFullPath))
            {
                Logger.LogWarning("[Parameter Check]This image path is invalid! imagePath:" + imageFullPath);
                return null;
            }

            if (!File.Exists(imageFullPath))
            {
                Logger.LogWarning("[Parameter Check]This image is not exists! imagePath:" + imageFullPath);
                return null;
            }

            try
            {
                if (HasEmptyCell())
                {
                    var filmingCard = GetFilmingCard();
                    filmingCard._dataLoader.LoadSopByPath(imageFullPath);

                    ImageLoadedCount++;

                    foreach (var aCell in Cells)
                    {
                        if (aCell.CellIndex == cellIndex)
                        {
                            return aCell;
                        }
                    }
                }
                else
                {
                    MessageBoxHandler.Instance.ShowWarning("UID_Filming_Warning_NoEmptyCellForPaste");
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException("Exception: " + ex.StackTrace);
            }

            return null;
        }

        public MedViewerControlCell ShowImageInEmptyCell(FilmImageObject imageObject)
        {
            if (imageObject == null)
            {
                Logger.LogWarning("The FilmImageObject is null!");
                return null;
            }
            var cell = ShowImageInEmptyCell(imageObject.ImageSopInstanceUid, true);

            if (cell != null)
            {
                imageObject.CellIndex = cell.CellIndex;
            }

            return cell;
        }

        /// <summary>
        /// Open image with SOP instance UID, and identify whether load PS file
        /// </summary>
        /// <param name="imageUID"></param>
        /// <param name="isLoadPS"></param>
        /// 
        /// <returns></returns>
        public MedViewerControlCell ShowImageInEmptyCell(string imageUID, bool isLoadPS)
        {
            if (string.IsNullOrEmpty(imageUID))
            {
                Logger.LogWarning("[Parameter Check]This image UID is invalid! imageUID:" + imageUID);
                return null;
            }
            try
            {
                Logger.LogInfo("ShowImageInEmptyCell:" + FilmPageIndex + ":" + ImageLoadedCount + "--UID:" + imageUID);
                var filmingCard = GetFilmingCard();
                filmingCard._dataLoader.LoadSopByUid(imageUID); //load image by front end
                ImageLoadedCount++;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException("Exception: " + ex.StackTrace);
            }

            return null;
        }

        public MedViewerControlCell ShowImageInEmptyCell(string imageFullPath)
        {
            if (string.IsNullOrEmpty(imageFullPath))
            {
                Logger.LogWarning("[Parameter Check]This image path is invalid! imagePath:" + imageFullPath);
                return null;
            }

            if (!File.Exists(imageFullPath))
            {
                Logger.LogWarning("[Parameter Check]This image is not exists! imagePath:" + imageFullPath);
                return null;
            }
            try
            {
                var filmingCard = GetFilmingCard();
                filmingCard._dataLoader.LoadSopByPath(imageFullPath);
                ImageLoadedCount++;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException("ShowImageInEmptyCell(string imageFullPath) Exception: " + ex.StackTrace);
            }

            return null;
        }

        public MedViewerControlCell CreateMultiFormatCellWithLayoutCell(MedViewerLayoutCell multiFormatLayoutCell)
        {
            try
            {
                Logger.LogFuncUp();

                MedViewerScreenSaver viewerScreenSaver = new MedViewerScreenSaver(filmingViewerControl);
                var bitmap = viewerScreenSaver.RenderLayoutCellToBitmap(new Size(multiFormatLayoutCell.ActualWidth, multiFormatLayoutCell.ActualHeight),
                                                                       multiFormatLayoutCell);
                WriteableBitmap writableLayoutCellBitmap = new WriteableBitmap(new FormatConvertedBitmap(bitmap, PixelFormats.Gray8, null, 0));

                byte[] data = RenderBitmapHelper.ProcessImage(writableLayoutCellBitmap);

                var dataHeader = new DicomAttributeCollection();

                FilmingHelper.AddConstDICOMAttributes(dataHeader);
                FilmingHelper.InsertStringDicomElement(dataHeader, DICOM.Tag.ImageType, FilmingUtility.MultiFormatCellImageType);

                FilmingHelper.InsertUInt16DicomElement(dataHeader, DICOM.Tag.Columns, (ushort)writableLayoutCellBitmap.Width);
                FilmingHelper.InsertUInt16DicomElement(dataHeader, DICOM.Tag.Rows, (ushort)writableLayoutCellBitmap.Height);

                FilmingHelper.InsertStringDicomElement(dataHeader, DICOM.Tag.RescaleSlope, "1");
                FilmingHelper.InsertStringDicomElement(dataHeader, DICOM.Tag.RescaleIntercept, "0");
                FilmingHelper.InsertStringDicomElement(dataHeader, DICOM.Tag.InstanceNumber, Convert.ToString(FilmPageIndex + 1));

                FilmingHelper.InsertStringDicomElement(dataHeader, DICOM.Tag.StudyInstanceUid,
                                                       PageTitle.GetPatientInfo(multiFormatLayoutCell.ControlCells, ServiceTagName.StudyInstanceUID));
                FilmingHelper.InsertStringDicomElement(dataHeader, DICOM.Tag.StudyId,
                                                      PageTitle.GetPatientInfo(multiFormatLayoutCell.ControlCells, ServiceTagName.StudyID));
                FilmingHelper.InsertStringDicomElement(dataHeader, DICOM.Tag.AcquisitionDate,
                                                      PageTitle.GetPatientInfo(multiFormatLayoutCell.ControlCells, ServiceTagName.AcquisitionDate, "", ""));
                FilmingHelper.InsertStringDicomElement(dataHeader, DICOM.Tag.AcquisitionTime,
                                                     PageTitle.GetPatientInfo(multiFormatLayoutCell.ControlCells, ServiceTagName.AcquisitionTime, "", ""));
                FilmingHelper.InsertStringDicomElement(dataHeader, DICOM.Tag.PatientID,
                                                     PageTitle.GetPatientInfo(multiFormatLayoutCell.ControlCells, ServiceTagName.PatientID));
                FilmingHelper.InsertStringDicomElement(dataHeader, DICOM.Tag.PatientsName,
                                                     PageTitle.GetPatientInfo(multiFormatLayoutCell.ControlCells, ServiceTagName.PatientName));
                FilmingHelper.InsertStringDicomElement(dataHeader, DICOM.Tag.PatientsBirthDate,
                                                      PageTitle.GetPatientInfo(multiFormatLayoutCell.ControlCells, ServiceTagName.PatientBirthDate, FilmingHelper.EmptyString, ""));
                FilmingHelper.InsertStringDicomElement(dataHeader, DICOM.Tag.PatientsAge,
                                                      PageTitle.GetPatientInfo(multiFormatLayoutCell.ControlCells, ServiceTagName.PatientAge, FilmingHelper.EmptyString, FilmingHelper.StarsString));

                FilmingHelper.InsertStringDicomElement(dataHeader, DICOM.Tag.PatientsSex,
                                                      PageTitle.GetPatientInfo(multiFormatLayoutCell.ControlCells, ServiceTagName.PatientSex, FilmingHelper.EmptyString, "O"));
                FilmingHelper.InsertStringDicomElement(dataHeader, DICOM.Tag.AccessionNumber,
                                                      PageTitle.GetPatientInfo(multiFormatLayoutCell.ControlCells, ServiceTagName.AccessionNumber, "", ""));
                DicomAttribute element = DicomAttribute.CreateAttribute(DICOM.Tag.PixelData, DICOM.VR.OB);
                if (!element.SetBytes(0, data))
                {
                    Logger.LogWarning("Failed to Insert image Data to Data header");
                }
                dataHeader.AddDicomAttribute(element);
                Logger.LogFuncDown();

                return CreateCellWith(dataHeader);
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        private MedViewerControlCell CreateCellWith(DicomAttributeCollection dataHeader)
        {
            Logger.LogFuncUp();

            var sop = SopInstanceFactory.Create(dataHeader, string.Empty);
            //new a cell
            var cell = new McsfFilmingMultiformatCell();

            //add display data to cell
            var accessor = new DataAccessor();
            var imgSop = sop as ImageSop;
            byte[] pixelData = null;
            var ps = string.Empty;
            if (imgSop != null)
            {
                pixelData = imgSop.GetNormalizedPixelData();
                ps = imgSop.PresentationState;
            }
            var printerImageTextConfigContent = "";
            if (Printers.Instance.Modality2FilmingImageTextConfigContent.ContainsKey(sop.Modality))
            {
                printerImageTextConfigContent = Printers.Instance.Modality2FilmingImageTextConfigContent[sop.Modality];
            }
            var displayData = accessor.CreateImageDataForFilmingF1Process(sop.DicomSource,
                                                                                                                       pixelData,
                                                                                                                       ps,
                                                                                                                       printerImageTextConfigContent);
            cell.Image.AddPage(displayData);
            Logger.LogFuncDown();

            return cell;
        }
        public MedViewerControlCell ShowSOPImageInEmptyCell(string sopInstanceUID)
        {
            if (string.IsNullOrEmpty(sopInstanceUID))
            {
                Logger.LogWarning("The SOPInstanceUID is null!");
                return null;
            }

            try
            {
                if (HasEmptyCell())
                {
                    //add new cell to show image
                    int cellIndex = -1;
                    filmingViewerControl.Controller.LoadImageBySOPInstanceUID(sopInstanceUID, cellIndex);

                    ImageLoadedCount++;

                    foreach (var aCell in Cells)
                    {
                        if (aCell.CellIndex == cellIndex)
                        {
                            return aCell;
                        }
                    }
                }
                else
                {
                    MessageBoxHandler.Instance.ShowWarning("UID_Filming_Warning_NoEmptyCellForPaste");
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException("Exception: " + ex.StackTrace);
            }
            return null;
        }

        public MedViewerControlCell ShowSOPImageInEmptyCell(ImageJobModel imageJobModel)
        {
            if (null == imageJobModel)
            {
                Logger.LogWarning("The image job model is null!");
                return null;
            }
            try
            {
                AppendImageJobModel(imageJobModel);
                ImageLoadedCount++;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException("Exception: " + ex.StackTrace);
            }

            return null;
        }

        private MedViewerControlCell AddDataHeaderToCell(ImageJobModel imageJobModel, MedViewerControlCell cell)
        {
            try
            {
                Logger.LogFuncUp();

                if (FilmingViewerContainee.FilmingViewerWindow == null)
                {
                    return null;
                }

                var filmingCard = GetFilmingCard();
                var dataHeader = imageJobModel.DataHeader;
                var sop = SopInstanceFactory.Create(dataHeader, string.Empty);
                var dataAccessor = new DataAccessor(filmingCard.FilmingViewerConfiguration);
                var imgSop = sop as ImageSop;
                byte[] pixelData = null;
                var ps = string.Empty;
                if (imgSop != null)
                {
                    pixelData = imgSop.GetNormalizedPixelData();
                    ps = imgSop.PresentationState;
                }

                var imgTxtConfigContentFromAdvApp = imageJobModel.ImageTextFileContext;
                //var textItemConfig = imageJobModel.TextItemFileContext;
                var imgTxtDataHeaderContentFromAdvApp = imageJobModel.SerializedImageText;

                DisplayData displayData = null;
                if (string.IsNullOrEmpty(imgTxtConfigContentFromAdvApp))
                {
                    var printerImageTextConfigContent = "";
                    if (Printers.Instance.Modality2FilmingImageTextConfigContent.ContainsKey(sop.Modality))
                    {
                        printerImageTextConfigContent = Printers.Instance.Modality2FilmingImageTextConfigContent[sop.Modality];
                    }
                    displayData = dataAccessor.CreateImageDataForFilmingF1Process(sop.DicomSource,
                                                                                                                                  pixelData,
                                                                                                                                  ps,
                                                                                                                                  printerImageTextConfigContent);
                }
                else
                {
                    displayData = dataAccessor.CreateImageDataForFilmingF1Process(sop.DicomSource,
                                                                                                                                 pixelData,
                                                                                                                                 ps,
                                                                                                                                 imgTxtConfigContentFromAdvApp,
                                                                                                                                imgTxtDataHeaderContentFromAdvApp);
                   // displayData.PState.DisplayMode = ImgTxtDisplayState.FromApplication;

                }
               
                displayData.RecvOriginWindowLevel = displayData.PState.WindowLevel;
                displayData.Unit = RetriveDisplayDataUnits(displayData.Unit);
                //需copy一份
                displayData.DicomHeader = sop.DicomSource.Clone();
                displayData.GetHashCode();
                if (imageJobModel.UserInfo != string.Empty)
                {
                    displayData.UserSpecialInfo = imageJobModel.UserInfo + ";" + imageJobModel.FilmingIdentifier;
                }
                else
                {
                    displayData.UserSpecialInfo = filmingCard.LoadSeriesTimeStamp.ToString("yyyy-MM-dd-HH-mm-ss-ffffff") + ";" + imageJobModel.FilmingIdentifier;
                }

                //if (!string.IsNullOrEmpty(imgTxtConfigContentFromAdvApp) && !string.IsNullOrEmpty(imgTxtDataHeaderContentFromAdvApp))
                //{
                //    if (dataHeader.Contains(DICOM.Tag.WindowWidth) && dataHeader.Contains(DICOM.Tag.WindowCenter))
                //    {
                //        var ww = dataHeader[DICOM.Tag.WindowWidth];
                //        string widthString = displayData.RecvOriginWindowLevel.WindowWidth.ToString();
                //        ww.GetString(0, out widthString);

                //        double width = double.Parse(widthString);

                //        var wc = dataHeader[DICOM.Tag.WindowCenter];
                //        string centerString = displayData.RecvOriginWindowLevel.WindowCenter.ToString();
                //        wc.GetString(0, out centerString);

                //        double center = double.Parse(centerString);

                //        displayData.PState.WindowLevel = new WindowLevel(center, width);
                //        displayData.RecvOriginWindowLevel = displayData.PState.WindowLevel;
                //    }
                //    else
                //    {
                //        Logger.LogError("ww/wl in DataHeader from UIDeal or 3D is not set ");
                //    }
                //}

                var shutterOverlay = displayData.GetOverlay(OverlayType.Shutter);
                if (shutterOverlay != null)
                {
                    shutterOverlay.IsVisible = false;    //MG Filming禁止显示shutter边框
                }


                var overlay = displayData.GetOverlay(OverlayType.FilmingF1ProcessText) as OverlayFilmingF1ProcessText;
                if (overlay != null)
                {
                    overlay.SetRulerDisplayMode(filmingCard.commands.IfShowImageRuler);
                }


                displayData.IsDirty = true;

                var image = cell.Image;
                if (image == null) throw new NullReferenceException("cell.image is null");
                if (image.Count == 0) image.AddPage(displayData);
                else image.ReplacePage(displayData, 0);

                //todo: performance optimization page title
                if (displayData != null)
                {
                    ImagePatientInfo ipi = new ImagePatientInfo();
                    ipi.AppendPatientInfo(displayData);
                }
                //todo: performance optimization end


                var filmAnnotationType = filmingCard.ImageTextDisplayMode;
                if (filmAnnotationType != ImgTxtDisplayState.Customization)
                    FilmingHelper.UpdateCornerTextDisplayData(displayData, filmAnnotationType, IsVisible);
                FilmingHelper.RefereshDisplayMode(displayData);

                // Select the newly loaded cell
                cell.IsSelected = true;
                var viewPort = FilmPageUtil.ViewportOfCell(cell, this);
                viewPort.IsSelected = true;
                this.IsSelected = true;

                Logger.LogFuncDown();

                return cell;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        private string RetriveDisplayDataUnits(string unit)
        {
            if (unit != null)
            {
                if(unit.Contains("SUV bw"))
                {
                    return " SUV bw";
                } 
                else if (unit.Contains("SUV bsa"))
                {
                    return " SUV bsa";
                }
                else if (unit.Contains("SUV lbm"))
                {
                    return " SUV lbm";
                }
                else if (unit.Contains("Bq/ml"))
                {
                    return " Bq/ml";
                }
                else if (unit.Contains("%"))
                {
                    return " %";
                }
                else
                {
                    return unit;
                }
            }
            return null;
        }     

        private List<MedViewerControlCell> ImageMgJobList = new List<MedViewerControlCell>();
        private void AppendImageJobModel(ImageJobModel imageJobModel)
        {
            try
            {
                Logger.LogFuncUp();

                //new a cell
                var cellIndex = imageJobModel.CellIndex;
                if (cellIndex == -1)
                {
                    var lastImageCell = Cells.LastOrDefault(c => !c.IsEmpty);
                    cellIndex = lastImageCell == null ? 0 : lastImageCell.CellIndex + 1;
                }

                var cell = Cells.ElementAt(cellIndex);


                //add cell to viewcontrol
                AddDataHeaderToCell(imageJobModel, cell);
                FilmPageUtil.SetAllActions(cell, CurrentActionType);
                if (this.IsVisible)
                {
                    Logger.LogInfo("FilmingPageControl is Visible: Refresh!");
                    Dispatcher.Invoke(new Action(() => { AppendCell(cell, false); }));
                }
                else
                {
                    IsBeenRendered = false;

                }
                if (cell.Image.CurrentPage.Modality == Modality.MG)
                {
                    ImageMgJobList.Add(cell);
                }

                //todo: performance optimization begin pageTitle
                // PageTitle.PatientInfoChanged();
                //RefereshPageTitle();
                //todo: performance optimization end
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        //todo: performance optimization begin remove film page refresh
        //public void Refresh()
        //{
        //    try
        //    {
        //        Logger.LogTimeStamp("开始刷新胶片");
        //        Logger.LogFuncUp();

        //        Dispatcher.Invoke(new Action(() =>
        //        {
        //            try
        //            {
        //                if (filmingViewerControl.CellCount > 0)
        //                {
        //                    filmingViewerControl.LayoutManager.Refresh();
        //                }

        //                foreach (var cell in Cells)
        //                {
        //                    //set the current action to the new cell.

        //                    FilmPageUtil.SetAllActions(cell, CurrentActionType);

        //                    RegisterEventFromCell(cell);

        //                    SetDisplayThreshold(cell);

        //                    UpdateFourCornerFontSizeForCell(cell.Control as MedViewerControlCellImpl);

        //                    var filmingCard = GetFilmingCard();

        //                    if (cell.Image != null && cell.Image.CurrentPage != null)
        //                        SetOverlayVisibility(cell.Image.CurrentPage, OverlayType.Ruler, filmingCard.IfShowImageRuler);


        //                    var currentAnnotationType = filmingCard.ImageTextDisplayMode;

        //                    if (currentAnnotationType != ImgTxtDisplayState.All)
        //                        FilmingHelper.UpdateCornerTextForCell(cell, currentAnnotationType);
        //                    #region //key451178
        //                    if (!cell.IsEmpty && cell.Image.CurrentPage.Modality == Modality.MG && ImageMgJobList.Count > 0)
        //                    {
        //                        if (ImageMgJobList.Contains(cell))
        //                        {
        //                            cell.ViewerControl.ApplyDisplayWhenMg(cell);
        //                            ImageMgJobList.Remove(cell);
        //                        }

        //                    }
        //                    #endregion
        //                    cell.Refresh();
        //                    cell.Refresh(CellRefreshType.ImageText);    //Annotation Display WorkAround
        //                }
        //                IsBeenRendered = true;
        //            }
        //            catch (Exception ex)
        //            {
        //                Logger.LogWarning(ex.Message);
        //            }
        //        }));

        //        Logger.LogFuncDown();
        //        Logger.LogTimeStamp("结束刷新胶片");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //    }

        //}
        //todo: performance optimization end

        public MedViewerControlCell ShowSOPImageInEmptyCell(DicomAttributeCollection dataHeader)
        {
            if (null == dataHeader)
            {
                Logger.LogWarning("The data header is null!");
                return null;
            }

            try
            {
                if (HasEmptyCell())
                {
                    //add new cell to show image
                    int cellIndex = Cells.Count();
                    //createsop
                    var sop = SopInstanceFactory.Create(dataHeader, string.Empty);
                    AppendSop(sop);
                    ImageLoadedCount++;
                    foreach (var aCell in Cells)
                    {
                        if (aCell.CellIndex == cellIndex)
                            return aCell;
                    }
                }
                else
                {
                    MessageBoxHandler.Instance.ShowWarning("UID_Filming_Warning_NoEmptyCellForPaste");
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException("Exception: " + ex.StackTrace);
            }

            return null;
        }

        /// <summary>
        /// delete all image cells after the specify index
        /// </summary>
        /// <param name="index"></param>
        public List<MedViewerControlCell> DeleteImagesAfterIndex(int index)
        {
            try
            {
                var deletedFilePathIndexList = new List<MedViewerControlCell>();
                //step 1: delete the cell and image from viewerControl
                for (int i = index + 1; i < Cells.Count(); i++)
                {
                    var deleteCell = Cells.ToList()[i];
                    //record the deleted file path in a list
                    deletedFilePathIndexList.Insert(0, deleteCell);
                    UnRegisterMouseDownEventFromCell(deleteCell);
                }

                //step 2: delete the selected file path from the FilmingPageImagePathCollection
                foreach (var cell in deletedFilePathIndexList)
                {
                    filmingViewerControl.RemoveCell(cell.CellIndex);

                    ImageLoadedCount--;
                }

                ReBuildViewportList();

                //todo: performance optimization begin pageTitle
                // PageTitle.PatientInfoChanged();
                //RefereshPageTitle();
                //todo: performance optimization end 
                deletedFilePathIndexList.Reverse(0, deletedFilePathIndexList.Count);

                return deletedFilePathIndexList;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
                throw;
            }
        }

        public void DeleteSelectedImages()
        {
            try
            {
                if (SelectedCells().Count > 0)
                {
                    var deletedFilePathIndexList = new List<MedViewerControlCell>();

                    //step 1: delete the cell and image from viewerControl
                    foreach (var cell in SelectedCells())
                    {
                        //record the deleted file path in a list
                        deletedFilePathIndexList.Insert(0, cell);
                        UnRegisterMouseDownEventFromCell(cell);
                    }
                    //step 2: delete the selected file path from the FilmingPageImagePathCollection
                    foreach (var cell in deletedFilePathIndexList)
                    {
                        filmingViewerControl.RemoveCell(cell.CellIndex);

                        ImageLoadedCount--;
                    }

                    ReBuildViewportList();

                    //todo: performance optimization begin pageTitle
                    //  PageTitle.PatientInfoChanged();
                    //RefereshPageTitle();
                    //todo: performance optimization end
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
                //throw;
            }
        }

        public void UnRegisterMouseDownEventFromCell(MedViewerControlCell cell)
        {
            //remove the mouse button down event on this cell, because if cell move to 
            //another film page, there will be occur a exception.
            var control = cell.CellControl;
            UnRegisterMouseDownEventFromCellImpl(control);
        }

        private void UnRegisterMouseDownEventFromCellImpl(Control control)
        {
            if (control != null)
            {
                control.MouseRightButtonDown -= OnCellMouseRightButtonDown;
                control.MouseLeftButtonDown -= OnCellMouseLeftButtonDown;
                control.MouseLeftButtonUp -= OnCellMouseLeftButtonUp;
                control.SizeChanged -= OnCellSizeChanged;
                control.MouseDown -= OnCellMouseDown;
                control.MouseUp -= OnCellMouseUp;
            }
        }

        public void RegisterEventFromCell(MedViewerControlCell cell)
        {
            //avoid duplicated register event handler, and lead to exception action
            var control = cell.CellControl;
            RegisterEventFromCellImpl(control);
        }

        private bool isRegisting = false;
        public void RegisterEventFromCellImpl(Control control)
        {
            if (control != null)
            {
                control.MouseRightButtonDown -= OnCellMouseRightButtonDown;
                control.MouseLeftButtonDown -= OnCellMouseLeftButtonDown;
                control.MouseLeftButtonUp -= OnCellMouseLeftButtonUp;
                control.MouseDoubleClick -= OnCellMouseDoubleClick;

                control.SizeChanged -= OnCellSizeChanged;
                control.MouseDown -= OnCellMouseDown;
                control.MouseUp -= OnCellMouseUp;
                control.MouseRightButtonUp -= OnCellMouseRightButtonUp;
                isRegisting = true;
                control.MouseRightButtonDown += OnCellMouseRightButtonDown;
                control.MouseLeftButtonDown += OnCellMouseLeftButtonDown;
                control.MouseLeftButtonUp += OnCellMouseLeftButtonUp;
                control.MouseRightButtonUp += OnCellMouseRightButtonUp;
                control.MouseDoubleClick += OnCellMouseDoubleClick;
                control.SizeChanged += OnCellSizeChanged;
                control.MouseDown += OnCellMouseDown;
                control.MouseUp += OnCellMouseUp;
                isRegisting = false;
            }
        }

        private void DeleteAllImages()
        {
            ImageLoadedCount = 0;

            //todo: performance optimization begin New Page
            foreach (var cell in Cells)
            {
                //UnRegisterMouseDownEventFromCell(cell);
                if (!cell.IsEmpty) { 
                    FilmPageUtil.ClearAllActions(cell); 
                    cell.Image.Clear();
                    if (cell.ViewerControl != null)
                    {
                        cell.ViewerControl.ClearAction();
                    }
                    cell.Refresh();
                }
            }

            // filmingViewerControl.RemoveAll();
            ReBuildViewportList();
            //  PageTitle.PatientInfoChanged();
            //RefereshPageTitle();
            txtComment.Text = "";
            // clear the filming title bar content

            //todo: performance optimization end
        }

        public void Clear()
        {
            try
            {
                txtComment.Text = string.Empty;
                DeleteAllImages();
                IsBeenRendered = false;
                this.PageTitle.Comment = string.Empty;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        #endregion [--Need to be classified--]

        #region Image Adjusting Interface

        private void OnActionCleared(MedViewerControl obj)
        {
            var filmingCard = GetFilmingCard();
            if (null != filmingCard)
            {
                filmingCard.ClearAction();
            }
        }

        public void SetAction(ActionType actionType)
        {
            CurrentActionType = actionType;
            _actionType = actionType;

            //edit by jinyang.li,解决九院快捷三键性能慢。此处设置Action会导致MedviewerControl强制ForceEndAction。
            //filmingViewerControl.SetAction(actionType);

            foreach (MedViewerControlCell cell in Cells)
            {
                FilmPageUtil.SetAction(cell, actionType);
            }
            if (actionType == ActionType.Pan || actionType == ActionType.Zoom)
            {
                foreach (var cell in Cells)
                {
                    if (cell.Image.CurrentPage != null)
                    {
                        var overlayLocalizedImage = cell.GetOverlay(OverlayType.LocalizedImage) as OverlayLocalizedImage;
                        if (null != overlayLocalizedImage)
                        {
                            overlayLocalizedImage.GraphicLocalizedImage.MiniCell.ActionManager.SetActionForSingleUseCell
                                (actionType, MouseButton.Left);
                        }
                    }
                }
            }
            else
            {
                foreach (var cell in Cells)
                {
                    if (cell.Image.CurrentPage != null)
                    {
                        var overlayLocalizedImage = cell.GetOverlay(OverlayType.LocalizedImage) as OverlayLocalizedImage;
                        if (null != overlayLocalizedImage)
                        {
                            overlayLocalizedImage.GraphicLocalizedImage.MiniCell.ActionManager.SetActionForSingleUseCell
                                                 (ActionType.Pointer, MouseButton.Left);
                        }
                    }
                }
            }

        }
        public void ClearAction()
        {
            CurrentActionType = ActionType.Pointer;
            _actionType = ActionType.Pointer;

            foreach (var cell in Cells)
            {
                cell.SetAction(ActionType.Pointer, MouseButton.Left);
            }
        }

        public void Reset()
        {
            foreach (var cell in Cells)
            {
                DisplayData currentPage = cell.Image.CurrentPage;
                if (currentPage != null)
                {
                    currentPage.Reset();
                }
                cell.Refresh();
            }
        }

        public void UpdateCornerTextOfSelectedCells(ImgTxtDisplayState type, bool shouldRefresh = true)
        {
            try
            {
                foreach (var cell in this.SelectedCells())
                {
                    if (null != cell.Image && null != cell.Image.CurrentPage)
                    {
                        FilmingHelper.UpdateCornerTextDisplayData(cell.Image.CurrentPage, type, shouldRefresh);
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.LogFuncException(exp.Message);
            }
        }

        public void UpdateCornerText(ImgTxtDisplayState type, bool shouldRefresh = true)
        {
            try
            {
                foreach (var cell in this.Cells)
                {
                    if (null != cell.Image && null != cell.Image.CurrentPage)
                    {
                        FilmingHelper.UpdateCornerTextDisplayData(cell.Image.CurrentPage, type, shouldRefresh);
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.LogFuncException(exp.Message);
            }
        }

        public void ZoomCells(double scale)
        {
            try
            {
                foreach (MedViewerControlCell cell in Cells)
                {
                    foreach (DisplayData page in cell.Image.Pages)
                    {
                        var overlayText = (OverlayFilmingF1ProcessText)page.GetOverlay(OverlayType.FilmingF1ProcessText);
                        var currentPage = cell.Image.CurrentPage;
                        if (null != overlayText)
                        {
                            //get ScaleX, ScaleY
                            PresentationState ps = currentPage.PState;
                            ps.ScaleX = _originalScaleTransform.ScaleX * scale;
                            ps.ScaleY = _originalScaleTransform.ScaleY * scale;
                            cell.Refresh();
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.LogFuncException(exp.Message);
            }
        }

        public void FitWindow()
        {
            try
            {
                foreach (MedViewerControlCell cell in Cells)
                {
                    foreach (DisplayData page in cell.Image.Pages)
                    {
                        if (null != page)
                        {
                            page.FitWindow();
                        }
                    }
                    cell.Refresh();
                }
            }
            catch (Exception exp)
            {
                Logger.LogFuncException(exp.Message);
            }
        }

        #endregion//Image Adjusting Interface

        #region Image Processing Interface

        //public void CreateFilmInfo(
        //    ref FilmingPrintJob.Types.FilmBox.Builder filmBox,
        //    out FilmsInfo.Types.FilmInfo.Builder filmInfo)
        //{
        //    try
        //    {
        //        Logger.LogFuncUp("Film index: " + FilmPageIndex);
        //        //film
        //        filmInfo = new FilmsInfo.Types.FilmInfo.Builder { FilmIndex = FilmPageIndex };
        //        //set film index
        //        //Set Layout
        //        string sLayout = "STANDARD\\"
        //            + filmingViewerControl.LayoutManager.Columns.ToString(CultureInfo.InvariantCulture) + ","
        //            + filmingViewerControl.LayoutManager.Rows.ToString(CultureInfo.InvariantCulture);
        //        filmBox.Layout = sLayout;       //TODO: need to get from UI
        //        //image
        //        int cellCount = filmingViewerControl.LayoutManager.RootCell.Rows
        //                * filmingViewerControl.LayoutManager.RootCell.Columns;  //displayed cell of the film
        //        int imageCount = filmingViewerControl.CellCount;    //all cell of the film
        //        int countOfcellsTobeSaved =
        //            (imageCount < cellCount ? imageCount : cellCount);

        //        for (int cellIndex = 0; cellIndex < countOfcellsTobeSaved; cellIndex++)
        //        {
        //            try
        //            {
        //                string imageFilePath = CreatePrintImageFullPath(cellIndex, 0);

        //                var imageInfo = new FilmsInfo.Types.ImageInfo.Builder { CellIndex = cellIndex, StackIndex = 0, File = imageFilePath };
        //                //set image files path
        //                filmInfo.AddImage(imageInfo);
        //                var imagebox = new FilmingPrintJob.Types.ImageBox.Builder { ImagePath = imageFilePath };
        //                IViewerControlCell cell = filmingViewerControl.GetCell(cellIndex);
        //                Dictionary<string, string> dicomHeader = cell.Image.CurrentPage.ImageHeader.DicomHeader;
        //                imagebox.SOPInstanceUID = dicomHeader["SOPInstanceUID"];

        //                filmBox.AddImageBox(imagebox);
        //            }
        //            catch (Exception ex)
        //            {
        //                Logger.LogFuncException("Film index: " + FilmPageIndex
        //                    + ", Cell index: " + cellIndex
        //                    + ", Exception: " + ex.StackTrace);
        //                filmInfo = null;
        //                filmBox = null;
        //                return;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException("Film index: " + FilmPageIndex + ", Exception: " + ex.StackTrace);
        //        filmInfo = null;
        //        filmBox = null;
        //        return;
        //    }

        //    Logger.LogFuncDown("Film index: " + FilmPageIndex);
        //}

        public DicomAttributeCollection CreateFilmInfo(ref FilmingPrintJob.Types.FilmBox.Builder filmBox, bool ifSaveEfilm, Size size = new Size(), Size eFilmSize = new Size(), bool isPrintingEFilm = false)
        {
            DicomAttributeCollection dataHeader = null;
            try
            {
                Logger.LogFuncUp("Film index: " + FilmPageIndex);

                //Set Layout, 1*1
                filmBox.Layout = "STANDARD\\1,1";
                filmBox.SetPatientName(PageTitle.IsMixed ? FilmingHelper.MixedPatientName : PageTitle.PatientName); //2013.5.16 for BUG226440 
                filmBox.SetStudyInstanceUid(PageTitle.StudyInstanceUid);

                //image
                string imageFilePath = null;
                string eFilmFilePath = null;

                if (isPrintingEFilm)
                {
                    try
                    {
                        var db = FilmingDbOperation.Instance.FilmingDbWrapper;
                        var cell = filmingViewerControl.Cells.FirstOrDefault();
                        var dicomHeader = cell.Image.CurrentPage.ImageHeader.DicomHeader;
                        var sopInstanceUid = dicomHeader[ServiceTagName.SOPInstanceUID];
                        var imageBase = db.GetImageBaseBySOPInstanceUID(sopInstanceUid);
                        imageFilePath = imageBase.FilePath;
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.LogDevError(e.Message+e.StackTrace);
                    }
                }
                else
                {
                    bool ifSaveImageAsGreyScale = !Printers.Instance.IfColorPrint;
                    var bmp = RenderBitmapHelper.RenderToBitmap(size,
                                                                   PageTitle,
                                                                   GetFilmingCard(),
                                                                   this,
                                                                   filmingViewerControl,
                                                                   filmPageBarGrid, true, ifSaveImageAsGreyScale);
                    string dicomFilePath = CreatePrintImageFullPath(0, 0);
                    WriteableBitmap wtb = ifSaveImageAsGreyScale
                                          ? new WriteableBitmap(new FormatConvertedBitmap(bmp, PixelFormats.Gray8, null, 0))
                                          : new WriteableBitmap(new FormatConvertedBitmap(bmp, PixelFormats.Rgb24, null, 0));
                    byte[] data = RenderBitmapHelper.ProcessImage(wtb);
                    dataHeader = PageTitle.AssembleSendData(data, wtb.PixelWidth, wtb.PixelHeight, this.FilmPageIndex, ifSaveImageAsGreyScale);
                    var dicomConvertorProxy = McsfDicomConvertorProxyFactory.Instance().CreateDicomConvertorProxy();
                    dicomConvertorProxy.SaveFile(dataHeader, dicomFilePath,
                                                 FilmingViewerContainee.Main.GetCommunicationProxy());

                    imageFilePath = dicomFilePath;
                }

                //if(ifSaveEfilm)
                //{
                //    eFilmFilePath = PageTitle.IsMixed ? string.Empty : SaveViewerControlAsDicom(eFilmSize);
                //}
                if (imageFilePath == null)
                    return dataHeader;
                if (eFilmFilePath != null)
                    filmBox.EfilmPath = eFilmFilePath;

                //step 1: add image file path first, for print use.
                var imagebox = new FilmingPrintJob.Types.ImageBox.Builder { ImagePath = imageFilePath };
                filmBox.AddImageBox(imagebox);

                //step 2: add origin file sop instance uid for print status update.
                foreach (var cell in filmingViewerControl.Cells)
                {
                    if (cell.Image != null && cell.Image.Pages.Any())
                    {
                        Dictionary<uint, string> dicomHeader = cell.Image.CurrentPage.ImageHeader.DicomHeader;
                        if (dicomHeader.ContainsKey(ServiceTagName.SOPInstanceUID))
                        {
                            var originalImageBox = new FilmingPrintJob.Types.ImageBox.Builder();
                            originalImageBox.OriginSOPInstanceUID = dicomHeader[ServiceTagName.SOPInstanceUID];

                            if (string.IsNullOrEmpty(originalImageBox.OriginSOPInstanceUID))
                                continue;

                            filmBox.AddImageBox(originalImageBox);
                        }
                        //transfer the series instance UID list to BE for update the series and study table print status.
                        if (dicomHeader.ContainsKey(ServiceTagName.SeriesInstanceUID))
                        {
                            var seriesInstanceUID = dicomHeader[ServiceTagName.SeriesInstanceUID];
                            if (!string.IsNullOrEmpty(seriesInstanceUID) && !filmBox.SeriesInstanceUidListList.Contains(seriesInstanceUID))
                            {
                                filmBox.AddSeriesInstanceUidList(seriesInstanceUID);
                            }
                        }
                    }
                }

                Logger.LogFuncDown("Film index: " + FilmPageIndex);
            }
            catch (Exception e)
            {
                Logger.LogFuncException(e.Message);
                throw;
            }
            return dataHeader;
        }

        public string SaveAsDicomFile(Size size = new Size())
        {
            try
            {

                if (!filmingViewerControl.Cells.Any())
                {
                    return null;
                }
                return SaveFilmpageAsDicom(size);
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
#if DEVELOPER
                MessageBox.Show("拷屏失败，无法创建打印任务");
#else
                //MessageBoxHandler.Instance.ShowInfo("UID_Filming_ERROR_SaveEFilm_Fail");
#endif
                throw;
            }
        }

        #endregion  //Image Processing Interface

        #region Select related

        public void OnPageTitleBarRightClickHandler()
        {
            PageDelegate handler = PageTitleBarRightClickHandler;
            if (handler != null)
                handler(this);
        }

        public void OnPageTitleBarLeftClickHandler()
        {
            PageDelegate handler = PageTitleBarLeftClickHandler;
            if (handler != null)
                handler(this);
        }

        public void OnViewportLeftClickHandler(McsfFilmViewport viewport)
        {
            ViewportDelegate handler = ViewportLeftClickHandler;
            if (handler != null)
                handler(this, viewport);
        }

        public void OnViewportRightClickHandler(McsfFilmViewport viewport)
        {
            ViewportDelegate handler = ViewportRightClickHandler;
            if (handler != null)
                handler(this, viewport);
        }

        public void OnCellLeftButtonUpHandler(McsfFilmViewport viewport, MedViewerControlCell cell)
        {
            CellDelegate handler = CellLeftButtonUpHandler;
            if (handler != null)
                handler(this, viewport, cell);
        }

        private void OnCellLeftButtonDownHandler(McsfFilmViewport viewport, MedViewerControlCell cell)
        {
            CellDelegate handler = CellLeftButtonDownHandler;
            if (handler != null)
                handler(this, viewport, cell);
        }

        public void OnCellRightButtonDownHandler(McsfFilmViewport viewport, MedViewerControlCell cell)
        {
            CellDelegate handler = CellRightButtonDownHandler;
            if (handler != null)
                handler(this, viewport, cell);
        }
        private void OnCellRightButtonUpHandler(McsfFilmViewport viewport, MedViewerControlCell cell)
        {
            CellDelegate handler = CellRightButtonUpHandler;
            if (handler != null)
                handler(this, viewport, cell);
        }
        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var viewport = FilmPageUtil.GetSelectedViewport(filmingViewerControl.LayoutManager.RootCell, ViewportList);
            if (null != viewport)
            {
                OnViewportRightClickHandler(viewport);
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                var viewport = FilmPageUtil.GetSelectedViewport(filmingViewerControl.LayoutManager.RootCell, ViewportList);
                if (null != viewport)
                {
                    OnViewportLeftClickHandler(viewport);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }
        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
        public void OnCellMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //todo: performance optimization begin event
            if (isRegisting)
                return;
            //todo: performance optimization end	
            var cellControl = sender as MedViewerControlCellImpl;
            if (cellControl != null)
            {
                var cell = cellControl.DataSource;
                //fix bug 142700 2012-11-30
                var viewport = ViewportOfCell(cell);
                if (viewport != null)
                {
                    OnCellLeftButtonDownHandler(viewport, cell);
                }
            }

            if (cellControl != null && cellControl.DataSource.IsSelected && e.RightButton == MouseButtonState.Pressed)
            {
                var cell = cellControl.DataSource;
                IAction ac = cell.ActionManager.CurrentAction;
                var tempActionType = ActionType.Pointer;
                if (ac is ActionGroupWindowLevel)
                    tempActionType = ActionType.Windowing;
                else if (ac is ActionGroupPan)
                    tempActionType = ActionType.Pan;
                else if (ac is ActionGroupZoom)
                    tempActionType = ActionType.Zoom;

                if (tempActionType == ActionType.Pan ||
                    tempActionType == ActionType.Zoom ||
                    tempActionType == ActionType.Windowing)
                {
                    GetFilmingCard().RegisterSpecialActionsForMultiplePage(cell);
                }

            }
        }

        //when cell clicked
        public bool _isGroupLRButtonDown = false;//左右键按下操作状态，延续到都释放,不改变选中状态
        private void OnCellMouseDown(object sender, MouseButtonEventArgs e)
        {
            //todo: performance optimization begin event
            if (isRegisting)
                return;
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Pressed)
            {
                _isGroupLRButtonDown = true;
            }
            //todo: performance optimization end	
            if (e.MiddleButton == MouseButtonState.Pressed && e.LeftButton != MouseButtonState.Pressed && e.RightButton != MouseButtonState.Pressed)
            {

                var cellControl = sender as MedViewerControlCellImpl;
                if (cellControl != null)
                {
                    var cell = cellControl.DataSource;
                    var viewport = ViewportOfCell(cell);
                    if (viewport != null)
                    {
                        OnCellLeftButtonDownHandler(viewport, cell);
                    }
                }
            }

           
        }
        private void OnCellMouseUp(object sender, MouseButtonEventArgs e)
        {
            //todo: performance optimization begin event
            if (isRegisting)
                return;
            //todo: performance optimization end	
            if (e.ChangedButton == MouseButton.Middle && e.MiddleButton == MouseButtonState.Released)
            {

                var cellControl = sender as MedViewerControlCellImpl;
                if (cellControl != null)
                {
                    var cell = cellControl.DataSource;
                    var viewport = ViewportOfCell(cell);
                    if (viewport != null)
                    {
                        OnCellLeftButtonUpHandler(viewport, cell);
                        // OnCellLeftButtonDownHandler(viewport, cell);
                    }
                }
            }
        }
        public void OnCellMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //todo: performance optimization begin event
            if (isRegisting)
                return;
            //todo: performance optimization end	
            var cellControl = sender as MedViewerControlCellImpl;
            MedViewerControlCell cell;
            if (cellControl != null && (cell = cellControl.DataSource) != null && cell.IsSelected == false)
            {
                var viewport = ViewportOfCell(cell);
                if (viewport != null)
                {
                    OnCellRightButtonDownHandler(viewport, cell);
                }
            }
           
        }

        public void OnCellMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //todo: performance optimization begin event
            if (isRegisting)
                return;
            //todo: performance optimization end	
            var cellControl = sender as MedViewerControlCellImpl;
            if (cellControl != null)
            {
                var cell = cellControl.DataSource;
                var viewport = ViewportOfCell(cell);
                if (viewport != null)
                {
                    OnCellLeftButtonUpHandler(viewport, cell);
                }
            }

            if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
            {
                _isGroupLRButtonDown = false;
            }
        }
        public void OnCellMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //todo: performance optimization begin event
            if (isRegisting)
                return;
            //todo: performance optimization end	
            var cellControl = sender as MedViewerControlCellImpl;
            if (cellControl != null)
            {
                var cell = cellControl.DataSource;
                //  var viewport = ViewportOfCell(cell);
                if (cell != null)
                {
                    OnCellRightButtonUpHandler(null, cell);
                }
            }
            if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
            {
                _isGroupLRButtonDown = false;
            }
        }

        private void OnCellMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //todo: performance optimization begin event 
            if (isRegisting)
                return;
            //todo: performance optimization end	
            var cellControl = sender as MedViewerControlCellImpl;
            if (cellControl != null)
            {
                var cell = cellControl.DataSource;
                var viewport = ViewportOfCell(cell);
                if (viewport != null)
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        CellLeftButtonDoubleClickHandler(this, viewport, cell);

                        IAction ac = cell.ActionManager.CurrentAction;
                        if ((ac is ActionPointerBase) || (ac is ActionGroupPan) || (ac is ActionGroupZoom))
                            e.Handled = true;
                    }
                }

            }
        }
        private void OnFilmingPageTitleBarLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnPageTitleBarLeftClickHandler();
        }

        private void OnFilmingPageTitleBarRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnPageTitleBarRightClickHandler();
        }

        //public void UpdateFourCornerFontSizeForCell(MedViewerControlCellImpl cell)
        //{
        //    if (cell == null) return;
        //    FilmingHelper.UpdateFourCornerFontSizeForCell(cell, new Size(cell.ActualWidth, cell.ActualHeight));
        //}

        public void OnCellSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //todo: performance optimization begin event
            if (isRegisting)
                return;
            //todo: performance optimization end	
        }

        public List<MedViewerControlCell> SelectedCells()
        {
            var cells = filmingViewerControl.Cells.ToList();
            //2013-4-8 pm sometimes  cells is empty, but selectedCells is not.  Add this line to avoid viewer control mistake.
            //and fix bug 
            if (cells.Count == 0) return new List<MedViewerControlCell>();
            return filmingViewerControl.Cells.Where(cell => cell.IsSelected//).ToList(); //cell中若有参考图像被选中也认为改cell被选中
            || (cell.Image != null
                 && cell.Image.CurrentPage != null
                 && cell.Image.CurrentPage.GetOverlay(OverlayType.LocalizedImage) != null
                 && cell.Image.CurrentPage.GetOverlay(OverlayType.LocalizedImage) as OverlayLocalizedImage != null
                 && (cell.Image.CurrentPage.GetOverlay(OverlayType.LocalizedImage) as OverlayLocalizedImage).GraphicLocalizedImage.MiniCell.IsSelected)).ToList();
        }

        public List<MedViewerControlCell> NotSelectedCells()
        {
            var cells = filmingViewerControl.Cells.ToList();

            if (cells.Count == 0) return new List<MedViewerControlCell>();
            return filmingViewerControl.Cells.Where(cell => !cell.IsEmpty && !cell.IsSelected).ToList();
        }

        public List<McsfFilmViewport> SelectedViewports()
        {
            return ViewportList.Where(viewport => viewport.IsSelected).ToList();
        }

        public McsfFilmViewport ViewportOfCell(MedViewerControlCell cell)
        {
            return FilmPageUtil.ViewportOfCell(cell, this);
        }
        public MedViewerControlCell GetCellByIndex(int index)
        {
            return Cells.ElementAt(index);
        }

        public void ReBuildViewportList()
        {
            try
            {
                Logger.LogFuncUp();

                var filmingCard = GetFilmingCard();

                var selectedViewportIndices = new List<int>();
                int lastSelectedViewportIndex = -1;
                var viewportList = SelectedViewports();
                foreach (var viewport in viewportList)
                {
                    selectedViewportIndices.Add(viewport.IndexInFilm);

                    if (lastSelectedViewportIndex == -1)
                    {
                        if (filmingCard != null && filmingCard.IsLastSelectedViewport(viewport))
                        {
                            lastSelectedViewportIndex = viewport.IndexInFilm;
                        }
                    }
                }

                // MedViewer team rebuilds layout cell, so we have to rebuild viewport list.
                ViewportList.Clear();

                foreach (var viewport in ViewportList)
                {
                    if (selectedViewportIndices.Contains(viewport.IndexInFilm))
                    {
                        viewport.IsSelected = true;
                        if (viewport.IndexInFilm == lastSelectedViewportIndex)
                        {
                            if (filmingCard != null)
                            {
                                UpdateLastSelectedViewport(viewport);
                            }
                        }
                        foreach (var cell in viewport.GetCells())
                        {
                            RegisterEventFromCell(cell);
                        }
                    }
                    //else
                    //{
                    //    foreach (var cell in viewport.GetCells())
                    //    {
                    //        if (cell.IsSelected)
                    //        {
                    //            viewport.IsSelected = true;
                    //            if (viewport.IndexInFilm == lastSelectedViewportIndex)
                    //            {
                    //                if (filmingCard != null)
                    //                {
                    //                    UpdateLastSelectedViewport(viewport);
                    //                }
                    //            }
                    //        }
                    //        //else
                    //        //{
                    //        //    RegisterEventFromCell(cell);
                    //        //}
                    //    }
                    //}
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        // It's for ReBuildViewportList in FilmingPageControl.
        public void UpdateLastSelectedViewport(McsfFilmViewport viewport)
        {
            var filmingCard = GetFilmingCard();
            filmingCard.LastSelectedViewport = viewport;

            if (filmingCard.LastSelectedViewport == null)
            {
                filmingCard.LastSelectedCell = null;
                return;
            }
            var layoutcell = filmingCard.LastSelectedViewport.RootLayoutCell;
            var viewerControl = layoutcell.ViewerControl;
            var filmingPageControl = filmingCard.EntityFilmingPageList.GetFilmPageControlByViewerControl(viewerControl);
            foreach (var cell in viewport.GetCells())
            {
                filmingPageControl.RegisterEventFromCell(cell);
            }
        }

        public void SelectAllCellsOfSelectedViewport()
        {
            foreach (var viewport in ViewportList)
            {
                if (viewport.IsSelected)
                {
                    viewport.SelectAllCells(true);
                }
            }
        }

        public void SelectViewports(bool isSelected)
        {
            foreach (var viewport in ViewportList)
            {
                viewport.IsSelected = isSelected;
            }
        }

        public void SelectedAll(bool isSelected)
        {
            SelectViewports(isSelected);
            foreach (var cell in Cells)
            {
                if (cell.IsSelected != isSelected)
                {
                    cell.IsSelected = isSelected;
                }


                if (cell.IsEmpty || null == cell.Image || null == cell.Image.CurrentPage)
                {
                    continue;
                }

                //if(cell.IsHoldedByAction && !cell.IsSelected) cell.ForceEndAction();
                //mini cell
                //var overlayLocalizedImage = cell.GetOverlay(OverlayType.LocalizedImage) as OverlayLocalizedImage;
                //if (null == overlayLocalizedImage) continue;
                //overlayLocalizedImage.GraphicLocalizedImage.MiniCell.IsSelected = isSelected;
            }
        }

        #endregion

        #region Private Helper Methods

        private void GetOriginalScaleTransform()
        {
            _originalScaleTransform = new ScaleTransform();
            foreach (MedViewerControlCell cell in Cells)
            {
                DisplayData currentPage = cell.Image.CurrentPage;
                PresentationState ps = currentPage.PState;
                _originalScaleTransform.ScaleX = ps.ScaleX;
                _originalScaleTransform.ScaleY = ps.ScaleY;
                break;
            }
        }

        private String CreatePrintImageFullPath(int iCellIndex, int iStackIndex)
        {
            DateTime dateTime = DateTime.Now;
            string sPrintImageFullPath = Printers.Instance.PrintObjectStoragePath;
            sPrintImageFullPath += "\\" + dateTime.Hour.ToString(CultureInfo.InvariantCulture) + dateTime.Minute.ToString(CultureInfo.InvariantCulture) +
                dateTime.Second.ToString(CultureInfo.InvariantCulture) + dateTime.Millisecond.ToString(CultureInfo.InvariantCulture) +
                iCellIndex.ToString(CultureInfo.InvariantCulture) + iStackIndex.ToString(CultureInfo.InvariantCulture);
            return sPrintImageFullPath;
        }

        #endregion  //Private Helper Methods

        #region Fields

        private ScaleTransform _originalScaleTransform;

        #endregion  //Fields


        #region [--Need to be classified--]

        public List<MedViewerControlCell> PopAllMedViewCell()
        {
            try
            {
                Logger.LogFuncUp();
                List<MedViewerControlCell> cellList = filmingViewerControl.Cells.ToList();
                Logger.LogFuncDown();
                return cellList;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        public void SaveCustomerLayout(string name)
        {
            try
            {
                string customerLayoutBasePath = GetLayoutStoragePath() + "\\" + name;

                if (ViewportLayout.LayoutType != LayoutTypeEnum.DefinedLayout)
                    customerLayoutBasePath += FilmLayout.RegularLayoutString;

                if (string.IsNullOrEmpty(customerLayoutBasePath))
                {
                    Logger.LogError("null or empty layout save path.");
                    return;
                }

                string layoutXmlFullPath = customerLayoutBasePath + ".xml";
                string layoutPngFullPath = customerLayoutBasePath + ".png";

                //step 1: save the layout xml file
                string xml = filmingViewerControl.LayoutManager.SaveFlimingLayoutToXML();
                if (string.IsNullOrEmpty(xml))
                {
                    MessageBoxHandler.Instance.ShowWarning("UID_Filming_CustomViewPortLayout_Save_Warning_Just_Support_Saving_Irreguar_Layout");
                    return;
                }

                using (StreamWriter writer = new StreamWriter(layoutXmlFullPath))
                {
                    writer.Write(xml);
                    writer.Flush();
                    writer.Close();
                }
                //step 2: save the thumbnail image of the layout
                BitmapSource layoutBitmapSource = FilmingHelper.CaptureScreen(filmingViewerControl, 96.0, 96.0);

                if (layoutBitmapSource == null)
                {
                    return;
                }

                using (var filestream = new FileStream(layoutPngFullPath, System.IO.FileMode.Create))
                {
                    var encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(layoutBitmapSource));

                    encoder.Save(filestream);
                    filestream.Flush();
                    filestream.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
                throw;
            }
        }

        private string GetLayoutStoragePath()
        {
            try
            {
                string entryPath = mcsf_clr_systemenvironment_config.GetApplicationPath("FilmingConfigPath");

                string layoutRootPath = entryPath + @"\McsfMedViewerConfig\MedViewerLayouts\CustomerLayout\";

                if (!Directory.Exists(layoutRootPath))
                {
                    Directory.CreateDirectory(layoutRootPath);
                }

                return layoutRootPath;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
                throw;
            }
        }

        public void SetOverlayVisibility(OverlayType type, bool isVisible)
        {
            foreach (var cell in filmingViewerControl.Cells)
            {
                SetOverlayVisibility(cell.Image.CurrentPage, type, isVisible);
            }
        }

        public void SetOverlayVisibility(DisplayData page, OverlayType type, bool isVisible)
        {
            if (page != null)
            {
                var overlay = page.GetOverlay(type) as OverlayFilmingF1ProcessText;
                if (overlay != null)
                {
                    overlay.SetRulerDisplayMode(isVisible);
                    //overlay.Refresh(); [edit by jinyang.li 性能优化]
                }
            }
        }

        #endregion [--Need to be classified--]

        #region [--Display--]

        public void SetPageBreakLabel(bool isBreakPage)
        {
            _pageBreakFlag.Visibility = isBreakPage ? Visibility.Visible : Visibility.Hidden;
        }


        #endregion [--Display--]



        #region	[--[---Save As Dicom---]--]

        private string SaveBitmapAsDicom(BitmapSource bmp)
        {
            try
            {
                Logger.LogFuncUp();

                string dicomFilePath = CreatePrintImageFullPath(0, 0);
                WriteableBitmap wtb = new WriteableBitmap(new FormatConvertedBitmap(bmp, PixelFormats.Gray8, null, 0));
                byte[] data = RenderBitmapHelper.ProcessImage(wtb);
                var dataHeader = PageTitle.AssembleSendData(data, wtb.Width, wtb.Height, this.FilmPageIndex);
                var dicomConvertorProxy = McsfDicomConvertorProxyFactory.Instance().CreateDicomConvertorProxy();
                dicomConvertorProxy.SaveFile(dataHeader, dicomFilePath,
                                             FilmingViewerContainee.Main.GetCommunicationProxy());

                Logger.LogFuncDown();
                return dicomFilePath;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        private string SaveFilmpageAsDicom(Size filmSize)
        {
            try
            {
                return SaveBitmapAsDicom(RenderBitmapHelper.RenderToBitmap(filmSize,
                                                                           PageTitle,
                                                                           GetFilmingCard(),
                                                                           this,
                                                                           filmingViewerControl,
                                                                           filmPageBarGrid));
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        public string SaveViewerControlAsDicom(Size viewerControlSize, bool ifSaveImageAsGreyScale = true)
        {
            try
            {
                Logger.LogFuncUp();
                string ret = SaveBitmapAsDicom(RenderBitmapHelper.RenderViewerControlToBitmap(viewerControlSize,
                                                                                              GetFilmingCard(),
                                                                                              this, filmingViewerControl, true, ifSaveImageAsGreyScale));
                Logger.LogFuncDown();
                return ret;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }


        #endregion	[--[---Save As Dicom---]--]

        private void AppendSop(Sop sop)
        {
            try
            {
                Logger.LogFuncUp();
                MedViewerControlCell cell = null;
                cell = new MedViewerControlCell();
                //add cell to viewcontrol
                Logger.LogTimeStamp("[Add Cell into Film page] ");
                filmingViewerControl.LayoutManager.AddControlCell(cell);
                if (sop != null)
                {
                    AddSopToCell(sop, cell);
                }
                else
                {
                    Logger.LogError("SOP is null, will lead to appear empty cell");
                }

                Dispatcher.Invoke(new Action(() =>
                {
                    AppendCell(cell);
                }));

                //todo: performance optimization begin pageTitle
                // PageTitle.PatientInfoChanged();
                //RefereshPageTitle();
                //todo: performance optimization end
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        private void AppendCell(MedViewerControlCell cell, bool bEnd = true)
        {
            var filmingCard = GetFilmingCard();
            try
            {
                Logger.LogFuncUp();
                if (IsVisible)
                {
                    Logger.LogTimeStamp("[Refresh cell to cell impl] ");

                    var controlCellImpls = filmingViewerControl.LayoutManager.RootCellImpl.ControlCellImpls;

                    MedViewerControlCellImpl cellImplToDisplayCell = null;

                    cellImplToDisplayCell =
                        controlCellImpls.FirstOrDefault(cellImpl => cellImpl.DataSource == null);

                    if (cellImplToDisplayCell != null)
                    {
                        cellImplToDisplayCell.DataSource = cell;
                    }

                    Logger.LogTimeStamp(
                        "[Update Cell corner text & Register Cell Action] ");
                    //OnNewCellAdded
                    RegisterEventFromCell(cell);
                    var displayData = cell.Image.CurrentPage;

                    SetOverlayVisibility(displayData,
                                         OverlayType.FilmingF1ProcessText,
                                         filmingCard.commands.IfShowImageRuler);

                    var annotationType = filmingCard.ImageTextDisplayMode;
                    if (annotationType != ImgTxtDisplayState.Customization)
                        FilmingHelper.UpdateCornerTextDisplayData(displayData, filmingCard.ImageTextDisplayMode);
                }
                cell.Refresh();
                cell.Image.CurrentPage.IsDirty = false;
                //FilmingHelper.RefereshDisplayMode(cell.Image.CurrentPage);
                filmingCard.SelectObject(this, FilmPageUtil.ViewportOfCell(cell, this), cell);

                Logger.LogFuncDown();
            }
            catch (Exception exp)
            {
                Logger.LogFuncException(exp.Message);
            }
            finally
            {
                if (filmingCard != null && bEnd) filmingCard.ImageLoaded();
            }
        }

        private MedViewerControlCell AddSopToCell(Sop sop, MedViewerControlCell cell)
        {
            try
            {
                Logger.LogFuncUp();
                var accessor = new DataAccessor(filmingViewerControl.Configuration);  
                var imgSop = sop as ImageSop;
                byte[] pixelData = null;
                string ps = string.Empty;
                if (imgSop != null)
                {
                    pixelData = imgSop.GetNormalizedPixelData();
                    ps = imgSop.PresentationState;
                }
                var printerImageTextConfigContent = "";
                if (Printers.Instance.Modality2FilmingImageTextConfigContent.ContainsKey(sop.Modality))
                {
                    printerImageTextConfigContent = Printers.Instance.Modality2FilmingImageTextConfigContent[sop.Modality];
                }
                var displayData = accessor.CreateImageDataForFilmingF1Process(sop.DicomSource,
                                                                                                                           pixelData,
                                                                                                                           ps,
                                                                                                                           printerImageTextConfigContent);
                if (!displayData.ContainsPixelData)
                {
                    Logger.LogError("DisplayData is  incorrect, will lead to Unsupported Image Type");
                }
                cell.Image.AddPage(displayData);

                Logger.LogFuncDown();

                return cell;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        //todo: performance optimization begin refresh

        public bool IsBeenRendered { get; set; }
        //public static int visibleRefereshed = 0;
        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Logger.LogFuncUp(" Filming Page Index : " + FilmPageIndex);

            bool bNewValue = (bool)e.NewValue;
            if (!bNewValue) return;
            //if (bNewValue && !IsBeenRendered)
            //{
            //    //this.Refresh();
            //}
            if (this.Visibility == Visibility.Visible)
            {
                foreach (var cell in this.filmingViewerControl.Cells)
                {
                    if (cell.Image != null && cell.Image.CurrentPage != null)
                    {
                        if (cell.Image.CurrentPage.IsDirty)
                        {
                            cell.Refresh();
                            //visibleRefereshed++;
                            cell.Image.CurrentPage.IsDirty = false;
                        }
                    }
                    //else if (cell.Image==null || cell.Image.CurrentPage == null)
                    //{
                    //    cell.Refresh();
                    //}
                }
            }


            Logger.LogFuncDown();
        }
        //public void SelectLastCellOrFirstViewport(out McsfFilmViewport lastSelectedViewport
        //                                          out MedViewerControlCell cell)
        //{

        //}

        //todo: performance optimization end

        //todo: performance optimization pageTitle
        public void RefereshPageTitle()
        {
            PageTitle.UpdatePatientInfo(filmingViewerControl.Cells);
            PageTitle.PatientInfoChanged();
        }
        //todo: performance optimization end

        #region [Rendering related]
        private bool _rendered = false;
        public bool Rendered
        {
            get { return _rendered; }
            set { _rendered = value; }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Logger.LogInfo("page" + FilmPageIndex + "rendering");
            base.OnRender(drawingContext);
        }

        #endregion [Rendering related]

        public int GetImageCount()
        {
            return Cells.Aggregate(0, (total, next) => total += next.IsEmpty ? 0 : 1);
        }

        public MedViewerLayoutCell CreateMedViewerLayoutCell()
        {
            return new FilmingLayoutCell() { BorderThickness = -1D };
        }

        public MedViewerControlCell CreateMedViewerControlCell()
        {
            return new FilmingControlCell();
        }

        #region [Jinyang.li Performance]

        public bool SerializeToXml(XmlNode parentNode, out  Dictionary<string, byte[]> filePath2DataHeader, out List<string> originalUidsInPage)
        {
            try
            {
                Logger.LogFuncUp();

                filePath2DataHeader = new Dictionary<string, byte[]>();
                originalUidsInPage = new List<string>();

                var xDoc = parentNode.OwnerDocument;

                var filmingPageInfoNode = xDoc.CreateElement(OffScreenRenderXmlHelper.FIlMING_PAGE_INFO);
                parentNode.AppendChild(filmingPageInfoNode);

                //胶片标题信息 
                var filmingPageTitleInfoNode = xDoc.CreateElement(OffScreenRenderXmlHelper.FILMING_PAGE_TITLE_INFO);
                filmingPageInfoNode.AppendChild(filmingPageTitleInfoNode);
                this.PageTitle.SerializedToXml(filmingPageTitleInfoNode);

                //胶片Layout信息
                filmingPageInfoNode.SetAttribute(OffScreenRenderXmlHelper.FILMING_PAGE_LAYOUT,
                                                                        this.filmingViewerControl.LayoutManager.SaveFlimingLayoutToXML());

                //胶片图元设置项
                SerializeGraphicsOperationToXml(xDoc, filmingPageInfoNode);

                //胶片所有图像信息
                foreach (var cell in this.filmingViewerControl.Cells)
                {
                    var imgDataNode = xDoc.CreateElement(OffScreenRenderXmlHelper.IMAGE_DATA);
                    filmingPageInfoNode.AppendChild(imgDataNode);

                    imgDataNode.SetAttribute(OffScreenRenderXmlHelper.SOP_UID, string.Empty);

                    imgDataNode.SetAttribute(OffScreenRenderXmlHelper.IMG_TEXT_FILE_PATH_OR_CONTENT, string.Empty);
                    imgDataNode.SetAttribute(OffScreenRenderXmlHelper.IMG_TEXT_ITEM_PATH_OR_CONTENT, string.Empty);
                    imgDataNode.SetAttribute(OffScreenRenderXmlHelper.IMG_TEXT_CONTENT, string.Empty);

                    imgDataNode.SetAttribute(OffScreenRenderXmlHelper.SCALE_RULER, false.ToString());

                    imgDataNode.SetAttribute(OffScreenRenderXmlHelper.LOCALIZED_SOP_UID, string.Empty);

                    imgDataNode.SetAttribute(OffScreenRenderXmlHelper.DATA_HEADER_FILE_PATH, string.Empty);
                    imgDataNode.SetAttribute(OffScreenRenderXmlHelper.DATA_HEADER_LENGTH, string.Empty);

                    if (null != cell.Image && null != cell.Image.CurrentPage)
                    {
                        var displayData = cell.Image.CurrentPage;

                        //图像SopInstanceUID
                        imgDataNode.SetAttribute(OffScreenRenderXmlHelper.SOP_UID, displayData.SOPInstanceUID);
                        if (displayData.ImageHeader != null && displayData.ImageHeader.DicomHeader != null
                            && displayData.ImageHeader.DicomHeader.ContainsKey(ServiceTagName.StudyInstanceUID))
                            originalUidsInPage.Add(displayData.ImageHeader.DicomHeader[ServiceTagName.StudyInstanceUID]);

                        //四角信息配置路径or内容
                        bool isApp = false; //todo:不要用下述这种方式判断（如果高级应用不发四角信息）
                        var overlayF1FilmingTxt = displayData.GetOverlay(OverlayType.FilmingF1ProcessText) as OverlayFilmingF1ProcessText;

                        var displayMode = displayData.PState.DisplayMode == ImgTxtDisplayState.None ? ImgTxtDisplayState.None : overlayF1FilmingTxt.ImgTxtDisplayMode;
                        imgDataNode.SetAttribute(OffScreenRenderXmlHelper.IMG_TEXT_POSITION, overlayF1FilmingTxt.GraphicFilmingF1ProcessText.ImgTxtMgPostionF1);
                        imgDataNode.SetAttribute(OffScreenRenderXmlHelper.MG_Orientation_IsShow,
                            overlayF1FilmingTxt.GraphicFilmingF1ProcessText.IsShowOrientation?"1":"0");
                        imgDataNode.SetAttribute(OffScreenRenderXmlHelper.Is_Show_ImgText,
                            overlayF1FilmingTxt.GraphicFilmingF1ProcessText.IsImgTxtShowF1 ? "1" : "0");
                        imgDataNode.SetAttribute(OffScreenRenderXmlHelper.Is_Show_Ruler,
                            overlayF1FilmingTxt.GraphicFilmingF1ProcessText.IsRulerShowF1 ? "1" : "0");

                        imgDataNode.SetAttribute(OffScreenRenderXmlHelper.PT_UNIT, displayData.PState.PtUnit);

                        if (ImgTxtDisplayState.All == displayMode || ImgTxtDisplayState.Customization == displayMode)
                        {
                            imgDataNode.SetAttribute(OffScreenRenderXmlHelper.IMG_TEXT_FILE_PATH_OR_CONTENT,
                                                                        FilmingHelper.LeaveFactoryImageTextPath(displayData.Modality.ToString()));

                            imgDataNode.SetAttribute(OffScreenRenderXmlHelper.IMG_TEXT_ITEM_PATH_OR_CONTENT, value: FilmingHelper.LeaveFactoryTextItemPath(displayData.Modality.ToString()));
                        }
                        else if (ImgTxtDisplayState.FromApplication == displayMode)
                        {
                            imgDataNode.SetAttribute(OffScreenRenderXmlHelper.IMG_TEXT_FILE_PATH_OR_CONTENT,
                                                                        overlayF1FilmingTxt.FilmingImageTextConfigReader.FilmingImageTextConfigContent);
                            imgDataNode.SetAttribute(OffScreenRenderXmlHelper.IMG_TEXT_CONTENT,
                                                                        overlayF1FilmingTxt.GraphicFilmingF1ProcessText.SerializedImageText);
                           
                            isApp = true;
                            displayData.PState.DisplayMode = displayMode;
                        }

                        //比例尺
                       // var overlayRuler = displayData.GetOverlay(OverlayType.FilmingF1ProcessText) as OverlayFilmingF1ProcessText;
                        if (null != overlayF1FilmingTxt && overlayF1FilmingTxt.GraphicFilmingF1ProcessText.IsVisibleOfRuler)
                        {
                            imgDataNode.SetAttribute(OffScreenRenderXmlHelper.SCALE_RULER, true.ToString());
                        }

                        //定位像参考线
                        var overlayLocalizedImage =
                            displayData.GetOverlay(OverlayType.LocalizedImage) as OverlayLocalizedImage;
                        if (null != overlayLocalizedImage)
                        {
                            imgDataNode.SetAttribute(OffScreenRenderXmlHelper.LOCALIZED_SOP_UID,
                                                     overlayLocalizedImage.SmallDisplayData.SOPInstanceUID);
                            imgDataNode.SetAttribute(OffScreenRenderXmlHelper.LOCALIZED_IMAGE_PS_INFO,
                                overlayLocalizedImage.SmallDisplayData.SerializePSInfo());
                            imgDataNode.SetAttribute(OffScreenRenderXmlHelper.LOCALIZED_IMAGE_POSITION,
                                overlayLocalizedImage.GraphicLocalizedImage.LocalizedImagePos.ToString());
                        }


                        var userSpecialInfo = displayData.UserSpecialInfo;
                        if (!isApp && (userSpecialInfo == null || !userSpecialInfo.Contains(';')))
                        {
                            var psXDoc = displayData.SerializePSInfoForFilming();
                            if (null != psXDoc)
                            {
                                imgDataNode.InnerXml = psXDoc.InnerXml;
                            }
                        }
                        else
                        {
                            var fileName = displayData.GetHashCode().ToString() + ".txt";
                            imgDataNode.SetAttribute(OffScreenRenderXmlHelper.DATA_HEADER_FILE_PATH,
                                                                        OffScreenRenderXmlHelper.DataHeaderFilePath + fileName);

                            //pixel data 
                            var pixelData = displayData.PixelData;
                            if (pixelData == null)
                            {
                                pixelData = new byte[displayData.ImageDataLength];
                                Marshal.Copy(displayData.ImageDataPtr, pixelData, 0, displayData.ImageDataLength);
                            }
                            DicomAttribute element = DicomAttribute.CreateAttribute(DICOM.Tag.PixelData);
                            //displayData.DicomHeader.RemoveDicomAttribute(element);
                            if (!element.SetBytes(0, pixelData))
                            {
                                displayData.DicomHeader.Dispose();
                                throw new Exception("failed to set the pixel data");
                            }
                            displayData.DicomHeader.AddDicomAttribute(element);

                            //ps
                            string psStr = "";
                            if (displayData.IsTableOrCurve)      //Table控件的打印使用cell原有PSXml信息。
                            {
                                psStr = displayData.PSXml;
                            }
                            else
                            {
                                psStr = displayData.SerializePSInfo();
                            }
                            if (!string.IsNullOrEmpty(psStr))
                            {
                                byte[] ps = Encoding.UTF8.GetBytes(psStr);
                                DicomAttribute pselement = DicomAttribute.CreateAttribute(0x00613100);
                                displayData.DicomHeader.RemoveDicomAttribute(pselement);

                                if (!pselement.SetBytes(0, ps))
                                {
                                    displayData.DicomHeader.Dispose();
                                    throw new Exception("failed to set ps from data header");
                                }
                                displayData.DicomHeader.AddDicomAttribute(pselement);
                            }

                            //序列化
                            byte[] tempBytes = null;
                            if (!displayData.DicomHeader.Serialize(out tempBytes))
                            {
                                displayData.DicomHeader.Dispose();
                                throw new Exception("failed to serialized DicomAttributeCollection");
                            }
                            filePath2DataHeader.Add(OffScreenRenderXmlHelper.DataHeaderFilePath + fileName, tempBytes);
                            imgDataNode.SetAttribute(OffScreenRenderXmlHelper.DATA_HEADER_LENGTH, tempBytes.Length.ToString());
                        }
                    }
                }

                Logger.LogFuncDown();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogDevError(ex.Message + ex.StackTrace);
                filePath2DataHeader = null;
                originalUidsInPage = null;
                return false;
            }
        }

        #endregion

        private void SerializeGraphicsOperationToXml(XmlDocument doc, XmlNode filmingPageInfoNode)
        {
            var filmingPageGraphicOperation = doc.CreateElement(OffScreenRenderXmlHelper.FIlMING_PAGE_Graphic_OPERATION);
            filmingPageInfoNode.AppendChild(filmingPageGraphicOperation);

            var filmingPageGraphicMenu = doc.CreateElement(OffScreenRenderXmlHelper.FIlMING_PAGE_Graphics_ROI_MENU);
            filmingPageGraphicOperation.AppendChild(filmingPageGraphicMenu);

            var modes = this.filmingViewerControl.GraphicContextMenu.GraphicsStatisticItemsMode;

            Dictionary<string, StatisticGraphicType> graphicMode2Xml = new Dictionary<string, StatisticGraphicType>();
            graphicMode2Xml.Add(OffScreenRenderXmlHelper.FIlMING_PAGE_Circle_ROI_MENU, StatisticGraphicType.Circle2D);
            graphicMode2Xml.Add(OffScreenRenderXmlHelper.FIlMING_PAGE_Ellipse_ROI_MENU, StatisticGraphicType.Ellipse2D);
            graphicMode2Xml.Add(OffScreenRenderXmlHelper.FIlMING_PAGE_Freehand_ROI_MENU, StatisticGraphicType.FreeHand2D);
            graphicMode2Xml.Add(OffScreenRenderXmlHelper.FIlMING_PAGE_Poligon_ROI_MENU, StatisticGraphicType.Polygon2D);
            graphicMode2Xml.Add(OffScreenRenderXmlHelper.FIlMING_PAGE_Spline_ROI_MENU, StatisticGraphicType.Spline2D);
            graphicMode2Xml.Add(OffScreenRenderXmlHelper.FIlMING_PAGE_Rectangle_ROI_MENU, StatisticGraphicType.Rectangle2D);

            foreach (var gm2Xml in graphicMode2Xml)
            {
                filmingPageGraphicMenu.SetAttribute(gm2Xml.Key, ((Int32)modes[gm2Xml.Value]).ToString());

            }
        }

        private void txtComment_GotFocus(object sender, RoutedEventArgs e)
        {
            var filmingcard = GetFilmingCard();
            if (filmingcard != null)
            {
                filmingcard.ClearPtInputBindings();
            }
        }

        private void txtComment_LostFocus(object sender, RoutedEventArgs e)
        {
            var filmingcard = GetFilmingCard();
            if (filmingcard != null &&
                CurrentActionType != ActionType.AnnotationText && CurrentActionType != ActionType.AnnotationArrow)
            {
                filmingcard.RecoverPtInputBindings();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Logger.Instance.LogDevInfo("FilmingpageControl:MeasureOverride");
            try
            {
                if (Double.IsNaN(availableSize.Width))
                {
                    Logger.LogFuncException("filmpage MeasureOverride erro ");
                }
                if (Math.Abs(availableSize.Width - 0) < Double.Epsilon)
                {
                    Logger.LogFuncException("filmpage Width 0 ");
                }
                if (Math.Abs(availableSize.Height - 0) < Double.Epsilon)
                {
                    Logger.LogFuncException("filmpage Height 0 ");
                }
                
                return base.MeasureOverride(availableSize);

            }
            catch (Exception erro)
            {
                Logger.LogFuncException("filmpage MeasureOverride: " + erro.StackTrace);
                var card = GetFilmingCard();
                if(card!=null)
                    card.SetFilmPageSize(this);
                return base.MeasureOverride(availableSize);
            }
            
        }

        protected override Size ArrangeOverride(Size availableSize)
        {
            Logger.Instance.LogDevInfo("FilmingpageControl:ArrangeOverride");
            try
            {
                if (Double.IsNaN(availableSize.Width))
                {
                    Logger.LogFuncException("filmpage ArrangeOverride erro ");
                }
                if (Math.Abs(availableSize.Width - 0) < Double.Epsilon)
                {
                    Logger.LogFuncException("filmpage Width 0 ");

                }
                if (Math.Abs(availableSize.Height - 0) < Double.Epsilon)
                {
                    Logger.LogFuncException("filmpage Height 0 ");
                }

                return base.ArrangeOverride(availableSize);

            }
            catch (Exception erro)
            {
                Logger.LogFuncException("filmpage ArrangeOverride: " + erro.StackTrace);
                var card = GetFilmingCard();
                if (card != null)
                    card.SetFilmPageSize(this);
                return base.ArrangeOverride(availableSize);

            }
        }

    }


    //public class FilmingLayoutCell : MedViewerLayoutCell
    //{
    //    public FilmingLayoutCell()
    //    {
    //        BorderThickness = -1D;
    //    }
    //    public bool IsMultiformatLayoutCell { get; set; }

    //    public bool IsEmpty()
    //    {
    //        return Children.OfType<FilmingControlCell>().All(controlCell => controlCell.IsEmpty);
    //    }

    //    public void ReplaceBy(MedViewerCellBase cell)
    //    {
    //        if (cell is FilmingLayoutCell) ReplaceBy(cell as FilmingLayoutCell);
    //        if (cell is FilmingControlCell) ReplaceBy(cell as FilmingControlCell);
    //    }

    //    public void ReplaceBy(FilmingControlCell controlCell)
    //    {
    //        RemoveAll();
    //        SetLayout(1, 1);
    //        IsMultiformatLayoutCell = false;
    //        AddCell(controlCell);
    //        Refresh();
    //    }

    //    public void ReplaceBy(FilmingLayoutCell replaceCell)
    //    {
    //        int cellCount = replaceCell.Rows * replaceCell.Columns;
    //        if (Rows != replaceCell.Rows || Columns != replaceCell.Columns)
    //        {
    //            RemoveAll();
    //            SetLayout(replaceCell.Rows, replaceCell.Columns);
    //            IsMultiformatLayoutCell = replaceCell.IsMultiformatLayoutCell;
    //            for (int i = 0; i < cellCount; i++)
    //            {
    //                AddCell(new FilmingControlCell());
    //            }
    //        }

    //        for (int i = 0; i < cellCount; i++)
    //        {
    //            ReplaceCell(replaceCell.Children.ElementAt(i), i);
    //        }
    //        Refresh();
    //    }

    //    public void Clear()
    //    {
    //        //todo: 如果是多分格，需要抹掉

    //        //foreach (var controlCell in Children.OfType<FilmingControlCell>().Where(c=>!c.IsEmpty))
    //        //{
    //        //    controlCell.Image.Clear();
    //        //    controlCell.Refresh();
    //        //}
    //        if (IsEmpty()) return;
    //        RemoveAll();
    //        SetLayout(1, 1);
    //        AddCell(new FilmingControlCell());
    //        Refresh();
    //    }

    //    public bool IsSelected()
    //    {
    //        return Children.OfType<FilmingControlCell>().Any(c => c.IsSelected);
    //    }
    //}

    //public class FilmingLayoutCellImpl : MedViewerLayoutCellImpl
    //{
    //    protected override MedViewerLayoutCellImpl CreateLayoutCellImpl()
    //    {
    //        return new FilmingLayoutCellImpl() { Margin = new Thickness(1) };
    //    }

    //    protected override MedViewerControlCellImpl CreateControlCellImpl()
    //    {
    //        return new FilmingControlCellImpl() { Margin = new Thickness(1) };
    //    }

    //    protected override void SetScrollBarResource()
    //    {

    //    }
    //}

    //public class FilmingControlCellImpl : MedViewerControlCellImpl
    //{
    //    protected override Panel[] CreateOverlayControl()
    //    {
    //        return new Panel[] {
    //            new OverlayGraphicsImpl(),
    //            new OverlayColorbarImpl(),
    //            new OverlayRulerImpl(),                
    //        };
    //    }
    //}


}
