using System.ComponentModel.Composition;
using System.Windows.Controls;
using UIH.Mcsf.Filming.Configure;
using UIH.Mcsf.Filming.Interface;
using UIH.Mcsf.MvvmLight;
using UIH.Mcsf.Viewer;

namespace MEFViewer
{
    [Export(typeof (UserControl))]
    [ExportMetadata("ParentName", "ViewerGrid")]
    public partial class ViewerControl
    {
        public ViewerControl()
        {
            InitializeComponent();
            ViewControl.InitializeWithoutCommProxy(Environment.Instance.FilmingUserConfigPath);
            SetLayout(LayoutFactory.Instance.CreateLayout(2, 2));
            

            Messenger.Default.Register<LayoutBase>(this, SetLayout);
            Messenger.Default.Register<ActionType>(this, SetActionType);
        }

        private void SetActionType(ActionType action)
        {
            ViewControl.SetAction(action);
        }

        private void SetLayout(LayoutBase layout)
        {
            ViewControl.LayoutManager.SetLayout(layout.Rows, layout.Columns);
        }
    }
}