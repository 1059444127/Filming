using System.Windows;
using UIH.Mcsf.NLS;

namespace UIH.Mcsf.Filming
{
    public class Nls
    {
        #region [--Singleton--]

        private static volatile Nls _instance;
        private static readonly object LockHelper = new object();

        private Nls()
        {
            var nls = ResourceMgr.Instance();
            ResourceDictionary = nls.Init(FilmingNlsResourceName);
        }

        public static Nls Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockHelper)
                    {
                        if (_instance == null)
                            _instance = new Nls();
                    }
                }
                return _instance;
            }
        }

        #endregion //[--Singleton--]

        #region [--Properties--]

        public ResourceDictionary ResourceDictionary { get; private set; }

        #endregion // [--Properties--]


        private const string FilmingNlsResourceName = "Filming";
    }
}
