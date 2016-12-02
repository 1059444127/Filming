using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;
using UIH.Mcsf.Filming.Interface;
using UIH.Mcsf.MvvmLight;

namespace MEFViewer
{
    [Export(typeof (UserControl))]
    [ExportMetadata("ParentName", "LayoutGrid")]
    public partial class LayoutControl
    {
        public LayoutControl()
        {
            InitializeComponent();
            DataContext = new LayoutViewModel();
        }
    }

    public class LayoutViewModel
    {
        private void SetLayout(string layout)
        {
            Messenger.Default.Send(LayoutFactory.Instance.CreateLayout(layout));
        }

        #region [--Layout2X2Command--] 

        private ICommand _layout2X2Command;

        public ICommand Layout2X2Command
        {
            get { return _layout2X2Command = _layout2X2Command ?? new RelayCommand(SetLayout2X2); }
        }

        private void SetLayout2X2()
        {
            SetLayout("2x2");
        }

        #endregion [--Layout2X2Command--]     

        #region [--Layout3X3Command--] 

        private ICommand _layout3X3Command;

        public ICommand Layout3X3Command
        {
            get { return _layout3X3Command = _layout3X3Command ?? new RelayCommand(SetLayout3X3); }
        }

        private void SetLayout3X3()
        {
            SetLayout("3x3");
        }

        #endregion [--Layout3X3Command--]     
    }
}