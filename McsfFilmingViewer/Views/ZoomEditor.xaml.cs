using System.Windows;
using System.Windows.Input;

namespace UIH.Mcsf.Filming.Views
{
    /// <summary>
    /// Interaction logic for ZoomEditor.xaml
    /// </summary>
    public partial class ZoomEditor
    {
        public ZoomEditor()
        {
            InitializeComponent();
        }

// ReSharper disable InconsistentNaming
        private void scaleTextBox_GotFocus(object sender, RoutedEventArgs e)
// ReSharper restore InconsistentNaming
        {
            scaleTextBox.SelectAll();
        }


        //文本输入变化的事件
        public delegate void Handler();
        public event Handler ScaleFactorInput = delegate { };

// ReSharper disable InconsistentNaming
        private void scaleTextBox_KeyDown(object sender, KeyEventArgs e)
// ReSharper restore InconsistentNaming
        {
            if (e.Key == Key.Enter)
                ScaleFactorInput();
        }
    }
}
