using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Utility
{
    public static class FilmPageUtil
    {
        //public static void SetLayoutCellVisibility(MedViewerLayoutCell layoutCell, bool visible)
        //{
        //    MedViewerLayoutCellImpl layoutCellImpl;
        //    if (layoutCell != null  && (layoutCellImpl = layoutCell.Control as MedViewerLayoutCellImpl)!=null )
        //    {
        //        layoutCellImpl.Border.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        //    }
        //}

        public static bool FilmLayoutEquals(FilmLayout a, FilmLayout b)
        {
            return (a.LayoutName == b.LayoutName
                && a.LayoutRowsSize == b.LayoutRowsSize
                && a.LayoutColumnsSize == b.LayoutColumnsSize);
        }

        #region Get objects(pages, viewports, cells, etc.)

        public static McsfFilmViewport ViewportOfCell(MedViewerControlCell cell, FilmingPageControl page)
        {
            var viewport = ViewportOfCell(cell, page.ViewportList);

            if (viewport == null)
            {
                page.ReBuildViewportList();

                viewport = ViewportOfCell(cell, page.ViewportList);
            }

            //if (viewport == null)
            //{
            //    Logger.LogError(string.Format("Failed to get viewport of cell in page {0}", page.FilmPageTitle));
            //}

            return viewport;
        }

        private static McsfFilmViewport ViewportOfCell(MedViewerControlCell cell, List<McsfFilmViewport> viewportList)
        {
            if (cell == null)
            {
                return null;
            }

            var layoutCell = cell.ParentCell as MedViewerLayoutCell;
            while (layoutCell != null)
            {
                foreach (var viewport in viewportList)
                {
                    if (viewport.RootLayoutCell == layoutCell)
                    {
                        return viewport;
                    }
                }

                layoutCell = layoutCell.ParentCell as MedViewerLayoutCell;
            }

            return null;
        }

        public static McsfFilmViewport GetSelectedViewport(MedViewerCellBase selectedCellBase, List<McsfFilmViewport> viewportList)
        {
            try
            {
                Logger.LogFuncUp();

                var layoutCell = selectedCellBase as MedViewerLayoutCell;
                if (layoutCell != null && layoutCell.Control!=null)
                {
                    if (layoutCell.Control.IsMouseOver)
                    {
                        if (layoutCell.Children == null || !layoutCell.Children.Any())
                        {
                            return viewportList.FirstOrDefault(viewport => viewport.RootLayoutCell.Equals(layoutCell));
                        }

                        foreach (var child in layoutCell.Children)
                        {
                            var subLayoutCell = child as MedViewerLayoutCell;
                            if (subLayoutCell != null)
                            {
                                var selectedFilmRegion = GetSelectedViewport(subLayoutCell, viewportList);
                                if (selectedFilmRegion != null)
                                {
                                    return selectedFilmRegion;
                                }
                            }
                            else
                            {
                                foreach (var viewport in viewportList.Where(viewport => viewport.RootLayoutCell.Equals(layoutCell)))
                                {
                                    return viewport;
                                }
                            }
                        }
                    }
                }

                Logger.LogFuncDown();
                return null;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                //throw;
                return null;
            }
        }

        //public static MedViewerControlCell GetFirstCellOfLayoutCell(MedViewerLayoutCell layoutCell)
        //{
        //    if (layoutCell == null || layoutCell.Children == null)
        //    {
        //        return null;
        //    }

        //    MedViewerControlCell cell = null;
        //    foreach (var child in layoutCell.Children)
        //    {
        //        var subLayoutCell = child as MedViewerLayoutCell;
        //        if (subLayoutCell != null)
        //        {
        //            cell = GetFirstCellOfLayoutCell(subLayoutCell);
        //            if (cell != null)
        //            {
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            cell = child as MedViewerControlCell;
        //            if (cell != null)
        //            {
        //                break;
        //            }
        //        }
        //    }

        //    return cell;
        //}

        //public static int GetSelectedCellsCount(MedViewerLayoutCell layoutCell)
        //{
        //    if (layoutCell == null || layoutCell.Children == null)
        //    {
        //        return 0;
        //    }

        //    return GetSelectedCellsOfLayoutCell(layoutCell).Count;
        //}

        public static List<MedViewerControlCell> GetSelectedCellsOfLayoutCell(MedViewerLayoutCell layoutCell)
        {
            var cells = new List<MedViewerControlCell>();
            if (layoutCell == null || layoutCell.Children == null)
            {
                return cells;
            }

            foreach (var child in layoutCell.Children)
            {
                var subLayoutCell = child as MedViewerLayoutCell;
                if (subLayoutCell != null)
                {
                    cells.AddRange(GetSelectedCellsOfLayoutCell(subLayoutCell));
                }
                else
                {
                    var cell = child as MedViewerControlCell;
                    if (cell != null && cell.IsSelected)
                    {
                        cells.Add(cell);
                    }
                }
            }

            return cells;
        }

        public static List<MedViewerControlCell> GetCellsOfLayoutCell(MedViewerLayoutCell layoutCell)
        {
            var cells = new List<MedViewerControlCell>();
            if (layoutCell == null || layoutCell.Children == null)
            {
                return cells;
            }

            foreach (var child in layoutCell.Children)
            {
                var subLayoutCell = child as MedViewerLayoutCell;
                if (subLayoutCell != null)
                {
                    cells.AddRange(GetCellsOfLayoutCell(subLayoutCell));
                }
                else
                {
                    var cell = child as MedViewerControlCell;
                    if (cell != null)
                    {
                        cells.Add(cell);
                    }
                }
            }

            return cells;
        }


        public static int GetChildNodeCount(MedViewerCellBase cellBase)
        {
            var totalCount = 0;
            var layoutCell = cellBase as MedViewerLayoutCell;
            if (layoutCell != null)
            {
                totalCount += layoutCell.Rows * layoutCell.Columns;

                if (layoutCell.Children == null)
                {
                    totalCount += 1;
                }
                else
                {
                    totalCount += layoutCell.Children.Sum(child => GetChildNodeCount(child));
                    totalCount -= 1;
                }
            }

            return totalCount;
        }

        public static string GetSeriesUidFromImage(MedViewerImage image)
        {
            if (image != null && image.CurrentPage != null)
            {
                try
                {
                    var dicomHeader = image.CurrentPage.ImageHeader.DicomHeader;
                    return dicomHeader[ServiceTagName.SeriesInstanceUID];
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex.StackTrace);
                }
            }

            return null;
        }
        public static string GetImageNumberFromImage(MedViewerImage image)
        {
            if (image != null && image.CurrentPage != null)
            {
                try
                {
                    var dicomHeader = image.CurrentPage.ImageHeader.DicomHeader;
                    return dicomHeader[ServiceTagName.InstanceNumber];
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex.StackTrace);
                }
            }

            return null;
        }

        public static int GetImageCountOfViewport(McsfFilmViewport viewport)
        {
            if (viewport == null)
                return 0;

            return viewport.GetCells().Count(cell => cell.Image != null);
        }

        #endregion

        #region Selection related(select/unselect pages, viewports, cells, etc.)

        public static bool IsMultiplySelect()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }

        public static bool IsSuccessionalSelect()
        {
            return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        }

        //public static bool HasOtherSelectedCellsInViewport(McsfFilmViewport viewport, MedViewerControlCell exceptCell)
        //{
        //    var cellsInViewport = viewport.GetCells();
        //    return cellsInViewport.Any(aCell => aCell.IsSelected && aCell != exceptCell);
        //}

        public static List<string> GetSelectedSeriesUid(IEnumerable<FilmingPageControl> pageList)
        {
            try
            {
                Logger.LogFuncUp();

                var uidList = new List<string>();
                foreach (var page in pageList)
                {
                    foreach (var cell in page.SelectedCells())
                    {
                        if (cell.Image == null)
                            continue;

                        var seriesUid = GetSeriesUidFromImage(cell.Image);
                        if (!uidList.Contains(seriesUid))
                        {
                            uidList.Add(seriesUid);
                        }
                    }
                }

                Logger.LogFuncDown();

                return uidList;
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
                throw;
            }
        }

        //public static bool HasSelectedSameSeries(IEnumerable<FilmingPageControl> pageList, out string firstSeriesUID)
        //{
        //    bool isSameSeries = true;
        //    firstSeriesUID = null;

        //    foreach (var page in pageList)
        //    {
        //        foreach (var cell in page.SelectedCells())
        //        {
        //            string aSeriesUID = GetSeriesUidFromImage(cell.Image);
        //            if (!string.IsNullOrEmpty(aSeriesUID))
        //            {
        //                if (string.IsNullOrEmpty(firstSeriesUID))
        //                {
        //                    firstSeriesUID = aSeriesUID;
        //                }
        //                else if (!firstSeriesUID.Equals(aSeriesUID))
        //                {
        //                    isSameSeries = false;
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    if (isSameSeries && string.IsNullOrEmpty(firstSeriesUID))
        //    {
        //        isSameSeries = false;
        //    }

        //    return isSameSeries;
        //}

        private static bool CheckSortedCellsSuccessinal(List<MedViewerControlCell> sortedCells)
        {
            if (sortedCells == null || !sortedCells.Any())
                return false;

            var cellIndex = -1;
            foreach (var cell in sortedCells)
            {
                if (cellIndex < 0)
                {
                    cellIndex = cell.CellIndex;
                }
                else
                {
                    if (cell.CellIndex != cellIndex + 1 || cell.IsEmpty)
                    {
                        return false;
                    }

                    ++cellIndex;
                }
            }

            return true;
        }

        public static bool IsSelectedCellsSuccessional(IEnumerable<FilmingPageControl> pageList)
        {
            // no active page
            if (!pageList.Any())
            {
                return false;
            }
            var newPageList = new List<FilmingPageControl>();
            foreach (var page in pageList)
            {
                if (page.SelectedCells().Any())
                {
                    newPageList.Add(page);
                }
            }
            var sortedActivePageList = newPageList.OrderBy(page => page.FilmPageIndex);

            // single active page
            if (sortedActivePageList.Count() == 1)
            {
                var page = sortedActivePageList.First();
                var sortedCells = page.SelectedCells().OrderBy(cell => cell.CellIndex).ToList();
                if (sortedCells.Count <= 1)
                    return false;

                return CheckSortedCellsSuccessinal(sortedCells);
            }

            // multi active pages
            var startPage = sortedActivePageList.First();
            var endPage = sortedActivePageList.Last();

            var cellCount = 0;
            var pageIndex = -1;
            foreach (var page in sortedActivePageList)
            {
                if (!page.SelectedCells().Any())
                {
                    return false;
                }

                // check pages
                if (pageIndex < 0)
                {
                    pageIndex = page.FilmPageIndex;
                }
                else
                {
                    if (page.FilmPageIndex != pageIndex + 1)
                    {
                        return false;
                    }
                    ++pageIndex;
                }

                // check cells
                var sortedSelectedCells = page.SelectedCells().OrderBy(cell => cell.CellIndex).ToList();
                if (page == startPage)
                {
                    var sortedCells = page.Cells.OrderBy(cell => cell.CellIndex);
                    if (sortedCells.Last() != sortedSelectedCells.Last())
                    {
                        return false;
                    }
                }
                else if (page == endPage)
                {
                    var sortedCells = page.Cells.OrderBy(cell => cell.CellIndex);
                    if (sortedCells.First() != sortedSelectedCells.First())
                    {
                        return false;
                    }
                }

                if (!CheckSortedCellsSuccessinal(sortedSelectedCells))
                    return false;

                cellCount += sortedSelectedCells.Count;
            }

            return (cellCount > 1);
        }

        public static void SelectCellsOfLayoutCell(MedViewerLayoutCell layoutCell, bool isSelected)
        {
            if (layoutCell == null || layoutCell.Children == null)
            {
                return;
            }

            foreach (var child in layoutCell.Children)
            {
                var subLayoutCell = child as MedViewerLayoutCell;
                if (subLayoutCell != null)
                {
                    SelectCellsOfLayoutCell(subLayoutCell, isSelected);
                }
                else
                {
                    var cell = child as MedViewerControlCell;
                    if (cell != null && cell.IsSelected != isSelected)
                    {
                        cell.IsSelected = isSelected;
                    }
                   // if(cell != null && cell.IsHoldedByAction) cell.ForceEndAction();
                }
            }
        }

        // Active/Deactive and select/unselect all viewport, cells of filming page.
        public static void SelectAllOfFilmingPage(FilmingPageControl page, Boolean isSelected)
        {
            page.IsSelected = isSelected;

            page.SelectedAll(isSelected);
        }

        public static void UnselectOtherFilmingPages(IEnumerable<FilmingPageControl> pageList, FilmingPageControl page)
        {
            var unselectList = pageList.Where(a => a != page).ToList();
            foreach (var filmingPage in unselectList)
            {
                SelectAllOfFilmingPage(filmingPage, false);
            }
        }

        public static void UnselectOtherViewports(FilmingPageControl page, McsfFilmViewport viewport)
        {
            foreach (var aViewport in page.ViewportList)
            {
                if (aViewport.IsSelected && aViewport != viewport)
                {
                    aViewport.IsSelected = false;
                    aViewport.SelectAllCells(false);
                }
            }
        }

        public static void UnselectOtherCellsInViewport(McsfFilmViewport viewport, MedViewerControlCell cell, bool _isGroupLRButtonDown = false)
        {
            //if (cell.ViewerControl.CellHoldedByAction != null)
            //{
            //    cell.ViewerControl.CellHoldedByAction.ForceEndAction();
            //}
            var cells = viewport.GetCells();
            foreach (var aCell in cells)
            {
                if (aCell.IsSelected && aCell != cell && !_isGroupLRButtonDown)
                {
                    aCell.IsSelected = false;
                }
            }
        }

        public static void UnselectCellInViewport(McsfFilmViewport viewport, MedViewerControlCell cell)
        {
            if (cell == null) return;
            if (viewport == null)
            {
                cell.IsSelected = false;
                return;
            }
            var cells = viewport.GetCells();
            cell.IsSelected = false;
            if (cells.All(c => !c.IsSelected))
            {
                viewport.IsSelected = false;
            }
        }

        #region  [edit by jinyang.li]
        /// <summary>
        /// 设置minicell的状态
        /// </summary>
        /// <param name="pageList"></param>IEnumerable<FilmingPageControl> pageList
        /// <param name="args"></param>
        //public static void MakeSingleUseCellSelectStatus(List<MedViewerControlCell> cellList , SingleUseCellSelectedEventArgs args)
        //{
        //    foreach (var cell in cellList)
        //    {
        //        var overlayLocalizedImage = cell.Image.CurrentPage.GetOverlay(OverlayType.LocalizedImage) as OverlayLocalizedImage;
        //        if (null != overlayLocalizedImage)
        //        {
        //            pageControl.IsSelected = true;
        //            viewport.IsSelected = true;
        //            overlayLocalizedImage.GraphicLocalizedImage.MiniCell.IsSelected = true;
        //            //shouldBreakFilmingPage = true;
        //            //break;
        //        }
        //    }
        //}
        #endregion

        public static void GetMinMaxInfoFromStartAndEndSelections(
            FilmingPageControl startPage, McsfFilmViewport startViewport, MedViewerControlCell startCell,
            FilmingPageControl endPage, McsfFilmViewport endViewport, MedViewerControlCell endCell,
            out FilmingPageControl minPage, out FilmingPageControl maxPage,
            out int minViewportIndex, out int maxViewportIndex,
            out int minCellIndex, out int maxCellIndex)
        {
            minPage = (startPage.FilmPageIndex > endPage.FilmPageIndex) ? endPage : startPage;
            maxPage = (minPage == endPage) ? startPage : endPage;
            minViewportIndex = (startPage.FilmPageIndex == minPage.FilmPageIndex) ? startViewport.IndexInFilm : endViewport.IndexInFilm;
            maxViewportIndex = (startPage.FilmPageIndex == maxPage.FilmPageIndex) ? startViewport.IndexInFilm : endViewport.IndexInFilm;
            minCellIndex = (startPage.FilmPageIndex == minPage.FilmPageIndex) ? startCell.CellIndex : endCell.CellIndex;
            maxCellIndex = (startPage.FilmPageIndex == maxPage.FilmPageIndex) ? startCell.CellIndex : endCell.CellIndex;

            // same page
            if (minPage.FilmPageIndex == maxPage.FilmPageIndex)
            {
                minViewportIndex = Math.Min(startViewport.IndexInFilm, endViewport.IndexInFilm);
                maxViewportIndex = Math.Max(startViewport.IndexInFilm, endViewport.IndexInFilm);

                // same viewport
                if (startViewport.IndexInFilm == endViewport.IndexInFilm)
                {
                    minCellIndex = Math.Min(startCell.CellIndex, endCell.CellIndex);
                    maxCellIndex = Math.Max(startCell.CellIndex, endCell.CellIndex);
                }
                else
                {
                    // different viewport
                    if (minViewportIndex == startViewport.IndexInFilm)
                    {
                        minCellIndex = startCell.CellIndex;
                        maxCellIndex = endCell.CellIndex;
                    }
                    else /*(minViewportIndex == endViewport.Index)*/
                    {
                        minCellIndex = endCell.CellIndex;
                        maxCellIndex = startCell.CellIndex;
                    }
                }
            }
        }

        public static void SelectCellsOnSuccessionalPage(
            FilmingPageControl currentPage, FilmingPageControl minPage, FilmingPageControl maxPage,
            int minVIndex, int maxVIndex, int minCIndex, int maxCIndex)
        {
            currentPage.IsSelected = true;

            if (minPage == maxPage)
            {
                // only one page is selected
                foreach (var viewport in currentPage.ViewportList)
                {
                    if (viewport.IndexInFilm >= minVIndex && viewport.IndexInFilm <= maxVIndex)
                    {
                        foreach (var cell in viewport.GetCells())
                        {
                            viewport.IsSelected = true;
                            if (cell.CellIndex >= minCIndex && cell.CellIndex <= maxCIndex)
                            {
                                if (!cell.IsSelected)
                                    cell.IsSelected = true;
                            }
                            else
                            {
                                if (cell.IsSelected)
                                    cell.IsSelected = false;
                            }
                        }
                    }
                    else
                    {
                        if (viewport.IsSelected)
                        {
                            viewport.IsSelected = false;
                            viewport.SelectAllCells(false);
                        }
                    }
                }
            }

            else if (currentPage == minPage)
            {
                // current page is min page.
                foreach (var viewport in currentPage.ViewportList)
                {
                    if (viewport.IndexInFilm > minVIndex)
                    {
                        viewport.IsSelected = true;
                        viewport.SelectAllCells(true);
                    }
                    else if (viewport.IndexInFilm == minVIndex)
                    {
                        viewport.IsSelected = true;

                        foreach (var cell in viewport.GetCells())
                        {
                            if (cell.CellIndex >= minCIndex)
                            {
                                if (!cell.IsSelected)
                                    cell.IsSelected = true;
                            }
                            else
                            {
                                if (cell.IsSelected)
                                    cell.IsSelected = false;
                            }
                        }
                    }
                    else
                    {
                        if (viewport.IsSelected)
                        {
                            viewport.IsSelected = false;
                            viewport.SelectAllCells(false);
                        }
                    }
                }
            }

            else if (currentPage == maxPage)
            {
                // current page is max page.
                foreach (var viewport in currentPage.ViewportList)
                {
                    if (viewport.IndexInFilm < maxVIndex)
                    {
                        viewport.IsSelected = true;
                        viewport.SelectAllCells(true);
                    }
                    else if (viewport.IndexInFilm == maxVIndex)
                    {
                        viewport.IsSelected = true;
                        foreach (var cell in viewport.GetCells())
                        {
                            if (cell.CellIndex <= maxCIndex)
                            {
                                if (!cell.IsSelected)
                                    cell.IsSelected = true;
                            }
                            else
                            {
                                if (cell.IsSelected)
                                    cell.IsSelected = false;
                            }
                        }
                    }
                    else
                    {
                        if (viewport.IsSelected)
                        {
                            viewport.IsSelected = false;
                            viewport.SelectAllCells(false);
                        }
                    }
                }
            }
            else
            {
                // current page is between min and max page.
                currentPage.SelectedAll(true);
            }
        }

        #endregion

        #region Insert object

        /// <summary>
        /// change a selected cell to layout cell,the selected cell will hold.
        /// </summary>
        /// <param name="replacedCell">the cell which will be replaced to layout cell</param>
        /// <param name="multiFormatLayout">the new layout cell's layout</param>
        /// <returns>the new layout cell</returns>
        public static MedViewerLayoutCell InsertMultiFormatToCell(MedViewerControlCell replacedCell, FilmLayout multiFormatLayout)
        {
            try
            {
                if (multiFormatLayout == null)
                {
                    Logger.LogError("The multiFormatLayout can not be NULL!");
                    return null;
                }

                if (multiFormatLayout.LayoutType != LayoutTypeEnum.StandardLayout)
                {
                    Logger.LogError("MultiFormat Cell don't support defined layout, just standard layout!");
                    return null;
                }

                if (replacedCell == null)
                {
                    Logger.LogError("The cell will be replaced is NULL!");
                    return null;
                }

                var parentLayoutCell = replacedCell.ParentCell as MedViewerLayoutCell;
                if (parentLayoutCell != null)
                {
                    var newMultiLayoutCell = new MedViewerLayoutCell
                                                 {
                                                     Rows = multiFormatLayout.LayoutRowsSize,
                                                     Columns = multiFormatLayout.LayoutColumnsSize
                                                 };

                    //NOTE: don't specify the cell index, need the parent child index of this cell.
                    var replacedCellIndex = parentLayoutCell.Children.ToList().IndexOf(replacedCell);
                    parentLayoutCell.ReplaceCell(newMultiLayoutCell, replacedCellIndex);
                    newMultiLayoutCell.AddCell(replacedCell);

                    parentLayoutCell.Refresh();

                    return newMultiLayoutCell;
                }

                Logger.LogError("The cell will be replaced parent is NULL!");
                return null;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
                throw;
            }
        }


        public static bool IsMultiFormatCell(MedViewerControlCell cell)
        {
            //if (cell.IsEmpty) return false;

            //var dataHeader = cell.Image.CurrentPage.ImageHeader.DicomHeader;
            //if (dataHeader.ContainsKey("ImageType"))
            //{
            //    var imageType = dataHeader["ImageType"];
            //    if (imageType == FilmingUtility.MultiFormatCellImageType) return true;
            //}

            return false;
        }

        public static bool IsEfilmCell(MedViewerControlCell cell)
        {
            if (cell == null) return false;
            var image = cell.Image;
            if (image == null) return false;
            var displayData = image.CurrentPage;
            if (displayData == null) return false;
            var imageHeader = displayData.ImageHeader;
            if (imageHeader == null) return false;
            var dataHeader = imageHeader.DicomHeader;
            if (dataHeader == null) return false;
            //string modality = string.Empty;
            var imageType = string.Empty;
            //if (dataHeader.ContainsKey("Modality")) modality = dataHeader["Modality"];
            if (dataHeader.ContainsKey(ServiceTagName.ImageType)) imageType = dataHeader[ServiceTagName.ImageType];
            if (imageType == FilmingUtility.EFilmImageType) return true;
            return false;
        }

        private static bool IsStitchingCell(MedViewerControlCell cell)
        {
            if (cell == null) return false;
            var image = cell.Image;
            if (image == null) return false;
            var displayData = image.CurrentPage;
            if (displayData == null) return false;
            var imageHeader = displayData.ImageHeader;
            if (imageHeader == null) return false;
            var dataHeader = imageHeader.DicomHeader;
            if (dataHeader == null) return false;
            //string modality = string.Empty;
            var imageType = string.Empty;
            //if (dataHeader.ContainsKey("Modality")) modality = dataHeader["Modality"];
            if (dataHeader.ContainsKey(ServiceTagName.ImageType)) imageType = dataHeader[ServiceTagName.ImageType];
            if (imageType == FilmingUtility.StitchingImageType) return true;
            return false;
        }

        private static bool IsSCCell(MedViewerControlCell cell)
        {
            if (cell == null) return false;
            var image = cell.Image;
            if (image == null) return false;
            var displayData = image.CurrentPage;
            if (displayData == null) return false;
            return displayData.IsSecondCapture;
        }

        private static bool IsRoiAction(ActionType actionType)
        {
            if (actionType == ActionType.AnnotationLine || actionType == ActionType.AnnotationAngle || actionType == ActionType.RectangleHistogram
                || actionType == ActionType.ImageProfile || actionType == ActionType.EllipseHistogram || actionType == ActionType.PixelLens || actionType.ToString().Contains("Region"))
                return true;
            return false;
        }

        public static void SetAction(MedViewerControlCell cell, ActionType actionType, MouseButton button = MouseButton.Left)
        {
            //fix bug 167574 2012/10/29, E-film has no action except windowing
            if (cell.Image == null || IsMultiFormatCell(cell) || (IsRoiAction(actionType) && (IsStitchingCell(cell))))
            {
                cell.SetAction(ActionType.Pointer, button);
                return;
            }

            if (IsRoiAction(actionType) && IsSCCell(cell))
            {
                cell.SetAction(ActionType.Empty, button);
                return;
            }

            cell.SetAction(actionType, button);

        }


        public static void SetAllActions(MedViewerControlCell cell, ActionType action)
        {
            SetAllActionExceptLeftButton(cell);

            SetAction(cell, action);
        }

        public static void SetAllActionExceptLeftButton(MedViewerControlCell cell)
        {
            if (cell == null || cell.Image==null || cell.Image.CurrentPage==null ) return;
               //|| cell.Image.CurrentPage.IsTableOrCurve) return;

            SetAction(cell, ActionType.Windowing,
                  MouseButton.Middle);
            SetAction(cell, ActionType.Pan,
                      MouseButton.XButton1);
            SetAction(cell, ActionType.Zoom,
                      MouseButton.Right);
        }

        public static void ClearAllActions(MedViewerControlCell cell)
        {
            SetAction(cell, ActionType.Pointer);
            SetAction(cell, ActionType.Pointer,
                  MouseButton.Middle);
            SetAction(cell, ActionType.Pointer,
                      MouseButton.XButton1);
            SetAction(cell, ActionType.Pointer,
                      MouseButton.Right);
        }

        #endregion

        /// <summary>
        /// judge the cell image whether inverted!
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsValueEqualOfBitmapPalette(BitmapPalette a, BitmapPalette b)
        {
            try
            {
                if (a.Colors.Count == b.Colors.Count && a.Colors.Count == 256)
                {
                    for (var i = 0; i < 256; i++)
                    {
                        if (a.Colors[i] != b.Colors[i])
                        {
                            return false;
                        }
                    }
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
                throw;
            }
            
        }

        
    }
}
