using UIH.Mcsf.Filming.DataModel;

namespace UIH.Mcsf.Filming.DemoView
{
    /// <summary>
    ///     Interaction logic for CardControl.xaml
    /// </summary>
    public partial class CardControl
    {
        public CardControl()
        {
            InitializeComponent();
        }

        public void SetBoardProvider(IBoardProvider boardProvider)
        {
            BoardControl.SetBoardChange(boardProvider.BoardChange);
        }
    }
}