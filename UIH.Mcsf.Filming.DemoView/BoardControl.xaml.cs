using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UIH.Mcsf.Filming.DataModel;
using UIH.Mcsf.Filming.Widgets;
using UIH.Mcsf.Filming.Wrapper;

namespace UIH.Mcsf.Filming.DemoView
{
    /// <summary>
    ///     Interaction logic for BoardControl.xaml
    /// </summary>
    [CallTrace(true)]
    public partial class BoardControl
    {
        private int _visiblePageCount;
        private readonly IList<PageControl> _pages;

        public BoardControl()
        {
            InitializeComponent();

            CreateBoard();
            _pages = _grid.Children.OfType<PageControl>().ToList();
        }

        public void SetBoardChange(IBoardChange boardChange)
        {
            boardChange.DisplayModeChanged += BoardChangeOnDisplayModeChanged;
            boardChange.BoardChanged += BoardChangeOnBoardChanged;
        }

        private void CreateBoard()
        {
            for (var i = 0; i < GlobalDefinition.MaxDisplayMode; i++)
                _grid.Children.Add(new PageControl());
            _visiblePageCount = 1;
            DisplayMode = 1;
        }

        #region [--Event Handler--]

        private void BoardChangeOnBoardChanged(object sender, BoardEventArgs boardEventArgs)
        {
            var pages = boardEventArgs.Pages.ToList();
            _visiblePageCount = pages.Count;
            var pageIndex = 0;
            while (pageIndex < _visiblePageCount) //visible page control
            {
                var pageControl = _pages[pageIndex];
                var page = pages[pageIndex];
                pageControl.Visibility = Visibility.Visible;
                pageControl.PageModel = page;
                pageIndex++;
            }
            while (pageIndex < _pages.Count) //collapsed page control
            {
                var pageControl = _pages[pageIndex];
                pageControl.Visibility = Visibility.Collapsed;
                pageIndex++;
            }
        }

        private void BoardChangeOnDisplayModeChanged(object sender, IntEventArgs intEventArgs)
        {
            DisplayMode = intEventArgs.Int;
        }

        #endregion [--Event Handler--]

        #region [--Page Management--]

        private int _displayMode;

        private int DisplayMode
        {
            set
            {
                if (_displayMode == value) return;
                _displayMode = value;
                switch (value)
                {
                    case 1:
                        SetGrid(1, 1);
                        break;
                    case 2:
                        SetGrid(1, 2);
                        break;
                    case 3:
                        SetGrid(1, 3);
                        break;
                    case 4:
                        SetGrid(2, 2);
                        break;
                    case 6:
                        SetGrid(2, 3);
                        break;
                    case 8:
                        SetGrid(2, 4);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void SetGrid(int row, int col)
        {
            #region Add Or Remove Row and Col

            var rows = _grid.RowDefinitions;
            var curRow = rows.Count;
            var cols = _grid.ColumnDefinitions;
            var curCol = cols.Count;

            var rowDelta = row - curRow;
            var colDelta = col - curCol;

            for (var i = 0; i < rowDelta; i++)
            {
                rows.Add(new RowDefinition());
            }
            for (var i = 0; i < colDelta; i++)
            {
                cols.Add(new ColumnDefinition());
            }
            if (rowDelta < 0) rows.RemoveRange(row, -rowDelta);
            if (colDelta < 0) cols.RemoveRange(col, -colDelta);

            #endregion Add Or Remove Row and Col

            #region Refresh Element Location

            var scale = new ScaleTransform(1.0/col, 1.0/row);
            var elementIndex = 0;
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < col; j++, elementIndex++)
                {
                    var uiElement = _grid.Children[elementIndex];
                    Grid.SetRow(uiElement, i);
                    Grid.SetColumn(uiElement, j);
                    var frameworkElement = uiElement as FrameworkElement;
                    frameworkElement.Visibility = elementIndex >= _visiblePageCount
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                    frameworkElement.LayoutTransform = scale;
                }
            }

            while (elementIndex < GlobalDefinition.MaxDisplayMode)
            {
                var uiElement = _grid.Children[elementIndex];
                uiElement.Visibility = Visibility.Collapsed;
                elementIndex++;
            }

            #endregion Refresh Element Location
        }

        #endregion [--Page Management--]
    }
}