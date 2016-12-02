using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MEFViewer
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [ImportMany(typeof (UserControl))] private IEnumerable<Lazy<UserControl, IUserControlMetaData>> _controls;

        public MainWindow()
        {
            InitializeComponent();

            ComposePlugins();

            InsertUserControls();
        }

        private void ComposePlugins()
        {
            var catalog = new AssemblyCatalog(typeof(MainWindow).Assembly);
            var container = new CompositionContainer(catalog);

            container.ComposeParts(this);

        }

        private void InsertUserControls()
        {
            
            foreach (var controlData in _controls)
            {
                var grid = GetGrid(MainGrid, controlData.Metadata.ParentName);
                Debug.Assert(grid != null);
                grid.Children.Add(controlData.Value);
            }



        }


        private Grid GetGrid(DependencyObject dependencyObject, string name)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);
                var grid = child as Grid;
                if (grid != null && grid.Name == name) return grid;

                var grandChild = GetGrid(child, name);
                if (grandChild != null) return grandChild;
            }
            return null;
        }

        public interface IUserControlMetaData
        {
            string ParentName { get; }
        }
    }
}