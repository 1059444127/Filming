using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UIH.Mcsf.Filming.Model;
using UIH.Mcsf.Viewer;
using UIH.Mcsf.Filming.Utility;
using System.Diagnostics;
namespace UIH.Mcsf.Filming
{
    internal class RepackRegion
    {
        public RepackRegion(List<FilmingPageControl> pages)
        {
            _pages=pages;
        }

        #region [--多分格 Repack--]

        //Layoutcell repack
        public void Repack()
        {
            try
            {
                //有图cell和位置准备
                var filmingImageLayoutCells = GetAllImageLayoutCells().ToList(); 
                var filmingLayoutCells = GetAllLayoutCells().ToList();
                //有图cell放置个数记录index
                int cellIndex = 0;
                //循环每个可放图片的位置(SSFS要求空多分格不参与repack)，放置图片并刷新
                foreach (var originalCellKeyPair in filmingLayoutCells) 
                {
                    var originalCell = originalCellKeyPair.Key;
                    var page = originalCellKeyPair.Value;
                    
                    //图片放置完成后，将后面多余的cell的UI清空
                    if (cellIndex >= filmingImageLayoutCells.Count)
                    {
                        if (originalCell != null&&!originalCell.IsEmpty())
                        {
                            foreach (var ctrlCell in originalCell.ControlCells)
                            {
                                FilmPageUtil.ClearAllActions(ctrlCell);
                                ctrlCell.IsSelected = false;
                            }
                            originalCell.Clear();
                        }
                        if (originalCell != null)
                        {
                            originalCell.DeSelected();
                        }
                        continue;
                    }
                    //originalCell为胶片的空格位置，replaceCell为带图片的cell
                    if (originalCell != null && originalCell.IsMultiformatLayoutCell&& originalCell.IsEmpty()) continue;

                    var replaceCell = filmingImageLayoutCells[cellIndex];
                    //以layoutcell为单位替换cell，可见即刷新，不可见置刷新属性
                    if (originalCell != replaceCell)
                    {
                        if (page.Visibility == Visibility.Visible)
                        {
                            originalCell.ReplaceBy(replaceCell, true, page.CurrentActionType);
                        }
                        else
                        {
                            originalCell.ReplaceBy(replaceCell, false, page.CurrentActionType);
                        }
                        var controlCells = originalCell.ControlCells;
                        if(controlCells.Count > 1) controlCells.ForEach(page.RegisterEventFromCell);
                        if (originalCell.IsSelected() && !page.IsSelected)
                        {
                            page.IsSelected = true;
                        }
                    }
                    cellIndex++;
                }
                if (filmingImageLayoutCells.Count == 0 && filmingLayoutCells.Count != 0)
                {
                    var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
                    if (filmingCard != null && !filmingCard.IsCellSelected)
                    {
                        var cell = filmingLayoutCells.FirstOrDefault().Key;
                        var page = filmingLayoutCells.FirstOrDefault().Value;
                        var cCell = cell.ControlCells.FirstOrDefault();
                        if (page.IsVisible && cCell != null) cCell.IsSelected = true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.LogDevError("Repack Error" + e.Message + e.StackTrace);
            }
        }

        //Controlcell repack
        public void RepackControlCell()
        {
            //有图cell和位置准备
            var filmingImageControlCells = GetAllImageControlCells().ToList();
            var filmingControlCells = GetAllControlCells().ToList();
            var filmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            //有图cell放置个数记录index
            int cellIndex = 0;
            //循环每个可放图片的位置(SSFS要求空多分格不参与repack)，放置图片并刷新
            foreach (var originalCellKeyPair in filmingControlCells)
            {
                var originalCell = originalCellKeyPair.Key;
                var page = originalCellKeyPair.Value;

                //图片放置完成后，将后面多余的cell的UI清空
                if (cellIndex >= filmingImageControlCells.Count)
                {
                    if (originalCell != null && !originalCell.IsEmpty)
                    {
                        FilmPageUtil.ClearAllActions(originalCell);
                        
                        originalCell.Image.Clear();
                        originalCell.Refresh();
                    }
                    if (originalCell != null)
                    {
                        originalCell.IsSelected = false;
                        var viewport = page.ViewportOfCell(originalCell);
                        FilmPageUtil.UnselectCellInViewport(viewport, originalCell);
                    }
                    continue;
                }
                //originalCell为胶片的空格位置，replaceCell为带图片的cell
                //if (originalCell != null && originalCell.IsMultiformatLayoutCell && originalCell.IsEmpty()) continue;

                var replaceCell = filmingImageControlCells[cellIndex];
                //以layoutcell为单位替换cell，可见即刷新，不可见置刷新属性
                if (originalCell != replaceCell)
                {
                    if (page.Visibility == Visibility.Visible)
                    {
                        originalCell.ReplaceBy(replaceCell,true,page.CurrentActionType);
                    }
                    else
                    {
                        originalCell.ReplaceBy(replaceCell, false, page.CurrentActionType);
                    }
                    page.RegisterEventFromCell(originalCell);
                    if (originalCell.IsSelected && !page.IsSelected)
                    {
                        page.IsSelected = true;
                    }
                    var vp = page.ViewportOfCell(originalCell);
                    if (originalCell.IsSelected && vp != null) vp.IsSelected = true;
                    if (!originalCell.IsSelected && vp != null)
                        FilmPageUtil.UnselectCellInViewport(vp, originalCell);
                }
                cellIndex++;
            }
            if (filmingImageControlCells.Count == 0 && filmingControlCells.Count != 0)
            {
                if (filmingCard != null && !filmingCard.IsCellSelected)
                {
                    var cell = filmingControlCells.FirstOrDefault().Key;
                    var page = filmingControlCells.FirstOrDefault().Value;
                    if (page.IsVisible && cell != null)
                    {
                        cell.IsSelected = true;
                        var vp = page.ViewportOfCell(cell);
                        if (vp != null) vp.IsSelected = true;
                    }
                }
            }
        }

        private IEnumerable<FilmingLayoutCell> GetAllImageLayoutCells()
        {
            return _pages.SelectMany(page => page.RootCell.Children).OfType<FilmingLayoutCell>().Where(layoutCell => !layoutCell.IsEmpty());
        }

        private IEnumerable<FilmingControlCell> GetAllImageControlCells()
        {
            return _pages.SelectMany(page => page.RootCell.ControlCells).OfType<FilmingControlCell>().Where(ctrlCell => !ctrlCell.IsEmpty);
        }

        private IEnumerable<KeyValuePair<FilmingLayoutCell, FilmingPageControl>> GetAllLayoutCells()
        {
            foreach (var page in _pages)
            {
                foreach (var cell in page.RootCell.Children.OfType<FilmingLayoutCell>())
                {
                    yield return new KeyValuePair<FilmingLayoutCell, FilmingPageControl>(cell, page);
                }
            }
        }

        private IEnumerable<KeyValuePair<FilmingControlCell, FilmingPageControl>> GetAllControlCells()
        {
            foreach (var page in _pages)
            {
                foreach (var cell in page.RootCell.ControlCells.OfType<FilmingControlCell>())
                {
                    yield return new KeyValuePair<FilmingControlCell, FilmingPageControl>(cell, page);
                }
            }
        }

        public bool IsRegularLayout()
        {
            if (!Printers.Instance.IfUseFilmingServiceFE) return false; //Feature in developing 
            return _pages.All(page => page.ViewportLayout.LayoutType == LayoutTypeEnum.RegularLayout);
        }


        #endregion //[--多分格 Repack--]

        #region [--Fields--]

        private readonly List<FilmingPageControl> _pages;

        #endregion [--Private Methods--]
      
    }
}
