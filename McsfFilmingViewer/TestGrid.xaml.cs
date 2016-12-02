using System;
using System.Collections.Generic;
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

using UIH.Mcsf.Viewer;
using System.Diagnostics;

namespace UIH.Mcsf.Filming
{
    /// <summary>
    /// Interaction logic for TestGrid.xaml
    /// </summary>
    public partial class TestGrid : Window
    {
        public TestGrid(FilmingPageControl filmPage)
        {
            InitializeComponent();

            //copy filmPage;
            string xaml = System.Windows.Markup.XamlWriter.Save(filmPage.filmingViewerControl);
            UserControl uc = System.Windows.Markup.XamlReader.Parse(xaml) as UserControl;
            mainGrid.Children.Add(uc);

            _layout = filmPage.filmingViewerControl.LayoutManager;

            //var root = _layout.RootCell;

            //GenerateAnnonationLayer(root, mainGrid);

        }

        //private int _count = 0;

        private LayoutManager _layout;




        #region     [--Generate Annonation Layer--]
        private void GenerateAnnonationLayer(MedViewerCellBase layout, Grid grid)
        {
            if (layout is MedViewerControlCell) // leaf cell
            {
                var cell = layout as MedViewerControlCell;
                var btn = new Button();
                //btn.Content = _count++;
                //grid.Children.Add(btn);
                FillAnnotationToGrid(cell, grid);
            }
            else if (layout is MedViewerLayoutCell) //layout cell
            {
                var root = layout as MedViewerLayoutCell;

                //Generate grid
                GenerateGrid(root, grid);

                //fill sub-grid to the grid
                FillSubAnnonationGrid(root, grid);

            }
        }

        private void FillAnnotationToGrid(MedViewerControlCell cell, Grid grid)
        {

            //////////////////////////////////////////////////////////////////////////
            //Assert

            Debug.Assert(cell != null);
            Debug.Assert(grid != null);

            var image = cell.Image;
            if (image == null) return;

            var page = cell.Image.CurrentPage;
            if (page == null) return;

            var overlayText = (OverlayText)page.GetOverlay(OverlayType.Text);
            if (overlayText == null) return;
            var graphicImageText = overlayText.Graphics[0] as GraphicImageText;
            if (graphicImageText == null) return;

            //////////////////////////////////////////////////////////////////////////
            //new a 3x3 grid 1:8:1, and add to the cell grid
            var r1 = new RowDefinition(); r1.Height = new GridLength(1, GridUnitType.Star);
            var r2 = new RowDefinition(); r2.Height = new GridLength(1, GridUnitType.Star);
            var r3 = new RowDefinition(); r3.Height = new GridLength(1, GridUnitType.Star);
            grid.RowDefinitions.Add(r1);
            grid.RowDefinitions.Add(r2);
            grid.RowDefinitions.Add(r3);

            var c1 = new ColumnDefinition(); c1.Width = new GridLength(1, GridUnitType.Star);
            var c2 = new ColumnDefinition(); c2.Width = new GridLength(1, GridUnitType.Star);
            var c3 = new ColumnDefinition(); c3.Width = new GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(c1);
            grid.ColumnDefinitions.Add(c2);
            grid.ColumnDefinitions.Add(c3);

            //////////////////////////////////////////////////////////////////////////
            //Traverse four annotation text panel, and fetch texts to fill into the 
            for (ImgTxtPosEnum pos = 0; pos < ImgTxtPosEnum.Max; pos++)
            {
                var textPanel = graphicImageText.GetTextItemPanel(pos);
                var textGroups = textPanel.Children;
                if (textGroups.Count == 0) continue;

                //new a stackpanel at current pos
                var textStack = new StackPanel();
                textStack.Orientation = Orientation.Vertical;
                textStack.Margin = new Thickness(2, 2, 2, 2);
                var viewBox = new Viewbox();
                viewBox.Child = textStack;

                var point = GetAnnotationPos(pos, viewBox);
                Grid.SetColumn(viewBox, (int)point.Y);
                Grid.SetRow(viewBox, (int)point.X);
                grid.Children.Add(viewBox);


                foreach (var item in textGroups)
                {
                    var textGroup = item as GraphicImageText.TextItemGroup;
                    string textItem = textGroup.TextItemBody.Text;

                    //add a textBlock with Text = textItem into the stackPanel
                    var textBlock = new TextBlock()
                    {
                        Text = textItem,
                        FontSize = 5,
                        FontWeight = FontWeights.Bold,
                        //Margin = new Thickness(10,0,10,0),
                        //HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        //VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        //Opacity = 0.5,
                        //Foreground = Brushes.White,
                        //Effect = new DropShadowEffect()
                        //{
                        //    Color = Colors.Black,
                        //    ShadowDepth = 1
                        //}
                    };
                    textBlock.HorizontalAlignment = textStack.HorizontalAlignment;
                    textBlock.VerticalAlignment = textStack.VerticalAlignment;


                    textStack.Children.Add(textBlock);
                }

            }
            //var textControl = graphicImageText.Control;

            //grid.Children.Add(null);
        }

