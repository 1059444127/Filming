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

namespace UIH.Mcsf.Filming
{
    /// <summary>
    /// Interaction logic for PreFilmingProgressDialog.xaml
    /// </summary>
    public partial class PreFilmingProgressDialog : Window
    {
        public PreFilmingProgressDialog()
        {
            InitializeComponent();
        }

        public void DisplayProgress()
        {
            //double value = -1;
            //double step = 0;
            //while (true)
            //{
            //    progressBar.Dispatcher.Invoke(
            //        new Action<
            //            System.Windows.DependencyProperty, object>(progressBar.SetValue),
            //            System.Windows.Threading.DispatcherPriority.Background,
            //            ProgressBar.ValueProperty, value);
            //    step = value > progressBar.Maximum ? -progressBar.LargeChange : progressBar.LargeChange;
            //    step = value < progressBar.Minimum ? progressBar.LargeChange : -progressBar.LargeChange;
            //    value += step;
            //} 
        }
    }
}
