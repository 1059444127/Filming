namespace UIH.Mcsf.Filming.Adapters
{
    /// <summary>
    ///     Interaction logic for ViewerControlAdapter.xaml
    /// </summary>
    public partial class ViewerControlAdapter
    {
        public ViewerControlAdapter()
        {
            InitializeComponent();
            ViewerControl.InitializeWithoutCommProxy(Configure.Environment.Instance.ApplicationPath);
            ViewerControl.LayoutManager.SetLayout(2,2);
        }

        // TODO-New-Feature: Layout Dependency Property

        // TODO-New-Feature: Focus

        // TODO-New-Feature: Selection

        // TODO-New-Feature: Cells Dependecy Property

        // TODO-New-Feature: ViewPort Layout Dependency Property

        // TODO-New-Feature: regularLayout & irregularLayout

        // TODO-New-Feature: MultiFormatCell
    }
}