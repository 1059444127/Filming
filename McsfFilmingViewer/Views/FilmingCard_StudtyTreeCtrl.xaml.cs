using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.AppControls.Viewer;
using UIH.Mcsf.Database;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Filming.ImageManager;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Filming.Widgets;
using UIH.Mcsf.Filming.Wrappers;
using UIH.Mcsf.MHC;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.Viewer;
using ICommand = System.Windows.Input.ICommand;

namespace UIH.Mcsf.Filming.Views
{
    /// <summary>
    /// Interaction logic for FilmingCard_StudtyTreeCtrl.xaml
    /// </summary>
    public partial class FilmingCardStudtyTreeCtrl : UserControl
    {
        public StudySelectionCtrl seriesSelectionCtrl;
        private FilmingCard Card;
        public ContextMenu studyListContextMenu;
        public MenuItem batchMenuItem;
        public MenuItem compareMenuItem;

        public FilmingCardStudtyTreeCtrl(FilmingCard card)
        {
            Card = card;
            InitializeComponent();
            InitCtrl();
            //seriesSelectionCtrl.NewStudyAdded += Card.OnNewStudyAdded;
        }

        public void InitCtrlContextMenu()
        {
            studyListContextMenu = new ContextMenu();
            studyListContextMenu.Style = Card.TryFindResource("Style_ContextMenu_CSW_CC_Default") as Style;
            studyListContextMenu.Foreground = Brushes.White;
            batchMenuItem = new MenuItem();
            batchMenuItem.Header = Card.TryFindResource("UID_Filming_Batch");
            batchMenuItem.Command = BatchFilmCommand;
            AutomationProperties.SetAutomationId(batchMenuItem, "ID_MNU_FILMING_BATCH");
            studyListContextMenu.Items.Add(batchMenuItem);

            compareMenuItem = new MenuItem();
            compareMenuItem.Header = Card.TryFindResource("UID_Filming_Compare_Series");
            compareMenuItem.Command = CompareSeriesFilmCommand;
            AutomationProperties.SetAutomationId(compareMenuItem, "ID_MNU_FILMING_COMPARESERIES");
            studyListContextMenu.Items.Add(compareMenuItem);

            studyTreeCtrl.ContextMenu = studyListContextMenu;

        }

        private void InitCtrl()
        {
            seriesSelectionCtrl = new StudySelectionCtrl();
            seriesSelectionCtrl.HorizontalAlignment = HorizontalAlignment.Stretch;
            seriesSelectionCtrl.VerticalAlignment = VerticalAlignment.Stretch;
            AutomationProperties.SetAutomationId(seriesSelectionCtrl, "ID_EX_FILMING_STUDYSELECTIONCTRL");
            //seriesSelectionCtrl.DataContext = StudyListCtrlViewModel;
            studyTreeCtrl.Children.Add(seriesSelectionCtrl);
            Grid.SetRow(seriesSelectionCtrl, 0);

            seriesSelectionCtrl.Initialize();
            
        }
      

