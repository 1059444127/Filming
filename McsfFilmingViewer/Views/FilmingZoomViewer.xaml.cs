using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UIH.Mcsf.Core;
using UIH.Mcsf.Filming.Model;
using UIH.Mcsf.MHC;
using UIH.Mcsf.Viewer;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.AppControls.Viewer;
using UIH.Mcsf.Filming.Widgets;
using ICommand = System.Windows.Input.ICommand;

namespace UIH.Mcsf.Filming.Views
{
    /// <summary>
    /// Interaction logic for FilmingZoomViewer.xaml
    /// </summary>
    public partial class FilmingZoomViewer 
    {
        //关闭小窗口时Repack;
        public Action RepackWhenClose;

        //弹出窗口
        //private readonly MessageWindow _msgWindow = new MessageWindow();
        public bool IfMiniCellSelected = false;
        
        //所有展示图片（cell）
        private readonly List<FilmingControlCell> _allImages = new List<FilmingControlCell>();

        //所有展示图片的PS状态（克隆）
        //private readonly List<PresentationState> _presentationStates = new List<PresentationState>();

        //记录cell在哪个屏幕的索引（后方翻页需要）
        private readonly List<int> _allCellsDisplayFilmBordIndexList = new List<int>();

        //记录cell所在page Index
        private readonly List<int> _allCellsFilmPageIndexList = new List<int>();

        //记录选中图片
        private readonly List<int>  _selectedCellIndexes = new List<int>();

        //当前显示图片在所有图片中的编号
        private int _currentCellIndex;

        //当前ViewControl所支持的动作列表
        private readonly Dictionary<MouseButton, ActionType> _actionMap = new Dictionary<MouseButton, ActionType>();

        //记录展示图片原始PS信息(克隆）
        private PresentationState _originalPSState;

        //记录是否需要关闭时出发Repack
        private bool _ifNeedRepack = false;

        //记录是否序列选中状态
        private bool _ifSericeSelected = false;

        private readonly FilmingCard _filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
        //记录是否多个序列被选中
        private bool _ifMultiSericeSelected = false;
        //区分系列
        private List<string> seiresUIDandUserInfoList
        {
            get
            {
                var uidList = new List<string>();
                foreach (var cellIndex in _selectedCellIndexes)
                {
                    var cp = _allImages[cellIndex].Image.CurrentPage;
                    var dicomHeader = cp.ImageHeader.DicomHeader;
                    var seriesUid = dicomHeader[ServiceTagName.SeriesInstanceUID];
                    var userInfo = cp.UserSpecialInfo;
                    if (!uidList.Contains(seriesUid+userInfo))
                    {
                        uidList.Add(seriesUid + userInfo);
                    }
                }
                if (uidList.Count > 1) _ifMultiSericeSelected = true;
                else _ifMultiSericeSelected = false;
                return uidList;
            }
        }

        //public MessageWindow MsgWindow
        //{
        //    get { return _msgWindow; }
        //}

        //获取展示cell
        public MedViewerControlCell DisplayedZoomCell
        {
            get { return ctrlZoomViewer.Cells.FirstOrDefault(); }
        }
        //获取展示cell 的PS状态
        public PresentationState CurCellpsState
        {
            get { return DisplayedZoomCell.Image.CurrentPage.PState; }
        }

        public FilmingZoomViewer(List<FilmingControlCell> allImages, int currentCellIndex,List<int> allCellsFilmPageIndexList, List<int> allCellsDisplayFilmBordIndexList)
        {

            InitializeComponent();

            //初始化
            _allImages = allImages;
            _currentCellIndex = currentCellIndex;
            _allCellsDisplayFilmBordIndexList = allCellsDisplayFilmBordIndexList;
            _allCellsFilmPageIndexList = allCellsFilmPageIndexList;
            //初始化窗口

            ctrlZoomViewer.InitializeWithoutCommProxy(Configure.Environment.Instance.FilmingUserConfigPath);
            ctrlZoomViewer.LayoutManager.SetLayout(1,1);
            ctrlZoomViewer.GraphicContextMenu.IsGraphicsContextMenuEnabled = false;
            ctrlZoomViewer.Controller.LoadConfigReader();
            
            //事件注册
            //_msgWindow.Closing += (sender, args) => OnClosing();     //关闭放大窗口时需要执行操作集合。1.通知filming 执行Repack操作
            //_msgWindow.Activated += new EventHandler(msgWindow_Activated); //刚打开窗口时需要执行操作集合，1.放大窗口获得焦点
            ctrlZoomViewer.MedviewerControlIsInEditChanged += OnMedviewerControlIsInEditChanged;
            _filmingCard._maskBorder.MouseWheel += vwControl_OnMouseWheel;
            _filmingCard.zoomWindowGrid.ContextMenuOpening += ZoomGridContextMenuOpening;
            ctrlZoomViewer.MouseUp += ctrlZoomViewer_MouseUp;
            ctrlZoomViewer.SynGraphicToCellOperated += OnSynGraphicToCellOperated;
            InitializeSyncDrawGraphics();
            //将双击图片载入到放大窗口
            var displaycell = _allImages[_currentCellIndex].Clone();
            ctrlZoomViewer.AddCell(displaycell);
            SetSuvBoundForPetImage();
            //displaycell.Refresh();

            //设置显示cell Action
            SetAction();
            
            //PSStateCopy(displaycell.Image.CurrentPage.PState,_presentationStates[currentCellIndex]);
            _originalPSState = CurCellpsState.Clone();
            displaycell.IsSelected = true;
            displaycell.Refresh();

            if (displaycell.Image.CurrentPage.ImageHeader.DicomHeader.ContainsKey(ServiceTagName.SeriesInstanceUID))
            {
                _ifSericeSelected = true;
                var uid = displaycell.Image.CurrentPage.ImageHeader.DicomHeader[ServiceTagName.SeriesInstanceUID];
                var userSpecialInfo = displaycell.Image.CurrentPage.UserSpecialInfo;
                var uidListandUserInforList = new List<string>() { uid + userSpecialInfo };
                _selectedCellIndexes.Clear();
                _selectedCellIndexes.AddRange(_filmingCard.SelectSeriesForZoomViewer(uidListandUserInforList));
            }
            else
            {
                //记录所选中的cell的Index
                _selectedCellIndexes.Clear();
                _selectedCellIndexes.Add(_currentCellIndex);
            }
            _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
            //SetCellFocusBorder(!_ifSericeSelected);
            BindInputings();
        }

