using System.Windows;

namespace UIH.Mcsf.Filming.Card.View
{
    /// <summary>
    /// Interaction logic for FilmPlate.xaml
    /// </summary>
    public partial class FilmPlate
    {
        public FilmPlate()
        {
            InitializeComponent();
        }

        public void Display(UIElement element)
        {
            if (element == null) element = cover;
            var children = plate.Children;
            if (!children.Contains(element)) children.Add(element);
            element.Visibility = Visibility.Visible;

            foreach (UIElement child in children)
            {
                if (child == element) continue;
                child.Visibility = Visibility.Hidden;
            }
        }

        public void Clear()
        {
            var children = plate.Children;
            children.Clear();
            children.Add(cover);
        }
    }
}
