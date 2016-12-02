using System.Collections.Generic;
using System.Windows;
using UIH.Mcsf.Filming.Model;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Utility
{
    public class McsfFilmViewport
    {
        public McsfFilmViewport(MedViewerLayoutCell rootLayoutCell, bool isInRegularLayout = false)
        {
            RootLayoutCell = rootLayoutCell;

            _isInRegularLayout = isInRegularLayout;

            _cellLayout = new FilmLayout(rootLayoutCell.Rows, rootLayoutCell.Columns);
        }

        public MedViewerLayoutCell RootLayoutCell { get; private set; } //just for dependency injection

        public FilmLayout IrregularCellLayout
        {
            set
            {

                _cellLayout = value;
                RootLayoutCell.Rows = _cellLayout.LayoutRowsSize;
                RootLayoutCell.Columns = _cellLayout.LayoutColumnsSize;
                var iMaxLayoutCellNum = RootLayoutCell.DisplayCapacity;
                if (RootLayoutCell.Count <= iMaxLayoutCellNum)
                {
                    for (var i = RootLayoutCell.Count; i < iMaxLayoutCellNum; i++)
                    {
                        RootLayoutCell.AddCell(new FilmingControlCell());
                    }
                }
                else
                {
                    var iOldLayoutCellCount = RootLayoutCell.Count;
                    for (var i = iOldLayoutCellCount; i >= iMaxLayoutCellNum; i--)
                    {
                        RootLayoutCell.RemoveCell(i);
                    }
                }

            }
        }

        public FilmLayout CellLayout
        {
            get
            {
                return _cellLayout;
            }

            set
            {
                _cellLayout = value;
                RootLayoutCell.Rows = _cellLayout.LayoutRowsSize;
                RootLayoutCell.Columns = _cellLayout.LayoutColumnsSize;
                for (var i = RootLayoutCell.Count; i < RootLayoutCell.DisplayCapacity; i++)
                {
                    RootLayoutCell.AddCell(new FilmingControlCell());
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                try
                {
                    if (_isSelected != value)
                    {
                        _isSelected = value;

                        var medViewerLayoutCellImpl = RootLayoutCell.Control as MedViewerLayoutCellImpl;
                        if (medViewerLayoutCellImpl != null)
                            medViewerLayoutCellImpl.Border.Visibility = _isSelected && !_isInRegularLayout
                                                                                                        ? Visibility.Visible
                                                                                                        : Visibility.
                                                                                                              Collapsed;
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.LogFuncException(ex.Message+ex.StackTrace);
                }
            }
        }

        public int IndexInFilm { get; set; }

        //public int MaxImagesCount
        //{
        //    get
        //    {
        //        int maxImagesCount = 0;
        //        if (CellLayout.LayoutType == LayoutTypeEnum.StandardLayout)
        //        {
        //            maxImagesCount = CellLayout.LayoutColumnsSize * CellLayout.LayoutRowsSize;
        //        }
        //        else if (CellLayout.LayoutType == LayoutTypeEnum.DefinedLayout)
        //        {
        //            //add 1 for root cell reduce.
        //            maxImagesCount = FilmPageUtil.GetChildNodeCount(RootLayoutCell) + 1;
        //            //MessageBox.Show(_maxImagesCount.ToString());
        //        }
        //        return maxImagesCount;
        //    }
        //}

        public void SelectAllCells(bool isSelected)
        {
            FilmPageUtil.SelectCellsOfLayoutCell(RootLayoutCell, isSelected);
        }

        public List<MedViewerControlCell> GetCells()
        {
            return FilmPageUtil.GetCellsOfLayoutCell(RootLayoutCell);
        }

        public IEnumerable<MedViewerControlCell> GetSelectedCells()
        {
            return FilmPageUtil.GetSelectedCellsOfLayoutCell(RootLayoutCell);
        }
        private FilmLayout _cellLayout;
        private bool _isSelected;
        private readonly bool _isInRegularLayout;

    }
}
