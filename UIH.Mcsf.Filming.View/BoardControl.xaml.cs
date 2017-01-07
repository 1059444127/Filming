﻿using System;
using System.Collections.Generic;
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

        private void SetGrid(int row, int col)
        {
            CompleteGrid(row, col);
            //TODO-later: Page Size Control
            PlacePagesToGrid(row, col);
        }

        //TODO-later: PageControl Position Management
        //TODO: BoardControl. When DisplayMode changed, Re-place Grid content
        //TODO: BoardControl. When Index of First BoardCell changed, Re-place Grid content
        //TODO-bug: 最后一张胶片的PageBreak无法显示
        // TODO-working-on: BoardControl. Replace BoardControl.PlacePagesToGrid  by BoardCell.Row/Col
        private void PlacePagesToGrid(int row, int col)
        {
            var scale = new ScaleTransform(1,1);
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
                SetGrid(_displayMode.Row, _displayMode.Col);
            }
        }

        #region [--BoardComponentProperty--]

        public IBoardModel BoardComponent
        {
            get { return (IBoardModel)GetValue(BoardComponentProperty); }
            set { SetValue(BoardComponentProperty, value); }
        }



        // Using a DependencyProperty as the backing store for BoardComponent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoardComponentProperty =
            DependencyProperty.Register("BoardComponent", typeof(IBoardModel), typeof(BoardControl), new PropertyMetadata(OnBoardComponentPropertyChanged));

        private static void OnBoardComponentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var boardControl = d as BoardControl;
            Debug.Assert(boardControl != null);

            boardControl.RegisterBoardEvent();
            boardControl.SetPageDataContext();
        }

        #endregion [--BoardComponentProperty--]

        private void SetPageDataContext()
        {
            var boardCells = BoardComponent.BoardCells;
            Debug.Assert(boardCells.Count >= GlobalDefinitions.MaxDisplayMode);

            for (int i = 0; i < GlobalDefinitions.MaxDisplayMode; i++)
            {
                // TODO-Later: 解除Project View 对 ViewModel的依赖
                _pages[i].DataContext = new PageControlViewModel(boardCells[i]);
            }
        }

        private void RegisterBoardEvent()
        {
            BoardComponent.CellCountChanged -= OnBoardCellCountChanged;
            BoardComponent.CellCountChanged += OnBoardCellCountChanged;
        }

        private void OnBoardCellCountChanged(object sender, EventArgs e)
        {
            DisplayMode = new DisplayMode(BoardComponent.CellCount);
        }
    }
}