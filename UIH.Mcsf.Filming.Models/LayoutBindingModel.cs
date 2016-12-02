using System;
using UIH.Mcsf.Filming.Interface;

namespace UIH.Mcsf.Filming.Models
{
    public class LayoutBindingModel
    {
        private LayoutBase _layout;
        public event EventHandler BindingSessionEnded = delegate { };

        public event EventHandler<LayoutEventArgs> LayoutChanged = delegate { };

        public void EndBindingSession()
        {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[LayoutBindingModel.EndBindingSession]" + "[]" );
            BindingSessionEnded(this, new EventArgs());
        }

        public LayoutBase Layout
        {
            get { return _layout; }
            set
            {
                _layout = value;
                LayoutChanged(this, new LayoutEventArgs(value));
            }
        }
    }

}
