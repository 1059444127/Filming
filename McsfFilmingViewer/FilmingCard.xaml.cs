using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;
using UIH.Mcsf.App.Common;
using UIH.Mcsf.AppControls.Viewer;
using UIH.Mcsf.AppControls.Viewer.Primitives;
using UIH.Mcsf.Filming.Configure;
using UIH.Mcsf.Filming.ImageManager;
using UIH.Mcsf.Filming.Model;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Filming.Wrappers;
using UIH.Mcsf.MedDataManagement;
using UIH.Mcsf.Database;
using UIH.Mcsf.Filming.Command;
using UIH.Mcsf.Pipeline.Dictionary;
using UIH.Mcsf.Utility;
using UIH.Mcsf.Viewer;
using UIH.Mcsf.Service;
using UIH.Mcsf.Core;
using WindowLevel = UIH.Mcsf.Viewer.WindowLevel;
using System.Collections;
using UIH.Mcsf.Filming.Views;
using UIH.Mcsf.Pipeline.Data;


namespace UIH.Mcsf.Filming
{
    using System.Windows.Media.Imaging;
    using System.Windows.Documents;
    using UIH.Mcsf.MainFrame;

    public interface IFimingCountChangeNotifier
    {
        void UpdateFilmingCount(int newCount);
        void UpdateImageCountRemain(int imageRemain);
    }
    public class WindowLevelInfo
    {
        public WindowLevelInfo(WindowLevel wl, Modality modality)
        {
            WindowLevel = wl;
            Modality = modality;
        }

        public Modality Modality { get; set; }
        public WindowLevel WindowLevel { get; set; }
    }
    /// <summary>
    /// Interaction logic for FilmingCard.xaml
    /// </summary>
    public partial class FilmingCard : INotifyPropertyChanged, IFimingCountChangeNotifier
    {
        public delegate void MethodInvoker();

        public enum LocalDragDropEffects
        {
            InsertBefore,
            InsertAfter,
            Forbit,            
            None
        }

        #region Fields

        private static double _defaultCurrentFilmSizeRatioOfPortrait = 14.0 / 17.0; //14x17
        private static double _defaultCurrentFilmSizeRatioOfLandscape = 17.0 / 14.0;//14X17
        private const double DefaultFilmPageWidth = 700;
        private const double DefaultFilmPageHeight = 850;

        private readonly FilmingPageCollection _entityFilmingPageList;
        private readonly FilmingPageCollection _deletedFilmingPageList;

        private static object EntityFilmingPageListSyncObj = new object();
        public volatile bool _isLayoutBatchJob = false;
        public volatile bool _isLayoutSetByOtherApplication = false;

        public FilmingPageCollection EntityFilmingPageList
        {
            get { return _entityFilmingPageList; }
        }



        private void UpdatePageIndexOfEntityFilmingPages()
        {
            int filmPageIndex = 0;
            lock (EntityFilmingPageListSyncObj)
            {
                foreach (var filmingPage in EntityFilmingPageList)
                {
                    filmingPage.FilmPageIndex = filmPageIndex++;
                }
            }
        }

        private void InsertToEntityFilmingPageList(int index, FilmingPageControl page)
        {
            lock (EntityFilmingPageListSyncObj)
            {
                EntityFilmingPageList.Insert(index, page);
            }
            UpdatePageIndexOfEntityFilmingPages();
        }

        public void RemoveFromEntityFilmingPageList(FilmingPageControl page)
        {
            lock (EntityFilmingPageListSyncObj)
            {
                EntityFilmingPageList.Remove(page);
            }
            if (CurrentFilmingState != FilmingRunState.ChangeLayout)
            {
                UpdatePageIndexOfEntityFilmingPages();
            }
        }

        public FilmingPageCollection DeletedFilmingPageList
        {
            get { return _deletedFilmingPageList; }
        }

        private bool _canDrapDrop = false;

        #endregion  //Fields

        #region Initialize&Destroy

        private readonly ProtocolDispatcher _protocolDispatcher = new ProtocolDispatcher();

        public ActionToolPanel actiontoolCtrl; //Action工具栏控件
        public LayoutCtrl layoutCtrl;// 切换布局控件
        public FilmingCardStudtyTreeCtrl studyTreeCtrl;
        public FilmingCardContextMenu contextMenu;

        public FilmingCardCommand commands;
        public FilmingCardBtnEdit BtnEditCtrl;
        public FilmingCardPrintAndSaveCtrl PrintAndSave;
        public FilmingCardPrintSetCtrl PrintSetCtrl;

        public FilmingCardForMG mgMethod; //MG相关方法

