using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace UIH.Mcsf.Filming
{
    /// <summary>
    /// Interaction logic for BurnFilmWindow.xaml
    /// </summary>
    public partial class BurnFilmWindow : Window
    {

        private FilmingPageControl _filmPage;

        public BurnFilmWindow(FilmingPageControl filmPage)
        {
            InitializeComponent();
            _filmPage = filmPage;
            mainGrid.Children.Add(_filmPage);

            this.Closing += OnClosing;

            WindowStartupLocation = WindowStartupLocation.Manual;
            Top = -1500;
            Left = -1500;
            this.ShowInTaskbar = false;

        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            mainGrid.Children.Remove(_filmPage);
        }
    }
}
