using UIH.Mcsf.Filming.DataModel;
using UIH.Mcsf.Filming.DemoView;
using UIH.Mcsf.Filming.DemoViewModel;

namespace UIH.Mcsf.Filming.TestApplication
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent(); //controller

            InitializeCard();
        }

        private void InitializeCard()
        {
            // Create view viewModel & model
            var cardControl = new CardControl(); //view
            var card = new Card(); //model
            cardControl.DataContext = new CardViewModel(card); //viewModel & model

            // MVC
            cardControl.SetBoardProvider(card); //view & model

            // Add control into Window
            LayoutGrid.Children.Add(cardControl);

            card.NewPage();
        }
    }
}