        private void ZoomGridContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (!ctrlZoomViewer.CanOpenContextMenu())
            {
                e.Handled = true;
            }
        }

        //刚打开窗口时需要执行操作集合，1.放大窗口获得焦点
        //private void msgWindow_Activated(object sender, EventArgs e)
        //{
        //    ctrlZoomViewer.Focus();
        //}

        //同步PS状态到后方选中胶片
        private bool _ifScaleStateChanged = false;
        private bool _ifPanStateChanged = false;
        private bool _ifWLStateChaged = false;
        private void SyncPSState(List<int> selectedCellIndexes)
        {
            if (IfPSStateChanged())
            {
                PSStateChangedRecord();
                foreach (var cellIndex in selectedCellIndexes)
                {
                    var cp = _allImages[cellIndex].Image.CurrentPage;
                    if(cp.Modality != DisplayedZoomCell.Image.CurrentPage.Modality&&_ifWLStateChaged) continue;
                    PSStateCopy(cp.PState, CurCellpsState);
                    _allImages[cellIndex].Refresh();
                }
                _originalPSState = CurCellpsState.Clone();
                if (_ifScaleStateChanged)
                {
                    _filmingCard.BtnEditCtrl.GetScaleOfSelectedCells();
                    _ifScaleStateChanged = false;
                }
                _ifPanStateChanged = false;
                _ifWLStateChaged = false;
            }
            else
            {
                if (!DisplayedZoomCell.Image.CurrentPage.IsTableOrCurve)
                {
                    _allImages[_currentCellIndex].Image.CurrentPage.PSXml =
                        DisplayedZoomCell.Image.CurrentPage.SerializePSInfo();
                }
                else
                {
                    _allImages[_currentCellIndex].Image.CurrentPage.PSXml =
                        DisplayedZoomCell.Image.CurrentPage.PSXml;
                }
                _allImages[_currentCellIndex].Image.CurrentPage.DeserializePSInfo();
            }
        }
        //为当前cell设置鼠标Action
        private void SetAction()
        {
            _actionMap.Clear();
            _actionMap[MouseButton.Left] = ActionType.Pointer;
            _actionMap[MouseButton.Right] = ActionType.Zoom;
            _actionMap[MouseButton.Middle] = ActionType.Windowing;
            _actionMap[MouseButton.XButton1] = ActionType.Pan;
            foreach (KeyValuePair<MouseButton, ActionType> pair in _actionMap)
            {
                ctrlZoomViewer.SetAction(pair.Value, pair.Key);
            }
        }

        //替换显示的cell
        private void DisplayCell()
        {
            try
            {
                var cell = _allImages[_currentCellIndex].Clone();
                ctrlZoomViewer.RemoveCell(0);
                //PSStateCopy(cell.Image.CurrentPage.PState , _presentationStates[_currentCellIndex]);
                ctrlZoomViewer.AddCell(cell);
                SetSuvBoundForPetImage();
                cell.IsSelected = true;
                _originalPSState = CurCellpsState.Clone();
                cell.Refresh();
                _filmingCard.BtnEditCtrl.GetScaleOfSelectedCells();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                CloseDialog();
            }
        }

        private void SetSuvBoundForPetImage()
        {
            var petExtension = DisplayedZoomCell.Image.CurrentPage.DisplayDataExtension as PETExtension;
            var imageHeader =    DisplayedZoomCell.Image.CurrentPage.ImageHeader;
            if (petExtension != null&& imageHeader != null)
            {
                petExtension.SetSUVBound(DisplayedZoomCell.ViewerControlSetting.Configuration, imageHeader);
            }
        }

        //关闭窗口执行操作
        protected void OnClosing()
        {
            try
            {
                //_msgWindow.Hide(); // 先隐藏窗口，展示胶片
                
                _filmingCard.IfZoomWindowShowState = false; //标记设置
                _filmingCard._maskBorder.MouseWheel -= vwControl_OnMouseWheel;
                _filmingCard._maskBorder.Cursor = Cursors.Wait;
                _filmingCard.zoomWindowGrid.ContextMenuOpening -= ZoomGridContextMenuOpening;
                ctrlZoomViewer.SynGraphicToCellOperated -= OnSynGraphicToCellOperated;
                if (_allImages.Count > 0)
                {
                    foreach (var cellIndex in _selectedCellIndexes)
                    {
                        _allImages[cellIndex].SetCellFocusSelected(true,false);
                    }
                }

                if (_ifNeedRepack)   //删除后执行repack操作
                {
                    RepackWhenClose();
                }
                //清空数据
                _allImages.Clear();
                //_presentationStates.Clear();
                _allCellsDisplayFilmBordIndexList.Clear();
                _allCellsFilmPageIndexList.Clear();

                grdZoomViewer.Children.Clear();
            }
            catch (Exception)
            {
                Logger.Instance.CreateLogger("Closing Window Failed.");
            }
        }
        //设置Zoom窗口的属性
        public void ShowDialog()
        {
            _filmingCard.IfZoomWindowShowState = true;
            //_msgWindow.WindowChild = this;
            //_msgWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            //_msgWindow.WindowDisplayMode = WindowDisplayMode.No_BackGround;
            _filmingCard.zoomWindowGrid.Children.Clear();
            _filmingCard.zoomWindowGrid.Children.Add(this);
            _filmingCard.zoomWindowGrid.HorizontalAlignment = _filmingCard.FilmingControlPanelLocation == "left" ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            //_filmingCard.DisableUIElement();
            //_filmingCard.HostAdornerCount++;
            //_filmingCard._maskBorder.Cursor = Cursors.Arrow;
            _filmingCard.UpdateButtonStatus();
            _filmingCard.zoomWindowGrid.Visibility = Visibility.Visible;
            //_msgWindow.Top = 1122-ctrlZoomViewer.Height;
            
            ctrlZoomViewer.Focus();
            Keyboard.Focus(_filmingCard.zoomWindowGrid);
            //_msgWindow.ShowModelDialog();
        }

