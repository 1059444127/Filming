using System;
using System.Windows;

namespace UIH.Mcsf.Filming.Card.View
{
    /// <summary>
    /// Interaction logic for Card.xaml
    /// </summary>
    public partial class Card
    {
        public Card()
        {
            InitializeComponent();
        }

        public void SetControlPanelToLeft()
        {
            throw new Exception("No Implemention");
        }

        public void Add(UIElement uiElement)
        {
            preview.Add(uiElement);
        }
    }
}
