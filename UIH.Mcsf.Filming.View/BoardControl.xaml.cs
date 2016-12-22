﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UIH.Mcsf.Filming.Interfaces;
using UIH.Mcsf.Filming.ViewModel;

namespace UIH.Mcsf.Filming.View
{
    /// <summary>
    ///     Interaction logic for BoardControl.xaml
    /// </summary>
    public partial class BoardControl
    {
        private DisplayMode _displayMode;
        //TODO-later: Page Management in BoardControl
        private readonly List<PageControl> _pages = new List<PageControl>();

        public BoardControl()
        {
            InitializeComponent();

            for (var i = 0; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                var pageControl = new PageControl {Margin = new Thickness(5)};
                _pages.Add(pageControl);
                MainGrid.Children.Add(pageControl);
            }
        }

        //TODO: Make BoardControl.FillPages Oneway
        //private void FillPages()
        //{
        //    var i = 0;
        //    while (i < PageModels.Count && i < _displayMode)
        //    {
        //        var pageControl = _pages[i];
        //        pageControl.Visibility = Visibility.Visible;   //TODO: PageControl.Visibility Binding to PageModel
        //        var pageModel = PageModels[i];
        //        //TODO-later: 出于性能方面的考虑，View（BoardControl）依赖了ViewModel（PageViewModel）
        //        pageControl.DataContext = new PageControlViewModel(pageModel);
        //        i++;
        //    }
        //    while (i < GlobalDefinitions.MaxDisplayMode)
        //    {
        //        var pageControl = _pages[i];
        //        pageControl.Visibility = Visibility.Hidden;
        //        i++;
        //    }
        //}

        //private void SetDisplayMode()
        //{
        //    if (_displayMode == DisplayMode) return;
        //    _displayMode = DisplayMode;
        //    SetGrid();
        //}


        private void SetGrid(int row, int col)
        {
            CompleteGrid(row, col);

            PlacePagesToGrid(row, col);
        }

        //TODO-later: PageControl Position Management
        //TODO: BoardControl. When DisplayMode changed, Re-place Grid content
        //TODO: BoardControl. When Index of First PageModel changed, Re-place Grid content
        private void PlacePagesToGrid(int row, int col)
        {
            //TODO-later: Page Size Control
            var scale = new ScaleTransform(1.0/col, 1.0/row);
            var pageIndex = 0;
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < col; j++)
                {
                    var page = _pages[pageIndex];
                    Grid.SetRow(page, i);
                    Grid.SetColumn(page, j);
                    page.LayoutTransform = scale;
                    pageIndex++;
                }
            }
        }

        private void CompleteGrid(int row, int col)
        {
            var rows = MainGrid.RowDefinitions;
            var curRow = rows.Count;
            var cols = MainGrid.ColumnDefinitions;
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
        }

        private DisplayMode DisplayMode
        {
            set
            {
                if (_displayMode.Equals(value) ) return;
                _displayMode = value;
                SetGrid(_displayMode.Col, _displayMode.Col);
            }
        }

        #region [--BoardModelProperty--]

        public BoardModel BoardModel
        {
            get { return (BoardModel)GetValue(BoardModelProperty); }
            set { SetValue(BoardModelProperty, value); }
        }


        //#region [--DisplayModeProperty--]

        //public int DisplayMode
        //{
        //    get { return (int) GetValue(DisplayModeProperty); }
        //    set { SetValue(DisplayModeProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for DisplayMode.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty DisplayModeProperty =
        //    DependencyProperty.Register("DisplayMode", typeof (int), typeof (BoardControl),
        //        new PropertyMetadata(OnDisplayModePropertyChanged));

        //private static void OnDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var boardControl = d as BoardControl;
        //    Debug.Assert(boardControl != null);

        //    var displayMode = (int) e.NewValue;
        //    Debug.Assert(displayMode >= 0 && displayMode < GlobalDefinitions.MaxDisplayMode);

        //    boardControl.SetDisplayMode();
        //}

        //#endregion [--DisplayModeProperty--]

        // TODO-Working-on : Modify BoardControl.DP to BoardModelProperty
        //#region [--PageModelsProperty--]

        //public IList<PageModel> PageModels
        //{
        //    get { return (IList<PageModel>) GetValue(PageModelsProperty); }
        //    set { SetValue(PageModelsProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for PageModels.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty PageModelsProperty =
        //    DependencyProperty.Register("PageModels", typeof (IList<PageModel>), typeof (BoardControl),
        //        new PropertyMetadata(OnPageModelsPropertyChanged));

        //private static void OnPageModelsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var boardControl = d as BoardControl;
        //    Debug.Assert(boardControl != null);

        //    boardControl.FillPages();
        //}

        //#endregion  [--PageModelsProperty--]


        // Using a DependencyProperty as the backing store for BoardModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoardModelProperty =
            DependencyProperty.Register("BoardModel", typeof(BoardModel), typeof(BoardControl), new PropertyMetadata(OnBoardModelPropertyChanged));

        private static void OnBoardModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var boardControl = d as BoardControl;
            Debug.Assert(boardControl != null);

            boardControl.RegisterBoardEvent();
            boardControl.SetPageDataContext();
        }

        #endregion [--BoardModelProperty--]

        private void SetPageDataContext()
        {
            var pageModels = BoardModel.PageModels;
            Debug.Assert(pageModels.Count == GlobalDefinitions.MaxDisplayMode);

            for (int i = 0; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                _pages[i].DataContext = new PageControlViewModel(pageModels[i]);
            }
        }

        private void RegisterBoardEvent()
        {
            BoardModel.DisplayModeChanged -= OnBoardDisplayModeChanged;
            BoardModel.DisplayModeChanged += OnBoardDisplayModeChanged;
        }

        private void OnBoardDisplayModeChanged(object sender, IntEventArgs e)
        {
            DisplayMode = new DisplayMode(e.Int);
        }
    }
}