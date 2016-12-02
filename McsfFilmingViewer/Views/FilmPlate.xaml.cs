using System;
using System.Windows;
using System.Windows.Controls;

namespace UIH.Mcsf.Filming.Views
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

        public void Display(FrameworkElement element)
        {
            try
            {
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag);
                if (element == null)
                {
                    Logger.Instance.LogDevInfo("Display cover(no page)");
                    element = cover;
                }
                var children = plate.Children;
                element.Visibility = Visibility.Visible;

                if (!children.Contains(element))
                {
                    var parent = element.Parent as Grid;
                    if (parent != null)
                    {
                        parent.Children.Remove(element);
                    }
                    element.HorizontalAlignment = HorizontalAlignment.Center;
                    element.VerticalAlignment = VerticalAlignment.Center;
                    children.Add(element);

                    this.UpdateLayout();
                }
            

                foreach (FrameworkElement child in children)
                {
                    if (child == element)
                    {
                        continue;
                    }
                    child.Visibility = Visibility.Collapsed;
                }
                Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceExitFlag);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogDevError(ex.Message);
            }
        }

        public void Clear()
        {
            var children = plate.Children;
            children.Clear();
            children.Add(cover);
        }

        public void Remove(FrameworkElement element)
        {
            var children = plate.Children;
            if (children.Contains(element))
            {
                children.Remove(element);
                element = null;
            }

        }
    }
}
