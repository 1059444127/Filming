using System.Windows;

namespace UIH.Mcsf.Filming.ControlTests.ViewModel
{
    class FooGridControlViewModel : TestViewModelBase
    {
        public FooGridControlViewModel()
        {
            FooControlViewModel = new FooControlViewModel();
        }

        public FooControlViewModel FooControlViewModel { get; set; }

        #region Overrides of TestViewModelBase

        protected override void UpdateViewModel()
        {
            MessageBox.Show("Middel wheel clicked in FooGridControl");

            MessageBox.Show("Move SubControl to GridCell (1,1)");
            FooControlViewModel.Row = 1;
            FooControlViewModel.Col = 1;
        }

        #endregion
    }
}
