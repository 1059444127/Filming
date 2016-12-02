using System;
using System.Linq;
using UIH.Mcsf.Filming.Interface;

namespace UIH.Mcsf.Filming.Utility
{
    //职责：通过私有tag，提供当前的布局设定，
    public class ProtocolDispatcher
    {
        private string _currentLayoutName;
        private string _currentProtocolName;

        public string CurrentLayoutName
        {
            get { return _currentLayoutName; }
        }
        //set when Selected film page changed
        public void SetLayout(string layoutName)
        {
            if(_currentLayoutName == layoutName) return;
            _currentLayoutName = layoutName;
            Logger.Instance.LogDevInfo("[CurrentLayout]"+_currentLayoutName);
        }

        //set when image added
        public void UpdateProtocol()
        {
            Configure.Environment.Instance.ReloadProtocolsConfig();
        }

        public void SetProtocol(string protocolName)
        {
            //if (0 == string.Compare(protocolName, _currentProtocolName, StringComparison.OrdinalIgnoreCase)) return;
            _currentProtocolName = protocolName;
            if (string.IsNullOrWhiteSpace(protocolName)) return;

            var protocolConfigure = Configure.Environment.Instance.GetProtocolsConfig();
            var protocols = protocolConfigure.Protocols;
            var protocol = protocols.FirstOrDefault(p => 0 == string.Compare(p.Name, _currentProtocolName, StringComparison.OrdinalIgnoreCase));
            if (protocol == null)
            {
                Logger.Instance.LogDevWarning("[Protocol]" + protocolName + "[Not Configured]");
                return;
            }

            var layout = protocol.Layout;
            var layoutName = layout.ToString();
            if (_currentLayoutName == layoutName) return;
            SetLayout(layoutName);
            if (layout is DefaultLayout) return;

            var filmLayout = new FilmLayout(layout.Rows, layout.Columns);
            Logger.Instance.LogDevInfo("[Protocol Change to]" + protocolName + "[Layout]" + layout);
            LayoutChanged(this, new ProtocolEventArgs(filmLayout));
        }

        public event EventHandler<ProtocolEventArgs> LayoutChanged = delegate { };

    }

    public class ProtocolEventArgs : EventArgs
    {
        public FilmLayout Layout { get; private set; }

        public ProtocolEventArgs(FilmLayout layout)
        {
            Layout = layout;
        }
    }
}
