using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.MHC;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Views
{
    /// <summary>
    /// Interaction logic for FilmingCard_ToolPanel.xaml
    /// </summary>
    public partial class ActionToolPanel : UserControl
    {
        #region Fields
        private CustomRotation _customRotation;

       // public ActionType CurrentActionType { get; set; }
        public FilmingCard Card { get; set; }
        public string CurModality { get; set; }
       

        #endregion

        public ActionToolPanel(FilmingCard _card, string modality)
        {
            Card = _card;
            CurModality = modality;
            InitializeComponent();
           
            _customRotation = new CustomRotation();
            
        }

        public void InitCommonTool()
        {
          //  System.Diagnostics.Debugger.Launch();
            rotateButton.ToolTip = btnRotateClock.ToolTip;//rotateButton.ToolTip = btnCustomRotate.ToolTip;
            flipButton.ToolTip = btnFlipHor.ToolTip;
            rdoFilmingRegionShape.ToolTip = rdoFilmingRegionEllipse.ToolTip;
            pointerButton.IsChecked = true;
            if (CurModality.ToUpper() == "MG" || CurModality.ToUpper() == "DBT")
            {
                freeHandButton.Visibility = Visibility.Visible;
                cmbFilmingRegionShape.Items.Remove(rdoFilmingRegionFreeHand);
                FilmingToolBar.Children.Remove(pixelLensButton);
                FilmingToolBar.Children.Remove(filmingFlipButton);
                FilmingToolBar.Children.Remove(filmingRotateButton);
                FilmingToolBar.Children.Remove(btnCustomRotate);

                FilmingToolBar.Children.Add(filmingFlipButton);
                FilmingToolBar.Children.Add(filmingRotateButton);
                FilmingToolBar.Children.Add(pixelLensButton);
            }
        }

        private void SetButtonStatusAccordingtoActionType(ActionType actType)
        {
            switch (actType)
            {
                //case ActionType.RegionEllipse:
                //    rdoFilmingRegionShape.IsChecked = true;
                //    break;
                case ActionType.PixelLens:
                    pixelLensButton.IsChecked = true;
                    break;
                case ActionType.AnnotationLine:
                    lineButton.IsChecked = true;
                    break;
                case ActionType.AnnotationAngle:
                    angleButton.IsChecked = true;
                    break;
                case ActionType.AnnotationText:
                    annotationTextButton.IsChecked = true;
                    break;
                case ActionType.AnnotationArrow:
                    arrowButton.IsChecked = true;
                    break;
                case ActionType.Pan:
                    panButton.IsChecked = true;
                    break;
                case ActionType.Zoom:
                    zoomButton.IsChecked = true;
                    break;
                case ActionType.Pointer:
                    pointerButton.IsChecked = true;
                    break;
                case ActionType.ZoomBox:
                    roiZoomButton.IsChecked = true;
                    break;
                case ActionType.Magnifier:
                    magnifierButton.IsChecked = true;
                    break;
            }
        }

        private void FitToWindowLongSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag);
                var miniCellsList = FilmingCard._miniCellsList;
                if (miniCellsList != null && miniCellsList.Count != 0 && miniCellsList[0].IsSelected)
                {
                    foreach (var miniCell in miniCellsList)
                    {
                        miniCell.FitToWindow();
                    }
                }
                var zoom = "";
                //if (Card.IsModalityForMammoImage())
                //{
                //    foreach (var filmPage in Card.ActiveFilmingPageList)
                //    {
                //        foreach (var selectedCell in filmPage.SelectedCells())
                //        {
                //            FilmingActionDeal.ZoomCell(selectedCell, 1.00, 1.00, true);
                //            Card.mgMethod.ApplyDisplayForMg(selectedCell);
                //        }
                //    }
                //    return;
                //}
                foreach (var filmPage in Card.ActiveFilmingPageList)
                {
                    foreach (var selectedCell in filmPage.SelectedCells())
                    {
                        selectedCell.FitToWindow();
                        if (string.IsNullOrEmpty(zoom))
                        {
                            zoom = Math.Round(Math.Abs(selectedCell.Image.CurrentPage.PState.ScaleX), 2).ToString("0.00");
                        }
                    }
                }

                Card.BtnEditCtrl.scaleTextBox.Text = zoom;
                Card.BtnEditCtrl.scaleTextBox.IsEnabled = true;


                if (Card.IfZoomWindowShowState && Card.ZoomViewer != null && Card.ZoomViewer.DisplayedZoomCell != null)
                {
                    Card.ZoomViewer.DisplayedZoomCell.FitToWindow();
                }
                //GetScaleOfSelectedCells();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }
        private void FitToWindowShortSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag);
                var miniCellsList = FilmingCard._miniCellsList;
                if (miniCellsList != null && miniCellsList.Count != 0 && miniCellsList[0].IsSelected)
                {
                    foreach (var miniCell in miniCellsList)
                    {
                        miniCell.FitToWindowByShort();
                    }
                }
                var zoom = "";

                foreach (var filmPage in Card.ActiveFilmingPageList)
                {
                    foreach (var selectedCell in filmPage.SelectedCells())
                    {
                        selectedCell.FitToWindowByShort();
                        if (string.IsNullOrEmpty(zoom))
                        {
                            zoom = Math.Round(Math.Abs(selectedCell.Image.CurrentPage.PState.ScaleX), 2).ToString("0.00");
                        }
                    }
                }

                Card.BtnEditCtrl.scaleTextBox.Text = zoom;
                Card.BtnEditCtrl.scaleTextBox.IsEnabled = true;


                if (Card.IfZoomWindowShowState && Card.ZoomViewer != null && Card.ZoomViewer.DisplayedZoomCell != null)
                {
                    Card.ZoomViewer.DisplayedZoomCell.FitToWindowByShort();
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void FlipHorComboBoxSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                Logger.Instance.LogPerformanceRecord("[Action][Horizontal Flip][Begin]");
                ActionType actType = Card.CurrentActionType;
                var content = flipButton.Content as Image;
                if (content != null)
                {
                    var itemContent = btnFlipHor.Content as Image;
                    if (itemContent != null)
                    {
                        content.Source = itemContent.Source;
                        flipButton.ToolTip = btnFlipHor.ToolTip;
                    }
                }
                CommonSelectedCellExcuteCommand<CmdFlipHorizontal>();
                RestorePreviousStatus(actType);
                Logger.Instance.LogPerformanceRecord("[Action][Horizontal Flip][End]");
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void RestorePreviousStatus(ActionType actType)
        {
            Card.CurrentActionType = actType;
            RefreshAction();
            SetButtonStatusAccordingtoActionType(actType);
        }

        public void RefreshAction()
        {
            CommonImageToolHandler(Card.CurrentActionType, true);
        }


        public void OnPointerButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                CommonImageToolHandler(ActionType.Pointer);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

       

        private void FlipVerComboBoxSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                Logger.Instance.LogPerformanceRecord("[Action][Vertical Flip][End]");
                ActionType actType = Card.CurrentActionType;
                var content = flipButton.Content as Image;
                if (content != null)
                {
                    var itemContent = btnFlipVer.Content as Image;
                    if (itemContent != null)
                    {
                        content.Source = itemContent.Source;
                        flipButton.ToolTip = btnFlipVer.ToolTip;
                    }
                }
                CommonSelectedCellExcuteCommand<CmdFlipVertical>();
                RestorePreviousStatus(actType);
                Logger.Instance.LogPerformanceRecord("[Action][Vertical Flip][End]");
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void CustomRotateComboBoxSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_customRotation == null) return;

                Logger.LogFuncUp();

                _customRotation.InitialActiveFilmingPage(Card.ActiveFilmingPageList);
                if (Card.IfZoomWindowShowState && Card.ZoomViewer != null && Card.ZoomViewer.DisplayedZoomCell != null)
                {
                    _customRotation.InitialZoomViewerCell(Card.ZoomViewer.DisplayedZoomCell);
                }
                else
                {
                    _customRotation.InitialZoomViewerCell(null);
                }
                var messageWindow = new MessageWindow
                {
                    WindowTitle = Card.Resources["UID_Filming_CustomRotate"] as string,
                    WindowChild = _customRotation,
                    WindowDisplayMode = WindowDisplayMode.Default
                };
                Card.HostAdornerCount++;
                messageWindow.ShowModelDialog();
                Card.HostAdornerCount--;

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void RotateComboBoxSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                Logger.Instance.LogPerformanceRecord("[Action][Rotate90][Begin]");
                ActionType actType = Card.CurrentActionType;
                var content = rotateButton.Content as Image;
                if (content != null)
                {
                    var itemContent = btnRotateClock.Content as Image;
                    if (itemContent != null)
                    {
                        content.Source = itemContent.Source;
                        rotateButton.ToolTip = btnRotateClock.ToolTip;
                    }
                }
                CommonSelectedCellExcuteCommand<CmdRotateClockwise>();
                RestorePreviousStatus(actType);
                Logger.Instance.LogPerformanceRecord("[Action][Rotate90][End]");
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void RotateAnticlockComboBoxSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                Logger.Instance.LogPerformanceRecord("[Action][RotateAnticlock90][Begin]");
                ActionType actType = Card.CurrentActionType;
                var content = rotateButton.Content as Image;
                if (content != null)
                {
                    var itemContent = btnRotateAnticlock.Content as Image;
                    if (itemContent != null)
                    {
                        content.Source = itemContent.Source;
                        rotateButton.ToolTip = btnRotateAnticlock.ToolTip;
                    }
                }
                CommonSelectedCellExcuteCommand<CmdRotateCounterClockwise>();
                RestorePreviousStatus(actType);
                Logger.Instance.LogPerformanceRecord("[Action][RotateAnticlock90][End]");
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void CommonSelectedCellExcuteCommand<TCmd>() where TCmd : Viewer.ICommand, new()
        {
            try
            {
                Logger.LogFuncUp();

                //set the action type to pointer.
                pointerButton.IsChecked = true;

                CommonImageToolHandler(ActionType.Pointer);

                foreach (var filmingPage in Card.ActiveFilmingPageList)
                {
                    foreach (var selectedCell in filmingPage.SelectedCells())
                    {
                        var cmd = new TCmd();
                        selectedCell.ExecuteCommand(cmd);
                    }
                }
                if (Card.IfZoomWindowShowState && Card.ZoomViewer != null && Card.ZoomViewer.DisplayedZoomCell != null)
                {
                    var cmd = new TCmd();
                    Card.ZoomViewer.DisplayedZoomCell.ExecuteCommand(cmd);
                }
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                throw;
            }
        }

        private void OnMagnifierClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                CommonImageToolHandler(ActionType.Magnifier);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void OnFlipClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnArrowButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                CommonImageToolHandler(ActionType.AnnotationArrow);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void OnAngleClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                CommonImageToolHandler(ActionType.AnnotationAngle);
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void OnImageInverseBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var filmPage in Card.ActiveFilmingPageList)
                {
                    foreach (var selectedCell in filmPage.SelectedCells())
                    {
                        selectedCell.ColorInverse();
                    }
                }
                if (Card.IfZoomWindowShowState && Card.ZoomViewer != null && Card.ZoomViewer.DisplayedZoomCell != null)
                {
                    Card.ZoomViewer.DisplayedZoomCell.ColorInverse();
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void RoiZoomBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                CommonImageToolHandler(ActionType.ZoomBox);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void RotateBtnClick(object sender, RoutedEventArgs e)
        {
        }

        private void AnnotationTextBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                CommonImageToolHandler(ActionType.AnnotationText);
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }


        private void PanBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                CommonImageToolHandler(ActionType.Pan);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        //private void RegionEllipseBtnClick(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();

        //        CommonImageToolHandler(ActionType.RegionEllipse);

        //        Logger.LogFuncDown();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message+ex.StackTrace);
        //    }
        //}

        private void LineBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();

                CommonImageToolHandler(ActionType.AnnotationLine);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

      
        private void ComboBoxRotate_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                //btnCustomRotate.IsSelected = false;
                btnRotateClock.IsSelected = false;
                btnRotateAnticlock.IsSelected = false;
            }
            catch (Exception exp)
            {
                Logger.LogError(
                    "Exception From Function ComboBoxRotate_DropDownOpened: " + exp.ToString());
            }
        }

        private void ComboBoxFlip_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                btnFlipHor.IsSelected = false;
                btnFlipVer.IsSelected = false;
            }
            catch (Exception exp)
            {
                Logger.LogError(
                    "Exception From Function ComboBoxFlip_DropDownOpened: " + exp.ToString());
            }
        }

        private void ComboBoxFitToWindow_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                fitToWindowLong.IsSelected = false;
                fitToWindowShort.IsSelected = false;
            }
            catch (Exception exp)
            {
                Logger.LogError(
                    "Exception From Function ComboBoxFitToWindow_DropDownOpened: " + exp.ToString());
            }
        }

        private void CommonImageToolHandler(ActionType actionType, bool bRefresh = false)
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + actionType);
                if (Card.CurrentActionType != ActionType.Pan && Card.CurrentActionType != ActionType.Zoom)
                {
                    GraphicTextImpl.IsInputCtrlClosed = true;
                    //UnSelectAllGraphics();
                }

                if (Card.CurrentActionType == actionType && !bRefresh)
                {
                    actionType = ActionType.Pointer;
                }

                Card.CurrentActionType = actionType;

                if (Card.EntityFilmingPageList == null ) return;
                foreach (var filmingPage in Card.EntityFilmingPageList)
                {
                    // fix bug 523815        MedviewerControl强制ForceEndAction
                    filmingPage.filmingViewerControl.SetAction(actionType);
                    filmingPage.SetAction(actionType);
                }

                if (Card.IfZoomWindowShowState)
                {
                    Card.ZoomViewer.ctrlZoomViewer.SetAction(actionType);
                }

                if (actionType == ActionType.Pointer) pointerButton.IsChecked = true;

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
                //throw;
            }
        }

        private void ZoomBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                CommonImageToolHandler(ActionType.Zoom);
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void PixelLensBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                CommonImageToolHandler(ActionType.PixelLens);
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void OnImageFitToWindowBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag);
                var miniCellsList =FilmingCard._miniCellsList;
                if (miniCellsList != null && miniCellsList.Count != 0 && miniCellsList[0].IsSelected)
                {
                    foreach (var miniCell in miniCellsList)
                    {
                        miniCell.FitToWindow();
                    }
                }
                var zoom = "";
                if (Card.IsModalityForMammoImage())
                {
                    foreach (var filmPage in Card.ActiveFilmingPageList)
                    {
                        foreach (var selectedCell in filmPage.SelectedCells())
                        {
                            FilmingActionDeal.ZoomCell(selectedCell,1.00,1.00,true);
                            Card.mgMethod.ApplyDisplayForMg(selectedCell);
                        }
                    }
                    return;
                }
                foreach (var filmPage in Card.ActiveFilmingPageList)
                {
                    foreach (var selectedCell in filmPage.SelectedCells())
                    {
                        selectedCell.FitToWindow();
                        if(string.IsNullOrEmpty(zoom))
                        {
                            zoom = Math.Round(selectedCell.Image.CurrentPage.PState.ScaleX,2).ToString("0.00");
                        }
                    }
                }

                Card.BtnEditCtrl.scaleTextBox.Text = zoom;
                Card.BtnEditCtrl.scaleTextBox.IsEnabled = true;
                

                if (Card.IfZoomWindowShowState && Card.ZoomViewer != null && Card.ZoomViewer.DisplayedZoomCell != null)
                {
                    Card.ZoomViewer.DisplayedZoomCell.FitToWindow();
                }
                //GetScaleOfSelectedCells();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void ComboBoxRegion_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                rdoFilmingRegionEllipse.IsSelected = false;
                rdoFilmingRegionRectangle.IsSelected = false;
                rdoFilmingRegionCircle.IsSelected = false;
                rdoFilmingRegionPolygon.IsSelected = false;
                rdoFilmingRegionSpline.IsSelected = false;
                rdoFilmingRegionFreeHand.IsSelected = false;
            }
            catch (Exception exp)
            {
                Logger.LogError(
                    "Exception From Function ComboBoxRegion_DropDownOpened: " + exp.ToString());
            }
        }

        private void btnRegionEllipse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rdoFilmingRegionShape.IsChecked = true;
                CommonImageToolHandler(ActionType.RegionEllipse);

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void btnRegionRectangle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rdoFilmingRegionShape.IsChecked = true;
                CommonImageToolHandler(ActionType.RegionRectangle);
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void btnRegionPolygon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rdoFilmingRegionShape.IsChecked = true;

                CommonImageToolHandler(ActionType.RegionPolygon);

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void btnRegionCircle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rdoFilmingRegionShape.IsChecked = true;

                CommonImageToolHandler(ActionType.RegionCircle);

            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void btnRegionSpline_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rdoFilmingRegionShape.IsChecked = true;

                CommonImageToolHandler(ActionType.RegionSpline);

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void btnRegionFreeHand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Card.IsModalityForMammoImage())
                {
                    rdoFilmingRegionShape.IsChecked = true;
                }
               
                CommonImageToolHandler(ActionType.RegionFreehand);
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        public void AdjustWlBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.LogFuncUp();
                var miniCellsList = FilmingCard._miniCellsList;
                if (miniCellsList != null && miniCellsList.Count != 0 && miniCellsList[0].IsSelected)
                {
                    foreach (var miniCell in miniCellsList)
                    {
                        if ((miniCell.Image.CurrentPage.RecvOriginWindowLevel.WindowCenter == -1) &&
                                        (miniCell.Image.CurrentPage.RecvOriginWindowLevel.WindowWidth == -1))
                            miniCell.Image.CurrentPage.PState.WindowLevel =
                                miniCell.Image.CurrentPage.WindowLevel[0];
                        else
                        {
                            miniCell.Image.CurrentPage.PState.WindowLevel =
                                miniCell.Image.CurrentPage.RecvOriginWindowLevel;
                        }
                        miniCell.Refresh();
                    }
                }
                foreach (var filmPage in Card.ActiveFilmingPageList)
                {
                    foreach (var selectedCell in filmPage.SelectedCells())
                    {
                        if (selectedCell != null && selectedCell.Image != null && selectedCell.Image.CurrentPage != null)
                        {
                            if ((selectedCell.Image.CurrentPage.RecvOriginWindowLevel.WindowCenter == -1) &&
                                (selectedCell.Image.CurrentPage.RecvOriginWindowLevel.WindowWidth == -1))
                                selectedCell.Image.CurrentPage.PState.WindowLevel =
                                    selectedCell.Image.CurrentPage.WindowLevel[0];
                            else
                            {
                                selectedCell.Image.CurrentPage.PState.WindowLevel =
                                    selectedCell.Image.CurrentPage.RecvOriginWindowLevel;
                            }
                            selectedCell.Refresh(); //DIM432811
                        }
                    }
                }
                if (Card.IfZoomWindowShowState && Card.ZoomViewer != null && Card.ZoomViewer.DisplayedZoomCell != null)
                {
                    Card.ZoomViewer.RefreshDisplayCell();
                    
                }
                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }

        private void DeleteAllGraphicsBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag);
                foreach (var filmPage in Card.ActiveFilmingPageList)
                {
                    foreach (var selectedCell in filmPage.SelectedCells())
                    {
                        if (selectedCell != null && selectedCell.Image != null && selectedCell.Image.CurrentPage != null)
                        {
                            var overlay = selectedCell.Image.CurrentPage.GetOverlay(OverlayType.Graphics);
                            var allGraphics = new List<IDynamicGraphicObj>();
                            foreach (IGraphicObj g in overlay.Graphics)
                            {
                                if (g is IDynamicGraphicObj)
                                    allGraphics.Add(g as IDynamicGraphicObj);
                            }

                            selectedCell.DeleteGraphics(allGraphics, overlay);
                        }
                    }
                }
                if (Card.IfZoomWindowShowState && Card.ZoomViewer != null && Card.ZoomViewer.DisplayedZoomCell != null)
                {
                    var overlay = Card.ZoomViewer.DisplayedZoomCell.Image.CurrentPage.GetOverlay(OverlayType.Graphics);
                    var allGraphics = new List<IDynamicGraphicObj>();
                    foreach (IGraphicObj g in overlay.Graphics)
                    {
                        if (g is IDynamicGraphicObj)
                            allGraphics.Add(g as IDynamicGraphicObj);
                    }

                    Card.ZoomViewer.DisplayedZoomCell.DeleteGraphics(allGraphics, overlay);
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message + ex.StackTrace);
            }
        }


        //private void UnSelectAllGraphics()
        //{
        //    try
        //    {
        //        Logger.LogFuncUp();
        //        foreach (var filmPage in Card.ActiveFilmingPageList)
        //        {
        //            foreach (var cell in filmPage.Cells)
        //            {
        //                if (cell != null && cell.Image != null && cell.Image.CurrentPage != null)
        //                {
        //                    var overlay = cell.Image.CurrentPage.GetOverlay(OverlayType.Graphics);
        //                    foreach (IGraphicObj g in overlay.Graphics)
        //                    {
        //                        if (g is DynamicGraphicBase)
        //                            (g as DynamicGraphicBase).IsSelected = false;
        //                    }
        //                }
        //            }
        //        }

        //        Logger.LogFuncDown();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogFuncException(ex.Message + ex.StackTrace);
        //    }
        //}
      


    }
}