        public bool IsEnableFilmSelectedSeries
        {
            get
            {
                var items = seriesSelectionCtrl.SelectedSeriesItems;
                if (items != null && items.Count >= 1)
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsEnableBatchFilm
        {
            get
            {
                var items = seriesSelectionCtrl.SelectedSeriesItems;
                if (items == null) return false;
                if (items.All(item => item.ImageCount == 0)) return false;
                if (items.Any(item => item.SeriesDBData == null || Card.IsEFilmSeries(item))) return false;

                //if (items == null || items.Count != 1)
                //{
                //    return false;
                //}
                //var item = items.FirstOrDefault();
                //if (item == null || item.ImageCount == 0)
                //{
                //    return false;
                //}
                //var seriesData = item.SeriesDBData;
                //if (seriesData != null && !IsEFilmSeries(item))
                //{
                //    return true;
                //}

                return true;
            }
        }

        private ICommand _batchFilmCommand;

        public ICommand BatchFilmCommand
        {
            get { return _batchFilmCommand ?? (_batchFilmCommand = new RelayCommand(param => BatchFilm(), param => IsEnableBatchFilm)); }
        }

        private void BatchFilm()
        {
            try
            {
                Logger.LogFuncUp();

                OnBatchFilmClick(null, null);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }


        public void OnBatchFilmClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                var items = seriesSelectionCtrl.SelectedSeriesItems;
                FilmingUtility.AssertEnumerable(items);
                //Debug.Assert(items.Count == 1);
                //var item = items.FirstOrDefault();
                //Debug.Assert(item != null);

                //int totalCount = item.ImageCount;
                var totalCount = items.Sum(item => item.ImageCount);

                Card.InterSelection.ViewModel.TotalImages = (uint)totalCount;
                Card.InterSelection.CheckNullParameter();
                Card.InterSelection.CreateMemento();

                var messageWindow = new MessageWindow
                {
                    WindowTitle = Card.Resources["UID_Filming_InterLeavedSelection_Title"] as string,
                    WindowChild = Card.InterSelection,
                    WindowDisplayMode = WindowDisplayMode.Default
                };
                Card.HostAdornerCount++;
                messageWindow.ShowModelDialog();
                Card.HostAdornerCount--;
                if (Card.InterSelection.IsQuit == true)
                {
                    FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
                    Card.InterSelection.RestoreMemento();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }




       



        public bool IsEnabelCompareWindowing
        {
            get
            {
                var items = seriesSelectionCtrl.SelectedSeriesItems;
                if (null == items || items.Count != 1)
                {
                    return false;
                }
                var item = items.FirstOrDefault();
                if (null == item || item.ImageCount <= 0 || null == item.SeriesDBData || "CT" != item.SeriesDBData.Modality.ToUpper()
                    || Card.IsEFilmSeries(item))
                {
                    return false;
                }

                return true;
            }
        }

        private ICommand _compareWindowingFilmCommand;

        public ICommand CompareWindowingFilmCommand
        {
            get
            {
                return _compareWindowingFilmCommand ??
                       (_compareWindowingFilmCommand = new RelayCommand(param => CompareWindowingFilm()));
            }
        }

        private void CompareWindowingFilm()
        {
            try
            {
                Logger.LogFuncUp();

                if (seriesSelectionCtrl.SelectedSeriesItems.Count == 1)
                {
                    var seriesVM = seriesSelectionCtrl.SelectedSeriesItems[0];
                    if (seriesVM != null)
                    {
                        var singleSeriesCompareControl = new SingleSeriesComparePrintWindow(seriesVM);

                        var messageWindow = new MessageWindow
                        {
                            WindowTitle = Card.Resources["UID_Filming_Single_Series_Compare_Print_Setting_Window_Title"] as string,
                            WindowChild = singleSeriesCompareControl,
                            WindowDisplayMode = WindowDisplayMode.Default
                        };
                        Card.HostAdornerCount++;
                        messageWindow.ShowModelDialog();
                        Card.HostAdornerCount--;
                        if (singleSeriesCompareControl.IsQuit == true)
                            FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
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

        public bool IsEnableCompareSeries
        {
            get
            {
                var items = seriesSelectionCtrl.SelectedSeriesItems;
                if (items == null)
                {
                    return false;
                }
                if (items.Count != 2 && items.Count != 3)
                {
                    return false;
                }
                if (items.Any((item) => item.SeriesDBData == null || Card.IsEFilmSeries(item)))
                {
                    return false;
                }
                //DIM 208618中PO定义图像数目不同的series不允许对比
                if (items.Any((item) => item.ImageCount != items[0].ImageCount))
                {
                    return false;
                }

                return true;
            }
        }

        private ICommand _compareSeriesFilmCommand;

        public ICommand CompareSeriesFilmCommand
        {
            get
            {
                return _compareSeriesFilmCommand ??
                       (_compareSeriesFilmCommand = new RelayCommand(param => CompareFilm(), param => IsEnableCompareFilm));
            }
        }

        private void CompareSeriesFilm()
        {
            try
            {
                Logger.LogFuncUp();

                _selectedCompareSeriesList.Clear();

                var seriesItems = seriesSelectionCtrl.SelectedSeriesItems;

                var db = FilmingDbOperation.Instance.FilmingDbWrapper;


                foreach (var item in seriesItems)
                {
                    var images =
                        db.GetImageListByConditionWithOrder("SeriesInstanceUIDFk='" + item.SeriesInstanceUID + "'",
                                                            "InstanceNumber");
                    _selectedCompareSeriesList.Add(images);
                }

                ShowSeriesComparePrintWindow();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        public bool IsEnableCompareFilm
        {
            get { return IsEnabelCompareWindowing || IsEnableCompareSeries; }
        }

        public void CompareFilm()
        {
            try
            {
                Logger.LogFuncUp();
                //windowing compare mode
                if (IsEnabelCompareWindowing)
                {
                    CompareWindowingFilm();
                }
                //series compare mode
                else if (IsEnableCompareSeries)
                {
                    CompareSeriesFilm();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private SeriesComparePrintWindow _seriesCompareWindow;
        private readonly List<List<ImageBase>> _selectedCompareSeriesList = new List<List<ImageBase>>();
        private int _compareSeriesCount;

        public SeriesComparePrintWindow SeriesCompareWindow
        {
            get { return _seriesCompareWindow ?? (_seriesCompareWindow = new SeriesComparePrintWindow()); }
        }

        private void ShowSeriesComparePrintWindow()
        {
            try
            {
                Logger.LogFuncUp();

                _compareSeriesCount = _selectedCompareSeriesList.Count;
                if (_compareSeriesCount != 2 && _compareSeriesCount != 3)
                {
                    return;
                }

                SeriesCompareWindow.SeriesCount = _compareSeriesCount;

                SeriesCompareWindow.InitUI();

                var items = seriesSelectionCtrl.SelectedSeriesItems;
                SeriesCompareWindow.Series1ImageCount = items[0].ImageCount;
                SeriesCompareWindow.Series1Modality = items[0].SeriesDBData.Modality;
                SeriesCompareWindow.Series2ImageCount = items[1].ImageCount;
                SeriesCompareWindow.Series2Modality = items[1].SeriesDBData.Modality;
                SeriesCompareWindow.UpdateSeries1Description(items[0].SeriesDescription);
                SeriesCompareWindow.UpdateSeries2Description(items[1].SeriesDescription);
                if (_compareSeriesCount == 3)
                {
                    SeriesCompareWindow.Series3ImageCount = items[2].ImageCount;
                    SeriesCompareWindow.Series3Modality = items[2].SeriesDBData.Modality;
                    SeriesCompareWindow.UpdateSeries3Description(items[2].SeriesDescription);
                }

                WindowHostManagerWrapper.IsQuittingJob = true;
                //WindowHostManagerWrapper.ShowSecondaryWindow(SeriesCompareWindow,
                //                                             Resources["UID_Filming_Series_Compare_Window_Title"] as
                //                                             string);
                var messageWindow = new MessageWindow
                {
                    WindowTitle = Card.Resources["UID_Filming_Series_Compare_Window_Title"] as string,
                    WindowChild = SeriesCompareWindow,
                    WindowDisplayMode = WindowDisplayMode.Default
                };
                Card.HostAdornerCount++;
                //messageWindow.Closing -= SeriesCompareWindow.OnCancelButtonClick;
                //messageWindow.Closing += SeriesCompareWindow.OnCancelButtonClick;
                messageWindow.ShowModelDialog();
                Card.HostAdornerCount--;

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }



        private FilmImageObject GetFilmImageObjectFromSeriesViewModel(List<ImageBase> seriesModel, int index)
        {
            FilmImageObject imageObject = null;
            var imageModel = seriesModel[index];
            if (imageModel != null)
            {
                imageObject = new FilmImageObject { ImageSopInstanceUid = imageModel.SOPInstanceUID };
            }

            return imageObject;
        }

        private void AddVerticalGroupImageToCompareFilmPage(FilmingPageControl page, int startIndex, ref int cellIndex)
        {
            try
            {
                Logger.LogFuncUp();

                // series 1
                var seriesModel = _selectedCompareSeriesList[0];
                var imageObject = GetFilmImageObjectFromSeriesViewModel(seriesModel, startIndex);
                if (imageObject != null)
                {
                    Card._dataLoader.LoadSopByUid(imageObject.ImageSopInstanceUid);

                }
                cellIndex++;

                // series 2
                seriesModel = _selectedCompareSeriesList[1];
                imageObject = GetFilmImageObjectFromSeriesViewModel(seriesModel, startIndex);
                if (imageObject != null)
                {
                    Card._dataLoader.LoadSopByUid(imageObject.ImageSopInstanceUid);
                }
                cellIndex++;

                // series 3
                if (_compareSeriesCount == 3)
                {
                    seriesModel = _selectedCompareSeriesList[2];
                    imageObject = GetFilmImageObjectFromSeriesViewModel(seriesModel, startIndex);
                    if (imageObject != null)
                    {
                        Card._dataLoader.LoadSopByUid(imageObject.ImageSopInstanceUid);
                    }
                    cellIndex++;
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        private void AddHorizontalImageToCompareFilmPage(List<ImageBase> seriesModel, FilmingPageControl page,
                                                         int startIndex, int count, int maxImageCount, ref int cellIndex)
        {
            try
            {
                Logger.LogFuncUp();

                for (int i = 0; i < count; i++)
                {
                    if (startIndex + i < maxImageCount)
                    {
                        var imageObject = GetFilmImageObjectFromSeriesViewModel(seriesModel, startIndex + i);
                        if (imageObject != null)
                        {
                            Card._dataLoader.LoadSopByUid(imageObject.ImageSopInstanceUid);
                        }
                    }

                    cellIndex++;
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        public void OnSeriesCompareSettingChanged(object sender, SeriesCompareSettingChangedEventArgs args)
        {
            try
            {
                Logger.LogFuncUp();

                var seriesUidAndCount = new List<Tuple<string, int>>();
                foreach (var series in seriesSelectionCtrl.SelectedSeriesItems)
                {
                    seriesUidAndCount.Add(new Tuple<string, int>(series.SeriesInstanceUID, series.ImageCount));
                }
                if (!FilmingHelper.CheckMemoryForLoadingSeries(seriesUidAndCount, FilmingViewerContainee.Main.GetCommunicationProxy()))
                {
                    MessageBoxHandler.Instance.ShowError("UID_Filming_Warning_NoEnoughMemory");
                    return;
                }

                bool isVertical = args.IsVertical;
                int imageCount = args.ImageCount;
                int totalImageCount = imageCount * _compareSeriesCount;
                int rows = args.Rows;
                int columns = args.Columns;

                var pageCount = (int)Math.Ceiling(totalImageCount * 1.0 / (rows * columns));

                int startIndex = 0;
                Card.ImagesLoadBeginning((uint)totalImageCount);


                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        Card.DeleteLastEmptyFilmPageIfHave();
                        var page = Card.AddFilmPage(new FilmLayout(rows, columns));
                        page.FilmPageType = FilmPageType.BreakFilmPage;

                        //page.ViewportLayout = new FilmLayout(rows, columns);

                        Card.ActiveFilmingPageList.UnSelectAllCells();

                        Card.DisplayFilmPage(page);
                        Card.ReOrderCurrentFilmPageBoard();

                        // select page
                        Card.SelectObject(page, null, null);

                        Card._pagesToBeRefreshed.Clear();
                        Card._cellsToBeMoveForward.Clear();
                        Card._loadingTargetPage = page;
                        Card._loadingTargetCellIndex = 0;

                        for (int i = 0; i < pageCount; i++)
                        {
                            int cellIndex = 0;

                            Card.SeriesCompareSettingForAdjust = args;

                            if (isVertical)
                            {
                                for (int row = 0; row < rows; row++)
                                {
                                    int columnGroup = columns / _compareSeriesCount;
                                    for (int column = 0;
                                         column < columnGroup;
                                         column++)
                                    {
                                        Dispatcher.Invoke(new Action(() =>
                                        {
                                            AddVerticalGroupImageToCompareFilmPage
                                                (page,
                                                 startIndex,
                                                 ref
                                                                                                                            cellIndex);
                                        }),
                                                          DispatcherPriority.
                                                              Background);

                                        startIndex += 1;
                                        if (startIndex >= imageCount)
                                        {
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int row = 0; row < rows; row++)
                                {
                                    int type = row % _compareSeriesCount;
                                    var seriesModel =
                                        _selectedCompareSeriesList[type];
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        if (
                                            type ==
                                            0)
                                        {
                                            // Series 1
                                            AddHorizontalImageToCompareFilmPage
                                                (seriesModel,
                                                 page,
                                                 startIndex,
                                                 columns,
                                                 imageCount,
                                                 ref
                                                                                                                            cellIndex);
                                        }
                                        else if (
                                            type ==
                                            1)
                                        {
                                            // Series 2
                                            AddHorizontalImageToCompareFilmPage
                                                (seriesModel,
                                                 page,
                                                 startIndex,
                                                 columns,
                                                 imageCount,
                                                 ref
                                                                                                                            cellIndex);
                                        }
                                        else
                                        {
                                            // Series 3
                                            AddHorizontalImageToCompareFilmPage
                                                (seriesModel,
                                                 page,
                                                 startIndex,
                                                 columns,
                                                 imageCount,
                                                 ref
                                                                                                                            cellIndex);
                                        }
                                    }),
                                                      DispatcherPriority.
                                                          Background);

                                    if (_compareSeriesCount == type + 1)
                                    {
                                        startIndex += columns;
                                        if (startIndex >= imageCount)
                                        {
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogFuncException(ex.Message + ex.StackTrace);
                    }
                }), DispatcherPriority.Background);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            finally
            {
                Card.DisableUIForElecFilmOperation(false);
            }
        }


        public void AdjustSeriesCompare()
        {
            try
            {
                Logger.LogFuncUp();

                //get film
                int filmCount = (Card.SeriesCompareSettingForAdjust.ImageCount * _compareSeriesCount) /
                                (Card.SeriesCompareSettingForAdjust.Rows * Card.SeriesCompareSettingForAdjust.Columns) + 1;

                for (int i = filmCount - 1; i < filmCount; i++)
                {
                    var film = Card.EntityFilmingPageList[Card.EntityFilmingPageList.Count - filmCount + i];
                    IList<MedViewerControlCell> cells = film.Cells.Where(cell => !cell.IsEmpty).ToList();
                    int cellCount = cells.Count;
                    if (!Card.SeriesCompareSettingForAdjust.IsVertical)
                    {
                        int step = Card.SeriesCompareSettingForAdjust.Columns;
                        for (int cellIndex = 0; cellIndex < cellCount; )
                        {
                            int cellRange = cellCount - cellIndex >= _compareSeriesCount * step
                                                ? step
                                                : (cellCount - cellIndex) / _compareSeriesCount;
                            //if there has 2 entire row of cells contains image
                            if (cellRange == 0) return;

                            cellIndex += _compareSeriesCount * cellRange;
                            //insert empty cells For last comparing row
                            if (cellRange < step)
                            {
                                for (int j = 1; j < _compareSeriesCount; j++)
                                {
                                    int emptycellIndex = cellIndex - cellRange * j;
                                    //for (columnIndex = cellRange; columnIndex < step; columnIndex++)
                                    //{
                                    //    insertEmptyCell(film, emptycellIndex++);
                                    //}
                                    //InsertEmptyCells(film, emptycellIndex, cellRange);
                                    if (InsertEmptyCells(film, emptycellIndex, step - cellRange))
                                        film.Cells.ToList().GetRange(emptycellIndex + step - cellRange, cellRange).ForEach(c => c.IsSelected = true);
                                }
                            }
                        }
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
                Card.SeriesCompareSettingForAdjust = null;
            }
        }

        public bool InsertEmptyCells(FilmingPageControl film, int cellIndex, int cellCount)
        {
            try
            {
                Logger.LogFuncUp();

                var isEnableRepack = Card.IsEnableRepack;
                if (isEnableRepack) return false;

                Card._dropFilmingPage = film;
                Card._dropViewCell = film.Cells.ElementAt(cellIndex);
                Card.SetFocusPosition();

                var cells = Enumerable.Repeat(new MedViewerControlCell(), cellCount).ToList();
                Card.InsertCells(cells);

                film.Cells.ToList().GetRange(cellIndex, cellCount).ForEach(c => c.IsSelected = false);

                Logger.LogFuncDown();

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        private List<SeriesTreeViewItemModel> _dragSeriesList = new List<SeriesTreeViewItemModel>();

        
        private void FilterEmptyAndUnsupportSeries()
        {
            _dragSeriesList.Clear();
            _dragSeriesList =
                (from series in seriesSelectionCtrl.SelectedSeriesItems where (/*series.ImageSource != null &&*/ series.ImageCount > 0) select series).ToList();
        }

        private void FilterEmptyAndUnsupportImages()
        {
            
        }

        private List<SeriesTreeViewItemModel> GetUnsupportSeriesList(IList<SeriesTreeViewItemModel> selectSeriesItems)
        {
            var unsupportSeriesList = new List<SeriesTreeViewItemModel>();
            unsupportSeriesList = (from series in selectSeriesItems where (series.ImageSource == null) select series).ToList();
            return unsupportSeriesList;
        }

        private bool IsEnabledLoad()
        {
            FilmingUtility.AssertEnumerable(_dragSeriesList);
            if (_dragSeriesList.Count > 1 &&
                (_dragSeriesList.Any((series) => Card.IsEFilmSeries(series) && series.ImageCount != 0)))
            {
                return false;
            }
            return true;
        }

        //if dragged series and Image displayed is mixed?
        private bool IsMixedLoad()
        {
            if (Card.FilmingCardModality == string.Empty)
            {
                return false;
            }

            FilmingUtility.AssertEnumerable(_dragSeriesList);

            string filmingCardModality = Card.FilmingCardModality;

            if (filmingCardModality != FilmingUtility.EFilmModality &&
                (_dragSeriesList.All((series) => !Card.IsEFilmSeries(series))))
            {
                return false;
            }
            if (filmingCardModality == FilmingUtility.EFilmModality && _dragSeriesList.Count == 1)
            {
                var series = _dragSeriesList.FirstOrDefault();
                if (Card.IsEFilmSeries(series) && Card.SeriesUID == series.SeriesInstanceUID)
                {
                    return false;
                }
            }
            return true;
        }

        private bool _bPreLoading = true;

        public void OnDrop(FilmingPageControl film)
        {
            //Logger.LogBegin("加载图片");
            _bPreLoading = true;
            try
            {
                Logger.LogFuncUp();

                #region [OnDrop Assert]

                Card._dropFilmingPage = film;

                bool isEnableDrop = Card._dropFilmingPage == null || !Card._dropFilmingPage.IsInSeriesCompareMode;
                //add film from PA or study list ctrl
                if (!isEnableDrop) return;

                if (Card._dropFilmingPage == null)
                {
                    Card._dropViewPort = null;
                    Card._dropViewCell = null;
                }

                var unsupportDataSeries = GetUnsupportSeriesList(seriesSelectionCtrl.SelectedSeriesItems);
                if (unsupportDataSeries.Count > 0)
                {
                    Logger.LogWarning("There are " + unsupportDataSeries.Count + " unsupported image series, will skip.");
                    int unsupportImageCount = unsupportDataSeries.Sum(series => series.ImageCount);
                    //FilmingViewerContainee.ShowStatusWarning("UID_Filming_Warning_Unsupport_Images", unsupportImageCount);
                    FilmingViewerContainee.Main.ShowStatusWarning("UID_Filming_Warning_Unsupport_Images", unsupportImageCount);
                }

                FilterEmptyAndUnsupportSeries();

                if (_dragSeriesList == null || _dragSeriesList.Count == 0)
                {
                    Logger.Instance.LogTraceInfo("Nothing to be loaded");
                    return;
                }

                #endregion [OnDrop Assert]
                
                if (!IsEnabledLoad())
                {
                    MessageBoxHandler.Instance.ShowSysWarning("UID_Filming_Cannot_Load_Films");
                    //修复DIM 504756
                    if (film != null)
                        film.IsSelected = true;
                    else
                    {
                        film = Card.EntityFilmingPageList.LastOrDefault();
                        film.IsSelected = true;
                        film.Cells.FirstOrDefault().IsSelected = true;
                    }
                    return;
                }

                //CreateImageTime = 0;
                Card.EntityFilmingPageList.UnSelectAllCells();

                //McsfFilmViewport vp = (Card._dropViewPort == null) ? null : new McsfFilmViewport(Card._dropViewPort, film.ViewportLayout.LayoutType != LayoutTypeEnum.DefinedLayout);
                if (film != null) Card.SelectObject(film, null, Card._dropViewCell);
                
                var loadCount=(uint)_dragSeriesList.Aggregate(0, (total, current) => total += current.ImageCount);               

                if (loadCount > FilmingUtility.COUNT_OF_IMAGES_WARNING_THRESHOLD_ONCE_LOADED)
                {
                    if (MessageBoxResponse.NO == MessageBoxHandler.Instance.ShowSysQuestion(
                        "UID_Filming_Warning_LoadingImagesWhichMayBeSlowForCountExceedThreshold",
                        LoadImageThresholdHandler))
                    {
                        if (Card.ActiveFilmingPageList.Count == 0)
                        {

                            Card.EntityFilmingPageList[0].IsSelected = true;
                            Card.ActiveFilmingPageList.AddPage(Card.EntityFilmingPageList[0]);
                            Card.ActiveFilmingPageList[0].GetCellByIndex(0).IsSelected = true;
                        }
                    }
                }
                else
                {
                    LoadImageThresholdHandler();
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
                if (_bPreLoading) FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
            }
        }

        public void OnDropMultiImages(List<string> imageUids)
        {
            bool sentry = true;
            try
            {
                var images = imageUids.Select(imageUid => DBWrapperHelper.DBWrapper.GetImageBaseBySOPInstanceUID(imageUid)).ToList();
                if (!IsMixedLoad(images))
                {
                    DropImages(images);
                }
                else
                {
                    MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Load_Different_Films_Need_To_Clear_Old_Films", (MessageBoxResponse response) => DropImages(images, response));
                }

                sentry = false;
            }
            catch (Exception e)
            {
                Logger.LogFuncException(e.StackTrace);
            }
            finally
            {
                if (sentry) FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
            }
        }

        private void LoadImageThresholdHandler(MessageBoxResponse response = MessageBoxResponse.YES)
        {
            try
            {
                Logger.LogFuncUp();

                if (response != MessageBoxResponse.YES)
                {
                    Card.UpdateUIStatus();
                    return;
                }

                if (!IsMixedLoad())
                {
                    LoadFilmHandler();
                }
                else
                {
                    MessageBoxHandler.Instance.ShowSysQuestion("UID_Filming_Load_Different_Films_Need_To_Clear_Old_Films",
                                                            LoadFilmHandler);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        private void LoadFilmHandler(MessageBoxResponse response = MessageBoxResponse.OK)
        {
            try
            {
                Logger.LogFuncUp();
                Logger.LogTimeStamp("[Begin To Load Images]");
                //Stopwatch sw=new Stopwatch();
                //sw.Start();
                if (response == MessageBoxResponse.NO || response == MessageBoxResponse.NONE)
                {
                    return;
                }

                FilmingUtility.AssertEnumerable(_dragSeriesList);
                //assert there is not the case that no image to be load

                bool toLoadSC = _dragSeriesList.All(Card.IsEFilmSeries);

                if (response == MessageBoxResponse.YES) //need to clear old films
                {
                    Card.DeleteAllFilmPage();
                    
                    
                    
                    FilmingPageControl film = Card.AddFilmPage();
                    if (toLoadSC)
                    {
                        Card.SelectObject(film, null, null);
                        film.SelectedAll(true);
                        film.ViewportLayout = new FilmLayout(1, 1);

                    }
                    Card._dropFilmingPage = film;
                    Card.LastSelectedFilmingPage = film;
                    Card._dropViewCell = null;
                    Card.LastSelectedCell = null;
                    Card.UpdateImageCount();
                }
                if (response == MessageBoxResponse.OK) //load common images
                {
                    if (!Card.IsAnyImageLoaded && toLoadSC)
                    {
                        Card.DeleteAllFilmPage();

                        FilmingPageControl film = Card.AddFilmPage();
                        Card._dropFilmingPage = film;

                        Card.SelectObject(film, null, null);
                        film.SelectedAll(true);
                        film.ViewportLayout = new FilmLayout(1, 1);
                        Card.UpdateImageCount();
                    }
                    else
                    {
                        GetLastSelectedCells();

                        FilmPageUtil.UnselectOtherFilmingPages(Card._activeFilmingPageList, Card._dropFilmingPage);

                    }
                }
                LoadSeries();
                //sw.Stop();
                //MessageBox.Show("Load Image:" + sw.ElapsedMilliseconds.ToString());
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }

        }

        private IList<MedViewerControlCell> _lastSelectedCells = new List<MedViewerControlCell>();
      
        private void GetLastSelectedCells()
        {
            _lastSelectedCells.Clear();
            foreach (var film in Card.EntityFilmingPageList)
            {
                foreach (var cell in film.SelectedCells())
                {
                    _lastSelectedCells.Add(cell);
                }
            }

        }

        private List<string> loadedStudyList = new List<string>();
        public List<string> LoadedStudyList
        {
            get { return loadedStudyList; }
            set { loadedStudyList = value; }
        }

        public void SwitchToFilmingWithSeries(IEnumerable<string> series)
        {
            try
            {
                Logger.LogFuncUp();
                var db = FilmingDbOperation.Instance.FilmingDbWrapper;
                var seriesList = series.ToList();
                foreach (var itemSeries in series)
                {
                    var seriesBase = db.GetSeriesBaseBySeriesInstanceUID(itemSeries);
                    var studyUid = seriesBase.StudyInstanceUIDFk;
                    if (loadedStudyList.Contains(studyUid)) seriesList.Remove(itemSeries);
                }
                //select Series from PA
                this.seriesSelectionCtrl.SelectSeries(seriesList);

                foreach (var itemSeries in seriesList)
                {
                    var seriesBase = db.GetSeriesBaseBySeriesInstanceUID(itemSeries);
                    var studyUid = seriesBase.StudyInstanceUIDFk;
                    if (!loadedStudyList.Contains(itemSeries))
                        loadedStudyList.Add(studyUid);
                }

                OnDrop((FilmingPageControl)(null));

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void SwitchToSeriesCompare(List<string> seriesUidList)
        {
            seriesSelectionCtrl.SelectSeries(seriesUidList.ToList());
            
            if (!IsEnableCompareFilm)
            {
                MessageBoxHandler.Instance.ShowError("UID_MessageBox_Series_NOT_Compare");
                return;
            }
            this.Card.BtnEditCtrl.OnCompareSeriesFilmClick(null, null);
        }


        private void LoadSeries()
        {
            try
            {
                Logger.LogTimeStamp("开始加载序列");
                Logger.LogFuncUp();
                Card.StartLoadTime = DateTime.Now;

                FilmingUtility.AssertEnumerable(_dragSeriesList);
                var seriesUidAndCount = new List<Tuple<string, int>>();
                foreach (var series in _dragSeriesList)
                {
                    seriesUidAndCount.Add(new Tuple<string, int>(series.SeriesInstanceUID, series.ImageCount));
                }
                if (!FilmingHelper.CheckMemoryForLoadingSeries(seriesUidAndCount, FilmingViewerContainee.Main.GetCommunicationProxy()))
                {
                    MessageBoxHandler.Instance.ShowError("UID_Filming_Warning_NoEnoughMemory");
                    return;
                }

                uint imageCount = 0;
                //get image count
                foreach (var series in _dragSeriesList)
                {
                    Debug.Assert(series != null);
                    imageCount += (uint)series.ImageCount;
                }

                Logger.Instance.LogPerformanceRecord("[Begin][Load images] " + imageCount);

                //ImagesLoadBeginning(imageCount);

                // Drop series on page header, then append the series to the last page in the same segment(page break)
                if (Card._dropViewCell == null)
                {
                    Card._cellsToBeMoveForward.Clear();
                    Card._pagesToBeRefreshed.Clear();

                    if (Card._dropFilmingPage != null)
                    {
                        Card._loadingTargetPage = Card.GetLastPageToHoldImage(Card.GetLinkedPage(Card._dropFilmingPage)); //
                        Card._pagesToBeRefreshed = Card.GetLinkedPage(Card._loadingTargetPage);
                        Card._pagesToBeRefreshed.Remove(Card._loadingTargetPage);
                    }
                    else
                        Card._loadingTargetPage = Card.EntityFilmingPageList.Last();

                    Card._loadingTargetCellIndex = Card._loadingTargetPage.LastNonEmptyCellIndex;

                    // There is no empty cell in the end in this page, add a new page
                    if (Card._loadingTargetCellIndex == Card._loadingTargetPage.Cells.Count() - 1)
                    {
                        // insert the newly created page after _loadingTargetPage
                        Card._loadingTargetPage = Card.InsertFilmPage(Card._loadingTargetPage.FilmPageIndex + 1,
                                                            Card._loadingTargetPage.ViewportLayout);
                        if (Card._loadingTargetPage.FilmPageIndex < (Card.CurrentFilmPageBoardIndex + 1) * Card.FilmingCardColumns * Card.FilmingCardRows
                            && Card._loadingTargetPage.FilmPageIndex > Card.CurrentFilmPageBoardIndex * Card.FilmingCardColumns * Card.FilmingCardRows)
                        {
                            Dispatcher.Invoke(new Action(() => Card.DisplayFilmPage(Card._loadingTargetPage)));
                        }
                        Card._loadingTargetCellIndex = 0;
                    }
                    else
                    {
                        Card._loadingTargetCellIndex = (Card._loadingTargetCellIndex == -1) ? 0 : Card._loadingTargetCellIndex + 1;
                    }
                }
                else
                {
                    Card._loadingTargetPage = Card._dropFilmingPage;
                    if (Card._dropViewCell.CellIndex < 0)
                        Card._loadingTargetCellIndex = 0;
                    else
                        Card._loadingTargetCellIndex = Card._dropViewCell.CellIndex;
                    Card.BackupCells(Card._dropFilmingPage, Card._dropViewCell, imageCount);
                }

                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag
                    + "[Film]" + Card._loadingTargetPage.FilmPageIndex
                    + "[Cell]" + Card._loadingTargetCellIndex
                    + "[ImageCount]" + imageCount);

                var thread = new Thread(LoadSeriesHelperMethod);
                thread.Start();


                _bPreLoading = false;

                Logger.LogFuncDown();

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                //throw;
            }

            finally
            {
                bool toLoadSC = _dragSeriesList.All(Card.IsEFilmSeries);
                Card.DisableUIForElecFilmOperation(toLoadSC);
                //Logger.Instance.LogPerformanceRecord("结束加载序列");
            }
        }

        private void LoadSeriesHelperMethod()
        {
            try
            {
                Logger.LogFuncUp();

                var allImages = new List<ImageBase>();
                var db = FilmingDbOperation.Instance.FilmingDbWrapper;
                foreach (var series in _dragSeriesList)
                {
                    
                    var images = db.GetImageListBySeriesInstanceUID(series.SeriesInstanceUID).OrderBy(image => image.InstanceNumber);

                    allImages.AddRange(images);
                    //_dataLoader.LoadSeries(series.SeriesInstanceUID);
                }

                Dispatcher.Invoke(new Action(() => Card.ImagesLoadBeginning((uint)allImages.Count)));

                foreach (var image in allImages)
                {
                    Card._dataLoader.LoadSopByUid(image.SOPInstanceUID);
                }

                Logger.LogFuncDown();

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void SwitchFilmingWithImages(List<string> imageUiDs)
        {
            bool sentry = true;
            try
            {
                var images = imageUiDs.Select(imageUid => DBWrapperHelper.DBWrapper.GetImageBaseBySOPInstanceUID(imageUid)).ToList();
                if (!IsMixedLoad(images))
                {
                    LoadImages(images);
                }
                else
                {
                    MessageBoxHandler.Instance.ShowQuestion("UID_Filming_Load_Different_Films_Need_To_Clear_Old_Films", (MessageBoxResponse response) => LoadImages(images, response));
                }

                sentry = false;
            }
            catch (Exception e)
            {
                Logger.LogFuncException(e.StackTrace);
            }
            finally
            {
                if (sentry) FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
            }
        }

        private bool IsMixedLoad(IEnumerable<ImageBase> images)
        {
            if (Card.FilmingCardModality == string.Empty) return false;

            int efImageCount = images.Count(i => i.ImageType == FilmingUtility.EFilmImageType);

            if (Card.FilmingCardModality != FilmingUtility.EFilmModality && efImageCount == 0) return false;
            if (Card.FilmingCardModality == FilmingUtility.EFilmModality && efImageCount != 0)
            {
                if (images.First().SeriesInstanceUIDFk == Card.SeriesUID) return false;
            }

            return true;
        }

        private void LoadImages(IEnumerable<ImageBase> images, MessageBoxResponse response = MessageBoxResponse.OK)
        {
            if (response == MessageBoxResponse.NO || response == MessageBoxResponse.NONE)
            {
                return;
            }
            if (images==null) return;
            bool toLoadEfImage = images.First().ImageType == FilmingUtility.EFilmImageType;
            if (response == MessageBoxResponse.YES)
            {
                if (toLoadEfImage)
                {
                    ChangeToEFLayout();
                }
                else
                {
                    ChangeToNormalLayout();
                }

                Card.UpdateImageCount();
            }

            if (response == MessageBoxResponse.OK)
            {
                if (!Card.IsAnyImageLoaded && toLoadEfImage) ChangeToEFLayout();
            }
            if (images.Count() > FilmingUtility.COUNT_OF_IMAGES_WARNING_THRESHOLD_ONCE_LOADED)
            {
                MessageBoxHandler.Instance.ShowSysQuestion("UID_Filming_Warning_LoadingImagesWhichMayBeSlowForCountExceedThreshold", (MessageBoxResponse thresholdresponse) => DoLoadImages(images, thresholdresponse));
            }
            else
            {
                DoLoadImages(images);
            }
        }
        private void DropImages(IEnumerable<ImageBase> images, MessageBoxResponse response = MessageBoxResponse.OK)
        {
            if (response == MessageBoxResponse.NO || response == MessageBoxResponse.NONE)
            {
                return;
            }
            bool toLoadEfImage = images.First().ImageType == FilmingUtility.EFilmImageType;
            if (response == MessageBoxResponse.YES)
            {
                if (toLoadEfImage)
                {
                    ChangeToEFLayout();
                }
                else
                {
                    ChangeToNormalLayout();
                }

                Card.UpdateImageCount();
            }

            if (response == MessageBoxResponse.OK)
            {
                if (!Card.IsAnyImageLoaded && toLoadEfImage) ChangeToEFLayout();
            }
            if (images.Count() > FilmingUtility.COUNT_OF_IMAGES_WARNING_THRESHOLD_ONCE_LOADED)
            {
                MessageBoxHandler.Instance.ShowSysQuestion("UID_Filming_Warning_LoadingImagesWhichMayBeSlowForCountExceedThreshold", (MessageBoxResponse thresholdresponse) => DoDropImages(images, thresholdresponse));
            }
            else
            {
                DoDropImages(images);
            }
        }
        private void DoLoadImages(IEnumerable<ImageBase> images, MessageBoxResponse response = MessageBoxResponse.YES)
        {
            if (response == MessageBoxResponse.NO || response == MessageBoxResponse.NONE)
            {
                return;
            }
            Card.SetTargetToReceiveImages();
            Card.ImagesLoadBeginning((uint)images.Count());
            var thread = new Thread(() => images.ToList().ForEach(image => Card._dataLoader.LoadSopByUid(image.SOPInstanceUID)));
            thread.Start();
        }

        private void DoDropImages(IEnumerable<ImageBase> images, MessageBoxResponse response = MessageBoxResponse.YES)
        {
            if (response == MessageBoxResponse.NO || response == MessageBoxResponse.NONE)
            {
                return;
            }
            var imageBases = images as IList<ImageBase> ?? images.ToList();
            if(!imageBases.Any() ) return;

            if (Card._dropViewCell == null)
            {
                Card._cellsToBeMoveForward.Clear();
                Card._pagesToBeRefreshed.Clear();

                if (Card._dropFilmingPage != null)
                {
                    Card._loadingTargetPage = Card.GetLastPageToHoldImage(Card.GetLinkedPage(Card._dropFilmingPage)); //
                    Card._pagesToBeRefreshed = Card.GetLinkedPage(Card._loadingTargetPage);
                    Card._pagesToBeRefreshed.Remove(Card._loadingTargetPage);
                }
                else
                    Card._loadingTargetPage = Card.EntityFilmingPageList.Last();

                Card._loadingTargetCellIndex = Card._loadingTargetPage.LastNonEmptyCellIndex;

                // There is no empty cell in the end in this page, add a new page
                if (Card._loadingTargetCellIndex == Card._loadingTargetPage.Cells.Count() - 1)
                {
                    // insert the newly created page after _loadingTargetPage
                    Card._loadingTargetPage = Card.InsertFilmPage(Card._loadingTargetPage.FilmPageIndex + 1,
                                                        Card._loadingTargetPage.ViewportLayout);
                    if (Card._loadingTargetPage.FilmPageIndex < (Card.CurrentFilmPageBoardIndex + 1) * Card.FilmingCardColumns * Card.FilmingCardRows
                        && Card._loadingTargetPage.FilmPageIndex > Card.CurrentFilmPageBoardIndex * Card.FilmingCardColumns * Card.FilmingCardRows)
                    {
                        Dispatcher.Invoke(new Action(() => Card.DisplayFilmPage(Card._loadingTargetPage)));
                    }
                    Card._loadingTargetCellIndex = 0;
                }
                else
                {
                    Card._loadingTargetCellIndex = (Card._loadingTargetCellIndex == -1) ? 0 : Card._loadingTargetCellIndex + 1;
                }
            }
            else
            {
                Card._loadingTargetPage = Card._dropFilmingPage;
                if (Card._dropViewCell.CellIndex < 0)
                    Card._loadingTargetCellIndex = 0;
                else
                    Card._loadingTargetCellIndex = Card._dropViewCell.CellIndex;
                Card.BackupCells(Card._dropFilmingPage, Card._dropViewCell,(uint) imageBases.Count());
            }
            Card.ImagesLoadBeginning((uint)imageBases.Count());
            var thread = new Thread(() => imageBases.ToList().ForEach(image => Card._dataLoader.LoadSopByUid(image.SOPInstanceUID)));
            thread.Start();
        }


        public void ChangeToNormalLayout()
        {
            Card.DeleteAllFilmPage();
            var film = Card.OnAddFilmPageAfterClearFilmingCard(null, null);
            Card.SelectObject(film, null, null);
            //film.SelectedAll(true);
        }

        public void ChangeToEFLayout()
        {
            Card.DeleteAllFilmPage();
            var film = Card.AddFilmPage();
            Dispatcher.Invoke(new Action(() =>
            {
                Card.SelectObject(film, film.ViewportList.FirstOrDefault(), null);
                Card.SelectObject(film, null, null);
                film.SelectedAll(true);
                film.SetSelectedCellLayout(new FilmLayout(1, 1));
                Card.DisableUIForElecFilmOperation();
            }));
        }

        #region Add test code for ImageJobWorker

        
        public void AddImagesToFilmCard(ImageJobModel imageJobModel)
        {
            try
            {
                //////////////////////////////////////////////////////////////////////////
                //电子胶片DataHeader处理
                string dataHeaderModality = imageJobModel.Modality;
                string dataHeaderSeriesUID = imageJobModel.SeriesInstanceUid;

                var filmingCardModality = Card.FilmingCardModality;

                if (dataHeaderModality == FilmingUtility.EFilmModality)
                {

                    if (filmingCardModality != FilmingUtility.EFilmModality && filmingCardModality != "")
                    {
                        //FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Receive_Image_From_Other_Module_Warning_Cannot_Load_EFilm");
                        FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Receive_Image_From_Other_Module_Warning_Cannot_Load_EFilm");
                        Card._unsupportedDataCount++;
                        return;
                    }
                    else if (Card.SeriesUID != dataHeaderSeriesUID && Card.SeriesUID != "")
                    //&&FilmingCardModality == "EFilm" 
                    {
                        //FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Receive_Image_From_Other_Module_Warning_Cannot_Load_EFilm_Of_Different_Series");
                        FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Receive_Image_From_Other_Module_Warning_Cannot_Load_EFilm_Of_Different_Series");
                        Card._unsupportedDataCount++;
                        return;
                    }
                    if (filmingCardModality == "")
                    {
                        Card.DeleteAllFilmPage();

                        FilmingPageControl film = Card.AddFilmPage();
                        Dispatcher.Invoke(new Action(() =>
                        {
                            Card.SelectObject(film, film.ViewportList.FirstOrDefault(), null);
                            Card.SelectObject(film, null, null);
                            film.SelectedAll(true);
                            //film.SetSelectedCellLayout(new FilmLayout(1, 1));
                            film.ViewportLayout = new FilmLayout(1, 1);
                            Card.DisableUIForElecFilmOperation();
                        }));
                    }
                }
                else //dataHeaderModality != "EFilm"
                {
                    if (filmingCardModality == FilmingUtility.EFilmModality)
                    {
                        //FilmingViewerContainee.Main.ShowStatusInfo(
                        //    "UID_Filming_Receive_Image_From_Other_Module_Warning_Cannot_Load_Normal_Image");
                        FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Receive_Image_From_Other_Module_Warning_Cannot_Load_Normal_Image");
                        Card._unsupportedDataCount++;
                        return;
                    }
                }

                //key 267519
                Dispatcher.Invoke(new Action(() =>
                {
                    Card.DisableUI();

                    //on behalf of wanhong.xu
                    if (Card.EntityFilmingPageList.Count == 0)
                    {
                        Card.OnAddFilmPageAfterClearFilmingCard(null, null);
                    }

                    var filmPageControl =
                        !Card._isLayoutSetByOtherApplication//imageJobModel.CellIndex == -1 
                        ? Card.GetLastPageToHoldImage(Card.EntityFilmingPageList)
                        : Card.EntityFilmingPageList.Last();
                    var isCheckProtocol = Card.CheckProtocol(filmPageControl, imageJobModel.DataHeader);
                    if (isCheckProtocol)
                    {
                        //经过协议检查之后，重新计算图片加载位置
                        filmPageControl =
                            !Card._isLayoutSetByOtherApplication//imageJobModel.CellIndex == -1 
                            ? Card.GetLastPageToHoldImage(Card.EntityFilmingPageList)
                            : Card.EntityFilmingPageList.Last();
                    }

                    if (null != filmPageControl)
                    {
                        AddImageToFilmPage(filmPageControl, imageJobModel);
                        IncrementImageCount();
                    }
                }));
            }
            catch (Exception exp)
            {
                Logger.LogError("AddImageToFilmPage: " + exp.Message);
            }
            finally
            {
                Card._isLayoutSetByOtherApplication = false;
                FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
                Dispatcher.Invoke(new Action(Card.ImageLoaded));
            }
        }

        private void AddImageToFilmPage(FilmingPageControl page, ImageJobModel imageJobModel)
        {
            try
            {
                Logger.LogFuncUp();

                if (page.IsBeenRendered && !page.IsVisible)
                    page.IsBeenRendered = false;

                if (!page.IsVisible && Card.SelectedFilmCardDisplayMode > 1 && page.FilmPageIndex < Card.layoutCtrl.SelectedFilmCardDisplayMode)
                {
                    Card.DisplayMutiFilmPage(page);
                }

                page.ShowSOPImageInEmptyCell(imageJobModel);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }



        private void IncrementImageCount()
        {
            var newImageCount = int.Parse(Card.imageCount.Text) + 1;
            //imageCount.Text = newImageCount.ToString();
            Card.HasImageDisplayed = newImageCount != 0;
        }


        #endregion


    }
}
