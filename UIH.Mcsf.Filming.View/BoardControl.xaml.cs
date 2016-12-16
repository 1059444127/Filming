using System.Windows;

namespace UIH.Mcsf.Filming.View
{
    /// <summary>
    ///     Interaction logic for BoardControl.xaml
    /// </summary>
    public partial class BoardControl
    {
        public BoardControl()
        {
            InitializeComponent();
        }

        //TODO-working-on: BoardControl.Dependency Property : DisplayMode


        public int DisplayMode
        {
            get { return (int)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(int), typeof(BoardControl), new PropertyMetadata(1));

        


        //TODO: BoardControl.Dependency Property : Pages

        #region Overrides of FrameworkElement

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == DisplayModeProperty)
            {
                SetDisplayMode(DisplayMode);
            }
        }

        #endregion

        private void SetDisplayMode(int displayMode)
        {
            
        }
    }
}