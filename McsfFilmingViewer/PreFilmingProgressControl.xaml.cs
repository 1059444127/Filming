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
using System.Windows.Media.Animation;

namespace UIH.Mcsf.Filming
{
    /// <summary>
    /// Interaction logic for PreFilmingProgressControl.xaml
    /// </summary>
    public partial class PreFilmingProgressControl : UserControl
    {
        public PreFilmingProgressControl()
        {
            InitializeComponent();

            this.Initialized += new EventHandler(PreFilmingProgressControl_Initialized);
        }

        void PreFilmingProgressControl_Initialized(object sender, EventArgs e)
        {
            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                                   typeof(Timeline),
                                       new FrameworkPropertyMetadata { DefaultValue = 20 });
        }
    }
}
