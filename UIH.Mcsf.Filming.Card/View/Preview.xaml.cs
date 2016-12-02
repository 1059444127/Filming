using System.Windows;

namespace UIH.Mcsf.Filming.Card.View
{
    /// <summary>
    /// Interaction logic for Preview.xaml
    /// </summary>
    public partial class Preview
    {
        public Preview()
        {
            InitializeComponent();
        }

        public void Add(UIElement uiElement)
        {
            filmRegion.Children.Add(uiElement);
        }
    }
}