        public void CloseDialog()
        {
            Logger.Instance.LogPerformanceRecord("[Begin][ZoomViewerExit]");
            _filmingCard.IfZoomWindowShowState = false;

            _filmingCard.zoomWindowGrid.Children.Clear();
            
            _filmingCard.zoomWindowGrid.HorizontalAlignment = _filmingCard.FilmingControlPanelLocation == "left" ? HorizontalAlignment.Left : HorizontalAlignment.Right;
            OnClosing();
            //_filmingCard.EnableUI();;
            _filmingCard.UpdateButtonStatus();
            
            _filmingCard.zoomWindowGrid.Visibility = Visibility.Collapsed;
            //_msgWindow.Top = 1122-ctrlZoomViewer.Height;
            Logger.Instance.LogPerformanceRecord("[End][ZoomViewerExit]");
            //_msgWindow.ShowModelDialog();
        }
        //判断Action相关ps状态是否有变化,比较当签显示ps状态和原始ps状态
        private bool IfPSStateChanged()
        {
            var curCellPSState = DisplayedZoomCell.Image.CurrentPage.PState;
            if (_originalPSState.WindowLevel == curCellPSState.WindowLevel &&
                    Math.Abs(_originalPSState.ScaleX - curCellPSState.ScaleX) < 0.0001 &&
                    Math.Abs(_originalPSState.ScaleY - curCellPSState.ScaleY) < 0.0001 &&
                    Math.Abs(_originalPSState.OffsetTop - curCellPSState.OffsetTop) < 0.0001 &&
                    Math.Abs(_originalPSState.OffsetLeft - curCellPSState.OffsetLeft) < 0.0001 &&
                    Math.Abs(_originalPSState.RenderCenterX - curCellPSState.RenderCenterX) < 0.0001 &&
                    Math.Abs(_originalPSState.RenderCenterY - curCellPSState.RenderCenterY) < 0.0001)
                return false;
            return true;
        }

        private void PSStateChangedRecord()
        {
            var curCellPSState = DisplayedZoomCell.Image.CurrentPage.PState;
            if (Math.Abs(_originalPSState.ScaleX - curCellPSState.ScaleX) > 0.0001 ||
                Math.Abs(_originalPSState.ScaleY - curCellPSState.ScaleY) > 0.0001)
                _ifScaleStateChanged = true;
            if (Math.Abs(_originalPSState.OffsetTop - curCellPSState.OffsetTop) > 0.0001 ||
                Math.Abs(_originalPSState.OffsetLeft - curCellPSState.OffsetLeft) > 0.0001 ||
                Math.Abs(_originalPSState.RenderCenterX - curCellPSState.RenderCenterX) > 0.0001 ||
                Math.Abs(_originalPSState.RenderCenterY - curCellPSState.RenderCenterY) > 0.0001)
                _ifPanStateChanged = true;
            if (_originalPSState.WindowLevel != curCellPSState.WindowLevel)
                _ifWLStateChaged = true;
        }

        //判断Action相关ps状态是否有变化,比较传入的两个PS状态
        private bool IfPSStateChanged(PresentationState psState1,PresentationState psState2 )
        {
            try
            {
                if (psState1.WindowLevel == psState2.WindowLevel &&
                    Math.Abs(psState1.ScaleX - psState2.ScaleX) < 0.0001 &&
                    Math.Abs(psState1.ScaleY - psState2.ScaleY) < 0.0001 &&
                    Math.Abs(psState1.OffsetTop - psState2.OffsetTop) < 0.0001 &&
                    Math.Abs(psState1.OffsetLeft - psState2.OffsetLeft) < 0.0001 &&
                    Math.Abs(psState1.RenderCenterX - psState2.RenderCenterX) < 0.0001 &&
                    Math.Abs(psState1.RenderCenterY - psState2.RenderCenterY) < 0.0001)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                return false;
            }
        }

