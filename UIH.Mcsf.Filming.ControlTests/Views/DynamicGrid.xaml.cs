using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.ControlTests.Views
{
    /// <summary>
    /// Interaction logic for DynamicGrid.xaml
    /// </summary>
    public partial class DynamicGrid
    {
        public DynamicGrid()
        {
            InitializeComponent();
        }



        public int CellCount
        {
            get { return (int)GetValue(CellCountProperty); }
            set { SetValue(CellCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellCountProperty =
            DependencyProperty.Register("CellCount", typeof(int), typeof(DynamicGrid), new PropertyMetadata(1, OnCellCountChanged));

        private static void OnCellCountChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var dynamicGrid = dependencyObject as DynamicGrid;
            Debug.Assert(dynamicGrid != null);
            dynamicGrid.SetGrid();
        }

        private void SetGrid()
        {
            var displayMode = new DisplayMode(CellCount);
            SetGrid(displayMode.Row, displayMode.Col);
        }

        private void SetGrid(int row, int col)
        {
            ComplementGrid(row, col);
            PlaceGridCell(row, col);
        }
        
        // TODO: Move calculate logic From DynamicGrid.PlaceGridCell to Project Utilities
        private void PlaceGridCell(int rows, int cols)
        {
            var childrenCount = Grid.Children.Count;
            for (int i = 0; i <childrenCount; i++)
            {
                var pos = i%CellCount;
                var col = pos/cols;
                var row = pos%cols;
                var child = Grid.Children[i];
                Grid.SetRow(child, row);
                Grid.SetColumn(child, col);
            }
        }

        private void ComplementGrid(int row, int col)
        {
            var rows = Grid.RowDefinitions;
            var curRow = rows.Count;
            var cols = Grid.ColumnDefinitions;
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
    }
}
