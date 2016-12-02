using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using UIH.Mcsf.Viewer;
using System;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

//todo: performance optimization begin refresh
namespace UIH.Mcsf.Filming.Utility
{
    internal class FilmingActionDeal
    {
        public MedViewerControlCell CurrentCell
        {
            get;
            set;
        }

        public void UpdateSelectedFimingPage(ActionType actionType,
                                                                          PresentationState pState,
                                                                          FilmingPageControl page)
        {
            var transformRenderCenterPoint = new Point();
            if (actionType == ActionType.Pan)
            {
                var renderCenterPoint = new Point((pState.RenderCenterX - 0.5), (pState.RenderCenterY - 0.5));
                var matrix = new Matrix(pState.TransformMatrix.M11,
                                           pState.TransformMatrix.M12,
                                           pState.TransformMatrix.M21,
                                           pState.TransformMatrix.M22, 0, 0);
                transformRenderCenterPoint = matrix.Transform(renderCenterPoint);
            }

            var selectCells = page.SelectedCells();
            bool shouldRefreshUI = page.IsVisible;
            foreach (MedViewerControlCell cell in selectCells)
            {
                if (cell != CurrentCell && !cell.IsEmpty)
                {
                    switch (actionType)
                    {
                        case ActionType.Pan:
                            PanCell(cell, transformRenderCenterPoint.X + 0.5, transformRenderCenterPoint.Y + 0.5, shouldRefreshUI);
                            break;
                        case ActionType.Windowing:
                            if (CurrentCell.Image.CurrentPage.Modality == cell.Image.CurrentPage.Modality)
                            {
                                WindowingCell(cell, pState.WindowLevel, shouldRefreshUI);
                            }
                            break;
                        case ActionType.Zoom:
                            ZoomCell(cell, pState.ScaleX, pState.ScaleY, shouldRefreshUI);
                            break;
                    }
                }
            }
        }
        public void UpdateAllActiveFilmingPageControl(ActionType actionType,
                                                                                        PresentationState pState,
                                                                                        List<FilmingPageControl> activeFilmingPageControlList,
                                                                                        MedViewerControl currentViewerControl)
        {

            if (this.ColorImageInWIndowingAction(actionType))
            {
                return;
            }

            foreach (var fpc in activeFilmingPageControlList)
            {
                if (fpc.filmingViewerControl.Equals(currentViewerControl))
                {
                    continue;
                }

                fpc.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                                              new Action<ActionType, PresentationState, FilmingPageControl>(
                                                              UpdateSelectedFimingPage), actionType, pState, fpc);
            }

            if (activeFilmingPageControlList.Any())
            {
                activeFilmingPageControlList[0].Dispatcher.Invoke(new Action(() => { }));
            }
        }

        private bool ColorImageInWIndowingAction(ActionType actionType)
        {
            return CurrentCell.Image.CurrentPage.SamplesPerPixel == 3 && actionType == ActionType.Windowing;
        }

        /**********************************************************************************
             * 1.图像在cell中的位置应该由RenderCenter 来决定，offset是通过RenderCenter和cell 
             * 的weight&height 计算出来的, Offset只是用于在cell中渲染显示图像，
             * 不是用来决定图像在cell中的位置.
             * Edit by Duyaojun,2013.12.30
             * *********************************************************************************/
        private void PanCell(MedViewerControlCell cell, double renderCenterX, double renderCenterY, bool needRefresh)
        {
            Point renderCenterPoint = new Point((renderCenterX - 0.5), (renderCenterY - 0.5));
            Matrix matrix = new Matrix(cell.Image.CurrentPage.PState.TransformMatrix.M11,
                                                            cell.Image.CurrentPage.PState.TransformMatrix.M12,
                                                            cell.Image.CurrentPage.PState.TransformMatrix.M21,
                                                            cell.Image.CurrentPage.PState.TransformMatrix.M22, 0, 0);
            matrix.Invert();
            Point transformRenderCenterPoint = matrix.Transform(renderCenterPoint);
            cell.Image.CurrentPage.PState.CanRaiseValueChangedEvent = false;
            cell.Image.CurrentPage.PState.RenderCenterX = (transformRenderCenterPoint.X + 0.5);
            cell.Image.CurrentPage.PState.RenderCenterY = (transformRenderCenterPoint.Y + 0.5);
            cell.Image.CurrentPage.PState.UpdateOffset();
            cell.Image.CurrentPage.PState.CanRaiseValueChangedEvent = true;
            if (needRefresh)
            {
                cell.Refresh(CellRefreshType.Image);
            }
            else
            {
                cell.Image.CurrentPage.IsDirty = true;
            }
        }
        public static void ZoomCell(MedViewerControlCell cell, double scaleX, double scaleY, bool needRefresh)
        {
            double scaleXNew = Math.Abs(scaleX / cell.Image.CurrentPage.PState.ScaleX);
            double scaleYNew = Math.Abs(scaleY / cell.Image.CurrentPage.PState.ScaleY);
            cell.Image.CurrentPage.PState.CanRaiseValueChangedEvent = false;
            cell.Image.CurrentPage.PState.ScaleByRenderCenter(scaleXNew, scaleYNew);
            cell.Image.CurrentPage.PState.CanRaiseValueChangedEvent = true;
            if (needRefresh)
            {
                cell.Refresh();
            }
            else
            {
                cell.Image.CurrentPage.IsDirty = true;
            }
        }
        public static void SetEnhancementCell(MedViewerControlCell cell, string enhanceTag, bool needRefresh)
        {
            if (null != cell
                && null != cell.Image
                && null != cell.Image.CurrentPage
                && (Modality.CT == cell.Image.CurrentPage.Modality || Modality.MG == cell.Image.CurrentPage.Modality))
            {
                if (!cell.Image.CurrentPage.PState.EnhancePara.Equals(Convert.ToDouble(enhanceTag)))
                {
                    cell.Image.CurrentPage.PState.EnhancePara = Convert.ToDouble(enhanceTag);
                    if (needRefresh)
                    {
                        cell.Refresh();
                    }
                    else
                    {
                        cell.Image.CurrentPage.IsDirty = true;
                    }
                }
            }
        }
        private void WindowingCell(MedViewerControlCell cell, WindowLevel winLevel, bool needRefresh)
        {
            cell.Image.CurrentPage.PState.CanRaiseValueChangedEvent = false;
            cell.Image.CurrentPage.PState.WindowLevel = winLevel;
            cell.Image.CurrentPage.PState.CanRaiseValueChangedEvent = true;
            if (needRefresh)
            {
                cell.Refresh(CellRefreshType.Image);
                if (cell.ActualWidth >= FilmingUtility.FilmingPageSyncBoundary && cell.ActualHeight >= FilmingUtility.FilmingPageSyncBoundary)
                {
                    cell.Refresh(CellRefreshType.ImageText);
                }
            }
            else
            {
                cell.Image.CurrentPage.IsDirty = true;
            }
        }
    }
}
//todo: performance optimization end