        // 提供Action相关的PS状态的拷贝功能，不会引起绑定事件被覆盖问题
        private void PSStateCopy(PresentationState psDest,PresentationState psOriginal)
        {
            try
            {
                if (psOriginal != null && psDest != null)
                {
                    if(_ifWLStateChaged)
                        psDest.WindowLevel = psOriginal.WindowLevel;
                    if(_ifScaleStateChanged)
                    {
                        psDest.ScaleX = psOriginal.ScaleX;
                        psDest.ScaleY = psOriginal.ScaleY;
                    }
                    if (_ifPanStateChanged)
                    {
                        //double offsetX = psOriginal.OffsetLeft / psOriginal.RenderWidth * psDest.RenderWidth - psDest.OffsetLeft;
                        //double offsetY = psOriginal.OffsetTop / psOriginal.RenderHeight * psDest.RenderHeight - psDest.OffsetTop;
                        //psDest.Translate(offsetX, offsetY);
                        //psDest.RenderCenterX = psOriginal.RenderCenterX;
                        //psDest.RenderCenterY = psOriginal.RenderCenterY;
                        psDest.TranslateByRenderCenter(psOriginal.RenderCenterX,psOriginal.RenderCenterY);
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

        //窗口的滚轮翻页操作
        private void vwControl_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (IfMiniCellSelected)
            {
                
                return;
            }
            try
            {
                int toChooseCellIndex = _currentCellIndex;
                //Alt加滚轮逻辑
                if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                {
                    if (e.Delta < 0)
                    {
                        toChooseCellIndex = _selectedCellIndexes.Max();
                    }
                    else
                    {
                        toChooseCellIndex = _selectedCellIndexes.Min();
                    }
                }

                if (e.Delta < 0)
                {
                    bool ifNeedReSelect = false;
                    if (toChooseCellIndex + 1 < _allImages.Count)
                    {
                        ifNeedReSelect = (!_allImages[toChooseCellIndex + 1].IsSelected) || _ifMultiSericeSelected;
                        SetSelectStateAndSelectdCellIndexList(toChooseCellIndex + 1); //scrolldown
                    }
                    else
                        SetSelectStateAndSelectdCellIndexList(toChooseCellIndex);
                    DisplayCell();
                    if (_ifSericeSelected && ifNeedReSelect)
                    {
                        var temp = seiresUIDandUserInfoList;
                        _selectedCellIndexes.Clear();
                        _selectedCellIndexes.AddRange(_filmingCard.SelectSeriesForZoomViewer(temp));
                        _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                    }
                    //e.Handled = true;
                    return;
                }
                if (e.Delta > 0)
                {
                    bool ifNeedReSelect = false;
                    if (toChooseCellIndex - 1 >= 0)
                    {
                        ifNeedReSelect = (!_allImages[toChooseCellIndex - 1].IsSelected) || _ifMultiSericeSelected;
                        SetSelectStateAndSelectdCellIndexList(toChooseCellIndex - 1); //scrolldown
                    }
                    else
                        SetSelectStateAndSelectdCellIndexList(toChooseCellIndex);
                    DisplayCell();
                    if (_ifSericeSelected && ifNeedReSelect)
                    {
                        var temp = seiresUIDandUserInfoList;
                        _selectedCellIndexes.Clear();
                        _selectedCellIndexes.AddRange(_filmingCard.SelectSeriesForZoomViewer(temp));
                        _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                    }
                    //e.Handled = true;
                }
            }
            catch(Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
            finally
            {
                _filmingCard.UpdateLabelSelectedCount();
            }
        }
        //设置翻页的选中状态并更新显示Index记录，同时后方胶片的选中状态做刷新。
        private void SetSelectStateAndSelectdCellIndexList(int selectIndex)
        {
            var oldCurIndex = _currentCellIndex;
            _currentCellIndex = selectIndex;
            //ctrl多选逻辑
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                //设置不同颜色选中框
                _allImages[oldCurIndex].SetCellFocusSelected(true, true);
                _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                _allImages[_currentCellIndex].Refresh();
                if (!_selectedCellIndexes.Contains(_currentCellIndex))
                {
                    _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[_currentCellIndex]].IsSelected = true;
                    _selectedCellIndexes.Add(_currentCellIndex);
                }
                if (_allCellsDisplayFilmBordIndexList[oldCurIndex] != _allCellsDisplayFilmBordIndexList[_currentCellIndex]) 
                    ScrollFilmingPage();
                return;
            }

            //鼠标滚动单选选中逻辑
            if (!_selectedCellIndexes.Contains(_currentCellIndex)|| !_ifSericeSelected||_ifMultiSericeSelected)
            {
                foreach (var cellIndex in _selectedCellIndexes)
                {
                    _allImages[cellIndex].SetCellFocusSelected(false, false);
                    //_filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[cellIndex]].IsSelected = false;
                    var page = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[cellIndex]];
                    var viewport = page.ViewportOfCell(_allImages[cellIndex]);
                    page.IsSelected = false;
                    viewport.IsSelected = false;
                    //FilmPageUtil.UnselectOtherFilmingPages(_filmingCard.ActiveFilmingPageList, page);
                    //FilmPageUtil.UnselectOtherViewports(page, viewport);
                    _allImages[cellIndex].Refresh();
                }
                _selectedCellIndexes.Clear();
            }
            else
            {
                _allImages[oldCurIndex].SetCellFocusSelected(true, true);
            }
            //设置不同颜色选中框
            _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
            _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[_currentCellIndex]].IsSelected = true;
            if (!_selectedCellIndexes.Contains(_currentCellIndex))
                _selectedCellIndexes.Add(_currentCellIndex);
            _allImages[_currentCellIndex].Refresh();
            //设置page、viewport选中状态
            var newpage = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[_currentCellIndex]];
            if (!newpage.IsSelected)
            {
                newpage.IsSelected = true;
            }
            var newviewport = newpage.ViewportOfCell(_allImages[_currentCellIndex]);
            if (!newviewport.IsSelected)
            {
                newviewport.IsSelected = true;
            }
            