        public FilmingRunState CurrentFilmingState = FilmingRunState.Default; //记录当前filming操作状态
        public FilmingCard()
        {
            try
            {
                Logger.LogFuncUp();
                mcsf_clr_systemenvironment_config.GetModalityName(out _modalityName);
                InitializeComponent();
                _modalityName = _modalityName.ToUpper();
                InitControlInFilmCard();
                //todo: performance optimization begin ?
                defaultFilmingPageControl = new FilmingPageControl();
                //todo: performance optimization end
                _entityFilmingPageList = new FilmingPageCollection(this);
                _deletedFilmingPageList = new FilmingPageCollection(null);

                DataContext = this;

                //将控制面板调整到左边
                if (FilmingControlPanelLocation == "left")
                {
                    SetControlPanelToLeft();
                }

                //注册布局协议变化事件
                _protocolDispatcher.LayoutChanged += ProtocolDispatcherOnLayoutChanged;
                
                ImageTextDisplayMode = ImgTxtDisplayState.Customization;
                Initialize();
                if (FilmingHelper.IsEnablePresetLayoutBar)
                {
                    layoutCtrl.SetMIPreSetCellLayoutButton();
                }

                InitializeDataLoader();

                InitFilmCardEvent();

                Focus();
                Keyboard.Focus(this);

                #region	[--add by hui.wang: customized UI for BUs--]

                if (IsModalityForMammoImage())
                {
                    mgMethod = new FilmingCardForMG(this);
                }

                #endregion	[--add by hui.wang: customized UI for BUs--]

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }
     
        private void ProtocolDispatcherOnLayoutChanged(object sender, ProtocolEventArgs protocolEventArgs)
        {
            var layout = protocolEventArgs.Layout;


            //添加胶片，或者修改胶片的布局
            var page = EntityFilmingPageList.LastOrDefault();
            if (page == null || !page.IsEmpty())
            {
                Dispatcher.Invoke(new Action(() =>
                                                 {
                                                     page = AddFilmPage(layout);
                                                     //加图的过程中，第一次有协议变更的时候，跳转到新建胶片
                                                     if (_loadingTargetPage == LastSelectedFilmingPage)
                                                         DisplayFilmPage(page);
                                                 }));
            }
            else // page.IsEmpty
            {
                //Logger.LogBegin("协议变更，切换布局");
                Dispatcher.Invoke(new Action(() => page.ViewportLayout = layout));
                //Logger.LogEnd("协议变更，切换布局");
            }
            Dispatcher.Invoke(new Action(() => page.FilmPageType = FilmPageType.BreakFilmPage));

            //更新加图的位置
            _cellsToBeMoveForward.Clear();
            _pagesToBeRefreshed.Clear();
            emptyCellCountFromDrop = 0;
            _movingTargetPage = null;
            _movingTargetCellIndex = int.MaxValue;

            _loadingTargetPage = page;
            _loadingTargetCellIndex = 0;


        }

        private void InitControlInFilmCard()
        {
            #region 初始化工具栏

            commands = new FilmingCardCommand(this.filmingCard);

            contextMenu = new FilmingCardContextMenu(this.filmingCard);
            filmPageGrid.ContextMenuOpening += new ContextMenuEventHandler(contextMenu.FilmPageGridContextMenuOpening);

            studyTreeCtrl = new FilmingCardStudtyTreeCtrl(this.filmingCard);
            controlPanelGrid.Children.Add(studyTreeCtrl);
            Grid.SetRow(studyTreeCtrl, 0);

            layoutCtrl = new LayoutCtrl(this.filmingCard, _modalityName);
            controlPanelGrid.Children.Add(layoutCtrl);
            Grid.SetRow(layoutCtrl, 1);

            BtnEditCtrl = new FilmingCardBtnEdit(this.filmingCard);
            controlPanelGrid.Children.Add(BtnEditCtrl);
            

            actiontoolCtrl = new ActionToolPanel(this.filmingCard, _modalityName);
            controlPanelGrid.Children.Add(actiontoolCtrl);
            Grid.SetRow(actiontoolCtrl, 3);

            PrintSetCtrl = new FilmingCardPrintSetCtrl(this.filmingCard);
            PrintSetCtrl.Visibility = Visibility.Collapsed;
            controlPanelGrid.Children.Add(PrintSetCtrl);        


            if (IsModalityForMammoImage())
            {                
                BtnEditCtrl.Visibility = Visibility.Collapsed;
                PrintSetCtrl.Visibility = Visibility.Visible;
                PrintSetCtrl.Height = 220;
                layoutCtrl.viewportLayoutGrid.Margin = new Thickness(18, 10, 0, 20);
                Grid.SetRow(PrintSetCtrl, 2);                
            }
            else
            {
                Grid.SetRow(BtnEditCtrl, 2);                
                Grid.SetRow(PrintSetCtrl, 4);
            }

            PrintAndSave = new FilmingCardPrintAndSaveCtrl(this.filmingCard);
            controlPanelGrid.Children.Add(PrintAndSave);
            Grid.SetRow(PrintAndSave, 5);  

            #endregion
        }

        private void InitFilmCardEvent()
        {
            AddHandler(UserControl.KeyDownEvent, new KeyEventHandler(FilmingCardKeyDown), true);
            AddHandler(UserControl.PreviewKeyDownEvent, new KeyEventHandler(filmingCard_PreviewKeyDown), true);
            AddHandler(Button.ClickEvent, new RoutedEventHandler(FilmingCard_ButtonClick), true);  //点击控制面板按钮时使焦点始终定在filmpage上
            filmRegionGrid.AddHandler(UserControl.PreviewMouseWheelEvent, new MouseWheelEventHandler(filmingRegionGrid_PreviewMouseWheel), true);
            filmPageGrid.AddHandler(TreeViewMultipleDragDropHelper.OnDropAcceptedEvent,
                                    new SelectedSeriesListEventHandler(HandleFilmPageGridDropEvent));
            filmPageGrid.AddHandler(UserControl.PreviewDropEvent, new DragEventHandler(FilmPageGridPreviewDrop));
           
            ActionGroupBaseForSingleUseCell.SyncOtherSingleUseCellEvent += new EventHandler<SyncOtherSingleUseCellArgs>(ActionGroupBaseForSingleUseCell_SyncOtherSingleUseCellEvent);
            SingleUseMedViewerControlCellActionHelper.SingleUseCellSelectedEvent += new EventHandler<SingleUseCellSelectedEventArgs>(SingleUseMedViewerControlCellActionHelper_SingleUseCellSelectedEvent);
            SingleUseMedViewerControlCellActionHelper.SingleUseCellPositionChangedEvent += new EventHandler<SingleUseCellPositionChangedEventArgs>(this.contextMenu.SingleUseCellPositionChangedEvent);

        }

         private void FilmPageGridPreviewDrop(object sender, DragEventArgs e)
         {
             try
             {
                 Logger.LogFuncUp();
                 if (_dragDropEffects == LocalDragDropEffects.Forbit) return;
                 var draggedItem = (Object[])e.Data.GetData(typeof(Object[]));
                 if ((draggedItem == null) || (draggedItem.Count()!=4) || (draggedItem[3] == null)) return;
                 var cells = draggedItem[3] as List<MedViewerControlCell>;
               
                 if (cells!=null && IsSelectFromFimingPage(cells))
                 {
                     if (_dragDropEffects == LocalDragDropEffects.None) return; //DIM 390010 修复图片闪烁问题
                     ProcessDropSelectFromFilmingPage();
                 }
             }
             catch (Exception ex)
             {
                 Logger.LogError(ex.StackTrace);
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

        private void HandleFilmPageGridDropEvent(object sender, SelectedSeriesListEventArgs e)
        {
            try
            {
                ProcessDropSelectFromStudyTree(e.DragEventArgs);

                var data = (Object[])e.DragEventArgs.Data.GetData(typeof(Object[]));
                var level = data[0].ToString(); //selected level
                var list = data[1] as List<string>;
                var sourceApp = data.Count() > 2 ? data[2] as string : null;

                if ((data != null) && (data.Count() == 3))
                {
                    if ((level.ToLower() == "image") && (sourceApp.ToLower() == "minipa")) 
                    {
                        bool isAddLocalImage=Keyboard.IsKeyDown(Key.T);
                        bool isInsertReference = Keyboard.IsKeyDown(Key.R);
                        if ( isAddLocalImage||isInsertReference )
                        {
                            CurrentFilmingState =isAddLocalImage? FilmingRunState.LocalImageRefrence:FilmingRunState.InsertImageRefrence;
                            ProcessDragReferenceImage(list);
                            return;
                        }
                    }
                }
              
                FilmingViewerContainee.IsBusy = true;
                var filmingCard =
                           FilmingViewerContainee.FilmingViewerWindow
                           as FilmingCard;               
               
                e.ProcessDragEvent(new LoadSeriesInfo()
                {
                    TargetIsStudyList = sender is StudyTreeControl,
                    CommunicationProxy = ComProxyManager.GetCurrentProxy(),
                    StudyListViewModel = filmingCard.studyTreeCtrl.seriesSelectionCtrl.StudyListViewModel,
                    SupportAcceptMultiStudy = true,
                    NeedAppDataChecking = false,
                    MoreSettingsForInteractionInfo = (builder) =>
                    {
                        builder.SetDestAppName("FilmingCard");
                        int operateId = (int)SwitchToFilmingCommandHandler.PaOperationId.Study;
                        switch (builder.Level)
                        {
                            case LoadLevel.Study:
                                {
                                    operateId = (int) SwitchToFilmingCommandHandler.PaOperationId.Study;
                                    break;
                                }
                            case LoadLevel.Series:
                                {
                                    operateId = (int)SwitchToFilmingCommandHandler.PaOperationId.Series;
                                    break;
                                }
                            case LoadLevel.Image:
                                {
                                    operateId = (int)SwitchToFilmingCommandHandler.PaOperationId.Images;
                                    break;
                                }
                            default:
                                {
                                    Logger.LogError("builder.Level:"+builder.Level);
                                    break;
                                }
                        }
                        builder.SetOperationID(operateId);
                    },
                    NotifyBELoadSeries = (info) =>
                    {                        
                       // ProcessDropSelectFromStudyTree(e.DragEventArgs);
                        this.studyTreeCtrl.seriesSelectionCtrl.AppendStudyToFilmingCard(level, list,
                                                                                             sourceApp.ToLower());

                        _dragDropEffects = LocalDragDropEffects.None;
                        _canDrapDrop = false;

                    },

                });
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            finally
            {
                FilmingViewerContainee.IsBusy = false;
            }

        }

        private void ProcessDragReferenceImage(List<string> imageList)
        {
            if (_dropViewCell == null) return;
           // _dropViewCell.Image.CurrentPage.            
            var images = imageList.Select(imageUid => DBWrapperHelper.DBWrapper.GetImageBaseBySOPInstanceUID(imageUid)).ToList();
            SetTargetToReceiveImages();                        
            var thread = new Thread(() => images.ToList().ForEach(image => _dataLoader.LoadSopByUid(image.SOPInstanceUID)));
            thread.Start();

        }       


        private void filmingRegionGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (CurrentFilmPageBoardIndex <= 0 && e.Delta > 0 || CurrentFilmPageBoardIndex >= (FilmPageBoardCount - 1) && e.Delta < 0)
                e.Handled = true;
        }

        #region	[--add by hui.wang: customized UI for BUs--]

        private string _modalityName = string.Empty;
        public bool IsModalityForMammoImage()
        {
            return _modalityName == "MG" || _modalityName == "DBT";
        }
        public bool IsModalityMG()
        {
            return _modalityName == "MG" ;
        }
        public bool IsModalityDBT()
        {
            return _modalityName == "DBT";
        }
        
        private bool IsModality(string modality)
        {
            return _modalityName == modality;
        }

       

        #endregion	[--add by hui.wang: customized UI for BUs--]

        public bool IfZoomWindowShowState = false;  //为Zoom窗口设置，点击zoom窗口小cell，后方胶片选中不应取消。

        public FilmingZoomViewer ZoomViewer
        {
            get{return zoomWindowGrid.Children[0] as FilmingZoomViewer;}
        }
        public string SelectedLocalizedImageInfo = "";
        private void SingleUseMedViewerControlCellActionHelper_SingleUseCellSelectedEvent(object sender, SingleUseCellSelectedEventArgs e)
        {
            if (e.ParentCell.Image.CurrentPage.ImageHeader.DicomHeader.ContainsKey(ServiceTagName.SeriesInstanceUID))
                SelectedLocalizedImageInfo = e.ParentCell.Image.CurrentPage.ImageHeader.DicomHeader[
                    ServiceTagName.SeriesInstanceUID]
                                            + e.ParentCell.Image.CurrentPage.UserSpecialInfo;
            if (IfZoomWindowShowState)
            {
                var zoomViewer = zoomWindowGrid.Children[0] as FilmingZoomViewer;
                if (zoomViewer != null && zoomViewer.DisplayedZoomCell != null)
                {
                    zoomViewer.IfMiniCellSelected = true;
                    zoomViewer.DisplayedZoomCell.IsSelected = false;
                    zoomViewer.DisplayedZoomCell.Refresh();
                }
            }
            foreach (var page in EntityFilmingPageList)
            {
                page.SelectedAll(false);
                page.IsSelected = false;								
            }
            UpdateButtonStatus();
            GroupSelectedForSingleUseCell(e);
            //FilmPageUtil.MakeSingleUseCellSelectStatus(_miniCellsList, e);
            UpdateLabelSelectedCount();

        }


        public static void GroupUnselectedForSingleUseCell(MedViewerControlCell cell)
        {
            cell.SingleUseCell_MakeSameImagesInOtherMiniCellSelectedStatus(_miniCellsList, false);
            _miniCellsList.Clear();
            _miniCellsParentCellsList.Clear();
        }

        public void GroupSelectedForSingleUseCell(SingleUseCellSelectedEventArgs args)
        {
            try
            {
                var miniCell = args.MiniCell;
                miniCell.SingleUseCell_MakeSameImagesInOtherMiniCellSelectedStatus(_miniCellsList, false);
                _miniCellsList.Clear();
                _miniCellsParentCellsList.Clear();
                if (IfZoomWindowShowState) _miniCellsList.Add(args.MiniCell);

                var pCell = args.ParentCell;
                if(pCell == null) return;
                string baseduid = "";
                if ( pCell.Image.CurrentPage.ImageHeader.DicomHeader.ContainsKey(ServiceTagName.SeriesInstanceUID))
                    baseduid = pCell.Image.CurrentPage.ImageHeader.DicomHeader[ServiceTagName.SeriesInstanceUID];
                var basedIdenInfo = baseduid + pCell.Image.CurrentPage.UserSpecialInfo;

                foreach (var filmingPage in EntityFilmingPageList)
                {
                    List<MedViewerControlCell> cellsOFflimingPage = new List<MedViewerControlCell>();
                    cellsOFflimingPage.AddRange(filmingPage.Cells);

                    foreach (var cell in cellsOFflimingPage)
                    {
                        if (null == cell
                            || cell.IsEmpty
                            || null == cell.Image
                            || null == cell.Image.CurrentPage)
                        {
                            continue;
                        }
                        var overlayLocalizedImage = cell.GetOverlay(OverlayType.LocalizedImage) as OverlayLocalizedImage;
                        if (null != overlayLocalizedImage )//sopUID == overlayLocalizedImage.GraphicLocalizedImage.MiniCell.Image.CurrentPage.SOPInstanceUID)
                        {
                            if(!cell.Image.CurrentPage.ImageHeader.DicomHeader.ContainsKey(ServiceTagName.SeriesInstanceUID)) continue;
                            var uid = cell.Image.CurrentPage.ImageHeader.DicomHeader[ServiceTagName.SeriesInstanceUID];
                            var loadInfo =uid + cell.Image.CurrentPage.UserSpecialInfo;
                            if (basedIdenInfo != loadInfo) continue;
                            _miniCellsList.Add(overlayLocalizedImage.GraphicLocalizedImage.MiniCell);
                            _miniCellsParentCellsList.Add(cell);
                        }
                    }
                }
                miniCell.SingleUseCell_MakeSameImagesInOtherMiniCellSelectedStatus(_miniCellsList);
            }
            catch (Exception exp)
            {
                Logger.LogFuncException(exp.Message);
            }
        }

        public static List<MedViewerControlCell> _miniCellsList = new List<MedViewerControlCell>();
        public static List<MedViewerControlCell> _miniCellsParentCellsList = new List<MedViewerControlCell>();
        private void PSStateCopy(PresentationState psDest, PresentationState psOriginal,ActionType curAction)
        {
            try
            {
                if (psOriginal != null && psDest != null)
                {
                    if (curAction == ActionType.Windowing)
                        psDest.WindowLevel = psOriginal.WindowLevel;
                    if (curAction == ActionType.Zoom)
                    {
                        psDest.ScaleX = psOriginal.ScaleX;
                        psDest.ScaleY = psOriginal.ScaleY;
                    }
                    if (curAction == ActionType.Pan)
                    {
                        //double offsetX = psOriginal.OffsetLeft / psOriginal.RenderWidth * psDest.RenderWidth - psDest.OffsetLeft;
                        //double offsetY = psOriginal.OffsetTop / psOriginal.RenderHeight * psDest.RenderHeight - psDest.OffsetTop;
                        //psDest.Translate(offsetX, offsetY);
                        //psDest.RenderCenterX = psOriginal.RenderCenterX;
                        //psDest.RenderCenterY = psOriginal.RenderCenterY;
                        psDest.TranslateByRenderCenter(psOriginal.RenderCenterX, psOriginal.RenderCenterY);
                    }
                }
                else
                {
                    Logger.LogWarning("PresentationState psDest or psOriginal is null");
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void ActionGroupBaseForSingleUseCell_SyncOtherSingleUseCellEvent(object sender, SyncOtherSingleUseCellArgs e)
        {
            try
            {
                if (!IfZoomWindowShowState)
                {
                    if (_miniCellsList == null || _miniCellsList.Count == 0) return;
                    foreach (var miniCell in _miniCellsList)
                    {
                        PSStateCopy(miniCell.Image.CurrentPage.PState,e.BasedCell.Image.CurrentPage.PState, e.CurAction);
                        miniCell.Refresh();
                    }
                }
                else
                {
                    var grid = e.BasedCell.CellControl.Parent;
                    var cellImpl = (grid as Visual).FindAncestor<MedViewerControlCellImpl>();
                    var pCell = cellImpl.DataSource;
                    if (pCell == null) return;
                    string baseduid = "";
                    if (pCell.Image.CurrentPage.ImageHeader.DicomHeader.ContainsKey(ServiceTagName.SeriesInstanceUID))
                        baseduid = pCell.Image.CurrentPage.ImageHeader.DicomHeader[ServiceTagName.SeriesInstanceUID];
                    var basedIdenInfo = baseduid + pCell.Image.CurrentPage.UserSpecialInfo;

                    foreach (var filmingPage in EntityFilmingPageList)
                    {
                        List<MedViewerControlCell> cellsOFflimingPage = new List<MedViewerControlCell>();
                        cellsOFflimingPage.AddRange(filmingPage.Cells);

                        foreach (var cell in cellsOFflimingPage)
                        {
                            if (null == cell
                                || cell.IsEmpty
                                || null == cell.Image
                                || null == cell.Image.CurrentPage)
                            {
                                continue;
                            }
                            var overlayLocalizedImage = cell.GetOverlay(OverlayType.LocalizedImage) as OverlayLocalizedImage;
                            if (null != overlayLocalizedImage )//&& sopUID == overlayLocalizedImage.GraphicLocalizedImage.MiniCell.Image.CurrentPage.SOPInstanceUID)
                            {
                                if (!cell.Image.CurrentPage.ImageHeader.DicomHeader.ContainsKey(ServiceTagName.SeriesInstanceUID)) continue;
                                var uid = cell.Image.CurrentPage.ImageHeader.DicomHeader[ServiceTagName.SeriesInstanceUID];
                                var loadInfo = uid + cell.Image.CurrentPage.UserSpecialInfo;
                                if (basedIdenInfo != loadInfo) continue;
                                PSStateCopy(
                                    overlayLocalizedImage.GraphicLocalizedImage.MiniCell.Image.CurrentPage.PState,
                                    e.BasedCell.Image.CurrentPage.PState, e.CurAction);
                                overlayLocalizedImage.GraphicLocalizedImage.MiniCell.Refresh();
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.LogFuncException(exp.Message);
            }

        }

        #region PresetWindowing
        public readonly List<KeyBinding> _ptInputBindingList = new List<KeyBinding>();
        public void ClearPtInputBindings()
        {
            foreach (var kb in _ptInputBindingList)
            {
                if (this.InputBindings.Contains(kb))
                {
                    this.InputBindings.Remove(kb);
                }
            }
        }

        public void RecoverPtInputBindings()
        {
            foreach (var kb in _ptInputBindingList)
            {
                if (!this.InputBindings.Contains(kb))
                {
                    this.InputBindings.Add(kb);
                }
            }
        }

        #endregion

        public void UpdateFilmingCount(int newCount)
        {
            if (CurrentFilmingState == FilmingRunState.ChangeLayout) return;
            var currentStartFilmIndex = CurrentFilmPageBoardIndex * SelectedFilmCardDisplayMode + 1;
            var currentEndFilmIndex = Math.Min((CurrentFilmPageBoardIndex + 1) * SelectedFilmCardDisplayMode, newCount);
            if (currentStartFilmIndex != currentEndFilmIndex)
            {
                pageCurrent.Text = currentStartFilmIndex.ToString() + "-" + currentEndFilmIndex.ToString();
            }
            else
            {
                pageCurrent.Text = currentEndFilmIndex.ToString(); 
            }

            filmingPageCount.Text = newCount.ToString();

        }

        public void UpdateImageCountRemain(int imageRemain)
        {
            imageCountRemain.Text = imageRemain.ToString();
        }

        public bool InitialRepackStatus
        {
            get { return filmingCard.contextMenu.filmingContextMenuRepack.IsChecked; }
            set { filmingCard.contextMenu.filmingContextMenuRepack.IsChecked = value; }
        }

        #region !For Test

        #region [--Image Loading Performance Optimization--]
      

        public IDataLoader _dataLoader;

        public StudyTree _studyTree = new StudyTree();

        private void InitializeDataLoader()
        {
            //adopt medDataManagement to load image at front end
            //_studyTree = new StudyTree();
            _dataLoader = DataLoaderFactory.Instance().CreateLoader(null, DBWrapperHelper.DBWrapper);
            _dataLoader.SopLoadedHandler -= OnImageDataLoaded;
            _dataLoader.SopLoadedHandler += OnImageDataLoaded;

        }

        #endregion //[--Image Loading Performance Optimization--]

        

        private DataAccessor _dataAccessor;
        public DisplayData CreateDisplayDataBy(Sop sop)
        {
            try
            {
                Logger.LogFuncUp();
                Logger.LogTimeStamp("开始创建DataAccessor");

                //add display data to cell
                if (_dataAccessor == null)
                {
                    _dataAccessor = new DataAccessor(defaultFilmingPageControl.filmingViewerControl.Configuration);
                }
                Logger.LogTimeStamp("结束创建DataAccessor");
                //HiPerfTimer hip=new HiPerfTimer();
                //hip.Start();
                var imgSop = sop as ImageSop;
                byte[] pixelData = null;
                string ps = string.Empty;
                if (imgSop != null)
                {
                    pixelData = imgSop.GetNormalizedPixelData();
                    ps = imgSop.PresentationState;
                }
                //hip.Stop();
                //Console.WriteLine("GetPixelData:"+hip.Duration);
                Logger.LogTimeStamp("Begin to CreateImageDataForFilming");

                //HiPerfTimer hip1 = new HiPerfTimer();
                //hip1.Start();

                var printerImageTextConfigContent = "";
                if (Printers.Instance.Modality2FilmingImageTextConfigContent.ContainsKey(sop.Modality))
                {
                    printerImageTextConfigContent = Printers.Instance.Modality2FilmingImageTextConfigContent[sop.Modality];
                }
                var displayData = _dataAccessor.CreateImageDataForFilmingF1Process(sop.DicomSource, pixelData, ps, printerImageTextConfigContent);
                displayData.PState.DisplayMode = Viewer.ImgTxtDisplayState.Customization; //fix DIM 520984 保存出来的图像打印失败 | 因为保存出的有些图的PS中四角信息配置是FromApplication | 在加载的过程中直接将它设置为Customization

                //hip1.Stop();
                //Console.WriteLine("CreateImageData:" + hip1.Duration);
                if (!displayData.ContainsPixelData)
                {
                    Logger.LogError("DisplayData is  incorrect, will lead to Unsupported Image Type");
                }

                Logger.LogTimeStamp("End to CreateImageDataForFilming");

                var userSpecialInfo = LoadSeriesTimeStamp.ToString("yyyy-MM-dd-HH-mm-ss-ffffff");
                displayData.UserSpecialInfo = userSpecialInfo;


                #region For MG add Mask array
                if(IsModalityForMammoImage())
                {
                    mgMethod.AddMask(displayData, sop);
                }              

                #endregion
                Logger.LogFuncDown();
                return displayData;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                return new DisplayData(); //exception hint
            }
        }

       

        public void ReplaceCellBy(MedViewerControlCell cell, bool bSelected = false, FilmLayout filmLayout = null)
        {
            
            if (_loadingTargetCellIndex < _loadingTargetPage.MaxImagesCount)
            {
                Dispatcher.Invoke(new Action(() => { ReplaceCell(cell, bSelected); }));
                _loadingTargetCellIndex++;
            }
            else
            {
                if (_pagesToBeRefreshed.Count != 0)
                {
                    _loadingTargetPage = _pagesToBeRefreshed[0];
                    _pagesToBeRefreshed.Remove(_loadingTargetPage);
                }
                else
                {
                    Dispatcher.Invoke(
                        new Action(
                            () =>
                            {
                                _loadingTargetPage = InsertFilmPage(_loadingTargetPage.FilmPageIndex + 1, filmLayout);
                            }));
                }
                _loadingTargetCellIndex = 0;
                Dispatcher.Invoke(new Action(() => { ReplaceCell(cell, bSelected); }));
                _loadingTargetCellIndex++;
            }
        }

        public void ReplaceCell(MedViewerControlCell cell, bool bSelected = false)
        {
            try
            {
                Logger.LogFuncUp();
               
                if (cell == null || cell.Image == null) return;

                var displayData = cell.Image.CurrentPage;
                var targetCell =
                    _loadingTargetPage.filmingViewerControl.GetCell(_loadingTargetCellIndex) as MedViewerControlCell;

                //if (_startLayoutCell != null)
                //{
                //    var layoutcell = targetCell.ParentCell as FilmingLayoutCell;
                //}

                if (targetCell == null)
                {
                    Logger.LogWarning("Loading target cell is null");
                    return;
                }

                //targetCell.ForceEndAction();

                bool refreshNeeded = _loadingTargetPage.IsVisible;
                //CellPStateToBeChanged(targetCell, cell.IsEmpty ? null : cell.Image.CurrentPage);

                var image = targetCell.Image;
                if (image != null)
                {
                    if (cell.IsEmpty)
                    {
                        image.Clear();
                    }
                    else if (image.Count == 0) image.AddPage(displayData);
                    else image.ReplacePage(displayData, 0);

                    if (refreshNeeded || targetCell.Image.CurrentPage == null)
                    {
                        targetCell.Refresh();
                    }
                    else
                    {
                        image.CurrentPage.IsDirty = true;
                    }
                }

                //to do: remove to layout setting funciton
                _loadingTargetPage.RegisterEventFromCell(targetCell);
                //set the current action to the new cell.
                //should add to cell creating
                FilmPageUtil.SetAllActions(targetCell, CurrentActionType);

                if (bSelected)
                {
                    targetCell.IsSelected = true;
                    var viewPort = FilmPageUtil.ViewportOfCell(targetCell, _loadingTargetPage);
                    viewPort.IsSelected = true;
                    _loadingTargetPage.IsSelected = true;
                }

             


                ////Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private bool CellPStateToBeChanged(MedViewerControlCell cell, DisplayData displayData)
        {
            // Cell is empty and still be empty after replacing display data
            if (cell.IsEmpty && displayData == null) return false;

            // Cell becomes empty from non-empty or vice versa
            if (cell.IsEmpty || displayData == null) return true;

            // Cell is non-empty and will not be empty after replacing the data
            return cell.Image.CurrentPage.PState.DisplayMode != displayData.PState.DisplayMode;
        }

        //todo: performance optimization begin Image Loading

        private void ReplaceCell(DisplayData displayData, FilmingPageControl loadingTargetPage, MedViewerControlCell targetCell,string type)
        {
            try
            {
                //var targetCell =
                //    loadingTargetPage.filmingViewerControl.GetCell(cellindex) as MedViewerControlCell;
               
                if (targetCell == null)
                {
                    Logger.LogWarning("Loading target cell is null");
                    return;
                }

                // bool refreshNeeded = CellPStateToBeChanged(targetCell, displayData);

                var image = targetCell.Image;
                if (image != null)
                {
                    if (displayData != null)
                    {
                        if (image.Count == 0) image.AddPage(displayData);
                        else image.ReplacePage(displayData, 0);
                    }
                   
                }

                loadingTargetPage.RegisterEventFromCell(targetCell);

                FilmPageUtil.SetAllActions(targetCell, CurrentActionType);

                targetCell.IsSelected = true;
                var viewPort = FilmPageUtil.ViewportOfCell(targetCell, loadingTargetPage);
                if(viewPort!=null)
                    viewPort.IsSelected = true;
                loadingTargetPage.IsSelected = true;

                if (ImageTextDisplayMode != ImgTxtDisplayState.Customization )
                    FilmingHelper.UpdateCornerTextDisplayData(displayData, ImageTextDisplayMode, loadingTargetPage.IsVisible);

                if (displayData != null)
                {
                    var overlay = displayData.GetOverlay(OverlayType.FilmingF1ProcessText) as OverlayFilmingF1ProcessText;
                    if (overlay != null)
                    {
                        overlay.SetRulerDisplayMode(commands.IfShowImageRuler);
                    }
                }

                if ((IsModalityForMammoImage()) && type == "loading")
                {
                    mgMethod.ApplyDisplayForMg(targetCell);
                }

                if (loadingTargetPage.IsVisible)
                {
                    targetCell.Refresh();
                    if (targetCell.Image != null && targetCell.Image.CurrentPage != null)
                        targetCell.Image.CurrentPage.IsDirty = false;
                    Logger.LogInfo("%%%%图片已刷新");
                }
                else
                {
                    // Mark the page as not rendered.
                    if (targetCell.Image != null && targetCell.Image.CurrentPage != null)
                        targetCell.Image.CurrentPage.IsDirty = true;
                }

                if (targetCell.Image != null &&
                         targetCell.Image.CurrentPage != null &&
                         targetCell.Image.CurrentPage.Modality == Modality.MG)
                    targetCell.ViewerControl.ApplyDisplayWhenMg(targetCell);

                //Logger.LogFuncDown();
                //Logger.LogTimeStamp("End to Replace cell by displaydata");
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }
      

        public void SetCellsRealSize()
        {
            var size = PrintAndSave.ConvertFilmSizeFrom(PrintAndSave.CurrentFilmSize, 1);

            var filmRealWidthIn_mm = size.Width / 0.3937 * 10;  //1cm = 0.3937inch
            var filmRealHeightIn_mm = size.Height / 0.3937 * 10;
            var titleRatio = 1.00;
            foreach (var page in ActiveFilmingPageList)
            {
                if (page.IsEmpty()) continue;
                if (page.PageTitle.DisplayPosition != "0")
                {
                    titleRatio = 0.95;
                }
                else if (page.PageTitle.DisplayPosition == "0"
                         && page.PageTitle.PageNoVisibility == Visibility.Visible)
                {
                    titleRatio = 0.97;
                }
                foreach (var cell in page.Cells)
                {
                    if (cell.IsEmpty||!cell.IsSelected) continue;
                    if (cell.Image == null || cell.Image.CurrentPage == null) continue;
                    var displaydata = cell.Image.CurrentPage;
                    var imageRealWidthIn_mm = displaydata.Width * displaydata.PixelSizeX;
                    var imageRealHeightIn_mm = displaydata.Height * displaydata.PixelSizeY;
                    if (imageRealWidthIn_mm < 1.0 || imageRealHeightIn_mm < 1.0) continue; //FOV 小于1mm*1mm的小图不支持真实缩放

                    var widthNo = page.RootCell.Columns;
                    var heightNo = page.RootCell.Rows;

                    var cellRealWidthIn_mm = filmRealWidthIn_mm / widthNo;
                    var cellRealHeightIn_mm = filmRealHeightIn_mm * titleRatio / heightNo;

                    var scale = 1 / Math.Min(cellRealWidthIn_mm / imageRealWidthIn_mm,
                        cellRealHeightIn_mm / imageRealHeightIn_mm);

                    scale = scale * PrintAndSave.CorrectedRatio;
                    if (page.Visibility == Visibility.Visible)
                        FilmingActionDeal.ZoomCell(cell, scale, scale, true);
                    else
                        FilmingActionDeal.ZoomCell(cell, scale, scale, false);
                    mgMethod.ApplyDisplayForMg(cell);
                }
            }
        }

        public void SetCurCellRealSize(FilmingPageControl page,MedViewerControlCell cell)
        {
            var size = PrintAndSave.ConvertFilmSizeFrom(PrintAndSave.CurrentFilmSize, 1);

            var filmRealWidthIn_mm = size.Width / 0.3937 * 10;  //1cm = 0.3937inch
            var filmRealHeightIn_mm = size.Height / 0.3937 * 10;
            var titleRatio = 1.00;
            if (page.IsEmpty()) return;
            if (page.PageTitle.DisplayPosition != "0")
            {
                titleRatio = 0.95;
            }
            else if (page.PageTitle.DisplayPosition == "0"
                     && page.PageTitle.PageNoVisibility == Visibility.Visible)
            {
                titleRatio = 0.97;
            }
            if (cell.IsEmpty) return;
            if (cell.Image == null || cell.Image.CurrentPage == null) return;
            var displaydata = cell.Image.CurrentPage;
            var imageRealWidthIn_mm = displaydata.Width * displaydata.PixelSizeX;
            var imageRealHeightIn_mm = displaydata.Height * displaydata.PixelSizeY;
            if (imageRealWidthIn_mm < 10.0 || imageRealHeightIn_mm < 10.0) return;

            var widthNo = page.RootCell.Columns;
            var heightNo = page.RootCell.Rows;

            var cellRealWidthIn_mm = filmRealWidthIn_mm / widthNo;
            var cellRealHeightIn_mm = filmRealHeightIn_mm * titleRatio / heightNo;

            var scale = 1 / Math.Min(cellRealWidthIn_mm / imageRealWidthIn_mm,
                cellRealHeightIn_mm / imageRealHeightIn_mm);

            scale = scale * PrintAndSave.CorrectedRatio;
            if (page.Visibility == Visibility.Visible)
                FilmingActionDeal.ZoomCell(cell, scale, scale, true);
            else
                FilmingActionDeal.ZoomCell(cell, scale, scale, false);
            mgMethod.ApplyDisplayForMg(cell);
        }
        #endregion

        //todo: performance optimization end


        //todo: performance optimization Image Loading

        public uint _unsupportedDataCount = 0;

        #region [--Image Loading Performance Optimization--]

        private void OnImageDataLoaded(object sender, DataLoaderArgs e)
        {
            var sop = e.Target as Sop;
            if(CurrentFilmingState != FilmingRunState.LocalImageRefrence && CurrentFilmingState !=FilmingRunState.InsertImageRefrence)
            { 
                LoadImageBy(sop);
            }
            else
            {
                LoadRefrenceImageBy(sop);
            }
        }

        private void LoadRefrenceImageBy(Sop sop)
        {
            try
            {
                var refDisplayData = CreateDisplayDataBy(sop);
                if (refDisplayData != null)
                {
                    ImagePatientInfo ipi = new ImagePatientInfo();
                    ipi.AppendPatientInfo(refDisplayData);
                }
                refDisplayData.DicomHeader = sop.DicomSource.Clone();

                this.Dispatcher.Invoke(new Action(() => {
                    if (CurrentFilmingState == FilmingRunState.InsertImageRefrence)
                    {                       
                        this.BtnEditCtrl.InsertRefImage("manual", refDisplayData);
                    }
                    else
                    {
                        this.BtnEditCtrl.AddLocalImage(refDisplayData, sop);
                    }
                    
                }));

            }catch(Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            finally
            {
                CurrentFilmingState = FilmingRunState.Default;
            }
            
        }
       

        public void LoadImageBy(Sop sop)
        {
            try
            {
                Logger.LogTimeStamp("开始加载一张图");
               
                if (sop == null)
                {
                    Logger.LogWarning("nothing loaded by MedDataManagement");                  
                    Logger.Instance.LogSvcError(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcErrorImgTxtConfig,
                                                "[Fail to load an image for study]" +
                                                FilmingViewerContainee.Main.StudyInstanceUID);
                   
                    FilmingViewerContainee.Main.ShowStatusWarning("UID_Filming_Warning_Load_Non_Images");
                    ++_unsupportedDataCount;
                    return;
                }

                CheckProtocol(_loadingTargetPage, sop.GetFrameDicomSet());
                
                var displayData = CreateDisplayDataBy(sop);
                
                if (displayData != null)
                {
                    ImagePatientInfo ipi = new ImagePatientInfo();
                    ipi.AppendPatientInfo(displayData);
                }               

                #region [edit by jinyang.li]

                displayData.DicomHeader = sop.DicomSource.Clone();
               
                #endregion
               
                FilmingViewerContainee.DataHeaderJobManagerInstance.SetWorkingFlag();
                ReplaceCellBy(displayData);
                HasImageDisplayed = true;               
                Logger.LogTimeStamp("结束加载一张图");
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            finally
            {
                Dispatcher.Invoke(new Action(() => { ImageLoaded(); }));
            }
        }

        #endregion //[--Image Loading Performance Optimization--]

        

        public bool CheckProtocol(FilmingPageControl page, DicomAttributeCollection dataHeader)
        {
            //Logger.LogBegin("检查协议");
            if (cellWindowLevelsViewModel != null || SeriesCompareSettingForAdjust != null) return false;   //序列对比的情况下忽略图片的布局协议

            if (_isLayoutBatchJob)
            {
                _protocolDispatcher.SetProtocol(string.Empty);
                return false;
            }

            if (page == null) page = EntityFilmingPageList.LastOrDefault();
            var layout = page == null ? layoutCtrl.DefaultViewportLayoutCollection.First() : page.ViewportLayout;
            _protocolDispatcher.SetLayout(layout.LayoutName);

            var protocolName = FilmingHelper.GetTagValueFrom(dataHeader, FilmingUtility.ProtocolTag);
            _protocolDispatcher.SetProtocol(protocolName);

            //Logger.LogEnd("检查协议");

            return !string.IsNullOrWhiteSpace(protocolName);
        }

        //todo: performance optimization end

        public int displayPageCount = 0;
        public void ReplaceCellBy(DisplayData displayData,string type="loading")
        {
            try
            {
                Logger.LogFuncUp();

                //how to move to next cellimpl?
                if (_loadingTargetCellIndex < _loadingTargetPage.MaxImagesCount)
                {
                 //   Dispatcher.Invoke(new Action(() => { ReplaceCell(displayData); }));
                    ReplaceCellByDispacher(displayData,type);
                }
                else
                {
                    if (_pagesToBeRefreshed.Count != 0)
                    {
                        _loadingTargetPage = _pagesToBeRefreshed[0];

                        _pagesToBeRefreshed.Remove(_loadingTargetPage);
                    }
                    else
                    {
                        Dispatcher.Invoke(
                            new Action(() =>
                            {
                                try
                                {
                                    Logger.LogTimeStamp("开始创建并显示一张胶片");
                                    //Stopwatch sw=new Stopwatch();
                                    //sw.Start();

                                    Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "Insert Film when loading images or changing layout");

                                    _loadingTargetPage =
                                        InsertFilmPage(_loadingTargetPage.FilmPageIndex + 1,
                                                       _loadingTargetPage.ViewportLayout);

                                    if (_loadingTargetPage.FilmPageIndex < (CurrentFilmPageBoardIndex + 1) * FilmingCardColumns * FilmingCardRows
                                        && _loadingTargetPage.FilmPageIndex > CurrentFilmPageBoardIndex * FilmingCardColumns * FilmingCardRows)
                                    {
                                        DisplayFilmPage(_loadingTargetPage);
                                    }



                                    if (type == "layout")
                                    {
                                        if (CurrentFilmPageBoardIndex == 0 && _loadingTargetPage.FilmPageIndex == SelectedFilmCardDisplayMode)
                                        {
                                            DisplayFilmPage(EntityFilmingPageList[SelectedFilmCardDisplayMode-1]);
                                        }
                                    }

                                    //sw.Stop();
                                    //MessageBox.Show("Insert one page in replace cell:" +
                                    //                sw.ElapsedMilliseconds.ToString());
                                    //sw.Stop();
                                    //CreateImageTime += sw.ElapsedMilliseconds;
                                    //MessageBox.Show("Create FilmPage:" + sw.ElapsedMilliseconds.ToString());
                                    Logger.LogTimeStamp("结束创建并显示一张胶片");
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogFuncException(ex.Message + ex.StackTrace);
                                }

                            }));
                    }
                    _loadingTargetCellIndex = 0;
                    ReplaceCellByDispacher(displayData,type);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }

        }


        private void ReplaceCellByDispacher(DisplayData displayData,string type="loading")
        {
            var targetCell =
                 _loadingTargetPage.filmingViewerControl.GetCell(_loadingTargetCellIndex) as MedViewerControlCell;
           
            var dispatcherPriority = _loadingTargetPage.IsVisible
                                         ? DispatcherPriority.Input
                                         : DispatcherPriority.Background;
            Dispatcher.Invoke(dispatcherPriority,
                new Action<DisplayData, FilmingPageControl, MedViewerControlCell,string>((data, page, cell,curtype) => { ReplaceCell(data, page, cell,curtype); }),
                                                            displayData, _loadingTargetPage, targetCell,type);

            if (targetCell != null)
            {
                var image = targetCell.Image;
                if (image != null)
                {
                    _loadingTargetCellIndex++;
                }

            }
        }

        #endregion // !For Test

        public string FilmingControlPanelLocation
        {
            get
            {
                McsfServiceApplication tar = new McsfServiceApplication();
                String location = tar.GetPanelLocation(ApplicationType.Filming);
                return location.ToLower();
            }
        }

        private int _hostAdornerCount = 0;

        public int HostAdornerCount
        {
            get { return _hostAdornerCount; }
            set
            {
                if (_hostAdornerCount != value)
                {
                    _hostAdornerCount = value;
                    Logger.LogWarning("HostAdornerCount = " + HostAdornerCount);
                    if (_hostAdornerCount > 0) DisableUIElement();
                    else
                    {
                        EnableUIElement();
                        _hostAdornerCount = 0;
                    }
                }
            }
        }

        public bool IsInFilmingMainWindow
        {
            get { return _hostAdornerCount == 0; }
        }

        private bool IsCurModalityEnableSyncDrawGraphic
        {
            get { return IsModality("MR") || IsModality("CT") || IsModality("PT") || IsModality("PETMR"); }
        }



        private void SetControlPanelToLeft()
        {
            try
            {
                Logger.LogFuncUp();

                filmingCardGrid.ColumnDefinitions[0].Width = new GridLength(364);
                filmingCardGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                filmingCardGrid.ColumnDefinitions[2].Width = new GridLength(20);

                filmingCardGrid.Children.Clear();

                Grid.SetColumn(controlPanelGrid, 0);
                filmingCardGrid.Children.Add(controlPanelGrid);
                controlPanelGrid.HorizontalAlignment = HorizontalAlignment.Left;
                controlPanelGrid.Margin = new Thickness(0, 0, 9, 0); //designed panel margin is 10

                Grid.SetColumn(filmRegionGrid, 1);
                filmingCardGrid.Children.Add(filmRegionGrid);

                Grid.SetColumn(scrollBarGird, 2);
                filmingCardGrid.Children.Add(scrollBarGird);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        private void RegisterFilmingPageHandler(FilmingPageControl page)
        {
            page.PageActiveStatusChangedHander += OnFlimingPageActiveStatusChanged;
            page.PageTitleBarLeftClickHandler += OnFilmingPageTitleBarLeftClick;
            page.PageTitleBarRightClickHandler += OnFilmingPageTitleBarRightClick;
            page.ViewportLeftClickHandler += OnViewportLeftClick;
            page.ViewportRightClickHandler += OnViewportRightClick;
            page.CellRightButtonDownHandler += OnCellRightButtonDown;
            page.CellLeftButtonDownHandler += OnCellLeftButtonDown;
            page.CellLeftButtonUpHandler += OnCellLeftButtonUp;
            page.CellRightButtonUpHandler += OnCellRightButtonUp;
            page.CellLeftButtonDoubleClickHandler += OnCellLeftButtonDoubleClick;

            page.filmingViewerControl.GraphicStatisticMenuChanged -= new EventHandler<MedViewerEventArgs>(filmingViewerControl_GraphicStatisticMenuChanged);
            page.filmingViewerControl.GraphicStatisticMenuChanged += new EventHandler<MedViewerEventArgs>(filmingViewerControl_GraphicStatisticMenuChanged);
            page.filmingViewerControl.GraphicContextMenu.SetStatisticModeVisible(StatisticMode.Perimeter, true);
           
        }


        //todo: performance optimization begin page flipping

        public bool initializded = false;
        public List<FilmPlate> _filmPlates;
        private FilmingPageControl defaultFilmingPageControl;

        //todo: performance optimization end

        private void Initialize()
        {
            try
            {
                Logger.LogFuncUp();

                if (FilmingViewerContainee.FilmingResourceDict != null)
                {
                    AppCore.ApplicationResource = this.Resources;
                    Resources.MergedDictionaries.Add(AppNLSHelper.AppCommonNLSResource);
                    Resources.MergedDictionaries.Add(FilmingViewerContainee.FilmingResourceDict);
                }
                else
                {
                    Logger.Instance.LogSvcError(Logger.LogUidSource, FilmingSvcLogUid.LogUidSvcErrorResfile, "Failed to get resource dictionary");
                }
                this.actiontoolCtrl.InitCommonTool();
                this.filmingCard.contextMenu.InitContextMenu();
                this.studyTreeCtrl.InitCtrlContextMenu();

                RegisterFilmingPageHandler(defaultFilmingPageControl);

                defaultFilmingPageControl.Initialize();
                defaultFilmingPageControl.FilmPageIndex = 0;
                EntityFilmingPageList.Add(defaultFilmingPageControl);

                //InitLayout();
                //SetFilmPageOrientation((FilmOrientationEnum)PrinterSettingDialog.DataViewModal.CurrentFilmOrientation); //default orientation

                filmRegionGrid.MouseWheel += OnfilmRegionGrid_MouseWheel;
                //insertOperationButton.ToolTip = btnInsertEmptyCell.ToolTip;

                CurrentActionType = ActionType.Pointer;

                UpdateUIStatus();

                AddPreFilmingProgressControl();

                //InitializeBackGroundFilmingPage();

                //todo: performance optimization begin page flipping
                _filmPlates = Enumerable.Repeat(0, 8).Select(i => new FilmPlate()).ToList();
                //var filmPlate = _filmPlates[0];
                //filmPlate.Display(defaultFilmingPageControl);
                //filmPageGrid.Children.Add(filmPlate);
                //todo: performance optimization end

                initializded = true;
                defaultFilmingPageControl.filmingViewerControl.GraphicContextMenu.CutGraphicCommand.Command = commands.CutGraphicCommand;
                defaultFilmingPageControl.filmingViewerControl.GraphicContextMenu.CopyGraphicCommand.Command = commands.CopyGraphicCommand;
                defaultFilmingPageControl.filmingViewerControl.GraphicContextMenu.DeleteGraphicCommand.Command = commands.DeleteGraphicCommand;
                
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public List<Dictionary<StatisticGraphicType, StatisticMode>> FilmingCollectionGraphicsOperate =
            new List<Dictionary<StatisticGraphicType, StatisticMode>>();
        private void filmingViewerControl_GraphicStatisticMenuChanged(object sender, MedViewerEventArgs e)
        {
            var currentMedvieweControll = sender as MedViewerControl;
            Dictionary<StatisticGraphicType, StatisticMode> collectionGraphicsOperated =
                e.Target as Dictionary<StatisticGraphicType, StatisticMode>;
           
            FilmingCollectionGraphicsOperate.Clear();
            FilmingCollectionGraphicsOperate.Add(collectionGraphicsOperated);
            foreach (var filmpage in EntityFilmingPageList)
            {
                if (filmpage.filmingViewerControl != currentMedvieweControll)
                {
                    foreach (var graphictype in collectionGraphicsOperated)
                    {
                        filmpage.filmingViewerControl.GraphicContextMenu.GraphicsStatisticItemsMode[graphictype.Key]
                                = graphictype.Value;
                    }

                    foreach(var cell in filmpage.filmingViewerControl.Cells)
                    {
                        if (filmpage.IsVisible)
                        {
                            cell.Refresh(CellRefreshType.Graphics);
                        }
                        else
                        {
                            cell.Image.CurrentPage.IsDirty = true;
                        }

                    }
                }

            }

        }

        public void filmingViewerControl_ResetGraphicStatisticMenu()
        {
            if (FilmingCollectionGraphicsOperate.Count > 0)
            {
                foreach (var filmpage in EntityFilmingPageList)
                {
                    filmpage.filmingViewerControl.GraphicContextMenu.SetStatisticModeVisible(StatisticMode.Perimeter, true);
                    foreach (var itemGraphicsOperate in FilmingCollectionGraphicsOperate)
                    {
                        foreach (var graphictype in itemGraphicsOperate)
                        {
                            filmpage.filmingViewerControl.GraphicContextMenu.GraphicsStatisticItemsMode[
                                graphictype.Key] = graphictype.Value;

                        }
                    }

                }
            }
        }

        public void InitializeBackGroundFilmingPage()
        {
            for (int i = 0; i < 10; i++)
            {
                var newFilmPage = new FilmingPageControl();
                newFilmPage.Initialize();
                RegisterFilmingPageHandler(newFilmPage);
                DeletedFilmingPageList.Add(newFilmPage);
                newFilmPage.filmingViewerControl.GraphicContextMenu.CutGraphicCommand.Command = commands.CutGraphicCommand;
                newFilmPage.filmingViewerControl.GraphicContextMenu.CopyGraphicCommand.Command = commands.CopyGraphicCommand;
                newFilmPage.filmingViewerControl.GraphicContextMenu.DeleteGraphicCommand.Command = commands.DeleteGraphicCommand;
            }
        }


        public void InitializeDefaultFilmingPage()
        {
            if (FilmingViewerContainee.IsInitialized) return;

            try
            {
                this.studyTreeCtrl.SeriesCompareWindow.SeriesCompareSettingChangedHandler += this.studyTreeCtrl.OnSeriesCompareSettingChanged;
                this.commands.IfShowImageRuler = Printers.Instance.IfShowImageRuler;

                this.layoutCtrl.filmCustomViewportLayout.Visibility = Printers.Instance.CustomizedLayoutVisible && !(IsModalityForMammoImage()) ? Visibility.Visible : Visibility.Hidden;

                // PrintAndSave.SetFilmPageOrientation((FilmOrientationEnum)PrintAndSave.PrinterSettingDialog.DataViewModal.CurrentFilmOrientation);

                //default orientation
                this.layoutCtrl.InitLayout();
                this.FilmingCardChangedHandler += PrintAndSave.PrinterSetting.OnFilmingCardModalityChanged;
                
                defaultFilmingPageControl.Visibility = Visibility.Visible;

                //bug 580370 memory leak, 2016-4-18      bug 543569, 2016-4-29
                defaultFilmingPageControl.ViewportLayout = IsModalityForMammoImage() 
                                                    ? FilmLayout.CreateStandardLayout(this.layoutCtrl.DefaultViewportLayoutCollection.Last().LayoutXmlFileStream) 
                                                    : FilmLayout.CreateStandardLayout(this.layoutCtrl.DefaultViewportLayoutCollection.First().LayoutXmlFileStream);
                
                
                defaultFilmingPageControl.filmingViewerControl.LayoutManager.RootCell.Refresh();

                SelectFirstCellOrViewport(defaultFilmingPageControl);

                //cellLayoutCombox.SelectedIndex = 0;
                //filmViewportLayoutComboBox.SelectedIndex = 0;

                UpdateUIStatus();
                layoutCtrl.btnRealSize.Visibility = IsModalityForMammoImage()&&Printers.Instance.RealSizePrintingAvailable
                                                 ? Visibility.Visible
                                                 : Visibility.Hidden;
                PrintAndSave.saveEFilmButton.Visibility = Printers.Instance.IfSaveEFilmsAvailable
                                                 ? Visibility.Visible
                                                 : Visibility.Hidden;

                FilmingViewerContainee.IsInitialized = true; //to do : thread safe
                this.PrintSetCtrl.InitPrintDataViewModel();
                this.Dispatcher.BeginInvoke(new Action(() =>
                                                           {
                                                               if (layoutCtrl.PresetLayoutPanel != null) layoutCtrl.PresetLayoutPanel.Initialize();
                                                               this.layoutCtrl.SelectedFilmCardDisplayMode = Printers.Instance.DisplayMode;//FilmingHelper.GetDisplayModeFromConfigFile();
                                                               PrintAndSave.SetFilmSize();
                                                               UpdateFilmCardScrollBar();

                                                           }));
                
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }finally
            {
                if (!FilmingViewerContainee.IsBusy)
                    this.HostAdornerCount = 0;
            }

        }


        private void AddPreFilmingProgressControl()
        {
            try
            {
                Logger.LogFuncUp();

                var bc = new BrushConverter();
                _maskBorder.Background = (Brush)bc.ConvertFrom("#00EEEEEE");
                _maskBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
                _maskBorder.VerticalAlignment = VerticalAlignment.Stretch;
                _maskBorder.Visibility = Visibility.Hidden;

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        private const int _maxFilmPageControlCount = 8;

        public int MaxFilmPageControlCount
        {
            get { return _maxFilmPageControlCount; }
        }

        public void UpdateFilmingPageTitleDisplay()
        {
            try
            {
                Logger.LogFuncUp();

                PageTitleConfigure.Instance.ParseFilmingPageConfig();
                EntityFilmingPageList.UpdatePageTitleDisplay();
                //EntityFilmingPageList.ForEach(film => film.UpdatePageTitleDisplay());
                //_deletedFilmingPageList.ForEach(film => film.UpdatePageTitleDisplay());
                DeletedFilmingPageList.UpdatePageTitleDisplay();
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
                Printers.Instance.ParseOrReloadImageTextConfigFileContent();
                foreach (var film in EntityFilmingPageList)
                {
                    foreach (var cell in film.Cells)
                    {
                        if (cell == null || cell.Image == null || cell.Image.CurrentPage == null) continue;
                        var psState = cell.Image.CurrentPage.PState;
                        if (psState != null && psState.DisplayMode == Viewer.ImgTxtDisplayState.FromApplication) continue;
                        
                     //   cell.ReloadImageTextConfig();
                        var overlayFilmingF1ProcessText =cell.Image.CurrentPage.GetOverlay(OverlayType.FilmingF1ProcessText) as OverlayFilmingF1ProcessText;
                        if (null != overlayFilmingF1ProcessText)
                        {                            
                            if (Printers.Instance.Modality2FilmingImageTextConfigContent.ContainsKey(cell.Image.CurrentPage.Modality.ToString()))
                            {
                                var printerImageTextConfigContent = Printers.Instance.Modality2FilmingImageTextConfigContent[cell.Image.CurrentPage.Modality.ToString()];
                                overlayFilmingF1ProcessText.ReloadImageTextConfig(printerImageTextConfigContent);
                            }
                            cell.Refresh();
                        }
                       
                    }
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }


        public void UpdateFilmingImageProperty()
        {
            try
            {
                Logger.LogFuncUp();

                defaultFilmingPageControl.filmingViewerControl.Controller.LoadConfigReader();

                foreach (var film in EntityFilmingPageList)
                {
                    film.filmingViewerControl.Controller.LoadConfigReader();
                    //edit by jinyang.li 
                    Printers.Instance.ParseOrReloadImageTextConfigFileContent();
                    //film.filmingViewerControl.GraphicContextMenu.SetStatisticModeVisible(StatisticMode.Perimeter, true);
                    foreach (FilmingControlCell cell in film.Cells)
                    {
                        cell.ReloadFilmingImageTextConfig();
                    }
                }
                foreach (var film in DeletedFilmingPageList)
                {
                    film.filmingViewerControl.Controller.LoadConfigReader();
                  //  film.filmingViewerControl.GraphicContextMenu.SetStatisticModeVisible(StatisticMode.Perimeter, true);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void UpdatePrintersConfig()
        {
            try
            {
                Logger.LogFuncUp();

                Printers.Instance.ReloadPrintersConfig();
                PrintAndSave.UpdatePrintDataViewModal();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void UpdateFilmingProtocol()
        {
            try
            {
                Logger.LogFuncUp();

                _protocolDispatcher.UpdateProtocol();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }



        #region Selection related

        private void OnFilmingCardGridMouseDown(object sender, MouseEventArgs e)
        {
            UpdateUIStatus();
        }

        #region selection defines

        private FilmingPageControl _lastSelectedFilmingPage;
        public FilmingPageControl LastSelectedFilmingPage
        {
            get { return _lastSelectedFilmingPage; }
            set { _lastSelectedFilmingPage = value; }
        }

        private MedViewerControlCell _lastSelectedCell;
        public MedViewerControlCell LastSelectedCell
        {
            get { return _lastSelectedCell; }
            set { _lastSelectedCell = value; }
        }

        public McsfFilmViewport LastSelectedViewport { get; set; }

        private bool _ignoreViewportClick;
        // private FilmingPageControl _backupDropFilmingPage = null;

        // When multiply cells was selected, we delay cell-click event until mouse-button-up
        // or cancel it when drag has happened.
        private bool _hasDelayedClickEvent;

        // you should not use _activeFilmingPageList and ActiveFilmingPageList.Add() or ActiveFilmingPageList.Remove().
        // if you want to active/de-active a filming page, please set IsSelected = true or IsSelected = false.
        public readonly FilmingPageCollection _activeFilmingPageList = new FilmingPageCollection(null);

        public FilmingPageCollection ActiveFilmingPageList
        {
            get { return _activeFilmingPageList; }
        }

        #endregion

        #region special selection

        public bool IsLastSelectedViewport(McsfFilmViewport viewport)
        {
            if (LastSelectedViewport != null && LastSelectedViewport == viewport)
            {
                return true;
            }

            return false;
        }



        public void SelectFirstCellOrViewport(FilmingPageControl page)
        {
            try
            {
                Logger.LogFuncUp();

                if (page != null)
                    page.IsSelected = true;

                LastSelectedViewport = null;
                LastSelectedCell = null;

                if (page != null)
                {
                    McsfFilmViewport viewport;
                    var cell = page.Cells.FirstOrDefault();

                    if (cell != null)
                    {
                        cell.IsSelected = true;
                        viewport = page.ViewportOfCell(cell);
                        viewport.IsSelected = true;
                    }
                    else
                    {
                        viewport = page.ViewportList.FirstOrDefault();
                        if (viewport != null)
                        {
                            viewport.IsSelected = true;
                        }
                    }
                    LastSelectedCell = cell;
                    LastSelectedViewport = viewport;
                }
                LastSelectedFilmingPage = page;
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        public void SelectLastCellOrFirstViewport(FilmingPageControl page)
        {
            try
            {
                Logger.LogFuncUp();

                if (page != null)
                    page.IsSelected = true;

                LastSelectedViewport = null;
                LastSelectedCell = null;

                if (page != null)
                {
                    McsfFilmViewport viewport;
                    var cell = page.Cells.LastOrDefault();
                    if (cell != null)
                    {
                        cell.IsSelected = true;
                        viewport = page.ViewportOfCell(cell);
                        viewport.IsSelected = true;
                    }
                    else
                    {
                        viewport = page.ViewportList.FirstOrDefault();
                        if (viewport != null)
                        {
                            viewport.IsSelected = true;
                        }
                    }

                    LastSelectedCell = cell;
                    LastSelectedViewport = viewport;
                }
                LastSelectedFilmingPage = page;
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        private void SetLatestSelectedAsLastSelectedObjects(FilmingPageControl page)
        {
            try
            {
                Logger.LogFuncUp();

                if (page != null)
                    page.IsSelected = true;

                LastSelectedCell = null;
                LastSelectedViewport = null;

                if (page != null)
                {
                    LastSelectedCell = page.SelectedCells().LastOrDefault();
                    LastSelectedViewport = (LastSelectedCell != null)
                                               ? page.ViewportOfCell(LastSelectedCell)
                                               : page.SelectedViewports().LastOrDefault();
                }

                LastSelectedFilmingPage = page;

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        #endregion



        #region page selection

        public void OnFlimingPageActiveStatusChanged(FilmingPageControl page)
        {
            try
            {
                Logger.LogFuncUp();

                if (page.IsSelected)
                    ActiveFilmingPageList.AddPage(page);
                else
                    ActiveFilmingPageList.RemovePage(page);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void SuccessionalSelectAll(FilmingPageControl page)
        {
            try
            {
                Logger.LogFuncUp();

                if (LastSelectedFilmingPage == null)
                {
                    FilmPageUtil.SelectAllOfFilmingPage(page, true);
                    SelectFirstCellOrViewport(page);
                    return;
                }

                if (page == LastSelectedFilmingPage)
                {
                    var unselectList = ActiveFilmingPageList.Where(a => a != page).ToList();
                    foreach (var filmingPage in unselectList)
                    {
                        FilmPageUtil.SelectAllOfFilmingPage(filmingPage, false);
                    }

                    return;
                }

                int minPageIndex = Math.Min(page.FilmPageIndex, LastSelectedFilmingPage.FilmPageIndex);
                int maxPageIndex = Math.Max(page.FilmPageIndex, LastSelectedFilmingPage.FilmPageIndex);

                ActiveFilmingPageList.SetPagesSelectedStatusNotIn(minPageIndex, maxPageIndex, false);
                EntityFilmingPageList.SetPagesSelectedStatusBetween(minPageIndex, maxPageIndex, true);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        public void OnFilmingPageTitleBarLeftClick(FilmingPageControl page)
        {
            try
            {
                Logger.LogFuncUp();
                if (IfZoomWindowShowState)
                {
                    ZoomViewer.CloseDialog();
                }
                if (_miniCellsList != null && _miniCellsList.Count > 0) GroupUnselectedForSingleUseCell(_miniCellsList[0]);
                // successional select has priority.
                if (FilmPageUtil.IsSuccessionalSelect())
                {
                    SuccessionalSelectAll(page);
                }
                else if (FilmPageUtil.IsMultiplySelect())
                {
                    FilmPageUtil.SelectAllOfFilmingPage(page, !page.IsSelected);
                    if (page.IsSelected)
                    {
                        SelectLastCellOrFirstViewport(page);
                    }
                    else
                    {
                        LastSelectedFilmingPage = ActiveFilmingPageList.LastOrDefault();
                    }
                }
                else
                {
                    ActiveFilmingPageList.UnSelectAllCells();
                    FilmPageUtil.SelectAllOfFilmingPage(page, true);
                    SelectLastCellOrFirstViewport(page);
                }

                UpdateUIStatus();
                filmRegionGrid.Focus();
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void OnFilmingPageTitleBarRightClick(FilmingPageControl page)
        {
            try
            {
                if (IfZoomWindowShowState)
                {
                    ZoomViewer.CloseDialog();
                }
                Logger.LogFuncUp();
                if (_miniCellsList != null && _miniCellsList.Count > 0) GroupUnselectedForSingleUseCell(_miniCellsList[0]);
                if (page.IsSelected)
                {
                }
                else
                {
                    ActiveFilmingPageList.UnSelectAllCells();
                    FilmPageUtil.SelectAllOfFilmingPage(page, true);
                    SelectLastCellOrFirstViewport(page);
                }
                UpdateUIStatus();
                filmRegionGrid.Focus();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }



        #endregion

        #region viewport selection

        private void SelectedViewport(FilmingPageControl page, McsfFilmViewport viewport)
        {
            try
            {
                Logger.LogFuncUp();

                if (!viewport.IsSelected)
                {
                    if (FilmPageUtil.IsMultiplySelect())
                    {
                        page.IsSelected = true;
                        viewport.IsSelected = true;
                    }
                    else
                    {
                        FilmPageUtil.UnselectOtherFilmingPages(ActiveFilmingPageList, page);
                        FilmPageUtil.UnselectOtherViewports(page, viewport);
                        page.IsSelected = true;
                        viewport.IsSelected = true;
                    }

                    LastSelectedFilmingPage = page;
                    LastSelectedViewport = viewport;
                    LastSelectedCell = viewport.RootLayoutCell.ControlCells.FirstOrDefault();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }


        public void OnViewportRightClick(FilmingPageControl page, McsfFilmViewport viewport)
        {
            try
            {
                Logger.LogFuncUp();

                if (page == null || viewport == null)
                    return;
                // only work for none selection viewport.
                if (viewport.GetCells().Any(cell => cell.IsSelected))
                    return;

                if (!viewport.IsSelected)
                {
                    SelectedViewport(page, viewport);
                    UpdateUIStatus();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void OnViewportLeftClick(FilmingPageControl page, McsfFilmViewport viewport)
        {
            try
            {
                Logger.LogFuncUp();

                if (page == null || viewport == null)
                {
                    return;
                }

                if (_ignoreViewportClick)
                {
                    _ignoreViewportClick = false;
                    return;
                }

                if (viewport.IsSelected)
                {
                    if (FilmPageUtil.IsMultiplySelect() || FilmPageUtil.IsSuccessionalSelect())
                    {
                        if (!viewport.GetCells().Any(cell => cell.IsSelected))
                        {
                            if (page.IsSelected)
                            {
                                // needs unselect page?
                                bool needsUnselectFilmingPage = (page.SelectedViewports().Count == 0);
                                if (needsUnselectFilmingPage)
                                {
                                    page.IsSelected = false;
                                    viewport.IsSelected = false;
                                    if (LastSelectedFilmingPage == page)
                                    {
                                        LastSelectedFilmingPage = ActiveFilmingPageList.LastOrDefault();
                                        SetLatestSelectedAsLastSelectedObjects(LastSelectedFilmingPage);
                                    }
                                }
                                else
                                {
                                    if (LastSelectedViewport == viewport)
                                    {
                                        SetLatestSelectedAsLastSelectedObjects(LastSelectedFilmingPage);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //不规则布局
                        if (page.ViewportList.Count > 1)
                        {
                            MedViewerControlCell lastUnEmptyCell = viewport.GetCells().FindLast(cell => !cell.IsEmpty);
                            //且空格
                            if (null == lastUnEmptyCell)
                            {
                                FilmPageUtil.UnselectOtherFilmingPages(ActiveFilmingPageList, page);
                                FilmPageUtil.UnselectOtherViewports(page, viewport);
                            }
                        }
                    }
                }
                //else
                //{
                //    SelectedViewport(page, viewport);
                //}
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }



        #endregion

        #region cell selection

        private void OnSuccessionalCellClick(FilmingPageControl page, McsfFilmViewport viewport,
                                        MedViewerControlCell cell)
        {
            try
            {
                Logger.LogFuncUp();

                if (LastSelectedCell == null || LastSelectedViewport == null || LastSelectedFilmingPage == null)
                {
                    ActiveFilmingPageList.UnSelectAllCells();
                    SelectObject(page, viewport, cell);
                    return;
                }

                EntityFilmingPageList.UnSelectAllCells();
                var beginPage = LastSelectedFilmingPage;
                var beginCell = LastSelectedCell;
                var endPage = page;
                var endCell = cell;

                if (beginPage.FilmPageIndex > endPage.FilmPageIndex
                    || beginPage.FilmPageIndex == endPage.FilmPageIndex && beginCell.CellIndex > endCell.CellIndex)
                {
                    beginPage = page;
                    beginCell = cell;
                    endPage = LastSelectedFilmingPage;
                    endCell = LastSelectedCell;
                }

                var pages = EntityFilmingPageList.GetRange(beginPage.FilmPageIndex,
                                                           endPage.FilmPageIndex - beginPage.FilmPageIndex + 1);

                var cells = pages.SelectMany(p => p.Cells).ToList();
                var selectedCellCount = cells.Count - beginCell.CellIndex -
                                        (endPage.Cells.Count() - 1 - endCell.CellIndex);

                var selectedCells = cells.GetRange(beginCell.CellIndex, selectedCellCount);

                selectedCells.ForEach(c => c.IsSelected = true);
                pages.ForEach(p => p.IsSelected = true);

                if (pages.Count > 0)
                {
                    foreach (var seCell in pages[0].Cells)
                    {
                        if (selectedCells.Contains(seCell))
                        {
                            pages[0].ViewportOfCell(seCell).IsSelected = true;
                        }
                    }
                }
                if (pages.Count >=  2)
                {
                    foreach (var seCell in pages.LastOrDefault().Cells)
                    {
                        if (selectedCells.Contains(seCell))
                        {
                            pages.LastOrDefault().ViewportOfCell(seCell).IsSelected = true;
                        }
                    }
                }
                if (pages.Count > 2)
                {
                    for(int i= 1;i<pages.Count-1;i++)
                    {
                        pages[i].ViewportList.ForEach(v => v.IsSelected = true);
                    }
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }

        }


        public void SelectObject(FilmingPageControl page, McsfFilmViewport viewport, MedViewerControlCell cell)
        {
            // select and active page
            if (page != null && !page.IsSelected)
                page.IsSelected = true;

            // select viewport
            if (viewport != null && !viewport.IsSelected)
                viewport.IsSelected = true;

            // set last selected object
            if (cell != null)
                cell.IsSelected = true;

            LastSelectedCell = cell;
            LastSelectedViewport = viewport;
            LastSelectedFilmingPage = page;
        }

        public bool IsMultiImageActionMode()
        {
            return (CurrentActionType == ActionType.Windowing
                    || CurrentActionType == ActionType.Zoom
                    || CurrentActionType == ActionType.Pan
                    || MouseGestureButton != MouseButton.Left
                   );
        }

        public bool IsDrawGraphicActionMode()
        {
            return (CurrentActionType == ActionType.PixelLens || CurrentActionType == ActionType.RegionEllipse
                    || CurrentActionType == ActionType.RegionPolygon || CurrentActionType == ActionType.RegionRectangle
                    || CurrentActionType == ActionType.RegionCircle || CurrentActionType == ActionType.RegionFreehand
                    || CurrentActionType == ActionType.RegionSpline || CurrentActionType == ActionType.AnnotationLine
                    || CurrentActionType == ActionType.AnnotationText || CurrentActionType == ActionType.AnnotationArrow
                    || CurrentActionType == ActionType.AnnotationAngle
                   );
        }

        private void OnMultiplyCellClick(FilmingPageControl page, McsfFilmViewport viewport, MedViewerControlCell cell)
        {
            try
            {
                Logger.LogFuncUp();

                if (cell.IsSelected)
                {
                    SelectObject(page, viewport, cell);
                    if (IsMultiImageActionMode())
                    {
                        return;
                    }
                }
                else
                {
                    //med view control will select the cell later   2013/3/27 pm
                    //on view port clicked event will select the viewport later  2013/4/7 pm  --  fix bug: not select viewport and page when select a cell use ctrl key
                    SelectObject(page, viewport, null); //med viewer control will select the cell
                    LastSelectedCell = cell;
                    LastSelectedViewport = viewport;
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }

        }

        private void OnNormalCellClick(FilmingPageControl page, McsfFilmViewport viewport, MedViewerControlCell cell)
        {
            try
            {
                Logger.LogFuncUp();

                if (IsCurModalityEnableSyncDrawGraphic && IsDrawGraphicActionMode() && cell.IsSelected)
                {
                    return;
                }

                if (IsMultiImageActionMode() && cell.IsSelected)
                {
                    return;
                }

                //if (CurrentActionType == ActionType.Pan)
                //    System.Diagnostics.Debug.WriteLine(CurrentActionType.ToString());
                if (!page._isGroupLRButtonDown)
                {
                    FilmPageUtil.UnselectOtherFilmingPages(ActiveFilmingPageList, page);

                    FilmPageUtil.UnselectOtherViewports(page, viewport);

                    FilmPageUtil.UnselectOtherCellsInViewport(viewport, cell, page._isGroupLRButtonDown);
                }

                //edit by jinyang.li begin
                //foreach (var pagecontrol in EntityFilmingPageList)
                //{
                //    foreach (var celltemp in pagecontrol.Cells)
                //    {
                //        if (null == celltemp
                //            || null == celltemp.Image
                //            || null == celltemp.Image.CurrentPage)
                //        {
                //            continue;
                //        }
                //        var overlayLocalizedImage = celltemp.GetOverlay(OverlayType.LocalizedImage) as OverlayLocalizedImage;
                //        if (null == overlayLocalizedImage) continue;
                //        overlayLocalizedImage.GraphicLocalizedImage.MiniCell.IsSelected = false;
                //    }
                //}
                //edit by jinyang.li end
                if (_miniCellsList != null && _miniCellsList.Count > 0)
                    GroupUnselectedForSingleUseCell(_miniCellsList[0]);
                SelectObject(page, viewport, cell);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        private void OnCellClick(FilmingPageControl page, McsfFilmViewport viewport, MedViewerControlCell cell)
        {
            try
            {
                Logger.LogFuncUp();

                if (FilmPageUtil.IsSuccessionalSelect())
                {
                    if (IfZoomWindowShowState)
                    {
                        ZoomViewer.CloseDialog();
                    }
                    OnSuccessionalCellClick(page, viewport, cell);
                }
                else if (FilmPageUtil.IsMultiplySelect())
                {
                    if (IfZoomWindowShowState)
                    {
                        ZoomViewer.CloseDialog();
                    }
                    OnMultiplyCellClick(page, viewport, cell);
                }
                else
                {
                    OnNormalCellClick(page, viewport, cell);
                }
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        public void OnCellRightButtonDown(FilmingPageControl page, McsfFilmViewport viewport, MedViewerControlCell cell)
        {
            try
            {
                Logger.LogFuncUp();
                if (IfZoomWindowShowState)
                {
                    if (cell == null || cell.IsEmpty)
                    {
                        ZoomViewer.CloseDialog();
                        return;
                    }
                    ZoomViewer.DisplayCellChanged(cell);
                    return;
                }
                OnCellClick(page, viewport, cell);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void OnCellLeftButtonDown(FilmingPageControl page, McsfFilmViewport viewport, MedViewerControlCell cell)
        {
            try
            {
                Logger.LogFuncUp();
                if (IfZoomWindowShowState)
                {
                    if (cell == null || cell.IsEmpty)
                    {
                        ZoomViewer.CloseDialog();
                        return;
                    }
                    ZoomViewer.DisplayCellChanged(cell);
                    return;
                }
                if (!cell.IsSelected)
                    OnCellClick(page, viewport, cell);
                if (!FilmPageUtil.IsSuccessionalSelect())
                {
                    SelectObject(page, viewport, null); //med viewer control will select the cell
                    LastSelectedCell = cell;
                }
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private bool isJustAdd = false;
        private FilmingActionDeal FilmingSpan;
        private ActionType tempActionType = ActionType.Pointer;

        private void CurrentAction_ActionDoneEvent(object sender, ActionDoneEventArgs e)
        {
            if (!_isMouseMoveAndButtonPressed && !IsLRButtonDown)
            {                 
               return;
            }
            if (!isJustAdd)
            {
                if (FilmingSpan != null && !FilmingSpan.CurrentCell.IsEmpty)
                {
                    MedViewerControl currentviewerControl = FilmingSpan.CurrentCell.ViewerControl;
                    if (sender is ActionGroupZoom)
                    {
                        tempActionType = ActionType.Zoom;
                    }

                    if (sender is ActionGroupPan)
                    {
                        tempActionType = ActionType.Pan;
                    }

                    if (sender is ActionGroupWindowLevel)
                    {
                        tempActionType = ActionType.Windowing;
                    }

                    FilmingSpan.UpdateAllActiveFilmingPageControl(tempActionType,
                                                                                                        FilmingSpan.CurrentCell.Image.CurrentPage.PState,
                                                                                                        this._activeFilmingPageList, 
                                                                                                        currentviewerControl);

                    if (IfZoomWindowShowState)
                    {
                        ZoomViewer.RefreshDisplayCell();
                    }
                    FilmingSpan.CurrentCell.ActionManager.CurrentAction.ActionDoneEvent -=
                        new EventHandler<ActionDoneEventArgs>(CurrentAction_ActionDoneEvent);
                }
            }
        }

        public void RegisterSpecialActionsForMultiplePage(MedViewerControlCell cell)
        {
            isJustAdd = true;
            FilmingSpan = new FilmingActionDeal();
            cell.ActionManager.CurrentAction.ActionDoneEvent +=
                new EventHandler<ActionDoneEventArgs>(CurrentAction_ActionDoneEvent);
            FilmingSpan.CurrentCell = cell;
            isJustAdd = false;
        }

        public void OnCellLeftButtonUp(FilmingPageControl page, McsfFilmViewport viewport, MedViewerControlCell cell)
        {
            try
            {
                Logger.LogFuncUp();
                if (IfZoomWindowShowState)
                {
                    ZoomViewer.RefreshDisplayCell();
                    return;
                }
                if (cell.IsSelected)
                    OnCellClick(page, viewport, cell);
                IAction ac = cell.ActionManager.CurrentAction;
                tempActionType = ActionType.Pointer;
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
                    RegisterSpecialActionsForMultiplePage(cell);
                }
                if (_hasDelayedClickEvent)
                {
                    _hasDelayedClickEvent = false;
                }
                if (cell.IsSelected)
                {
                    if (!viewport.IsSelected)
                    {
                        viewport.IsSelected = true;
                    }
                    if (!page.IsSelected)
                    {
                        page.IsSelected = true;
                    }

                }
                else
                {
                    //[2014-09-15 by hui.wang] fix bug 330082
                    page.filmingViewerControl.SetMouseDownFalse();
                    //[2014-09-15 by hui.wang] fix bug 330082

                    if (viewport.IsSelected)
                    {
                        if (!viewport.GetSelectedCells().Any())
                            viewport.IsSelected = false;

                    }
                    if (page.IsSelected)
                    {
                        if (0 == page.SelectedViewports().Count)
                        {
                            page.IsSelected = false;
                            if (ActiveFilmingPageList.Contains(page))
                            {
                                ActiveFilmingPageList.Remove(page);
                            }
                            var lastPage = ActiveFilmingPageList.LastOrDefault();
                            SetLatestSelectedAsLastSelectedObjects(lastPage);
                        }

                    }
                }
                UpdateUIStatus();
                Logger.LogFuncDown();

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void OnCellRightButtonUp(FilmingPageControl page, McsfFilmViewport viewport, MedViewerControlCell cell)
        {
            Logger.LogFuncUp();
            _canDrapDrop = false; 
            IAction ac = cell.ActionManager.CurrentAction;
            if (ac is ActionGroupZoom || ac is ActionGroupPan)
            {
                if (ac is ActionGroupZoom)
                    tempActionType = ActionType.Zoom;
                if (ac is ActionGroupPan)
                    tempActionType = ActionType.Pan;
                {
                    RegisterSpecialActionsForMultiplePage(cell);
                }
            }
            if (_miniCellsList.Count > 0)
            {
                ActiveFilmingPageList.UnSelectAllCells();
            }
            Logger.LogFuncDown();
        }

        private void OnCellLeftButtonDoubleClick(FilmingPageControl page, McsfFilmViewport viewport,
                                                 MedViewerControlCell cell)
        {

            if (IsCurModalityEnableSyncDrawGraphic && (cell.ActionManager.CurrentAction is IActionClickDraw || CurrentActionType == ActionType.AnnotationFreehand))
            {
                return;
            }
            if (IfZoomWindowShowState)
            {
                ZoomViewer.CloseDialog();
                return;
            }

            ActiveFilmingPageList.UnSelectAllCells();
            SelectObject(page, viewport, cell);
            if(IsModalityForMammoImage()) return;
            if (cell.IsHitOnGraphicObj) return;
            if (IsCellModalitySC) return;
            if (cell.Image == null || cell.Image.CurrentPage == null) return;
            IAction ac = cell.ActionManager.CurrentAction;

            if (!((ac is ActionPointerBase) || (ac is ActionGroupPan) || (ac is ActionGroupZoom))) return;
            
            Logger.Instance.LogPerformanceRecord("[Begin][ZoomViewer]");
            if (!PrepareZoomViewerCells()) return;


            var zoomViewerControl = new FilmingZoomViewer(_allCells, ZoomViewerSelectedCellIndexs.LastOrDefault(), _allCellsFilmPageIndexList, _allCellsDisplayFilmBordIndexList);
            //DisableUI();

            zoomViewerControl.RepackWhenClose = RepackWhenClose;
            //设置统计信息显示项，与filmingcard一致
            zoomViewerControl.ctrlZoomViewer.GraphicContextMenu.GraphicsStatisticItemsMode
                = page.filmingViewerControl.GraphicContextMenu.GraphicsStatisticItemsMode;
            zoomViewerControl.ShowDialog();

            Logger.Instance.LogPerformanceRecord("[End][ZoomViewer]");
        }

        #region 双击放大相关
        private List<FilmingControlCell> _allCells = new List<FilmingControlCell>();                  //使用之前调用PrepareZoomViewerCells()更新
        public List<int> ZoomViewerSelectedCellIndexs = new List<int>();
        private List<int> _allCellsFilmPageIndexList = new List<int>();
        private List<int> _allCellsDisplayFilmBordIndexList = new List<int>();

        public bool PrepareZoomViewerCells()
        {
            try
            {
                _allCells.Clear();
                ZoomViewerSelectedCellIndexs.Clear();
                _allCellsDisplayFilmBordIndexList.Clear();
                _allCellsFilmPageIndexList.Clear();
                foreach (var filmingPageControl in EntityFilmingPageList)
                {
                    foreach (var cell in filmingPageControl.Cells.OfType<FilmingControlCell>())
                    {
                        if (cell != null && cell.Image != null && cell.Image.CurrentPage != null)
                        {
                            _allCells.Add(cell);
                            _allCellsFilmPageIndexList.Add(EntityFilmingPageList.IndexOf(filmingPageControl));
                            _allCellsDisplayFilmBordIndexList.Add(EntityFilmingPageList.IndexOf(filmingPageControl) / SelectedFilmCardDisplayMode);
                            if (cell.IsSelected) ZoomViewerSelectedCellIndexs.Add(_allCells.IndexOf(cell));
                        }
                    }
                }
                if (ZoomViewerSelectedCellIndexs.Count < 1) return false;
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);

                return false;
            }
        }
        private void RepackWhenClose()
        {
            if (IsEnableRepack)
            {
                Mouse.SetCursor(CursorUtility.Cursors[CursorUtility.WAIT]);
                this.contextMenu.Repack();
                Mouse.SetCursor(CursorUtility.Cursors[CursorUtility.NORMAL]);
            }
        }
        //仅可用于双级放大后的选择系列
        public IEnumerable<int> SelectSeriesForZoomViewer(List<string> seiresUIDList)
        {

            var indexList = new List<int>();
            foreach (var filmingPageControl in EntityFilmingPageList)
            {
                foreach (var cell in filmingPageControl.Cells.OfType<FilmingControlCell>())
                {
                    if (cell != null && cell.Image != null && cell.Image.CurrentPage != null)
                    {
                        var uid = cell.Image.CurrentPage.ImageHeader.DicomHeader[ServiceTagName.SeriesInstanceUID];
                        var userInfo = cell.Image.CurrentPage.UserSpecialInfo;
                        if (seiresUIDList.Contains(uid + userInfo))
                        {
                            if (!cell.IsSelected)
                            {
                                cell.SetCellFocusSelected(true,true);
                                var viewport = filmingPageControl.ViewportOfCell(cell);
                                if (viewport != null && !viewport.IsSelected)
                                {
                                    viewport.IsSelected = true;
                                }

                                if (!filmingPageControl.IsSelected)
                                {
                                    filmingPageControl.IsSelected = true;
                                }
                            }
                            indexList.Add(_allCells.IndexOf(cell));
                        }
                    }
                }
            }
            return indexList;
        }
        #endregion

        #endregion

        #endregion
        #region Enable or disable UI buttons

        public bool IsAnyImageLoaded
        {
            get
            {
                try
                {
                    bool any = EntityFilmingPageList.Count > 0 &&
                               EntityFilmingPageList.Any(filmingPage => filmingPage.IsAnyImageLoaded());
                    return any;
                }
                catch (Exception ex)
                {
                    Logger.LogFuncException(ex.Message + ex.StackTrace);
                    return false;
                }
            }
        }



        public bool IsEnableSelectSucceed
        {
            get { return IsSelectSingleCell && !IsCellModalitySC; }
        }

        public bool IsSelectSingleCell
        {
            get
            {
                if (ActiveFilmingPageList.Count != 1)
                    return false;

                var page = ActiveFilmingPageList.First();
                if (page.SelectedCells().Count != 1 || page.SelectedCells().First().IsEmpty)
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsEnableSelectFilm
        {
            get
            {
                if (IsCellModalitySC) return false;
                return ActiveFilmingPageList.Any();
            }
        }


        public bool IsSuccessiveSelected()
        {
            try
            {
                var sortedActivePages = ActiveFilmingPageList.OrderBy(a => a.FilmPageIndex);
                if (sortedActivePages.Count() <= sortedActivePages.Last().FilmPageIndex - sortedActivePages.First().FilmPageIndex)
                    return false;
                foreach (var page in sortedActivePages)
                {
                    var cells = page.SelectedCells().OrderBy(a => a.CellIndex);
                    if (cells.Count() == 0) return false;
                    if (cells.Count() <= cells.Last().CellIndex - cells.First().CellIndex)
                        return false;
                }

                //首尾相接
                var count = sortedActivePages.Count();
                for (int i = 0; i < count - 1; i++)
                {
                    var page1 = sortedActivePages.ElementAt(i);
                    var page2 = sortedActivePages.ElementAt(i + 1);
                    if (page1.SelectedCells().Last().CellIndex != page1.Cells.Count() - 1
                        || page2.SelectedCells().First().CellIndex != 0)
                        return false;

                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsSuccessiveSelectedIgnoredEmptyCell()
        {
            try
            {
                var sortedActivePages = ActiveFilmingPageList.OrderBy(a => a.FilmPageIndex);
                if (sortedActivePages.Count() <= sortedActivePages.Last().FilmPageIndex - sortedActivePages.First().FilmPageIndex)
                    return false;
                foreach (var page in sortedActivePages)
                {
                    var cells = page.SelectedCells().OrderBy(a => a.CellIndex);
                    if (cells.Count() == 0) return false;
                    if (cells.Count() <= cells.Last().CellIndex - cells.First().CellIndex)
                        return false;
                }

                //首尾相接
                var count = sortedActivePages.Count();
                for (int i = 0; i < count - 1; i++)
                {
                    var page1 = sortedActivePages.ElementAt(i);
                    var page2 = sortedActivePages.ElementAt(i + 1);
                    var lastCell = page1.Cells.LastOrDefault(c => !c.IsEmpty);
                    var lastCellIndex = lastCell != null ? lastCell.CellIndex : 0;

                    if (page1.SelectedCells().Last().CellIndex < lastCellIndex
                        || page2.SelectedCells().First().CellIndex != 0)
                        return false;

                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsEnableChangeViewportCellLayouts()
        {
            try
            {
                if (ActiveFilmingPageList.Count() != 1)
                    return false;
                var page = ActiveFilmingPageList.FirstOrDefault();
                if (page == null) return false;
                if (page.ViewportLayout.LayoutType != LayoutTypeEnum.DefinedLayout) return false;
                if (page.SelectedViewports().Count == 0) return false;
                if (page.SelectedViewports().Count >
                    page.SelectedViewports().LastOrDefault().IndexInFilm -
                    page.SelectedViewports().FirstOrDefault().IndexInFilm)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool IsEnableRepack
        {
            get { return this.contextMenu.filmingContextMenuRepack.IsChecked; } // && !IsModalityMG(); }
        }

        public void DisableUIForElecFilmOperation(bool able = true)
        {
            bool disable = !able;

            PrintAndSave.saveEFilmButton.IsEnabled = disable;
        }

        public void UpdateButtonStatus()
        {
            // Enable or disable buttons
            this.layoutCtrl.UpdateBtnState();
            this.PrintAndSave.UpdateBtnState();
            this.BtnEditCtrl.UpdateBtnState();
            //Logger.LogInfo("FilmingCardModality = " + FilmingCardModality);
            Keyboard.Focus(this);
        }

        public void UpdateUIStatus()
        {
            try
            {
                Logger.LogFuncUp();

                Logger.LogTimeStamp("开始UpdateButtonStatus");
                UpdateButtonStatus();

                Logger.LogTimeStamp("开始GetScaleOfSelectedCells");
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.BtnEditCtrl.GetScaleOfSelectedCells()));
                
                Logger.LogTimeStamp("结束GetScaleOfSelectedCells");

                Logger.LogTimeStamp("开始UpdateEnhanceSelectedStatus");
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.BtnEditCtrl.UpdateEnhanceSelectedStatus()));
                Logger.LogTimeStamp("结束UpdateEnhanceSelectedStatus");

                if (ActiveFilmingPageList.Count >= 1)
                {
                    if (LastSelectedViewport != null && (LastSelectedViewport.GetCells().Any(cell => cell.IsSelected) ||
                        LastSelectedViewport.GetCells().Count == 0))
                    {
                        if (!LastSelectedViewport.IsSelected)
                            LastSelectedViewport.IsSelected = true;
                    }
                }

                UpdateLabelSelectedCount();
                Logger.LogTimeStamp("结束UpdateUIStatus");
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void UpdateLabelSelectedCount()
        {
            cellCountSelected.Text = SelectedCellsFromEntityFilmingPageList.ToString();
        }

        #endregion

        #region Cell Layout


        //todo: performance optimization begin change layout
        //assume that successive cells of regular film pages have been selected 
        //  private static long CreateImageTime=0;

        public static int Createpagecount = 0;

        //todo: performance optimization end
        public FilmingPageControl GetOrCreateNextFilmPage(FilmingPageControl pageSelected)
        {
            var pages = GetLinkedPage(pageSelected);
            pages.Remove(pageSelected);
            return pages.Count != 0 ? pages.First() : InsertFilmPage(pageSelected.FilmPageIndex + 1);
        }


        #region Viewport layout

        public FilmLayout SelectedViewportLayout
        {
            get
            {
                var viewport = (LastSelectedFilmingPage != null) ? LastSelectedFilmingPage.ViewportLayout : null;
                if (viewport == null)
                    viewport = this.layoutCtrl.DefaultViewportLayoutCollection.FirstOrDefault();

                return viewport;
            }
        }

        #endregion

        #region Display mode

        public int FilmingCardRows
        {
            get { return filmPageGrid.RowDefinitions.Count; }
        }

        public int FilmingCardColumns
        {
            get { return filmPageGrid.ColumnDefinitions.Count; }
        }

        //current display mode need how many film page board to display, used by next/previous.
        public int FilmPageBoardCount
        {
            get
            {
                if (FilmingCardRows == 0 || FilmingCardColumns == 0)
                    return 1;
                return (EntityFilmingPageList.Count + FilmingCardRows * FilmingCardColumns - 1) /
                       (FilmingCardRows * FilmingCardColumns);
            }
        }

        //used for record the current film page board index
        private int _currentFilmPageBoardIndex;

        public int CurrentFilmPageBoardIndex
        {
            get { return _currentFilmPageBoardIndex; }
            set
            {
                _currentFilmPageBoardIndex = value;
                //todo: performance optimization begin pageTitle
                UpdateFilmingCount(EntityFilmingPageList.Count);
                //EntityFilmingPageList.UpdatePageLabel();
                //todo: performance optimization end
            }
        }

        private Dictionary<FilmOrientationEnum, double> _orientation2CurrentFilmSizeRatio =
            new Dictionary<FilmOrientationEnum, double>() { { FilmOrientationEnum.Portrait, _defaultCurrentFilmSizeRatioOfPortrait }, { FilmOrientationEnum.Landscape, _defaultCurrentFilmSizeRatioOfLandscape } };
        public Dictionary<FilmOrientationEnum, double> Orientation2CurrentFilmSizeRatio
        {
            get { return _orientation2CurrentFilmSizeRatio; }
        }

        public double Orientation2CurrentFilmSizeRatioForMg
        {
            get
            {
                if (Printers.Instance.PeerNodes.Count == 0)
                {
                    return FilmOrientationEnum.Portrait == PrintAndSave.CurrentFilmOrientation
                            ? _defaultCurrentFilmSizeRatioOfPortrait
                            : _defaultCurrentFilmSizeRatioOfLandscape;
                }
                if (PrintAndSave.CurrentFilmSize == null) {
                    PrintAndSave.CurrentFilmSize = Printers.Instance.DefaultFilmSize;
                };
                var size = PrintAndSave.ConvertFilmSizeFrom(PrintAndSave.CurrentFilmSize, 1);
                var width = Math.Min(size.Width, size.Height);
                var height = Math.Max(size.Width, size.Height);
                
                var ratio = FilmOrientationEnum.Portrait == PrintAndSave.CurrentFilmOrientation
                    ? width/height
                    : height/width;
                return ratio;
            }
        }

        public double FilmPageWidth
        {
            get
            {
                try
                {
                    double arverageHeight = FilmPageCardSize.Height / FilmingCardRows;
                    double filmPageWidth;
                    if (IsModalityForMammoImage())
                    {
                        filmPageWidth = (arverageHeight) * Orientation2CurrentFilmSizeRatioForMg;
                    }
                    else
                    {
                        filmPageWidth = (arverageHeight) * Orientation2CurrentFilmSizeRatio[PrintAndSave.CurrentFilmOrientation];
                    }
                    if (filmPageWidth > FilmPageCardSize.Width / FilmingCardColumns)
                    {
                        return FilmPageCardSize.Width / FilmingCardColumns;
                    }
                    return filmPageWidth;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.StackTrace);
                    return DefaultFilmPageWidth;
                }
            }
        }

        public double FilmPageHeight
        {
            get
            {
                try
                {
                    return FilmPageCardSize.Height / FilmingCardRows;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.StackTrace);
                    return DefaultFilmPageHeight;
                }
            }
        }

        private static readonly double _initHeight = 1005D;
        private static readonly double _initWidth = 1340D;
        private Size _filmPageCardSize = Size.Empty;

        public Size FilmPageCardSize
        {
            get
            {
                if (this._filmPageCardSize == Size.Empty)
                {
                    this._filmPageCardSize = new Size(_initWidth, _initHeight);
                }
                return _filmPageCardSize;
            }
            private set
            {
                this._filmPageCardSize = value;
            }
        }

        private void OnFilmCardSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (Double.IsNaN(filmPageGrid.Width)) return;
            Logger.Instance.LogDevInfo("OnFilmCardSizeChanged enter");
            if (e.NewSize.Width < 1 || e.NewSize.Height < 1) return;
            try
            {
                Logger.LogFuncUp();
                //reduce the bottom layout's height.
                if (e.NewSize.Height > 100 && e.NewSize.Width > 80)
                {
                    //距离左右两边各20，距离上下两边各10
                    this.FilmPageCardSize = new Size(e.NewSize.Width - 40, e.NewSize.Height - 20);
                }
                else
                {
                    //the first window size change by added to MainFrame
                    this.FilmPageCardSize = e.NewSize;
                }

                foreach (var filmingPage in EntityFilmingPageList)
                {
                    SetFilmPageSize(filmingPage);
                }

                Logger.LogFuncDown();
                Logger.Instance.LogDevInfo("OnFilmCardSizeChanged exit");
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void SetFilmPageSize(FilmingPageControl filmingPage)
        {
            try
            {
                Logger.LogFuncUp();
                Logger.Instance.LogDevInfo("SetFilmPageSize enter::" + filmingPage.FilmPageIndex);
                if (IsModalityForMammoImage())
                {
                    mgMethod.SetFilmPageSizeForMg(filmingPage);
                    return;
                }
                FilmingUtility.DisplayedFilmPageHeight = FilmPageCardSize.Height;

                var tempDisplayedFilmPageWidth = FilmingUtility.DisplayedFilmPageHeight * Orientation2CurrentFilmSizeRatio[this.PrintAndSave.CurrentFilmOrientation];
                if (tempDisplayedFilmPageWidth > FilmPageCardSize.Width)
                {
                    tempDisplayedFilmPageWidth = FilmPageCardSize.Width;
                }
                FilmingUtility.DisplayedFilmPageWidth = tempDisplayedFilmPageWidth;

                filmingPage.Width = FilmingUtility.DisplayedFilmPageWidth;
                filmingPage.Height = FilmingUtility.DisplayedFilmPageHeight;
                
                double xScale = this.FilmPageWidth / filmingPage.Width;
                double yScale = this.FilmPageHeight / filmingPage.Height;

                double scale = Math.Min(xScale, yScale);

                filmingPage.Scale.SetValue(ScaleTransform.ScaleYProperty, scale);
                filmingPage.Scale.SetValue(ScaleTransform.ScaleXProperty, scale);
                Logger.Instance.LogDevInfo("SetFilmPageSize exit::" + filmingPage.FilmPageIndex);
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

       
        public ActionType CurrentActionType { get; set; }

        #endregion


        #region Film page operations


        public FilmingPageControl OnAddFilmPageAfterClearFilmingCard(object sender, RoutedEventArgs e)
        {
            FilmingPageControl page = null;
            try
            {
                Logger.LogFuncUp();

                page = AddFilmPage();
                page.SetAction(CurrentActionType);

                ActiveFilmingPageList.UnSelectAllCells();
                SelectFirstCellOrViewport(page);

                //todo: performance optimization begin page flipping
                CurrentFilmPageBoardIndex = -1;
                GotoFilmBoardWithIndex(0);
                //todo: performance optimization end

                UpdateFilmCardScrollBar();
                DisableUIForElecFilmOperation(false);
                UpdateUIStatus();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            return page;
        }

        public FilmingPageControl OnAddFilmPageInFilmingCard()
        {
            FilmingPageControl page = null;
            try
            {
                Logger.LogFuncUp();

                page = AddFilmPage();
                page.SetAction(CurrentActionType);

                ActiveFilmingPageList.UnSelectAllCells();
                SelectFirstCellOrViewport(page);

               

                UpdateFilmCardScrollBar();
                DisableUIForElecFilmOperation(false);
                UpdateUIStatus();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            return page;
        }

        public FilmingPageControl InsertFilmPage(int index, FilmLayout filmLayout = null)
        {
            try
            {
                Logger.LogFuncUp();
                
                FilmingPageControl newFilmPage = CreateFilmPage(filmLayout);

                InsertToEntityFilmingPageList(index, newFilmPage);
                if (CurrentFilmingState != FilmingRunState.ChangeLayout)
                {
                    EntityFilmingPageList.UpdatePageLabel();
                }

                Logger.LogFuncDown();

                return newFilmPage;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        public FilmingPageControl AddFilmPage(FilmLayout filmLayout = null)
        {
            try
            {
                Logger.LogFuncUp();

                FilmingPageControl newFilmPage = CreateFilmPage(filmLayout);
                EntityFilmingPageList.Add(newFilmPage);

                //if there are empty position in film card, then display the new film page by default.
                Dispatcher.Invoke(
                    new Action(
                        () =>
                        {
                            bool isDisplayFilmPage = newFilmPage.FilmPageIndex <
                                                    (CurrentFilmPageBoardIndex + 1) * FilmingCardColumns * FilmingCardRows
                                                    &&
                                                    newFilmPage.FilmPageIndex >
                                                    CurrentFilmPageBoardIndex * FilmingCardColumns * FilmingCardRows;

                            if (isDisplayFilmPage)
                            {
                                DisplayFilmPage(newFilmPage);
                            }
                            EntityFilmingPageList.UpdatePageLabel();
                            UpdateFilmCardScrollBar();
                        }));

                Logger.LogFuncDown();

                return newFilmPage;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        //todo: performance optimization begin New Page

        private FilmingPageControl CreateFilmPage(FilmLayout filmLayout = null)
        {
            try
            {
                Logger.LogFuncUp();

                //bug 580370 memory leak, 2016-4-18      bug 543569, 2016-4-29
                if (filmLayout == null) filmLayout = 
                    0 !=ActiveFilmingPageList.Count
                        ? SelectedViewportLayout
                        : !IsModalityForMammoImage() ? FilmLayout.CreateStandardLayout(this.layoutCtrl.DefaultViewportLayoutCollection.First().LayoutXmlFileStream)
                                           : FilmLayout.CreateStandardLayout(this.layoutCtrl.DefaultViewportLayoutCollection.Last().LayoutXmlFileStream);

                FilmingPageControl newFilmPage = null;


                if (DeletedFilmingPageList.Count > 0)
                {
                    //newFilmPage = DeletedFilmingPageList[0];
                    //DeletedFilmingPageList.RemoveAt(0);
                    //todo: performance optimization begin New Page
                    SuitableType suitableType = SuitableType.Empty;
                    newFilmPage = FindBestPageFromPagePool(filmLayout, out suitableType);
                    Logger.Instance.LogDevInfo("New page suit type: " + suitableType.ToString());
                    //   newFilmPage = DeletedFilmingPageList.Last();
                    //todo: performance optimization end
                    DeletedFilmingPageList.Remove(newFilmPage);

                    Dispatcher.Invoke(new Action(() =>
                    {
                        newFilmPage.FilmPageType = FilmPageType.NormalFilmPage;
                        newFilmPage.IsBeenRendered = false;
                        newFilmPage.CurrentActionType = CurrentActionType;
                        newFilmPage.FilmPageIndex = EntityFilmingPageList.Count;

                        //if newFilmPage contains multi-format cell, clear
                        newFilmPage.RootCell.Children.OfType<FilmingLayoutCell>().Where(c => c.IsMultiformatLayoutCell).ToList()
                            .ForEach(c => c.Reset());


                        if (suitableType != SuitableType.Same)
                        {
                            if (filmLayout != null)
                            {
                                ProcessPageCellImpCount(newFilmPage, filmLayout.LayoutColumnsSize * filmLayout.LayoutRowsSize, suitableType);
                                newFilmPage.ViewportLayout = filmLayout;
                                CollectMoreCellImpAfterChangLayout(newFilmPage);
                            }
                            else
								//bug 543569 memory leak, 2016-4-29
                                newFilmPage.ViewportLayout = 0 !=
                                                             ActiveFilmingPageList.Count
                                                                 ? SelectedViewportLayout
                                                                 : !IsModalityForMammoImage() ? FilmLayout.CreateStandardLayout(this.layoutCtrl.DefaultViewportLayoutCollection.First().LayoutXmlFileStream)
                                                                                   : FilmLayout.CreateStandardLayout(this.layoutCtrl.DefaultViewportLayoutCollection.Last().LayoutXmlFileStream);
                        }

                        newFilmPage.SelectedAll(false);

                        //newFilmPage.SetOrientation(printAndSave.CurrentFilmOrientation);
                        //newFilmPage.filmingViewerControl.LayoutManager.Refresh();
                        //if(newFilmPage.filmPageBarGrid.Visibility != Visibility.Visible)
                        newFilmPage.filmPageBarGrid.Visibility =
                            (_filmingCardModality == FilmingUtility.EFilmModality || newFilmPage.PageTitle.DisplayPosition == "0")
                             ? Visibility.Collapsed
                             : Visibility.Visible;
                        if (newFilmPage.PageTitle.DisplayPosition == "0" && _filmingCardModality != FilmingUtility.EFilmModality)
                        {
                            newFilmPage.filmPageBarGridSimple.Visibility = newFilmPage.PageTitle.PageNoVisibility != Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

                        }
                        else
                        {
                            newFilmPage.filmPageBarGridSimple.Visibility = Visibility.Collapsed;
                        }

                    }));
                }
                else
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        newFilmPage = new FilmingPageControl();
                        newFilmPage.Initialize();

                        newFilmPage.CurrentActionType = CurrentActionType;
                        newFilmPage.FilmPageIndex = EntityFilmingPageList.Count;

                        //newFilmPage.ViewportLayout = SelectedViewportLayout;
                        if (filmLayout != null)
                            newFilmPage.ViewportLayout = filmLayout;
                        else
							//bug 543569 memory leak, 2016-4-29
                            newFilmPage.ViewportLayout = 0 !=
                                                         ActiveFilmingPageList.Count
                                                             ? SelectedViewportLayout
                                                                 : !IsModalityForMammoImage() ? FilmLayout.CreateStandardLayout(this.layoutCtrl.DefaultViewportLayoutCollection.First().LayoutXmlFileStream)
                                                                                   : FilmLayout.CreateStandardLayout(this.layoutCtrl.DefaultViewportLayoutCollection.Last().LayoutXmlFileStream);
                        //newFilmPage.SetOrientation(PrintAndSave.CurrentFilmOrientation);
                        newFilmPage.filmPageBarGrid.Visibility =
                            (_filmingCardModality == FilmingUtility.EFilmModality || newFilmPage.PageTitle.DisplayPosition == "0")
                             ? Visibility.Collapsed
                             : Visibility.Visible;
                        //newFilmPage.filmingViewerControl.LayoutManager.Refresh();
                        //SelectObject(newFilmPage, newFilmPage.ViewportList.FirstOrDefault(), null);

                        if (newFilmPage.PageTitle.DisplayPosition == "0" && _filmingCardModality != FilmingUtility.EFilmModality)
                        {
                            newFilmPage.filmPageBarGridSimple.Visibility = newFilmPage.PageTitle.PageNoVisibility != Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

                        }
                        else
                        {
                            newFilmPage.filmPageBarGridSimple.Visibility = Visibility.Collapsed;
                        }
                        RegisterFilmingPageHandler(newFilmPage);
                    }));
                   
                }

                //Createpagecount++;
                //CreateImageTime = CreateImageTime + sw.ElapsedMilliseconds;
                // MessageBox.Show("CreateFilmingPage:" + sw.ElapsedMilliseconds.ToString());
                Logger.LogFuncDown();
                newFilmPage.filmingViewerControl.GraphicContextMenu.CutGraphicCommand.Command = commands.CutGraphicCommand;
                newFilmPage.filmingViewerControl.GraphicContextMenu.CopyGraphicCommand.Command = commands.CopyGraphicCommand;
                newFilmPage.filmingViewerControl.GraphicContextMenu.DeleteGraphicCommand.Command = commands.DeleteGraphicCommand;
                if (FilmingCollectionGraphicsOperate.Count > 0)
                {
                    foreach (var itemGraphicsOperate in FilmingCollectionGraphicsOperate)
                    {
                        foreach (var graphictype in itemGraphicsOperate)
                        {
                            newFilmPage.filmingViewerControl.GraphicContextMenu.GraphicsStatisticItemsMode[graphictype.Key]= graphictype.Value;
                        }
                    }
                    
                }
                else
                {
                    foreach (StatisticGraphicType graphictype in Enum.GetValues(typeof(StatisticGraphicType)))
                    {
                        newFilmPage.filmingViewerControl.GraphicContextMenu.GraphicsStatisticItemsMode[graphictype] = StatisticMode.All;
                    }

                    newFilmPage.filmingViewerControl.GraphicContextMenu.SetStatisticModeVisible(StatisticMode.Perimeter, true);
                }
                return newFilmPage;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }
        //todo: performance optimization end


        //todo: performance optimization begin New Page 	
        //private int columnIndex = -1;
        //private int rowIndex = -1;
        enum SuitableType { Same, SameCount, BigestCellCount, Empty }
        private FilmingPageControl FindBestPageFromPagePool(FilmLayout filmLayout, out SuitableType suitableType)
        {

            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag);

            FilmingPageControl pagecontrol = null;
            FilmingPageControl sameCountPage = null;
            FilmingPageControl BigestCountPage = null;
            suitableType = SuitableType.Empty;

            if (filmLayout != null && string.IsNullOrWhiteSpace(filmLayout.LayoutXmlFileStream))
            {
                int ExpectedCellCount = filmLayout.LayoutColumnsSize * filmLayout.LayoutRowsSize;

                foreach (var page in DeletedFilmingPageList)
                {
                    if (page.ViewportLayout.LayoutRowsSize == 0 || page.ViewportLayout.LayoutColumnsSize == 0)
                        continue;
                    if (page.ViewportLayout.LayoutColumnsSize == filmLayout.LayoutColumnsSize &&
                        page.ViewportLayout.LayoutRowsSize == filmLayout.LayoutRowsSize)
                    {
                        pagecontrol = page;
                        suitableType = SuitableType.Same;
                        return pagecontrol;
                    }

                    int cellcount = page.MaxImagesCount;
                    if (ExpectedCellCount == cellcount)
                    {
                        sameCountPage = page;
                        suitableType = SuitableType.SameCount;
                    }
                    else
                    {
                        if (BigestCountPage == null)
                            BigestCountPage = page;
                        else if (BigestCountPage.ViewportLayout.MaxImagesCount < page.ViewportLayout.MaxImagesCount)
                        {
                            BigestCountPage = page;
                            suitableType = SuitableType.BigestCellCount;
                        }
                    }

                }
            }
            if (sameCountPage != null)
                return sameCountPage;
            if (BigestCountPage != null)
                return BigestCountPage;

            return DeletedFilmingPageList.Last();
        }
        private void ProcessPageCellImpCount(FilmingPageControl page, int ExpectedCount, SuitableType suitableType)
        {
            if (suitableType == SuitableType.Empty || suitableType == SuitableType.BigestCellCount)
            {
                // MedViewerLayoutCellImpl rootcellImp = page.filmingViewerControl.LayoutManager.RootCellImpl;
                Hashtable ht = page.filmingViewerControl.LayoutManager.GetFirstLayerLayoutCellImpTable();
                //if(page.MaxImagesCount>ExpectedCount)//need remove the cell from page
                //{
                //    int removeCount = page.MaxImagesCount - ExpectedCount;
                //    for(int i=0;i<removeCount;i++)
                //    {
                //        MedViewerLayoutCellImpl cellimp = ht[ht.Count - 1] as MedViewerLayoutCellImpl;
                //        ht.Remove(ht.Count - 1);
                //        if (rootcellImp.LayoutGrid.Children.Contains(cellimp))
                //            rootcellImp.LayoutGrid.Children.Remove(cellimp);
                //        FilmingHelper.MedViewLayoutCellImpPool.Add(cellimp);
                //    }
                //}
                //else  //need add cell to page
                if (ht.Count < ExpectedCount)
                {
                    int addCount = ExpectedCount - ht.Count;
                    for (int i = 0; i < addCount; i++)
                    {
                        if (FilmingHelper.MedViewLayoutCellImpPool.Count > 0)
                        {
                            MedViewerLayoutCellImpl cellimp = FilmingHelper.MedViewLayoutCellImpPool.Last();
                            FilmingHelper.MedViewLayoutCellImpPool.RemoveAt(FilmingHelper.MedViewLayoutCellImpPool.Count - 1);
                            ht.Add(ht.Count, cellimp);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        private void CollectMoreCellImpAfterChangLayout(FilmingPageControl page)
        {
            // MedViewerLayoutCellImpl rootcellImp = page.filmingViewerControl.LayoutManager.RootCellImpl;
            Hashtable ht = page.filmingViewerControl.LayoutManager.GetFirstLayerLayoutCellImpTable();
            if (ht.Count > page.MaxImagesCount)
            {
                int removeCount = ht.Count - page.MaxImagesCount;
                for (int i = 0; i < removeCount; i++)
                {
                    MedViewerLayoutCellImpl cellimp = ht[ht.Count - 1] as MedViewerLayoutCellImpl;
                    ht.Remove(ht.Count - 1);
                    //if (rootcellImp.LayoutGrid.Children.Contains(cellimp))
                    //    rootcellImp.LayoutGrid.Children.Remove(cellimp);
                    FilmingHelper.MedViewLayoutCellImpPool.Add(cellimp);
                }
            }

        }

        private void DispatcherDisplayFilmPage(FilmingPageControl filmingPage)
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + filmingPage.FilmPageIndex);
            
                int currentFilmPageIndex = filmingPage.FilmPageIndex;
                int plateIndex = currentFilmPageIndex % SelectedFilmCardDisplayMode;
                var plate = _filmPlates[plateIndex];
                plate.Display(filmingPage);

                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceExitFlag);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogDevError(ex.Message);
            }

        }

        public bool DisplayFilmPage(FilmingPageControl filmingPage,bool first=false)
        {
            try
            {

                if (filmingPage == null) return false;
                if (filmingPage.FilmPageIndex == 0 && filmingPage.IsVisible) return false;
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + filmingPage.FilmPageIndex);

                Logger.LogTimeStamp("开始显示胶片");
                Logger.LogFuncUp();
                Logger.LogTimeStamp("开始设置胶片尺寸");
                SetFilmPageSize(filmingPage);
                Logger.LogTimeStamp("结束设置胶片尺寸");
                var showPriority = first ? DispatcherPriority.Send : DispatcherPriority.Loaded;
                filmingCard.Dispatcher.Invoke(showPriority, new Action<FilmingPageControl>(DispatcherDisplayFilmPage), filmingPage);
              //  Console.WriteLine(filmingPage.FilmPageIndex);
              //  CurrentFilmPageBoardIndex = filmingPage.FilmPageIndex / SelectedFilmCardDisplayMode;
                Logger.LogTimeStamp("结束显示胶片");

                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceExitFlag + filmingPage.FilmPageIndex); 

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }

            return true;
        }


        public bool DisplayMutiFilmPage(FilmingPageControl filmingPage)
        {
            try
            {

                if (filmingPage == null) return false;

                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + filmingPage.FilmPageIndex);

                Logger.LogTimeStamp("开始显示胶片");


                Logger.LogFuncUp();

                Logger.LogTimeStamp("开始设置胶片尺寸");
                SetFilmPageSize(filmingPage);
                Logger.LogTimeStamp("结束设置胶片尺寸");
                
                int currentFilmPageIndex = filmingPage.FilmPageIndex;

                int plateIndex = currentFilmPageIndex % SelectedFilmCardDisplayMode;
                var plate = _filmPlates[plateIndex];
                plate.Display(filmingPage);
                
                CurrentFilmPageBoardIndex = filmingPage.FilmPageIndex / SelectedFilmCardDisplayMode;
               

                Logger.LogTimeStamp("结束显示胶片");

                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceExitFlag + filmingPage.FilmPageIndex); 

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }

            return true;
        }
        //todo: performance optimization end

        public int SelectedFilmCardDisplayMode
        {
            get { return this.layoutCtrl.SelectedFilmCardDisplayMode; }
        }


        //todo: performance optimization begin page flipping
        public void ReOrderFilmPage()
        {
            try
            {
                Logger.LogFuncUp();

                CurrentFilmPageBoardIndex = 0;
                for (int index = 0; index < EntityFilmingPageList.Count; index++)
                {
                    FilmingPageControl filmingPage = EntityFilmingPageList[index];
                    //filmPageGrid.Children.Remove(filmingPage);

                    //SetFilmPageSize(filmingPage);

                    //if (EntityFilmingPageList.Count <= index)
                    //{
                    //    filmPageGrid.Children.Remove(filmingPage);
                    //    continue;
                    //}
                    //if the filming card page have empty cell, add film page
                    if (index < (FilmingCardRows * FilmingCardColumns))
                    {
                        DisplayFilmPage(filmingPage);
                    }
                    //else
                    //{
                    //    filmingPage.IsDisplay = false;
                    //}
                }
                UpdateFilmCardScrollBar();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        public void ReOrderCurrentFilmPageBoard()
        {
            try
            {
                Logger.LogFuncUp();
                //filmPageGrid.Children.Clear();

                int displayedFilmPageCount = 0;

                int startFilmPageIndex = CurrentFilmPageBoardIndex * (SelectedFilmCardDisplayMode);

                int i = 0;
                for (; i < startFilmPageIndex && i < EntityFilmingPageList.Count; i++)
                {
                    EntityFilmingPageList[i].Visibility = Visibility.Collapsed;
                }
                if (EntityFilmingPageList.Count < SelectedFilmCardDisplayMode)
                {
                    for (int j = EntityFilmingPageList.Count; j < SelectedFilmCardDisplayMode; j++)
                    {
                        var plate = _filmPlates[j];
                        plate.Display(null);
                    }
                    
                }
                for (i = startFilmPageIndex; i < EntityFilmingPageList.Count; i++)
                {
                    FilmingPageControl filmPage = EntityFilmingPageList[i];

                    if (displayedFilmPageCount < (SelectedFilmCardDisplayMode))
                    {
                        if (!filmPageGrid.Children.Contains(filmPage))
                        {
                            if (SelectedFilmCardDisplayMode==1)
                                DisplayFilmPage(filmPage,true);
                            else
                                DisplayFilmPage(filmPage);
                        }
                    }
                    else
                    {
                        filmPage.Visibility = Visibility.Collapsed;
                    }
                    displayedFilmPageCount++;
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
                //throw;
            }
            finally
            {
                UpdateFilmCardScrollBar();
                Logger.LogFuncDown();
            }
        }

        #endregion

        #region [--Load Images From External Source--]

        private enum PageState
        {
            UnInitialized,
            Full,
            Empty,
            SemiFull
        }

        private IEnumerable<FilmingPageControl> GetLastRange(IEnumerable<FilmingPageControl> pages)
        {
            var  lastRegionPages = new List<FilmingPageControl>();
            foreach (var page in pages.Reverse())
            {
                lastRegionPages.Insert(0,page);
                if(page.FilmPageType == FilmPageType.BreakFilmPage) break;
            }
            return lastRegionPages;
        }

        public FilmingPageControl GetLastPageToHoldImage(IList<FilmingPageControl> pages)
        {
            if (pages.Count == 0) return AddFilmPage();
            FilmingPageControl curPage = null;
            var lastRegionPages = GetLastRange(pages);
            var curState = PageState.UnInitialized;
            IEnumerable<Tuple<FilmingPageControl, PageState>> pageStates = lastRegionPages.Select(GetPageState);
            foreach (var pageState in pageStates)
            {
                if (pageState.Item2 == PageState.Full)
                {
                    curPage = null;
                    curState = PageState.UnInitialized;
                    continue;
                }

                if (pageState.Item2 == PageState.SemiFull)
                {
                    curPage = pageState.Item1;
                    curState = pageState.Item2;
                }
                else //Empty
                {
                    if (curState == PageState.UnInitialized)
                    {
                        curPage = pageState.Item1;
                        curState = pageState.Item2;
                    }
                }
            }

            return curPage ?? AddFilmPage(pages.Last().ViewportLayout);
            
        }

        private Tuple<FilmingPageControl, PageState> GetPageState(FilmingPageControl page)
        {
            if (page.IsEmpty()) return Tuple.Create(page, PageState.Empty);
            return !page.HasEmptyCell() ? Tuple.Create(page, PageState.Full) : Tuple.Create(page, PageState.SemiFull);
        }



        #endregion Add test code for ImageJobWorker

        #endregion [--Load Images From External Source--]

        #region [--UI Interface--]

        public void UpdateFilmCardScrollBar()
        {
            filmCardScrollBar.Value = CurrentFilmPageBoardIndex;
            filmCardScrollBar.Maximum = FilmPageBoardCount - 1;
        }

        //todo: performance optimization begin page flipping

        public void GotoFilmBoardWithIndex(int filmBoardIndex)
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + filmBoardIndex);
                Logger.Instance.LogPerformanceRecord("[Begin][Go to Film Board] " + filmBoardIndex);
                Logger.LogFuncUp();
                if (filmBoardIndex == CurrentFilmPageBoardIndex)
                {
                    Logger.LogInfo("Already in film page board {0}. Ignore.", filmBoardIndex);
                    return;
                }
                if (filmBoardIndex >= FilmPageBoardCount || filmBoardIndex < 0)
                {
                    Logger.LogError("Invalid film board index to go!");
                    return;
                }

                //filmPageGrid.Children.Clear();

                CurrentFilmPageBoardIndex = filmBoardIndex;

                int displayedFilmPageCount = 0;

                int startFilmPageIndex = CurrentFilmPageBoardIndex * (SelectedFilmCardDisplayMode);
                for (int i = startFilmPageIndex; i < EntityFilmingPageList.Count; i++)
                {
                    FilmingPageControl filmPage = EntityFilmingPageList[i];

                    if (displayedFilmPageCount < (FilmingCardRows * FilmingCardColumns))
                    {
                        DisplayFilmPage(filmPage);
                        //filmPage.UpdateLayout();
                    }
                    else
                    {
                        UpdateFilmCardScrollBar();
                        return;
                    }
                    displayedFilmPageCount++;
                    //if (filmPage.Cells != null)
                    //{
                    //    foreach (var cell in filmPage.Cells)
                    //    {
                    //        cell.Refresh();
                    //    }
                    //}
                }

                for (; displayedFilmPageCount < SelectedFilmCardDisplayMode; displayedFilmPageCount++)
                {
                    _filmPlates[displayedFilmPageCount].Display(null);
                }

                UpdateFilmCardScrollBar();

                Logger.LogTimeStamp("结束翻页");

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                //throw;
            }
            finally
            {
                Logger.Instance.LogPerformanceRecord("[End][Go to Film Board] " + filmBoardIndex);
            }
        }



        //todo: performance optimization end

        public void DeleteAllFilmPage()
        {
            if (IfZoomWindowShowState)
            {
                ZoomViewer.CloseDialog();
            }
            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    ActiveFilmingPageList.UnSelectAllCells();
                    _miniCellsList.Clear();
                    _miniCellsParentCellsList.Clear();
                    foreach (var filmingPage in EntityFilmingPageList)
                    {
                        filmingPage.Clear();
                        filmingPage.FilmPageType = FilmPageType.NormalFilmPage;
                       // CreateBitmapHelperManager.Instance.ClearImageTextBitmap(filmingPage.filmingViewerControl);
                        filmingPage.filmingViewerControl.RemoveAll();
                        
                        filmingPage.filmingViewerControlGrid.Children.Remove(filmingPage.filmingViewerControl);
                        filmingPage.filmingViewerControl = null;
                        
                    }

                    //var filmPageControlArray =
                    //    new FilmingPageControl[EntityFilmingPageList.Count];
                   // EntityFilmingPageList.CopyTo(filmPageControlArray);
                    EntityFilmingPageList.Clear();
                    //if (DeletedFilmingPageList.Count + filmPageControlArray.Count() < 100)
                    //    DeletedFilmingPageList.AddRange(filmPageControlArray);

                    //filmPageGrid.Children.Clear();
                    foreach(var deleteFilm in DeletedFilmingPageList)
                    {
                        deleteFilm.Clear();
                       // CreateBitmapHelperManager.Instance.ClearImageTextBitmap(deleteFilm.filmingViewerControl);
                        deleteFilm.filmingViewerControl.RemoveAll();
                       
                        deleteFilm.filmingViewerControlGrid.Children.Remove(deleteFilm.filmingViewerControl);
                        deleteFilm.filmingViewerControl = null;

                    }
                    DeletedFilmingPageList.Clear();
                    CurrentFilmPageBoardIndex = 0;
                    EntityFilmingPageList.UpdatePageLabel();
                    UpdateFilmCardScrollBar();

                    UpdateUIStatus();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.StackTrace);
                    throw;
                }
            }
                                  ));
        }

        public void EnableUIElement()
        {
            _maskBorder.Visibility = Visibility.Hidden;
            Keyboard.Focus(filmingCard);

            controlPanelGrid.IsEnabled = true;
        }

        public void EnableUI(bool bLoadSC = false)
        {
            ////hide mask layer to accept user operation

            //2014-11-17 09:58:55   hui.wang    电子胶片加载后重置Action为pointer
            if(!IfZoomWindowShowState)
            {
                if (bLoadSC)
                {
                    actiontoolCtrl.OnPointerButtonClick(null, null);
                }

                //  EnableUIElement();

                HostAdornerCount = 0;
                if ((CurrentFilmingState != FilmingRunState.ChangeLayout))
                {
                    ReOrderCurrentFilmPageBoard();
                }
                //this.Dispatcher.BeginInvoke(new Action(() => { EntityFilmingPageList.ForEach((film) => film.PageTitle.PatientInfoChanged()); }),
                //                                        DispatcherPriority.Input);

               
            }
            else
            {
                var zoomViewer = zoomWindowGrid.Children[0] as FilmingZoomViewer;
                if (zoomViewer != null)
                {
                    zoomViewer.ReZoom();
                    _maskBorder.Cursor = Cursors.Arrow;
                }
                HostAdornerCount = 0;
            }

            FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
        }

        public void RefreshAnnotationDisplayMode()
        {
            try
            {
                Logger.LogFuncUp();

                foreach (
                    var cell in
                        EntityFilmingPageList.Where(film => film.IsVisible).SelectMany(fpc => fpc.Cells).Where(
                            cell =>
                            cell.Image.CurrentPage != null &&
                            cell.Image.CurrentPage.GetOverlay(OverlayType.Text) != null))
                {
                    cell.Refresh(CellRefreshType.ImageText);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void DisableUI()
        {
           
            FilmingViewerContainee.DataHeaderJobManagerInstance.SleepWorkThread();

            DisableUIElement();
           
            //加载图片及切换布局，即时刷新
           if ((CurrentFilmingState == FilmingRunState.ChangeLayout)
               ||(CurrentFilmingState ==FilmingRunState.Loading))
            {
                FilmingHelper.DoEvents();
            }
            HostAdornerCount++;

            //Mask();

        }

        public void DisableUIElement()
        {
            ////show mask layer to prevent user operation

            controlPanelGrid.IsEnabled = false;

            //printAllButton.IsEnabled = false;
            //saveEFilmButton.IsEnabled = false;
            _maskBorder.Visibility = Visibility.Visible;
            //_maskBorder.ForceRender();
            _maskBorder.Cursor = Cursors.Wait;
        }

        #endregion [--UI Interface--]

        #region [--Common Image Tools UI Event Handler--]

        #region [--MultiFormat--]

        /// <summary>
        /// obey SSFS key: 101996
        /// </summary>

        private MultiFormatLayoutWindow _multiFormatLayoutWindow;

        public MultiFormatLayoutWindow MultiFormatLayoutWindow
        {
            get
            {
                if (null == _multiFormatLayoutWindow)
                {
                    _multiFormatLayoutWindow = new MultiFormatLayoutWindow();
                }

                int i = ActiveFilmingPageList.SelectMany(activeFilmingPage => activeFilmingPage.SelectedCells()).Count();
                _multiFormatLayoutWindow.ImageCount = i;

                return _multiFormatLayoutWindow;
            }
        }


        #endregion [--MultiFormat--]

        private void ResetActiveFilmingPageList()
        {
            try
            {
                Logger.LogFuncUp();

                foreach (var filmingPage in ActiveFilmingPageList)
                {
                    foreach (var selectedCell in filmingPage.SelectedCells())
                    {
                        DisplayData currentPage = selectedCell.Image.CurrentPage;
                        if (currentPage != null)
                        {
                            currentPage.Reset();
                        }
                        selectedCell.Refresh();
                    }
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }



        public void ClearAction()
        {
            actiontoolCtrl.pointerButton.IsChecked = true;
            CurrentActionType = ActionType.Pointer;
            foreach (var film in EntityFilmingPageList)
            {
                if (film.filmingViewerControl != null)
                {
                    film.ClearAction();
                }
            }
        }

        #endregion [--Common Image Tools UI Event Handler--]


        #region [--Image Loading Status Interface--]

#if DEVELOPER
        double _memAvailbleStart = 0;
        double _memAvailble = 0;
        double _memAvailbleEnd = 0;
        DateTime _timeBegin;
        DateTime _timeEnd;
        DateTime _timeLoading;
#endif

        public SingleSeriesComparePrintViewModel cellWindowLevelsViewModel { get; set; }

        public SeriesCompareSettingChangedEventArgs SeriesCompareSettingForAdjust { get; set; }

        private bool IfNeedToRearrangeBackupedCells()
        {
            if (_movingTargetPage == null || _loadingTargetPage == null) return false;
            if (_movingTargetPage.FilmPageIndex > _loadingTargetPage.FilmPageIndex) return false;
            if (_movingTargetPage.FilmPageIndex == _loadingTargetPage.FilmPageIndex
                && _movingTargetCellIndex >= _loadingTargetCellIndex) return false;
            return true;
        }

        public void ImagesLoadBeginning(uint countOfImages)
        {
#if DEVELOPER
    //memory available before loading images
            _memAvailbleStart = MemTest.GetAvailableMemoryVolume();
            _memAvailble = _memAvailbleStart;
            _timeBegin = DateTime.Now;
            _timeLoading = _timeBegin;
            Trace.WriteLine("Before loading images: " + _memAvailbleStart + "Time: " + _timeBegin +  ", image counts: " + countOfImages, "[MemoryAvailable, unit: MB; Time, uinit: ms]");
#endif

            //if no image loaded, reset progress bar
            if (0 == countOfImages)
            {
                return;
            }

            Logger.LogInfo("[Image Start Loading] " + countOfImages + " images");
            Logger.LogTimeStamp("[Image Start Loading] " + countOfImages + " images");
            //FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Begin_To_Load_Images", countOfImages);
            FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_Begin_To_Load_Images", countOfImages);
            this.CurrentFilmingState = FilmingRunState.Loading;
            if (ActiveFilmingPageList.Count > 0)
                ActiveFilmingPageList.UnSelectAllCells();
            if (_filmingCardModality == string.Empty)
            {
                this.BtnEditCtrl.scaleTextBox.Text = "1.00";
            }

            DisableUI();
            imageLoadingprogressBar.Visibility = Visibility.Visible;
            _countOfImagesLoaded = 0;
            _countOfImagesToBeLoaded = countOfImages;
            imageLoadingprogressBar.Value = 0;
            LoadSeriesTimeStamp = DateTime.Now;

            imageLoadingprogressBar.Maximum = 100; // countOfImages;
            imageLoadingprogressBar.ToolTip = "";

            cellWindowLevelsViewModel = null;
            SeriesCompareSettingForAdjust = null;
        }

        public void ImagesLoadEnded()
        {
            try
            {
                Logger.LogTimeStamp("结束加载序列");
                Logger.LogTimeStamp("开始刷新Dirty Film");
                Logger.LogFuncUp();

#if DEVELOPER

    //sleep
    //Thread.Sleep(10000);

    //memory available after images loaded
                        _memAvailbleEnd = MemTest.GetAvailableMemoryVolume();
                        _timeEnd = DateTime.Now;
                        Trace.WriteLine("After loading images: " + _memAvailbleEnd + ", Time: " + _timeEnd , "[MemoryAvailable, unit: MB; Time, uinit: ms]");

                        //memory usage difference
                        Trace.WriteLine("Memory Usage: " + (_memAvailbleStart - _memAvailbleEnd) + ", Time Consumed: " + (_timeEnd - _timeBegin).TotalMilliseconds, "[MemoryAvailable, unit: MB; Time, uinit: ms]");

#endif
                //FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_End_To_Load_Images",

                if (null != cellWindowLevelsViewModel)
                {
                    this.PrintAndSave.SetWindowLevelsForSingleSeriesComparePrint();
                }
                if (null != SeriesCompareSettingForAdjust)
                {
                    this.studyTreeCtrl.AdjustSeriesCompare();
                }

                if (IfNeedToRearrangeBackupedCells())
                {
                    foreach (var cell in _cellsToBeMoveForward)
                    {
                        ReplaceCellBy(cell);
                    }
                }
                else
                {
                    for (int i = 0; i < _backupCells.Count; i++)
                    {
                        if (_backupCells[i].Image.CurrentPage != null)
                        {
                            if (_backupCells[i].Image.CurrentPage.Overlays.Count > 0)
                                _backupCells[i].Image.CurrentPage.Overlays[0].Page.Image.Cell =
                                _backupCells[i];
                        }

                        // DisplayData dd = _backupCells[i].Image.CurrentPage;
                    }
                }
                Logger.LogTimeStamp("Repack");
                if (IsEnableRepack)
                    this.contextMenu.Repack(RepackMode.RepackLoad);
                Trace.WriteLine(FilmingCardModality);
                Logger.LogInfo("[Image End Loading]");
                Logger.LogFuncDown();

                Logger.LogTimeStamp("结束刷新Dirty Film");
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
            finally
            {
                Logger.LogTimeStamp("开始清理工作和刷新界面");
                _cellsToBeMoveForward.Clear();
                _pagesToBeRefreshed.Clear();
                _backupCells.Clear();
                _isLayoutBatchJob = false;
                Logger.LogTimeStamp("开始UpdateUIStatus");
                Logger.LogTimeStamp("RefereshPageTitle");
                ////todo: performance optimization begin pageTitle
                //EntityFilmingPageList.ForEach((film) => film.RefereshPageTitle());
                ////todo: performance optimization end
                if (!IsEnableRepack)
                {
                    Logger.LogTimeStamp("EnableUI");
                    EnableUI(FilmingCardModality == FilmingUtility.EFilmModality);
                    Logger.LogTimeStamp("UpdatePageLabel");
                    EntityFilmingPageList.UpdatePageLabel();
                }

                UpdateUIStatus();
                this.CurrentFilmingState = FilmingRunState.Default;
                Logger.LogTimeStamp("[Image End Loading] ");
                Logger.LogTimeStamp("结束清理工作和刷新界面");
                Logger.LogTimeStamp("结束图像加载过程");

                Logger.Instance.LogPerformanceRecord("[End][Load images]");
            }
        }

        public void ResetUI()
        {

            UpdateUIStatus();
            EnableUI(FilmingCardModality == FilmingUtility.EFilmModality);
            EntityFilmingPageList.UpdatePageLabel();

            ResetImageLoadingProgressInfo();
        }
        
        private void ResetImageLoadingProgressInfo()
        {
            _unsupportedDataCount = 0;
            imageLoadingprogressBar.Visibility = Visibility.Hidden;
            _countOfImagesLoaded = 0;
            _countOfImagesToBeLoaded = 0;
            imageLoadingprogressBar.Value = 0;
            LoadSeriesTimeStamp = DateTime.Now;
        }

        public void ImageLoaded()
        {
            try
            {
                Logger.LogFuncUp();

#if DEVELOPER
                        var temp = MemTest.GetAvailableMemoryVolume();
                        var tempTime = DateTime.Now;
                        Trace.WriteLine("" + _countOfImagesLoaded + " of " + _countOfImagesToBeLoaded + " Loaded, Memory available: "
                            + temp + ", Memory Usage: " + (_memAvailble - temp) + ", Time consumed: " + (tempTime - _timeLoading).TotalMilliseconds + ".", "[MemoryAvailable, unit: MB; Time, uinit: ms]");
                        _memAvailble = temp;
                        _timeLoading = tempTime;
#endif
                lock (_countOfImagesLoadedLocker)
                {
                    _countOfImagesLoaded++;
                }
                //_loadingTargetCellIndex++;
                Logger.LogTimeStamp("[Image Loading] " + _countOfImagesLoaded + " of " + _countOfImagesToBeLoaded +
                                    " images Loaded");
                if (_countOfImagesLoaded >= _countOfImagesToBeLoaded)
                {
                    if (_countOfImagesToBeLoaded > 0)
                    {
                        Logger.LogTimeStamp("开始图片加载收尾工作");

                        var actualLoadedCount = (_countOfImagesToBeLoaded - _unsupportedDataCount < 0 ? 0 : _countOfImagesToBeLoaded - _unsupportedDataCount);
                        FilmingViewerContainee.Main.ShowStatusInfo("UID_Filming_End_To_Load_Images",
                                 actualLoadedCount);

                        Logger.Instance.LogSvcInfo(Logger.Source, FilmingSvcLogUid.LogUidSvcInfoImageLoaded,
       "Loaded images : " + actualLoadedCount);

                        ResetImageLoadingProgressInfo();
                        Dispatcher.BeginInvoke(new Action(ImagesLoadEnded), DispatcherPriority.Background);
                        Logger.LogTimeStamp("结束图片加载收尾工作");
                    }
                    else //dataheader sent one by one from UIDEAL
                    {
                        UpdateUIStatus();
                        EnableUI(FilmingCardModality == FilmingUtility.EFilmModality);
                        EntityFilmingPageList.UpdatePageLabel();
                        _isLayoutBatchJob = false;
                    }
                    return;

                }
                imageLoadingprogressBar.Value = ((_countOfImagesLoaded + 0.0) / _countOfImagesToBeLoaded) *
                                                imageLoadingprogressBar.Maximum;
                imageLoadingprogressBar.ToolTip = string.Empty + _countOfImagesLoaded
                                  + "/" + _countOfImagesToBeLoaded;
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void OnImageLoadingprogressBarToolTipOpening(object sender, RoutedEventArgs e)
        {
            imageLoadingprogressBar.ToolTip = string.Empty + _countOfImagesLoaded
                                              + "/" + _countOfImagesToBeLoaded;
            //imageLoadingprogressBar.ToolTip = imageLoadingprogressBar.Value;
        }

        public void SetTBValue(MedViewerControlCell cell, WindowLevel wl, bool refresh = true)
        {
            try
            {
                if (FilmingHelper.CellModality(cell) != Modality.PT)
                {
                    return;
                }

                if (cell.Image.CurrentPage.IsRGB)
                {
                    return;
                }

                if (cell.Image.CurrentPage.Unit != " SUV bw")
                {
                    return;
                }

                var displayData = cell.Image.CurrentPage;
                var petExtension = displayData.DisplayDataExtension as PETExtension;
                if (petExtension == null)
                {
                    return;
                }

                if (refresh)
                {
                    petExtension.SetTBValue(wl.WindowWidth, ServiceTagName.T);
                    petExtension.SetTBValue(wl.WindowCenter, ServiceTagName.B);
                }
                else
                {
                    petExtension.SetTBValueWithoutRefreshUI(wl.WindowWidth, "T");
                    petExtension.SetTBValueWithoutRefreshUI(wl.WindowCenter, "B");
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message);
                throw;
            }
        }

        public void SetWindowLevel(MedViewerControlCell cell, WindowLevel wl)
        {
            try
            {
                if (FilmingHelper.CellModality(cell) != Modality.CT)
                {
                    return;
                }

                var currentPage = cell.Image.CurrentPage;
                if (currentPage.IsRGB)
                {
                    return;
                }

                currentPage.PState.WindowLevel = wl;
                //todo: performance optimization begin refresh
                if (cell.ViewerControl.IsVisible)
                {
                    cell.Refresh();
                    currentPage.IsDirty = false;
                }
                else
                {
                    currentPage.IsDirty = true;
                }
                //todo: performance optimization end
                var overlayColorbar = currentPage.GetOverlay(OverlayType.Colorbar) as OverlayColorbar;
                if (overlayColorbar != null)
                {
                    overlayColorbar.Refresh();
                }


                IOverlay overlayText = currentPage.GetOverlay(OverlayType.FilmingF1ProcessText);
                var overlayFilmingText = overlayText as OverlayFilmingF1ProcessText;
                var graphicImageText = overlayFilmingText.GraphicFilmingF1ProcessText;
                if (graphicImageText != null)
                {
                    graphicImageText.RefreshTagSource(ServiceTagName.WindowWidth, wl.WindowWidth.ToString());
                    graphicImageText.RefreshTagSource(ServiceTagName.WindowCenter, wl.WindowCenter.ToString());
                }


            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        #endregion [--Image Loading Status Interface--]

        #region [--film page UI actions--]

        //todo: performance optimization end



        #endregion [--film page UI actions--]

        #region [--Need to be classified--]

        public void Dispose()
        {
            try
            {
                Logger.LogFuncUp();

                //notify the viewerController BL to remove all cell and images.
                foreach (var filmingPage in EntityFilmingPageList)
                {
                    filmingPage.Dispose();
                }

                foreach (var filmingPage in DeletedFilmingPageList)
                {
                    filmingPage.Dispose();
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            switch (_dragDropEffects)
            {

                case LocalDragDropEffects.InsertBefore:
                    Mouse.SetCursor(Cursors.SizeAll);
                    e.Handled = true;
                    break;
                case LocalDragDropEffects.InsertAfter:
                    Mouse.SetCursor(Cursors.ScrollE);
                    e.Handled = true;
                    break;
                case LocalDragDropEffects.Forbit:
                    Mouse.SetCursor(Cursors.No);
                    e.Handled = true;
                    break;
            }

        }

        //For SSFS-KEY 175262 SSFS_Filming_LoadImageOnFilmSheet_DefaultSelected
        //used @OnDrop & ImageLoadEnded  


        private bool IsEmpty
        {
            get { return EntityFilmingPageList.All(film => !film.IsAnyImageLoaded()); }
        }

        public string _filmingCardModality = string.Empty;

        public string FilmingCardModality
        {
            get
            {
                string ret = string.Empty;
                if (!IsEmpty)
                {
                    var film = EntityFilmingPageList.FirstOrDefault(page => page.IsAnyImageLoaded());
                    ret = GetFilmPageModality(film);
                }

                if (ret != _filmingCardModality)
                {
                    bool efilmFlag = ret == FilmingUtility.EFilmModality ||
                                   _filmingCardModality == FilmingUtility.EFilmModality;
                    _filmingCardModality = ret;
                    if (efilmFlag)
                    {
                        try
                        {
                            foreach (var film in EntityFilmingPageList)
                            {
                                var visibility = (_filmingCardModality == FilmingUtility.EFilmModality || film.PageTitle.DisplayPosition == "0")
                                    ? Visibility.Collapsed
                                    : Visibility.Visible;
                                if (film.filmPageBarGrid.Visibility != visibility)
                                {
                                    Dispatcher.Invoke(new Action(() => { film.filmPageBarGrid.Visibility = visibility; }));
                                }

                                var simpleHeadVisible = Visibility.Collapsed;
                                if (_filmingCardModality == FilmingUtility.EFilmModality)
                                {
                                    simpleHeadVisible = Visibility.Collapsed;
                                }
                                else if (film.PageTitle.DisplayPosition == "0")
                                {
                                    simpleHeadVisible = film.PageTitle.PageNoVisibility != Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

                                }

                                Dispatcher.Invoke(new Action(() => { film.filmPageBarGridSimple.Visibility = simpleHeadVisible; }));

                            }
                            Dispatcher.Invoke(new Action(() => OnFilmingCardChangedHandler(ret)), DispatcherPriority.Background);
                        }
                        catch (Exception e)
                        {
                            Logger.LogError(e.Message + "oriVale:" + _filmingCardModality + "-----NewValue:" + ret + "end");  //经常会产生大量system.windows内部的越界异常，需要查找原因
                        }
                    }
                }


                return ret;
            }
        }

        private static string GetFilmPageModality(FilmingPageControl film)
        {
            string ret = film.PageTitle.FilmingPageModality;
            var cell = film.Cells.FirstOrDefault(c => c != null && c.Image != null && c.Image.CurrentPage != null);
            if (cell != null)
            {
                var displayData = cell.Image.CurrentPage;
                var dataHeader = displayData.ImageHeader.DicomHeader;
                if (dataHeader.ContainsKey(ServiceTagName.ImageType))
                {
                    var imageType = dataHeader[ServiceTagName.ImageType];
                    if (imageType == FilmingUtility.EFilmImageType) ret = FilmingUtility.EFilmModality;
                }
            }

            return ret;
        }

        public string SeriesUID
        {
            get { return IsEmpty ? string.Empty : EntityFilmingPageList.FirstOrDefault().PageTitle.SeriesUid; }
        }

        public class FilmingCardChangedEventArgs : EventArgs
        {
            public FilmingCardChangedEventArgs(string modality)
            {
                ChangedModality = modality;
            }

            public string ChangedModality { get; set; }

        }

        public delegate void FilmingCardChangedDelegate(object sender, FilmingCardChangedEventArgs args);

        public event FilmingCardChangedDelegate FilmingCardChangedHandler;

        public void OnFilmingCardChangedHandler(string modality)
        {
            var handler = FilmingCardChangedHandler;
            if (handler != null)
                handler(this, new FilmingCardChangedEventArgs(modality));
        }



        public bool IsEFilmSeries(SeriesTreeViewItemModel series)
        {
            var seriesData = series.SeriesDBData;
            return seriesData.SeriesDescription.Contains(FilmingUtility.EFilmDescriptionHeader);
        }


        public void DeleteLastEmptyFilmPageIfHave()
        {
            if (EntityFilmingPageList.Count > 0 && !EntityFilmingPageList.LastOrDefault().IsAnyImageLoaded())
            {
                commands.DeleteFilmPage(EntityFilmingPageList.LastOrDefault());
                int pageIndex = EntityFilmingPageList.Count - 1;
                while (pageIndex >= 0 && !EntityFilmingPageList[pageIndex].IsAnyImageLoaded())
                {
                    commands.DeleteFilmPage(EntityFilmingPageList[pageIndex]);
                    pageIndex--;
                }
            }
        }

        public void LoadSeries(SingleSeriesComparePrintViewModel comparedSeriesViewModel)
        {
            try
            {
                Logger.LogFuncUp();

                //SeriesData series = comparedSeriesViewModel.SeriesData;
                var seriesUidAndCount = new List<Tuple<string, int>>();

                var db = FilmingDbOperation.Instance.FilmingDbWrapper;

                var images =
                    db.GetImageListByConditionWithOrder(
                        "SeriesInstanceUIDFk='" + comparedSeriesViewModel.SeriesUID + "'", "InstanceNumber");

                seriesUidAndCount.Add(new Tuple<string, int>(comparedSeriesViewModel.SeriesUID, images.Count * 2));

                if (null != images)
                {
                    if (!FilmingHelper.CheckMemoryForLoadingSeries(seriesUidAndCount, FilmingViewerContainee.Main.GetCommunicationProxy()))
                    {
                        MessageBoxHandler.Instance.ShowError("UID_Filming_Warning_NoEnoughMemory");
                        return;
                    }

                    ImagesLoadBeginning((uint)images.Count * 2);

                    //The long time pending UI is not update in realtime 
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            // 1, clear active pages and set target viewport
                            ActiveFilmingPageList.UnSelectAllCells();
                            //layoutCtrl.filmViewportLayoutComboBox.SelectedItem =
                            //    this.layoutCtrl.ViewportLayoutCollection.FirstOrDefault();

                            DeleteLastEmptyFilmPageIfHave();
                            FilmingPageControl film = AddFilmPage();
                            film.FilmPageType = FilmPageType.BreakFilmPage;

                            //filmPageGrid.Children.Clear();
                            DisplayFilmPage(film);
                            ReOrderCurrentFilmPageBoard();

                            // 2, select page
                            SelectObject(film, null, null);
                            //film.SelectedAll(true);

                            // 3, set viewport layout and cell layout
                            film.ViewportLayout =
                                new FilmLayout(
                                    (int)comparedSeriesViewModel.CurrentRow,
                                    (int)comparedSeriesViewModel.CurrentColumn);
                            //film.SetSelectedCellLayout = new FilmLayout((int)comparedSeriesViewModel.CurrentRow, (int)comparedSeriesViewModel.CurrentColumn);

                            _pagesToBeRefreshed.Clear();
                            _cellsToBeMoveForward.Clear();
                            _loadingTargetPage = film;
                            _loadingTargetCellIndex = 0;

                            // 4, load images
                            switch ((CompareStyleEnum)comparedSeriesViewModel.CurrentCompareStyle)
                            {
                                case CompareStyleEnum.Horizontal:
                                    int step = (int)comparedSeriesViewModel.CurrentColumn;
                                    for (int start = 0; start < images.Count; start += step)
                                    {
                                        var imageRow = images.GetRange(start, step <= images.Count - start ? step : images.Count - start);

                                        Dispatcher.Invoke(new Action(() =>
                                        {

                                            foreach (var imageData in imageRow)
                                            {
                                                LoadImageForComparePrint(ref film, imageData, comparedSeriesViewModel.Window1Width, comparedSeriesViewModel.Window1Center);

                                            }
                                            foreach (var imageData in imageRow)
                                            {
                                                LoadImageForComparePrint(ref  film, imageData, comparedSeriesViewModel.Window2Width, comparedSeriesViewModel.Window2Center);
                                            }
                                        }), DispatcherPriority.Background);

                                    }

                                    break;
                                case CompareStyleEnum.Vertical:
                                    foreach (var imageData in images)
                                    {
                                        Dispatcher.Invoke(new Action(() =>
                                        {

                                            //set winding1
                                            LoadImageForComparePrint(ref film, imageData, comparedSeriesViewModel.Window1Width, comparedSeriesViewModel.Window1Center);
                                            //set winding2
                                            LoadImageForComparePrint(ref  film, imageData, comparedSeriesViewModel.Window2Width, comparedSeriesViewModel.Window2Center);

                                        }), DispatcherPriority.
                                                              Background);
                                    }
                                    break;
                                default:
                                    throw new Exception(
                                        "not supported compare style");

                            }

                            //move focus to film pages
                            film.Focus();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogFuncException(ex.Message + ex.StackTrace);
                        }
                    }), DispatcherPriority.Background);

                }
                cellWindowLevelsViewModel = comparedSeriesViewModel;

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
            finally
            {
                DisableUIForElecFilmOperation(false);
            }
        }

        private void LoadImageForComparePrint(ref FilmingPageControl film, ImageBase imageData, double windowWidth,
                                              double windowLevel)
        {
            try
            {
                Logger.LogFuncUp();

                _dataLoader.LoadSopByUid(imageData.SOPInstanceUID);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        #region AutoLoad

        public void LoadSeriesBy(IList<string> seriesUids)
        {
            var bParameterMistake = true;
            try
            {
                Logger.LogFuncUp();

                if (seriesUids == null) return;
                InitializeDefaultFilmingPage();

                var images = GetImageBaseOfSeries(seriesUids);
                var filmingCardModality = FilmingCardModality;

                if (IsLoadingConflictExist(seriesUids, filmingCardModality, images)) return;
                if (IsEFImageAndEmptyFilmingCard(images.FirstOrDefault(), filmingCardModality)) this.studyTreeCtrl.ChangeToEFLayout();

                ImagesLoadBeginning((uint)images.Count);
                SetTargetToReceiveImages();
                ActiveFilmingPageList.UnSelectAllCells();
                MedViewerControlCell targetSelectedCell = _loadingTargetPage.GetCellByIndex(_loadingTargetCellIndex);
                SelectObject(_loadingTargetPage, FilmPageUtil.ViewportOfCell(targetSelectedCell, _loadingTargetPage),
                             targetSelectedCell);
                seriesUids.ToList().ForEach(seriesId => _dataLoader.LoadSeries(seriesId));

                bParameterMistake = false;

                Logger.LogFuncUp();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            finally
            {
                if (bParameterMistake) FilmingViewerContainee.DataHeaderJobManagerInstance.JobFinished();
            }
        }

        private bool IsLoadingConflictExist(IList<string> seriesUids, string filmingCardModality, List<ImageBase> images)
        {
            var isConflict = IsSeriesconflicts(images, seriesUids.Count);
            if (isConflict.Item1)
            {
                FilmingViewerContainee.Main.ShowStatusInfo(isConflict.Item2);
                return true;
            }

            var result = IsSeriesFilmingCardConfilcts(images.FirstOrDefault(), filmingCardModality);
            if (result.Item1)
            {
                FilmingViewerContainee.Main.ShowStatusInfo(result.Item2);
                return true;
            }

            return false;
        }

        public void SetTargetToReceiveImages()
        {
            _loadingTargetPage = GetLastPageToHoldImage(EntityFilmingPageList);
            var cells = _loadingTargetPage.Cells.ToList();
            _loadingTargetCellIndex = cells.FindLastIndex(cell => !cell.IsEmpty) + 1;
        }

        private static bool IsEFImageAndEmptyFilmingCard(ImageBase image, string filmingCardModality)
        {
            return image.ImageType == FilmingUtility.EFilmImageType && filmingCardModality == "";
        }

        private Tuple<bool, string> IsSeriesFilmingCardConfilcts(ImageBase image, string filmingCardModality)
        {
            if (image.ImageType == FilmingUtility.EFilmImageType)
            {
                if (filmingCardModality != FilmingUtility.EFilmModality && filmingCardModality != "")
                {
                    return new Tuple<bool, string>(true,
                                                   "UID_Filming_Receive_Image_From_Other_Module_Warning_Cannot_Load_EFilm");
                }
                if (this.SeriesUID != image.SeriesInstanceUIDFk && this.SeriesUID != "")
                //&&FilmingCardModality == "EFilm" 
                {
                    return new Tuple<bool, string>(true,
                                                   "UID_Filming_Receive_Image_From_Other_Module_Warning_Cannot_Load_EFilm_Of_Different_Series");
                }
            }
            else
            {
                if (filmingCardModality == FilmingUtility.EFilmModality)
                {
                    return new Tuple<bool, string>(true,
                                                   "UID_Filming_Receive_Image_From_Other_Module_Warning_Cannot_Load_Normal_Image");
                }
            }

            return new Tuple<bool, string>(false, null);
        }

        private static Tuple<bool, string> IsSeriesconflicts(IEnumerable<ImageBase> imagesOfSeries, int seriesCount)
        {
            if (seriesCount > 1 && imagesOfSeries.Any(item => item.ImageType == FilmingUtility.EFilmImageType))
            {
                return new Tuple<bool, string>(true,
                                               "UID_Filming_Receive_Image_From_Other_Module_Warning_Cannot_Load_Multi_Series_Contains_EFilm");
            }

            return new Tuple<bool, string>(false, null);
        }

        private static List<ImageBase> GetImageBaseOfSeries(IEnumerable<string> seriesUids)
        {
            var images = new List<ImageBase>();
            foreach (var seriesUid in seriesUids)
            {
                images.AddRange(
                    FilmingDbOperation.Instance.FilmingDbWrapper.GetImageListByConditionWithOrder(
                        "SeriesInstanceUIDFk='" + seriesUid + "'",
                        "InstanceNumber"));
            }
            return images;
        }

        #endregion  //AutoLoad



        private List<MedViewerControlCell> _backupCells = new List<MedViewerControlCell>();
        /// <summary>
        /// pre condition:  startCell == null or startCell in film
        /// </summary>
        /// <param name="film"></param>
        /// <param name="startCell"></param>
        /// <param name="imageCount"></param>
        /// <returns></returns>
        public void BackupCells(FilmingPageControl film, MedViewerControlCell startCell, uint imageCount)
        {
            try
            {
                Logger.LogFuncUp();

                ClearImageLoadingContext();

                List<MedViewerControlCell> tempCells = new List<MedViewerControlCell>();

                if (film == null || film.Cells == null) return;

                var linkedFilms = GetLinkedPage(film);
                foreach (var page in linkedFilms)
                {
                    tempCells.AddRange(page.Cells);
                }

                // Remove current page 
                linkedFilms.Remove(film);
                _pagesToBeRefreshed = linkedFilms;

                int start = startCell == null ? 0 : startCell.CellIndex;
                int firstNonEmpty = -1;
                if (start < 0)
                    firstNonEmpty = -1;
                else
                    // Calculate the empty cells from the drop cell.
                    // Need to Backup all the cells from the first non-empty cell if the empty cell number is less than the images of the dropped series
                    firstNonEmpty = tempCells.FindIndex(start, cell => !cell.IsEmpty);
                if (firstNonEmpty == -1)
                {
                    // There is not non-empty cell, no need to backup
                    firstNonEmpty = start;
                    emptyCellCountFromDrop = 0;
                    return;
                }
                else
                {
                    emptyCellCountFromDrop = (uint)(firstNonEmpty - start);

                }
                // No need to backup the cells, the consecutive empty cells from drop point is large enough for the dropped series
                if (emptyCellCountFromDrop >= imageCount)
                {
                    _pagesToBeRefreshed.Clear();
                    return;
                }

                var firstNonEmptyCell = tempCells[firstNonEmpty];
                _movingTargetPage =
                    EntityFilmingPageList.GetFilmPageControlByViewerControl(firstNonEmptyCell.ViewerControl);
                if (_movingTargetPage != null)
                {
                    _movingTargetCellIndex = _movingTargetPage.Cells.ToList().IndexOf(firstNonEmptyCell);
                }

                // lastNonEmpty can't be -1 when we come here
                int lastNonEmpty = tempCells.FindLastIndex(cell => !cell.IsEmpty);

                // Now we need to keep the displaydata from these cells to newly created cells
                for (int i = firstNonEmpty; i <= lastNonEmpty; i++)
                {
                    MedViewerControlCell cell = new MedViewerControlCell();
                    MedViewerControlCell existingCell = tempCells.ElementAt(i);
                    if (!existingCell.IsEmpty)
                    {
                        existingCell.ForceEndAction();
                        cell.Image.AddPage(existingCell.Image.CurrentPage);
                    }
                    _cellsToBeMoveForward.Add(cell);
                    _backupCells.Add(existingCell);
                }

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void ClearImageLoadingContext()
        {
            _cellsToBeMoveForward.Clear();
            _pagesToBeRefreshed.Clear();
            _backupCells.Clear();
            emptyCellCountFromDrop = 0;
            _movingTargetPage = null;
            _movingTargetCellIndex = int.MaxValue;
        }

        public FilmingPageControl _loadingTargetPage;
        public int _loadingTargetCellIndex;
        public List<MedViewerControlCell> _cellsToBeMoveForward = new List<MedViewerControlCell>();
        public List<FilmingPageControl> _pagesToBeRefreshed = new List<FilmingPageControl>();
        private uint emptyCellCountFromDrop = 0;
        private FilmingPageControl _movingTargetPage = null;
        private int _movingTargetCellIndex = int.MaxValue;
        public DateTime StartLoadTime;
        //private DateTime EndLoadTime;


        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            if (e.EscapePressed || e.KeyStates == DragDropKeyStates.None)
                e.Action = DragAction.Cancel;

            base.OnQueryContinueDrag(e);
        }

        public void AddDataRepositry(IList<string> studyInstanceUIDList)
        {

            try
            {
                Logger.LogFuncUp();
                this.studyTreeCtrl.seriesSelectionCtrl.LoadStudies(studyInstanceUIDList);
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }
       

        private delegate bool ScrollFilmingBoard();

        private void OnfilmRegionGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (filmRegionGrid == sender as Grid)
            {
                if (IfZoomWindowShowState)
                {
                    ZoomViewer.CloseDialog();
                }
                ScrollFilmingBoard scroll;
                scroll = e.Delta < 0
                             ? new ScrollFilmingBoard(commands.GotoNextFilmBoard)
                             : new ScrollFilmingBoard(commands.GotoPreviousFilmBoard);
                int scrollCount = Math.Abs(e.Delta) / Mouse.MouseWheelDeltaForOneLine;
                for (int i = 0; i < scrollCount; i++)
                {
                    if (!scroll())
                        Logger.Instance.LogTraceWarning("wrong scroll");
                }
            }
        }

        #endregion [--Need to be classified--]

        #region Properties

        public bool SavingImagesFlag { get; set; }

        public ImgTxtDisplayState ImageTextDisplayMode { get; set; }

        public IViewerConfiguration FilmingViewerConfiguration
        {
            get
            {
                try
                {
                    return defaultFilmingPageControl.filmingViewerControl.Configuration;
                }
                catch (Exception ex)
                {
                    Logger.LogFuncException(ex.Message + ex.StackTrace);
                    return null;
                }
            }
        }

        #endregion

        #region Fields



        private uint _countOfImagesToBeLoaded;

        public uint CountOfImagesToBeLoaded
        {
            get { return _countOfImagesToBeLoaded; }
        }

        private uint _countOfImagesLoaded;
        private static object _countOfImagesLoadedLocker = new object();

        public uint CountOfImagesLoaded
        {
            get { return _countOfImagesLoaded; }
        }

        public DateTime LoadSeriesTimeStamp;


        #endregion

        #region Menu Item

        public bool IsCellSelected
        {
            get
            {
                bool any = ActiveFilmingPageList.Any(filmingPage => filmingPage.SelectedCells().Any());
                return any;
            }
        }

        public bool IsCellModalitySC
        {
            get { return FilmingCardModality == FilmingUtility.EFilmModality; }
        }

        #region For Performance by Chang

        public IEnumerable<FilmingPageControl> DisplayedFilmPage
        {
            get
            {
                var filmCount = EntityFilmingPageList.Count;
                var start = CurrentFilmPageBoardIndex * SelectedFilmCardDisplayMode;
                var end = start + SelectedFilmCardDisplayMode;
                if (end > filmCount) end = filmCount;
                for (int i = start; i < end; i++)
                {
                    var page = EntityFilmingPageList[i];
                    yield return page;
                }
            }
        }

        #endregion

        private bool _hasImageDisplayed = false;
        public bool HasImageDisplayed
        {
            get { return _hasImageDisplayed; }
            set
            {
                if (value != _hasImageDisplayed)
                {
                    _hasImageDisplayed = value;
                    CommonCommand.NotifyMainFrameAboutFilmingStatus(_hasImageDisplayed ? "-1" : null);
                }
            }
        }

        public void UpdateImageCount()
        {
            var totalImageCount = GetTotalImageCount();
            HasImageDisplayed = totalImageCount != 0;
            //imageCount.Text = totalImageCount.ToString();
        }

        public List<MedViewerControlCell> CollectSelectedCells()
        {
            var selectedCells =
                ActiveFilmingPageList.OrderBy(a => a.FilmPageIndex).ToList().Aggregate(new List<MedViewerControlCell>(),
                                                                                       (total, current) =>
                                                                                       {
                                                                                           total.AddRange(current.SelectedCells().OrderBy(cell => cell.CellIndex));
                                                                                           return total;
                                                                                       });
            return selectedCells;
        }

        public bool CollectFirstSectedCell(out FilmingPageControl page, out MedViewerControlCell cell)
        {
            var pages = ActiveFilmingPageList.OrderBy(a => a.FilmPageIndex).ToList();
            foreach (var pageControl in pages)
            {
                var cells = pageControl.SelectedCells().OrderBy(t => t.CellIndex);
                if (cells.Any())
                {
                    page = pageControl;
                    cell = cells.FirstOrDefault();
                    return true;
                }
            }
            page = null;
            cell = null;
            return false;
        }

        public int GetTotalImageCount()
        {
            return EntityFilmingPageList.Aggregate(0, (total, next) => total += next.GetImageCount());
        }

        public List<FilmingPageControl> GetLinkedPage(FilmingPageControl startPage)
        {
            int endIndex = GetLinkedPageEndIndex(startPage);


            return EntityFilmingPageList.GetRange(startPage.FilmPageIndex,
                                                  endIndex - startPage.FilmPageIndex);
        }

        public int GetLinkedPageEndIndex(FilmingPageControl startPage)
        {
            int endIndex = EntityFilmingPageList.Count;

            for (int i = startPage.FilmPageIndex + 1; i < endIndex; i++)
            {
                if (EntityFilmingPageList[i].FilmPageType == FilmPageType.BreakFilmPage)
                {
                    endIndex = i;
                    break;
                }
            }

            return endIndex;
        }

        #endregion


        #region ShortCuts Up Down Left Right Select cells
        public bool IsEnableSingleSelectByShortCut { get { return true; } }
        public bool IsEnableShiftSelectByShortCut { get { return true; } }

        #endregion

        #region Drag & Drop For Image Cut & Paste

        public void HitTest(Point currentPt)
        {
            _dropFilmingPage = null;
            _dropViewCell = null;
            VisualTreeHelper.HitTest(filmPageGrid,
                                     FilmingPageHitTestFilter,
                                     FilmingPageHitTestResult,
                                     new PointHitTestParameters(currentPt));
        }

        private void OnFilmCardGridMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                RemoveDraggedAdorner();

                Point currentPt = e.GetPosition(filmPageGrid);
                _dropFilmingPage = null;
                _dropViewCell = null;
                VisualTreeHelper.HitTest(filmPageGrid,
                                         FilmingPageHitTestFilter,
                                         FilmingPageHitTestResult,
                                         new PointHitTestParameters(currentPt));

                if (_dropFilmingPage != null && _dropViewCell != null && !_dropFilmingPage.IsInSeriesCompareMode)
                {
                    _canDrapDrop = CurrentActionType == ActionType.Pointer;
                }
                else
                {
                    _canDrapDrop = false;
                }
                filmPageGrid.Focus();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void OnFilmCardGridMouseRightButtonUp(object sender, MouseEventArgs e)
        {
            _isMouseMoveAndButtonPressed = false;
                
                //if (IfZoomWindowShowState)
                //{
                //    var zoomViewer = this.zoomWindowGrid.Children[0] as FilmingZoomViewer;
                //    if (zoomViewer != null)
                //    {
                //        Keyboard.Focus(zoomViewer.ctrlZoomViewer);
                //    } 
                //    return;
                //}
            
            _canDrapDrop = false;
        }

        private FilmingDragDropAdorner _draggedAdorner;
        //private MedViewerControlCellImpl _draggedData;

        //fix bug 596594 2016.02.26
        private bool _isMouseMoveAndButtonPressed = false;
        private bool PointsNear(Point p1, Point p2)
        {
            var deltaX = p1.X - p2.X;
            var deltaY = p1.Y - p2.Y;
            var distance = Math.Sqrt(Math.Abs(deltaX*deltaX - deltaY*deltaY));
            return distance < 1e-6;
        }

        private void FilmPageGridMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Point currentPt = e.GetPosition(filmPageGrid);

                _isMouseMoveAndButtonPressed = (e.RightButton == MouseButtonState.Pressed 
                    || e.MiddleButton == MouseButtonState.Pressed 
                    || e.LeftButton == MouseButtonState.Pressed)
                    && !PointsNear(currentPt, _rightMouseButtonDownPositionRelativeToFilmPageGrid);

                if (isMouseRightButtonUp)
                {
                    isMouseRightButtonUp = false;
                    return;
                }
                if (e.LeftButton == MouseButtonState.Pressed && _canDrapDrop && CurrentActionType == ActionType.Pointer &&
                    e.RightButton != MouseButtonState.Pressed)
                {
                    if (!IsCellSelected)
                    {
                        return;
                    }
                    List<MedViewerControlCell> cells = new List<MedViewerControlCell>();

                    var films =
                        from f in ActiveFilmingPageList
                        orderby f.FilmPageIndex
                        select f;

                    foreach (var film in films)
                    {
                        cells.AddRange(
                            from cell in film.SelectedCells()
                            orderby cell.CellIndex
                            select cell
                            );
                    }

                    if (cells.Any(medViewerControlCell => medViewerControlCell.IsGraphicSelected()))
                    {
                        return;
                    }
                    if (cells.Any(medViewerControlCell => medViewerControlCell.IsShutterSelected()))
                    {
                        return;
                    }

                    _dropFilmingPage = null;
                    _dropViewCell = null;
                    var sourceCell = cells[0];
               
                    string levelType = "image";
                    var imageList = new List<string>() {  };
                    var dragData = new object[] { levelType, imageList, "studylist",cells };
                    
                    VisualTreeHelper.HitTest(filmPageGrid,
                                             FilmingPageHitTestFilter,
                                             FilmingPageHitTestResult,
                                             new PointHitTestParameters(currentPt));

                    if (_dropViewCell != null && !_dropViewCell.IsEmpty && !IsCellModalitySC)
                    {
                        if (this.SelectedCells == 1)
                        {
                            var cellBitMap = new MedViewerScreenSaver(sourceCell.ViewerControl);
                            BitmapSource draggedBitMap = cellBitMap.RenderControlCellToBitmap
                                (new Size(sourceCell.ActualWidth * _dropFilmingPage.Scale.ScaleX,
                                          sourceCell.ActualHeight * _dropFilmingPage.Scale.ScaleY), sourceCell);

                            var adornerLayer = AdornerLayer.GetAdornerLayer(filmPageGrid);
                            this._draggedAdorner = new FilmingDragDropAdorner(draggedBitMap,
                                                                              filmPageGrid, adornerLayer);
                            DragDrop.DoDragDrop(filmPageGrid, dragData, DragDropEffects.Move);

                            //if (sourceCell != _dropViewCell)
                            //    sourceCell.IsSelected = false;
                        }
                        else
                        {
                            DragDrop.DoDragDrop(filmPageGrid, dragData, DragDropEffects.None);
                        }
                    }
                    RemoveDraggedAdorner();
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                RemoveDraggedAdorner();
            }
           

        }

        private int SelectedCells
        {
            get
            {
                int count = 0;
                if (ActiveFilmingPageList.Count != 0)
                {
                    foreach (FilmingPageControl con in ActiveFilmingPageList)
                    {
                        count += con.SelectedCells().Count;
                    }
                }
                return count;
            }
        }

        private int SelectedCellsFromEntityFilmingPageList
        {
            get
            {
                int count = 0;
                if (EntityFilmingPageList.Count != 0)
                {
                    foreach (FilmingPageControl con in EntityFilmingPageList)
                    {
                        foreach (MedViewerControlCell cell in con.Cells)
                        {
                            if (cell.IsSelected)
                            {
                                count += 1;
                            }
                        }
                    }
                }
                return count;
            }
        }

        private void RemoveDraggedAdorner()
        {
            if (this._draggedAdorner != null)
            {
                this._draggedAdorner.Detach();
                this._draggedAdorner = null;
            }
        }


        private void ProcessDropSelectFromStudyTree(DragEventArgs e)
        {
            Point currentPt = e.GetPosition(filmPageGrid);

            _dropFilmingPage = null;
            _dropViewPort = null;
            _dropViewCell = null;

            // Hit test to find out the cell and viewport to be dropped on
            VisualTreeHelper.HitTest(filmPageGrid,
                                     FilmingPageHitTestFilter,
                                     FilmingPageHitTestResult,
                                     new PointHitTestParameters(currentPt));
        }

        private void ProcessDropSelectFromFilmingPage()
        {
            if (!CanAcceptImages()) return;

            var clipboardBackup = new List<DisplayData>();
            try
            {
                clipboardBackup.AddRange(commands._clipboard.ToList());
                commands.CutImagesToClipboard();
                DoDropFromFilmingPage();
                if (IsEnableRepack)
                {
                    this.contextMenu.Repack(RepackMode.RepackDrag);
                }
                else
                {
                    //todo: performance optimization begin pageTitle
                    _loadingTargetPage.RefereshPageTitle();
                    //todo: performance optimization end
                    RefreshAnnotationDisplayMode();
                }
                ReOrderCurrentFilmPageBoard();
            }
            catch (System.Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            finally
            {
                commands._clipboard.Clear();
                commands._clipboard.AddRange(clipboardBackup.ToList());

                UpdateUIStatus();
                EntityFilmingPageList.UpdateAllPageTitle();
            }
        }

        private void DoDropFromFilmingPage()
        {
            InitializeBeforeDrop();
            InsertCells(CreateCellsByDisplayData(commands._clipboard));
        }

        //
        // 插入图像步骤:首先进行移动控件的备份，然后进行相关的替换
        //
        //private FilmingLayoutCell _startLayoutCell;
        public void InsertCells(List<MedViewerControlCell> cells)
        {
            BackupCells(_dropFilmingPage, _dropViewCell, (uint)cells.Count());
            _loadingTargetPage = _dropFilmingPage;
            _loadingTargetCellIndex = _dropViewCell.CellIndex;

            cells.ForEach(cell => ReplaceCellBy(cell, true));

            _cellsToBeMoveForward.ForEach(cell => ReplaceCellBy(cell));

            _cellsToBeMoveForward.Clear();
        }

        public void InsertCells(List<MedViewerControlCell> cells, FilmingPageControl destFilmingPage, MedViewerControlCell destCell)
        {
            BackupCells(destFilmingPage, destCell, (uint)cells.Count());
            _loadingTargetPage = destFilmingPage;
            _loadingTargetCellIndex = destCell.CellIndex;

            cells.ForEach(cell => ReplaceCellBy(cell, true));

            _cellsToBeMoveForward.ForEach(cell => ReplaceCellBy(cell));

            _cellsToBeMoveForward.Clear();
        }

        private void InitializeBeforeDrop()
        {
            EntityFilmingPageList.UnselectOtherFilmingPages(_dropFilmingPage);
            EntityFilmingPageList.UnSelectAllCells();

            SetFocusPosition();
        }

        public void SetFocusPosition()
        {
            LastSelectedFilmingPage = _dropFilmingPage;
            LastSelectedCell = _dropViewCell;
            LastSelectedViewport = FilmPageUtil.ViewportOfCell(_dropViewCell, _dropFilmingPage);
        }

        public List<MedViewerControlCell> CreateCellsByDisplayData(List<DisplayData> displayDatas)
        {
            List<MedViewerControlCell> dCells = new List<MedViewerControlCell>();
            foreach (var displayData in displayDatas)
            {
                MedViewerControlCell cell = new MedViewerControlCell();
                if (displayData != null)
                    cell.Image.AddPage(displayData);
                dCells.Add(cell);
            }
            return dCells;
        }

        private bool CanAcceptImages()
        {
            return _dropFilmingPage != null && _dropViewCell != null && !_dropFilmingPage.IsInSeriesCompareMode;
        }

        private bool IsSelectFromFimingPage(List<MedViewerControlCell> cells)
        {
            return cells != null && cells.Any();
        }

        private void FilmPageGridPreviewDragOver(object sender, DragEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                //Get the filming page object under mouse point 
                if (e.Data == null)
                    return;

                //if (_draggedAdorner == null)
                //    return;
                if(IfZoomWindowShowState)
                    ZoomViewer.CloseDialog();
                if (e.Data.GetDataPresent(typeof(Object[])))
                {
                    var draggedItem = (Object[])e.Data.GetData(typeof(Object[]));
                    if ((draggedItem == null) || (draggedItem.Count() != 4) || (draggedItem[3] == null)) return;
                    var cells = draggedItem[3] as List<MedViewerControlCell>;
                    if (cells != null)
                    {
                        _hasDelayedClickEvent = false;

                        Point currentPt = e.GetPosition(filmPageGrid);
                        _dropFilmingPage = null;
                        _dropViewCell = null;
                        VisualTreeHelper.HitTest(filmPageGrid,
                                                 FilmingPageHitTestFilter,
                                                 FilmingPageHitTestResult,
                                                 new PointHitTestParameters(currentPt));

                        if (_dropFilmingPage != null && _dropViewCell != null && cells.IndexOf(_dropViewCell) == -1)
                        {
                            bool isEnableDrop = !_dropFilmingPage.IsInSeriesCompareMode;
                            if (isEnableDrop)
                            {
                                int selectedCellCnt = 0;
                                foreach (var page in ActiveFilmingPageList)
                                {
                                    selectedCellCnt += page.SelectedCells().Count;
                                }

                                if (selectedCellCnt > 1)
                                {
                                    _dragDropEffects = LocalDragDropEffects.Forbit;
                                    return;
                                }

                                // Point contentPoint = _dropViewCell.CellControl.PointFromScreen(NativeMethods.GetMousePosition());

                                Point mouseFilmingPagePosition = e.GetPosition(filmRegionGrid);
                                _draggedAdorner.SetPosition(mouseFilmingPagePosition.X, mouseFilmingPagePosition.Y);
                                _dragDropEffects = LocalDragDropEffects.InsertBefore;
                                return;
                            }
                        }
                        Point mouseViewCellPosition = e.GetPosition(filmRegionGrid);
                        if (_draggedAdorner != null)
                            _draggedAdorner.SetPosition(mouseViewCellPosition.X, mouseViewCellPosition.Y);
                    }
                    //_dragDropEffects = LocalDragDropEffects.Forbit;
                    return;
                }
                _dragDropEffects = LocalDragDropEffects.None;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                _dragDropEffects = LocalDragDropEffects.None;
            }
        }

        public FilmingPageControl _dropFilmingPage;
        public MedViewerControlCell _dropViewCell;
        public MedViewerControlCellImpl _dropViewCellImpl;
        public MedViewerLayoutCell _dropViewPort;
        public MedViewerLayoutCellImpl _dropViewPortImpl;
        public MedViewerControlCell _preDropViewCell;
        public MedViewerControlCellImpl _preDropViewCellImpl;

        private LocalDragDropEffects _dragDropEffects = LocalDragDropEffects.None;

        public HitTestFilterBehavior FilmingPageHitTestFilter(DependencyObject o)
        {
            if (null == o)
            {
                return HitTestFilterBehavior.Continue;
            }

            if (o.GetType().Name != typeof(FilmingPageControl).Name &&
                o.GetType() != typeof(FilmingControlCellImpl/*MedViewerControlCellImpl*/) &&
                o.GetType() != typeof(FilmingLayoutCellImpl/*MedViewerLayoutCellImpl*/))
            {
                return HitTestFilterBehavior.Continue;
            }

            var uiElement = o as UIElement;
            if (uiElement == null || uiElement.Visibility != Visibility.Visible)
            {
                return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
            }

            var filming = o as FilmingPageControl;
            if (filming != null)
            {
                _dropFilmingPage = filming;
                return HitTestFilterBehavior.Continue;
            }

            var viewCell = o as MedViewerControlCellImpl;
            if (viewCell != null)
            {
                #region [filming参考线临时代码，待需求定型后，需改进][edit by jinyang.li]
                if (null != viewCell.DataSource && -1 == viewCell.DataSource.CellIndex)
                {
                    _dropViewCellImpl = _preDropViewCellImpl;
                    _dropViewCell = _preDropViewCell;
                }
                else
                {
                    _dropViewCellImpl = viewCell;
                    _preDropViewCellImpl = _dropViewCellImpl;
                    _dropViewCell = viewCell.DataSource;
                    _preDropViewCell = _dropViewCell;
                }
                #endregion
            }

            var viewPort = o as MedViewerLayoutCellImpl;
            if (viewPort != null)
            {
                _dropViewPortImpl = viewPort;
                _dropViewPort = viewPort.DataSource;
            }

            return HitTestFilterBehavior.Continue;
        }

        public HitTestResultBehavior FilmingPageHitTestResult(HitTestResult result)
        {
            // Set the behavior to return visuals at all z-order levels.
            return HitTestResultBehavior.Continue;
        }

        #endregion


        #region [--Need to be classified--]
        private void FilmCardScrollBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                Logger.LogFuncUp();

                var newFilmBoardIndex = (int)(e.NewValue + 0.5);
                //(int)(e.NewValue+1 - filmCardScrollBar.ViewportSize + 0.5);
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + newFilmBoardIndex);
                GotoFilmBoardWithIndex(newFilmBoardIndex);

                Logger.LogTimeStamp("结束滚轮");
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }


        private void FilmingCardKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                var customCellLayoutWindow = layoutCtrl.CustomCellLayoutWindow;
                if (customCellLayoutWindow != null)
                {
                    customCellLayoutWindow.CloseProtoclWindow(customCellLayoutWindow.FindAncestorWindow(), new CancelEventArgs());
                    WindowHostManagerWrapper.CloseSecondaryWindow();
                }
                HostAdornerCount--;
                
                actiontoolCtrl.OnPointerButtonClick(null, null);
            }
        }

        #region study list control

        //private InterleavedSelection _interSelection;
        public InterleavedSelection InterSelection
        {
            get { return InterleavedSelection.Singleton; }
        }

        public CombinePrinting CombinePrint
        {
            get { return CombinePrinting.CombinePrint; }
        }

        #endregion

        private void filmingCard_PreviewKeyDown(object sender, KeyEventArgs e)
        {           
            Key curKey = e.Key;
            if (curKey == Key.ImeProcessed)
            {
                curKey = e.ImeProcessedKey;
                var txtBox = Keyboard.FocusedElement as TextBox;

                if (curKey == Key.Space && txtBox == null)
                {
                    BtnEditCtrl.OnSelectSeries(null, null);
                }
                if (curKey == Key.D1 || curKey == Key.NumPad1 ||
                    curKey == Key.D2 || curKey == Key.NumPad2 ||
                    curKey == Key.D3 || curKey == Key.NumPad3 ||
                    curKey == Key.D4 || curKey == Key.NumPad4 ||
                    curKey == Key.D5 || curKey == Key.NumPad5 ||
                    curKey == Key.D6 || curKey == Key.NumPad6 ||
                    curKey == Key.D7 || curKey == Key.NumPad7 ||
                    curKey == Key.D8 || curKey == Key.NumPad8 ||
                    curKey == Key.D9 || curKey == Key.NumPad9 ||
                    curKey == Key.D0 || curKey == Key.NumPad0)
                {
                    var inputBinding = this._ptInputBindingList.FirstOrDefault(t => t.Key == curKey);
                    if(inputBinding != null && this.InputBindings.Contains(inputBinding))
                        inputBinding.Command.Execute(null);
                }
            }

            if (FilmingViewerContainee.IsBusy)
            {               
                if (e.Key == Key.Escape)
                {
                    e.Handled = true;
                }
            }

            if (e.Key != Key.Delete)
            {
                return;
            }
            if (_miniCellsList.Count>0)
            {
                this.contextMenu.DeleteSelectedLocalizedImageReferenceLine();
                _miniCellsList.Clear();
                _miniCellsParentCellsList.Clear();
                if (!IfZoomWindowShowState)
                    commands._ifDeleteReferenceImage = true;
                var page = DisplayedFilmPage.FirstOrDefault();
                if (page != null && !IfZoomWindowShowState)
                {
                    var viewport = page.ViewportList.FirstOrDefault();
                    var cell = page.Cells.FirstOrDefault();
                    SelectObject(page, viewport, cell);
                }
            }
            commands._isDeleteGraphic =
                ActiveFilmingPageList.Any(filmPage => filmPage.SelectedCells().Any(cell => cell.IsGraphicSelected()));
        }

        private void OnImageLoadingprogressBarToolTipOpening(object sender, ToolTipEventArgs e)
        {

        }

        private MouseButton _mouseGestureButton = MouseButton.Left;

        public MouseButton MouseGestureButton
        {
            get { return _mouseGestureButton; }
            set { _mouseGestureButton = value; }
        }

        public bool IsLRButtonDown = false;
        private Point _rightMouseButtonDownPositionRelativeToFilmPageGrid;
        private void OnFilmPageGridPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                IsLRButtonDown = false;
                if (e.RightButton == MouseButtonState.Pressed)
                    _rightMouseButtonDownPositionRelativeToFilmPageGrid = e.GetPosition(filmPageGrid);

                MouseGestureButton = MouseButton.Left;

                if (e.LeftButton == MouseButtonState.Pressed
                    && e.RightButton == MouseButtonState.Pressed)
                {
                    MouseGestureButton = MouseButton.XButton1;
                    _canDrapDrop = false;
                    IsLRButtonDown = true;
                }
                else
                {
                    if (e.MiddleButton == MouseButtonState.Pressed)
                    {
                        MouseGestureButton = MouseButton.Middle;
                        _canDrapDrop = false;
                    }
                    else if (e.RightButton == MouseButtonState.Pressed)
                    {
                        MouseGestureButton = MouseButton.Right;
                        _canDrapDrop = false;
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.LogFuncException(exp.Message);
            }
        }

        private bool isMouseRightButtonUp = false;

        private void OnFilmPageGridMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
                isMouseRightButtonUp = true;
            //由于ctrl+mouseup取消选中是在medviewerCtrl里做的，无法取消page选中
            //故在此处检验是否page上无cell选中，若是则取消page选中，以防产生UI错误，DIM637750
            if (e.ChangedButton == MouseButton.Left
                && e.ClickCount == 1
                && e.ButtonState == MouseButtonState.Released
                && (Keyboard.IsKeyDown(Key.LeftCtrl)||Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                foreach (var pageControl in DisplayedFilmPage)
                {
                    if(!pageControl.IsSelected) continue;
                    if (pageControl.SelectedCells() == null || pageControl.SelectedCells().Count == 0)
                    {
                        pageControl.IsSelected = false;
                        pageControl.SelectedAll(false);
                    }
                }
            }
            if (IfZoomWindowShowState)
            {
                ZoomViewer.RefreshDisplayCell();
            }
            UpdateUIStatus();
        }

        #endregion [--Need to be classified--]

        public void SwitchToInterleavePrint(List<string> seriesUidList)
        {
            this.studyTreeCtrl.seriesSelectionCtrl.SelectSeries(seriesUidList.ToList());
            if (!this.studyTreeCtrl.IsEnableBatchFilm)
            {
                MessageBoxHandler.Instance.ShowError("UID_MessageBox_Series_NOT_Interleave_Print");
                return;
            }

            this.studyTreeCtrl.OnBatchFilmClick(null, null);
        }

        private void FilmingCard_ButtonClick(object sender, RoutedEventArgs e)  //控制面板中按钮点下时响应
        {
            if (this.actiontoolCtrl.comBoxRotate.IsDropDownOpen
                || this.actiontoolCtrl.comboxFlip.IsDropDownOpen
                || this.actiontoolCtrl.cmbFilmingRegionShape.IsDropDownOpen) return; //fix bug540096第二次以上点击镜像或旋转按钮时不能展开

            if (LastSelectedFilmingPage != null)
                LastSelectedFilmingPage.filmingViewerControl.Focus();
        }

       

        private void _maskBorder_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IfZoomWindowShowState)
            {
                var zoomViewer = this.zoomWindowGrid.Children[0] as FilmingZoomViewer;
                if (zoomViewer != null)
                {
                    Keyboard.Focus(zoomViewer.ctrlZoomViewer); 
                }
            }
        }
    }


}
