using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UIH.Mcsf.Filming.Model;
using UIH.Mcsf.Filming.Utility;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming
{
    public class FilmingLayoutCell : MedViewerLayoutCell
    {
        public FilmingLayoutCell()
        {
            BorderThickness = -1D;
        }
        public bool IsMultiformatLayoutCell { get; set; }

        public bool IsEmpty()
        {
            return Children.OfType<FilmingControlCell>().All(controlCell => controlCell.IsEmpty);
        }

        public bool ReplaceBy(MedViewerCellBase cell)
        {
            bool needToRegisterMouseEvent = false;
            if (cell is FilmingLayoutCell) needToRegisterMouseEvent = ReplaceBy(cell as FilmingLayoutCell);
            if (cell is FilmingControlCell) ReplaceBy(cell as FilmingControlCell);
            return needToRegisterMouseEvent;
        }

        public void ReplaceBy(FilmingControlCell controlCell)
        {
            RemoveAll();
            SetLayout(1, 1);
            IsMultiformatLayoutCell = false;
            AddCell(controlCell);
            Refresh();
        }

        public bool ReplaceBy(FilmingLayoutCell replaceCell)
        {
            bool needToRegisterMouseEvent = false;
            int cellCount = replaceCell.Rows * replaceCell.Columns;
            if (Rows != replaceCell.Rows || Columns != replaceCell.Columns)
            {
                RemoveAll();
                SetLayout(replaceCell.Rows, replaceCell.Columns);
                IsMultiformatLayoutCell = replaceCell.IsMultiformatLayoutCell;
                for (int i = 0; i < cellCount; i++)
                {
                    AddCell(new FilmingControlCell());
                }
                needToRegisterMouseEvent = true;
            }

            for (int i = 0; i < cellCount; i++)
            {
                ReplaceCell(replaceCell.Children.ElementAt(i), i);
            }
            Refresh();
            return needToRegisterMouseEvent;
        }

        public void ReplaceBy(FilmingLayoutCell replaceCell, bool needReferesh, ActionType curAction)
        {
            int cellCount = replaceCell.Rows * replaceCell.Columns;
            if (Rows != replaceCell.Rows || Columns != replaceCell.Columns)
            {
                RemoveAll();
                SetLayout(replaceCell.Rows, replaceCell.Columns);
                IsMultiformatLayoutCell = replaceCell.IsMultiformatLayoutCell;
                for (int i = 0; i < cellCount; i++)
                {
                    AddCell(new FilmingControlCell());
                }
                Refresh();
            }
            for (int i = 0; i < replaceCell.Children.Count(); i++)
            {
                var filmingControlCell = replaceCell.Children.ElementAt(i) as FilmingControlCell;
                var tempcell = this.Children.ElementAt(i) as FilmingControlCell;
                if (filmingControlCell == null || filmingControlCell.IsEmpty)
                {
                    if (tempcell != null && tempcell.Image != null && tempcell.Image.CurrentPage != null)
                    {
                        tempcell.Image.Clear();
                        tempcell.Refresh();
                    }
                    continue;
                }
                var displaydata = filmingControlCell.Image.CurrentPage;
                if (tempcell != null)
                {
                    bool flag = tempcell.IsEmpty;
                    tempcell.Image.Clear();
                    tempcell.Image.AddPage(displaydata);
                    tempcell.IsSelected = filmingControlCell.IsSelected;
                    if (flag) FilmPageUtil.SetAllActions(tempcell, curAction);
                    if (needReferesh)
                    {
                        tempcell.Refresh();
                    }
                    else
                        tempcell.Image.CurrentPage.IsDirty = true;
                }
            }
        }

        public void Clear()
        {
            if (IsEmpty()) return;

            if (!IsMultiformatLayoutCell)
            {
                var filmingControlCell = this.Children.ElementAt(0) as FilmingControlCell;
                if (filmingControlCell != null)
                {
                    filmingControlCell.Image.Clear();
                    filmingControlCell.Refresh();
                }
            }
            else
            {
                Reset();
            }
        }

        public void DeSelected()
        {
            foreach (var ctrlCell in Children.OfType<FilmingControlCell>())
            {
                ctrlCell.IsSelected = false;
            }
        }
        public void Reset()
        {
            IsMultiformatLayoutCell = false;
            RemoveAll();
            SetLayout(1, 1);
            AddCell(new FilmingControlCell());
            Refresh();
        }

        public bool IsSelected()
        {
            return Children.OfType<FilmingControlCell>().Any(c => c.IsSelected);
        }
    }
    public class FilmingLayoutCellImpl : MedViewerLayoutCellImpl
    {
        public FilmingLayoutCellImpl()
        {
            IsReUseUI = true;
        }

        protected override MedViewerLayoutCellImpl CreateLayoutCellImpl()
        {
            return new FilmingLayoutCellImpl() { Margin = new Thickness(1) };
        }

        protected override MedViewerControlCellImpl CreateControlCellImpl()
        {
            return new FilmingControlCellImpl() { Margin = new Thickness(1) };
        }

        protected override void SetScrollBarResource()
        {

        }
    }

    public class FilmingControlCellImpl : MedViewerControlCellImpl
    {
        protected override Panel[] CreateOverlayControl()
        {
            return new Panel[] {
                new OverlayGraphicsImpl(),
                new OverlayColorbarImpl(), //todo:[yy final][即时创建]
                //new OverlayRulerImpl(),   
                //new OverlayFilmingTextImpl(), 
                new OverlayFilmingF1ProcessTextImpl(), 
            };
        }

        /// <summary>
        /// 重写父类方法Refresh，调整Overlay和图像的刷新顺序。
        /// Overlay先刷新，Image后刷新
        /// </summary>
        public override void Refresh()
        {
            PreRefresh();
            RefreshOverlays();
            RefreshImage();
        }
    }

}
