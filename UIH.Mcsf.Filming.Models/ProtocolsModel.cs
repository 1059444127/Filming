using System.Collections.ObjectModel;
using System.Linq;
using UIH.Mcsf.Filming.Configure;
using UIH.Mcsf.Filming.Interface;

namespace UIH.Mcsf.Filming.Models
{
    public class ProtocolsModel
    {
        public ProtocolsModel()
        {
            var protocolConfigure = Environment.Instance.GetProtocolsConfig();
            _protocols = new ObservableCollection<Protocol>(protocolConfigure.Protocols);
        }

        private ObservableCollection<Protocol> _protocols;

        public ObservableCollection<Protocol> GetProtocols(string filter = "")
        {
            //return new ObservableCollection<Protocol>(_protocols.Where(p => p.Name.ToUpper().Contains(filter.Trim().ToUpper())));
            return new ObservableCollection<Protocol>(_protocols.Where(p => p.Name.ToUpper().IndexOf(filter.Trim().ToUpper(), System.StringComparison.Ordinal) == 0));
        }

	    public void SaveProtocols()
	    {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[ProtocolsModel.SaveProtocols]");
            var protocolConfigure = Environment.Instance.GetProtocolsConfig();
            protocolConfigure.WriteBack(_protocols);
	    }
    }
}