            if (_allCellsDisplayFilmBordIndexList[oldCurIndex] != _allCellsDisplayFilmBordIndexList[_currentCellIndex])
            {
                ScrollFilmingPage();
            }
        }

        //双击延时到up关闭标志
        private bool _isClosing = false;  //修复DIM579517需要，双击第二次mousedown关闭窗口，mouseup事件会响应到后续cell，造成单击选中逻辑

        ////双击关闭放大窗口
        //private void ctrlZoomViewer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if(e.ChangedButton == MouseButton.Left && DisplayedZoomCell.IsHitOnGraphicObj == false)
        //    {
        //        _isClosing = true;
        //        //_msgWindow.Close();
        //    }
        //}
        //MouseUp时同步ps信息
        private void ctrlZoomViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(!_filmingCard.IfZoomWindowShowState) return;
            if (_isClosing)
            {
                CloseDialog();
                return;
            }
            SyncPSState(_selectedCellIndexes);
        }

        private void ScrollFilmingPage()            //后方图片胶片显示翻动
        {
            _filmingCard.GotoFilmBoardWithIndex(_allCellsDisplayFilmBordIndexList[_currentCellIndex]);
        }

        public void ExDeleteKeyDown()
        {
            if (DisplayedZoomCell == null) return;
            int startIndex = _selectedCellIndexes.Min();
            int endIndex = _selectedCellIndexes.Max();
            bool ifSkipDelete = false;
            foreach (var cellIndex in _selectedCellIndexes)
            {
                _allImages[cellIndex].Image.RemoveAt(0);
                _allImages[cellIndex].SetCellFocusSelected(false);
                var page = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[cellIndex]];
                var viewport = page.ViewportOfCell(_allImages[cellIndex]);
                FilmPageUtil.UnselectOtherFilmingPages(_filmingCard.ActiveFilmingPageList, page);
                FilmPageUtil.UnselectOtherViewports(page, viewport);
                _allImages[cellIndex].Refresh();
            }
            if (endIndex + 1 < _allImages.Count)   //删除图片后当前图片应后移一位
            {
                if (_allCellsDisplayFilmBordIndexList[_currentCellIndex] != _allCellsDisplayFilmBordIndexList[endIndex + 1])
                {
                    _currentCellIndex = endIndex + 1;
                    _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                    var page = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[_currentCellIndex]];
                    var viewport = page.ViewportOfCell(_allImages[_currentCellIndex]);
                    page.IsSelected = true;
                    if (viewport != null)
                    {
                        viewport.IsSelected = true;
                    }
                    DisplayCell();
                    ScrollFilmingPage();
                }
                else
                {
                    _currentCellIndex = endIndex + 1;
                    _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                    var page = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[_currentCellIndex]];
                    var viewport = page.ViewportOfCell(_allImages[_currentCellIndex]);
                    page.IsSelected = true;
                    if (viewport != null)
                    {
                        viewport.IsSelected = true;
                    }
                    DisplayCell();
                }
                _currentCellIndex = _currentCellIndex - _selectedCellIndexes.Count;  //删除图片后当前选中图片index会前移
            }
            else if (startIndex - 1 >= 0)
            {
                if (_allCellsDisplayFilmBordIndexList[_currentCellIndex] != _allCellsDisplayFilmBordIndexList[startIndex - 1])
                {
                    _currentCellIndex = startIndex - 1;
                    _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                    var page = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[_currentCellIndex]];
                    var viewport = page.ViewportOfCell(_allImages[_currentCellIndex]);
                    page.IsSelected = true;
                    if (viewport != null)
                    {
                        viewport.IsSelected = true;
                    }
                    DisplayCell();
                    ScrollFilmingPage();
                }
                else
                {
                    _currentCellIndex = startIndex - 1;
                    _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                    var page = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[_currentCellIndex]];
                    var viewport = page.ViewportOfCell(_allImages[_currentCellIndex]);
                    page.IsSelected = true;
                    if (viewport != null)
                    {
                        viewport.IsSelected = true;
                    }
                    DisplayCell();
                }
            }
            else
                ifSkipDelete = true;

            if (_allImages.Count == _selectedCellIndexes.Count)
            {
                _allImages.Clear();
                _allCellsDisplayFilmBordIndexList.Clear();
                _allCellsFilmPageIndexList.Clear();
                _selectedCellIndexes.Clear();
                _ifNeedRepack = true;
                CloseDialog();
                _filmingCard.SelectFirstCellOrViewport(_filmingCard.DisplayedFilmPage.FirstOrDefault());
                _filmingCard.UpdateUIStatus();
                return;
                //_msgWindow.Close();
            }
            _selectedCellIndexes.Sort();
            _selectedCellIndexes.Reverse();
            foreach (var cellIndex in _selectedCellIndexes)
            {
                _allImages.RemoveAt(cellIndex);
                _allCellsDisplayFilmBordIndexList.RemoveAt(cellIndex);
                _allCellsFilmPageIndexList.RemoveAt(cellIndex);
            }
            _ifNeedRepack = true;
            if (ifSkipDelete)
            {
                _currentCellIndex = 0;
                _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                DisplayCell();
                ScrollFilmingPage();
            }
            _selectedCellIndexes.Clear();
            _selectedCellIndexes.Add(_currentCellIndex);
            if (_ifSericeSelected)
            {
                var temp = seiresUIDandUserInfoList;
                _selectedCellIndexes.Clear();
                _selectedCellIndexes.AddRange(_filmingCard.SelectSeriesForZoomViewer(temp));
                _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
            }
        }
        public void ExSpaceKeyDown()
        {
            _ifSericeSelected = !_ifSericeSelected;
            if (!_ifSericeSelected)
            {
                var page = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[_currentCellIndex]];
                var cell = _allImages[_currentCellIndex];
                var viewport = page.ViewportOfCell(cell);
                FilmPageUtil.UnselectOtherFilmingPages(_filmingCard.ActiveFilmingPageList, page);
                FilmPageUtil.UnselectOtherViewports(page, viewport);
                FilmPageUtil.UnselectOtherCellsInViewport(viewport, cell, page._isGroupLRButtonDown);
                foreach (var index in _selectedCellIndexes)
                {
                    _allImages[index].SetCellFocusSelected(false, false);
                }
                _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                _selectedCellIndexes.Clear();
                _selectedCellIndexes.Add(_currentCellIndex);
                _filmingCard.UpdateUIStatus();
                return;
            }
            var temp = seiresUIDandUserInfoList;

            _selectedCellIndexes.Clear();
            foreach (var index in _selectedCellIndexes) //改变选中框颜色
            {
                _allImages[index].SetCellFocusSelected(false, false);
            }
            _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
            _selectedCellIndexes.AddRange(_filmingCard.SelectSeriesForZoomViewer(temp));
            _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
            _filmingCard.UpdateUIStatus();
        }

        private void ctrlZoomViewer_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)      //删除功能
            {
                if(DisplayedZoomCell==null) return;
                if (DisplayedZoomCell.IsGraphicSelected())
                {
                    DeleteSelectedGraphics();
                    e.Handled = true;
                    return;
                }
                if (DeleteSelectedLocalizedImage())
                {
                    e.Handled = true;
                    return;
                }
                int startIndex = _selectedCellIndexes.Min();
                int endIndex = _selectedCellIndexes.Max();
                bool ifSkipDelete = false;
                foreach (var cellIndex in _selectedCellIndexes)
                {
                    _allImages[cellIndex].Image.RemoveAt(0);
                    _allImages[cellIndex].SetCellFocusSelected(false);
                    var page = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[cellIndex]];
                    var viewport = page.ViewportOfCell(_allImages[cellIndex]);
                    FilmPageUtil.UnselectOtherFilmingPages(_filmingCard.ActiveFilmingPageList, page);
                    FilmPageUtil.UnselectOtherViewports(page, viewport);
                    _allImages[cellIndex].Refresh();
                }
                if (endIndex + 1 < _allImages.Count)   //删除图片后当前图片应后移一位
                {
                    if (_allCellsDisplayFilmBordIndexList[_currentCellIndex] != _allCellsDisplayFilmBordIndexList[endIndex+1])
                    {
                        _currentCellIndex = endIndex + 1;
                        _allImages[_currentCellIndex].SetCellFocusSelected(true,false);
                        var page = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[_currentCellIndex]];
                        var viewport = page.ViewportOfCell(_allImages[_currentCellIndex]);
                        page.IsSelected = true;
                        if (viewport != null)
                        {
                            viewport.IsSelected = true;
                        }
                        DisplayCell();
                        ScrollFilmingPage();
                    }
                    else
                    {
                        _currentCellIndex = endIndex + 1;
                        _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                        var page = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[_currentCellIndex]];
                        var viewport = page.ViewportOfCell(_allImages[_currentCellIndex]);
                        page.IsSelected = true;
                        if (viewport != null)
                        {
                            viewport.IsSelected = true;
                        }
                        DisplayCell();
                    }
                    _currentCellIndex = _currentCellIndex - _selectedCellIndexes.Count;  //删除图片后当前选中图片index会前移
                }
                else if (startIndex - 1 >= 0)
                {
                    if (_allCellsDisplayFilmBordIndexList[_currentCellIndex] != _allCellsDisplayFilmBordIndexList[startIndex - 1])
                    {
                        _currentCellIndex = startIndex - 1;
                        _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                        var page = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[_currentCellIndex]];
                        var viewport = page.ViewportOfCell(_allImages[_currentCellIndex]);
                        page.IsSelected = true;
                        if (viewport != null)
                        {
                            viewport.IsSelected = true;
                        }
                        DisplayCell();
                        ScrollFilmingPage();
                    }
                    else
                    {
                        _currentCellIndex = startIndex - 1;
                        _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                        var page = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[_currentCellIndex]];
                        var viewport = page.ViewportOfCell(_allImages[_currentCellIndex]);
                        page.IsSelected = true;
                        if (viewport != null)
                        {
                            viewport.IsSelected = true;
                        }
                        DisplayCell();
                    }
                }
                else
                    ifSkipDelete = true;

                if (_allImages.Count == _selectedCellIndexes.Count)
                {
                    _allImages.Clear();
                    _allCellsDisplayFilmBordIndexList.Clear();
                    _allCellsFilmPageIndexList.Clear();
                    _selectedCellIndexes.Clear();
                    _ifNeedRepack = true;
                    CloseDialog();
                    _filmingCard.SelectFirstCellOrViewport(_filmingCard.DisplayedFilmPage.FirstOrDefault());
                    _filmingCard.UpdateUIStatus();
                    e.Handled = true;
                    return;
                    //_msgWindow.Close();
                }
                _selectedCellIndexes.Sort();
                _selectedCellIndexes.Reverse();
                foreach (var cellIndex in _selectedCellIndexes)
                {
                    _allImages.RemoveAt(cellIndex);
                    _allCellsDisplayFilmBordIndexList.RemoveAt(cellIndex);
                    _allCellsFilmPageIndexList.RemoveAt(cellIndex);
                }
                _ifNeedRepack = true;
                if (ifSkipDelete)
                {
                    _currentCellIndex = 0;
                    _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                    DisplayCell();
                    ScrollFilmingPage();
                }
                _selectedCellIndexes.Clear();
                _selectedCellIndexes.Add(_currentCellIndex);
                if (_ifSericeSelected)
                {
                    var temp = seiresUIDandUserInfoList;
                    _selectedCellIndexes.Clear();
                    _selectedCellIndexes.AddRange(_filmingCard.SelectSeriesForZoomViewer(temp));
                    _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                }  
                e.Handled = true;
            }
            if (e.Key == Key.Escape)
            {
                CloseDialog();
                //_msgWindow.Close();
            }
            if (e.Key == Key.Space)
            {
                _ifSericeSelected = !_ifSericeSelected;
                //SetCellFocusBorder(!_ifSericeSelected);
                if (!_ifSericeSelected)
                {
                    //SetSelectStateAndSelectdCellIndexList(_currentCellIndex);
                    var page = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[_currentCellIndex]];
                    var cell = _allImages[_currentCellIndex];
                    var viewport = page.ViewportOfCell(cell);
                    FilmPageUtil.UnselectOtherFilmingPages(_filmingCard.ActiveFilmingPageList, page);
                    FilmPageUtil.UnselectOtherViewports(page, viewport);
                    FilmPageUtil.UnselectOtherCellsInViewport(viewport, cell, page._isGroupLRButtonDown);
                    foreach (var index in _selectedCellIndexes)
                    {
                        _allImages[index].SetCellFocusSelected(false, false);
                    }
                    _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                    _selectedCellIndexes.Clear();
                    _selectedCellIndexes.Add(_currentCellIndex);
                    e.Handled = true;
                    _filmingCard.UpdateUIStatus();
                    return;
                }
                var temp = seiresUIDandUserInfoList;
                
                _selectedCellIndexes.Clear();
                foreach (var index in _selectedCellIndexes) //改变选中框颜色
                {
                    _allImages[index].SetCellFocusSelected(false, false);
                }
                _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                _selectedCellIndexes.AddRange(_filmingCard.SelectSeriesForZoomViewer(temp));
                _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                e.Handled = true;
                _filmingCard.UpdateUIStatus();
            }
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right||e.Key == Key.PageUp || e.Key == Key.PageDown)
            {
                e.Handled = true;
            }
            if (e.Key == Key.F12)
            {
                _filmingCard.actiontoolCtrl.AdjustWlBtnClick(null, null);
                RefreshDisplayCell();
            }
        }

        //处理小窗口点击事件
        private void ctrlZoomViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isClosing = false; //防止双击后移除cell up
            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left && DisplayedZoomCell.IsHitOnGraphicObj == false)
            {
                _isClosing = true;
            }
            if (DisplayedZoomCell.ActionManager.CurrentAction is IActionClickDraw ||
                _filmingCard.CurrentActionType == ActionType.AnnotationFreehand)
            {
                _isClosing = false;
            }
            if (IfMiniCellSelected)
            {
                IfMiniCellSelected = false;
                DisplayedZoomCell.IsSelected = true;
                foreach (var index in _selectedCellIndexes)
                {
                    var page = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[index]];
                    var cell = _allImages[index];
                    var viewport = page.ViewportOfCell(cell);
                    if (viewport != null)
                        viewport.IsSelected = true;
                    page.IsSelected = true;
                    cell.IsSelected = true;
                }
            }
            var overlayLocalizedImage = DisplayedZoomCell.GetOverlay(OverlayType.LocalizedImage) as OverlayLocalizedImage;
            if (overlayLocalizedImage != null && overlayLocalizedImage.GraphicLocalizedImage.MiniCell.IsSelected)
            {
                overlayLocalizedImage.GraphicLocalizedImage.MiniCell.IsSelected = false;
                FilmingCard.GroupUnselectedForSingleUseCell(overlayLocalizedImage.GraphicLocalizedImage.MiniCell);
            }
            _filmingCard.UpdateLabelSelectedCount();
        }

        //删除图元操作
        private void DeleteSelectedGraphics()
        {
            var cell = DisplayedZoomCell;
            if (cell != null && cell.Image != null && cell.Image.CurrentPage != null)
            {
                var overlay = cell.Image.CurrentPage.GetOverlay(OverlayType.Graphics);
                var Graphics = new List<IDynamicGraphicObj>();
                foreach (IGraphicObj g in overlay.SelectedGraphics)
                {
                    if (g is IDynamicGraphicObj)
                        Graphics.Add(g as IDynamicGraphicObj);
                }
                cell.DeleteGraphics(Graphics, overlay);
            }
            _allImages[_currentCellIndex].Image.CurrentPage.PSXml =
                DisplayedZoomCell.Image.CurrentPage.SerializePSInfo();
            _allImages[_currentCellIndex].Image.CurrentPage.DeserializePSInfo();
        }
        private bool DeleteSelectedLocalizedImage()
        {
            var overlayLocalizedImage = DisplayedZoomCell.GetOverlay(OverlayType.LocalizedImage) as OverlayLocalizedImage;
            if (overlayLocalizedImage != null && overlayLocalizedImage.GraphicLocalizedImage.MiniCell.IsSelected)
            {
                _filmingCard.contextMenu.DeleteSelectedLocalizedImageReferenceLine();
                IfMiniCellSelected = false;
                DisplayedZoomCell.IsSelected = true;
                foreach (var index in _selectedCellIndexes)
                {
                    var page = _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[index]];
                    var cell = _allImages[index];
                    var viewport = page.ViewportOfCell(cell);
                    if(viewport != null)
                        viewport.IsSelected = true;
                    page.IsSelected = true;
                    cell.IsSelected = true;
                }
                this.RefreshDisplayCell();
                return true;
            }
            return false;

        }

        private void BindInputings()
        {
            var selectAllKeyGesture = new KeyGesture(Key.A, ModifierKeys.Control);
            var selectAllCommandKeyBinding = new KeyBinding(SelectAllCommand, selectAllKeyGesture);
            this.InputBindings.Add(selectAllCommandKeyBinding);
            //_msgWindow.InputBindings.Add(selectAllCommandKeyBinding);
        }
        private ICommand _selectAllCommand;
        private ICommand SelectAllCommand
        {
            //get { return _selectAllCommand ?? (_selectAllCommand = new RelayCommand(param => OnSelectAllFilmPages(null, null), param => (IsInFilmingMainWindow))); }
            get
            {
                return _selectAllCommand ?? (_selectAllCommand = new RelayCommand(
                                                                     param =>
                                                                     {
                                                                         SelectAllCells();
                                                                         _selectedCellIndexes.Clear();
                                                                         _selectedCellIndexes.AddRange(Enumerable.Range(0,_allImages.Count));
                                                                         _ifMultiSericeSelected = true;
                                                                     },
                                                                     null));
            }
        }

        private void SelectAllCells()
        {
            foreach (var cell in _allImages)
            {
                cell.SetCellFocusSelected(true,true);
            }
            _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
            foreach (var pageControl in _filmingCard.EntityFilmingPageList)
            {
                pageControl.IsSelected = true;
            }

            _filmingCard.LastSelectedCell = _allImages.LastOrDefault();
            _filmingCard.LastSelectedFilmingPage = _filmingCard.EntityFilmingPageList.LastOrDefault();
            if (_filmingCard.LastSelectedFilmingPage != null)
            {
                var viewport = _filmingCard.LastSelectedFilmingPage.ViewportOfCell(_filmingCard.LastSelectedCell);
                _filmingCard.LastSelectedViewport = viewport;
            }
        }

        private void OnMedviewerControlIsInEditChanged(object sender, MedViewerEventArgs e)
        {
            try
            {
                bool isInEditing = (bool)e.Target;
                if (isInEditing)
                {
                    _filmingCard.ClearPtInputBindings();
                }
                else
                {
                    _filmingCard.RecoverPtInputBindings();
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void OnSynGraphicToCellOperated(object sender, MedViewerSynGraphicEventArgs e)
        {
            var filmingCard = _filmingCard;
            var synGraphicXml = e.GraphicXml;
            var sourceGraphic = e.SourceGrahpic;
            var isDrawHideCell = e.IsDrawHideCell;
            var isDeleteLastGraphic = e.IsDeleteLastGraphic;

            if (string.IsNullOrEmpty(synGraphicXml)) return;

            var graphicOverlay = new OverlayGraphics();

            foreach (var page in filmingCard.ActiveFilmingPageList)
            {
                var listCells = page.filmingViewerControl.SelectedCells.Cast<IViewerControlCell>().ToList();
                graphicOverlay.DeserializeXmlToCells(sourceGraphic, synGraphicXml, listCells, null, isDrawHideCell, isDeleteLastGraphic);
            }
        }

        public void RefreshDisplayCell()
        {
            DisplayCell();
            var cell = DisplayedZoomCell;
            if (cell != null && cell.Image != null && cell.Image.CurrentPage != null)
            {
                var overlay = cell.Image.CurrentPage.GetOverlay(OverlayType.Graphics);
                foreach (IGraphicObj g in overlay.Graphics)
                {
                    if (g is IDynamicGraphicObj)
                        (g as IDynamicGraphicObj).IsCreated = true;
                }
            }
        }

        public void ReZoom()
        {
            if (_allImages.Count > 0)
                _allImages[_currentCellIndex].SetCellFocusSelected(false, false);
            var oldCell = _allImages[_currentCellIndex];
            //oldCell.SetCellFocusSelected(false, false);
            if (!_filmingCard.PrepareZoomViewerCells())
            {
                //CloseDialog();
                foreach (var cellIndex in _selectedCellIndexes)
                {
                    _allImages[cellIndex].SetCellFocusSelected(true,true);
                }
                _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                return;
            }
            var oldCurIndex = _allImages.Find(a => Object.Equals(a, oldCell)).CellIndex;
            _currentCellIndex = _filmingCard.ZoomViewerSelectedCellIndexs.FirstOrDefault();
            DisplayCell();
            foreach (var cellIndex in _selectedCellIndexes)
            {
                _allImages[cellIndex].SetCellFocusSelected(false, false);
            }
            //处理小cell选中情况
            if(IfMiniCellSelected)
            {
                IfMiniCellSelected = false;
                var overlayLocalizedImage = _allImages[oldCurIndex].GetOverlay(OverlayType.LocalizedImage) as OverlayLocalizedImage;
                if (overlayLocalizedImage != null && overlayLocalizedImage.GraphicLocalizedImage.MiniCell.IsSelected)
                {
                    FilmingCard.GroupUnselectedForSingleUseCell(overlayLocalizedImage.GraphicLocalizedImage.MiniCell);
                }
            }
            _selectedCellIndexes.Clear();
            _selectedCellIndexes.AddRange(_filmingCard.ZoomViewerSelectedCellIndexs);
            //设置不同颜色选中框
            foreach (var cellIndex in _selectedCellIndexes)
            {
                _allImages[cellIndex].SetCellFocusSelected(true, true);
            }
            _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
            _filmingCard.EntityFilmingPageList[_allCellsFilmPageIndexList[_currentCellIndex]].IsSelected = true;
            if (!_selectedCellIndexes.Contains(_currentCellIndex))
                _selectedCellIndexes.Add(_currentCellIndex);
            _allImages[_currentCellIndex].Refresh();
            _filmingCard.ReOrderCurrentFilmPageBoard();
            if (_allCellsDisplayFilmBordIndexList[oldCurIndex] != _allCellsDisplayFilmBordIndexList[_currentCellIndex])
            {
                ScrollFilmingPage();
            }
            
        }

        public void DisplayCellChanged(MedViewerControlCell cell)
        {
            var newCellIndex = _allImages.FindIndex(c => c.GetHashCode() == cell.GetHashCode());
            if(newCellIndex < 0 || _allImages.Count == 0) return;
            if (_selectedCellIndexes.Contains(newCellIndex))
            {
                _allImages[_currentCellIndex].SetCellFocusSelected(true, true);
                _currentCellIndex = newCellIndex;
                _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
            }
            else
            {
                SetSelectStateAndSelectdCellIndexList(newCellIndex);
                if (_ifSericeSelected)
                {
                    var temp = seiresUIDandUserInfoList;
                    _selectedCellIndexes.Clear();
                    _selectedCellIndexes.AddRange(_filmingCard.SelectSeriesForZoomViewer(temp));
                    _allImages[_currentCellIndex].SetCellFocusSelected(true, false);
                }
            }
            RefreshDisplayCell();
            _filmingCard.UpdateLabelSelectedCount();
        }
        private void InitializeSyncDrawGraphics()
        {
            string modality;
            mcsf_clr_systemenvironment_config.GetModalityName(out modality);

            if (modality == "MR" || modality == "CT" || modality == "PT" || modality == "PETMR")
            {
                ctrlZoomViewer.CanSyncDrawGraphics = true;
            }
        }
    }
}


