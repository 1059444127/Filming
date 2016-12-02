using System.Windows;

namespace UIH.Mcsf.Filming.Views
{
    /// <summary>
    /// Interaction logic for MaskWindow.xaml
    /// </summary>
    public partial class MaskWindow
    {
        public MaskWindow()
        {
            InitializeComponent();

            //this.ShowInTaskbar = false;

            Opacity = 0.5;

            //var bc = new BrushConverter();
            //_maskBorder.Background = (Brush)bc.ConvertFrom("#00EEEEEE");
            //_maskBorder.Background = new SolidColorBrush(Colors.Black);
            //_maskBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
            //_maskBorder.VerticalAlignment = VerticalAlignment.Stretch;
            //_maskBorder.Visibility = Visibility.Visible;

            //var filmPreogressControl = new PreFilmingProgressControl();
            //var tmpThickNess = new Thickness(100, 100, 100, 100);
            ////if (FilmingControlPanelLocation == "left")
            ////{
            ////    tmpThickNess.Left = controlPanelGrid.Width + filmCardScrollBar.Width * 0.5;&
            ////    tmpThickNess.Right = filmCardScrollBar.Width;
            ////}
            ////else
            ////{
            ////    tmpThickNess.Right = controlPanelGrid.Width + filmCardScrollBar.Width * 1.5;
            ////}
            //filmPreogressControl.Margin = tmpThickNess;
            //_maskBorder.Child = filmPreogressControl;

        }

        public void SetWindowRect(Rect rect)
        {
            Left = rect.Left;
            Top = rect.Top;
            Width = rect.Width;
            Height = rect.Height;
        }

        public void SetRingWith(Thickness thickness)
        {
            ring.Margin = thickness;
        }
    }
}
