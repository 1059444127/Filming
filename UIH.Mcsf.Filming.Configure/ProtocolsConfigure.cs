using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using UIH.Mcsf.Core;
using UIH.Mcsf.Filming.Interface;
using UIH.Mcsf.Viewer;

namespace UIH.Mcsf.Filming.Configure
{
    public class ProtocolsConfigure
    {

        private readonly string _fileName;
        private ProtocolsConfigureInfo _protocolsConfigureInfo;

        public IList<Protocol> Protocols { get; private set; }
        public ProtocolsConfigure(string fileName)
        {
            _fileName = fileName;
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[ProtocolsConfigure.ProtocolsConfigure]" + "[fileName]" );
            Protocols = new List<Protocol>();
            _protocolsConfigureInfo = ConfigFileHelper.LoadConfigObject<ProtocolsConfigureInfo>(fileName);
            if(_protocolsConfigureInfo == null) return;
            foreach (var layoutItem in _protocolsConfigureInfo.ProtocolBindingLayouts)
            {
                Protocols.Add(ProtocolFactory.Instance.CreateProtocol(layoutItem.Name, layoutItem.Layout));
            }
            Protocols = (from protocol in Protocols
                         orderby protocol.Name.ToUpper()
                         select protocol).ToList();
        }

        public void WriteBack(Collection<Protocol> protocols)
        {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[ProtocolsConfigure.WriteBack]" +
                                       "[protocols]");
            foreach (var protocol in protocols)
            {
                Logger.Instance.LogDevInfo(protocol.ToString());
                var pro = _protocolsConfigureInfo.ProtocolBindingLayouts.First(l => l.Name == protocol.Name);
                if(pro != null)
                    pro.Layout = protocol.Layout.ToString();
            }
            ConfigFileHelper.SaveConfigObject(_protocolsConfigureInfo,_fileName);
        }

        public void ReloadProtocols()
        {
            Protocols.Clear();
            _protocolsConfigureInfo = ConfigFileHelper.LoadConfigObject<ProtocolsConfigureInfo>(_fileName);
            foreach (var layoutItem in _protocolsConfigureInfo.ProtocolBindingLayouts)
            {
                Protocols.Add(ProtocolFactory.Instance.CreateProtocol(layoutItem.Name, layoutItem.Layout));
            }
            Protocols = (from protocol in Protocols
                         orderby protocol.Name.ToUpper()
                         select protocol).ToList();
        }
    }


    [XmlRoot("Root")]
    public class ProtocolsConfigureInfo
    {
        [XmlArrayItem("Item")]
        public Collection<ProtocolBindingLayoutItem> ProtocolBindingLayouts;
    }

    public class ProtocolBindingLayoutItem
    {
        [XmlAttribute]
        public string Layout;
        [XmlAttribute]
        public string Name;
    }
}