        Point GetAnnotationPos(ImgTxtPosEnum pos, FrameworkElement fe)
        {
            Debug.Assert(pos >= 0 && pos < ImgTxtPosEnum.Max);
            switch (pos)
            {
                case ImgTxtPosEnum.TopLeft:
                    fe.HorizontalAlignment = HorizontalAlignment.Left;
                    fe.VerticalAlignment = VerticalAlignment.Top;
                    return new Point(0, 0);
                case ImgTxtPosEnum.TopRight:
                    fe.HorizontalAlignment = HorizontalAlignment.Right;
                    fe.VerticalAlignment = VerticalAlignment.Top;
                    return new Point(0, 2);
                case ImgTxtPosEnum.BottomRight:
                    fe.HorizontalAlignment = HorizontalAlignment.Right;
                    fe.VerticalAlignment = VerticalAlignment.Bottom;
                    return new Point(2, 2);
                case ImgTxtPosEnum.BottomLeft:
                    fe.HorizontalAlignment = HorizontalAlignment.Left;
                    fe.VerticalAlignment = VerticalAlignment.Bottom;
                    return new Point(2, 0);
                case ImgTxtPosEnum.Top:
                    fe.HorizontalAlignment = HorizontalAlignment.Center;
                    fe.VerticalAlignment = VerticalAlignment.Top;
                    return new Point(0, 1);
                case ImgTxtPosEnum.Right:
                    fe.HorizontalAlignment = HorizontalAlignment.Right;
                    fe.VerticalAlignment = VerticalAlignment.Center;
                    return new Point(1, 2);
                case ImgTxtPosEnum.Bottom:
                    fe.HorizontalAlignment = HorizontalAlignment.Center;
                    fe.VerticalAlignment = VerticalAlignment.Bottom;
                    return new Point(2, 1);
                case ImgTxtPosEnum.Left:
                    fe.HorizontalAlignment = HorizontalAlignment.Left;
                    fe.VerticalAlignment = VerticalAlignment.Center;
                    return new Point(1, 0);
                default:
                    throw new Exception("not supported position");
            }
        }

        private void FillSubAnnonationGrid(MedViewerLayoutCell root, Grid grid)
        {
            Debug.Assert(root != null);
            Debug.Assert(grid != null);

            int rows = root.Rows;
            int columns = root.Columns;
            var cellIter = root.Children.GetEnumerator();
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    if (!cellIter.MoveNext())
                    {
                        return;
                    }
                    var childGrid = new Grid();
                    Grid.SetRow(childGrid, r);
                    Grid.SetColumn(childGrid, c);
                    grid.Children.Add(childGrid);

                    GenerateAnnonationLayer(cellIter.Current, childGrid);
                }
            }
        }
        private void GenerateGrid(MedViewerLayoutCell root, Grid grid)
        {
            Debug.Assert(root != null);
            Debug.Assert(grid != null);


            int rows = root.Rows;
            int columns = root.Columns;

            for (int i = 0; i < rows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            for (int j = 0; j < columns; j++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
        #endregion  [--Generate Annonation Layer--]

    }
}
