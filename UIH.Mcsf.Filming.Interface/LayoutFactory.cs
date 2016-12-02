using System;

namespace UIH.Mcsf.Filming.Interface
{
    public class LayoutFactory
    {
        #region [--Singleton--]

        private static volatile LayoutFactory _instance;
        private static readonly object LockHelper = new object();

        private LayoutFactory()
        {

        }

        public static LayoutFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockHelper)
                    {
                        if (_instance == null)
                            _instance = new LayoutFactory();
                    }
                }
                return _instance;
            }
        }

        #endregion //[--Singleton--]

        public LayoutBase CreateLayout(string layout)
        {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[LayoutFactory.CreateLayout]" + "[layout]" + layout);

            if (layout == LayoutBase.Default) return DefaultLayout.Instance;


            layout = layout.ToLower();
            var numbers = layout.Split('x');
            if (numbers.Length != 2)
            {
                Logger.Instance.LogDevWarning(string.Format("[Layout]{0}[Format is wrong]", layout));
                return DefaultLayout.Instance;
            }
                
            int row, col;
            int.TryParse(numbers[0], out col);
            int.TryParse(numbers[1], out row);

            return CreateLayout(col, row);
        }

        public LayoutBase CreateLayout(int col, int row)
        {
            Logger.Instance.LogDevInfo(FilmingUtility.FunctionTraceEnterFlag + "[LayoutFactory.CreateLayout]" + "[col, row]" + col + "," + row);

            if (LayoutBase.Validate(col, row)) return new StandardLayout(col, row);
            Logger.Instance.LogDevInfo(string.Format("[Argument InValid][Cols]{0}[Rows]{1}", col, row));
            return DefaultLayout.Instance;
        }
    }

    public class LayoutEventArgs : EventArgs
    {
        public LayoutEventArgs(LayoutBase layout)
        {
            Layout = layout;
        }

        public LayoutBase Layout { get; private set; }
    }

}
