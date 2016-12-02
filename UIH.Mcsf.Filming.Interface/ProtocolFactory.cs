using System;

namespace UIH.Mcsf.Filming.Interface
{
    public class ProtocolFactory
    {
        #region [--Singleton--]

        private static volatile ProtocolFactory _instance;
        private static readonly object LockHelper = new object();

        private ProtocolFactory()
        {
        }

        public static ProtocolFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockHelper)
                    {
                        if (_instance == null)
                            _instance = new ProtocolFactory();
                    }
                }
                return _instance;
            }
        }

        #endregion //[--Singleton--]

        public Protocol CreateProtocol(string name, string layout)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("protocol name is null or white space");
            if(null == layout) layout = string.Empty;

            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[ProtocolFactory.CreateProtocol]" + "[name, layout]" + name + ", " + layout );

            return new Protocol(name, LayoutFactory.Instance.CreateLayout(layout));
        }
    }
}
