using System;

namespace UIH.Mcsf.Filming.Interface
{
    public abstract class LayoutBase
    {
        public abstract int Rows { get; protected set; }
        public abstract int Columns { get; protected set; }
        public const string Default = "Default";
        public const int MaxRows = 10;
        public const int MaxColumns = 10;

        public override string ToString()
        {
            return string.Format("{0}x{1}", Columns, Rows);
        }

        public static bool Validate(int col, int row)
        {
            return row >= 1 && row <= MaxRows && col >= 1 && col <= MaxColumns;
        }

        public int Capacity
        {
            get { return Rows*Columns; }
        }

        public override bool Equals(object obj)
        {
            var layout = obj as LayoutBase;
            if (layout == null) return false;
            return Rows == layout.Rows && Columns == layout.Columns;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //public static bool operator ==(LayoutBase layout1, LayoutBase layout2)
        //{
        //    if (layout1 == null)
        //    {
        //        if (layout2 == null) return true;
        //        return false;
        //    }
        //    if (layout2 == null) return false;
        //    return layout1.Rows == layout2.Rows && layout1.Columns == layout2.Columns;
        //}

        //public static bool operator !=(LayoutBase layout1, LayoutBase layout2)
        //{
        //    return !(layout1 == layout2);
        //}
    }

    public class StandardLayout : LayoutBase
    {
        public StandardLayout(int col, int row)
        {
            Rows = row;
            Columns = col;
        }

        #region Overrides of LayoutBase

        public override sealed int Rows { get; protected set; }
        public override sealed int Columns { get; protected set; }

        #endregion


    }

    public sealed class DefaultLayout : LayoutBase
    {

        #region [--Singleton--]

        private static volatile DefaultLayout _instance;
        private static readonly object LockHelper = new object();

        private DefaultLayout()
        {
        }

        public static DefaultLayout Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockHelper)
                    {
                        if (_instance == null)
                            _instance = new DefaultLayout();
                    }
                }
                return _instance;
            }
        }

        #endregion //[--Singleton--]


        #region Overrides of LayoutBase

        public override int Rows
        {
            get { throw new Exception("Not implement"); }
            protected set { throw new NotImplementedException(); }
        }

        public override int Columns
        {
            get { throw new Exception("Not implement"); }
            protected set { throw new NotImplementedException(); }
        }

        #endregion

        public override string ToString()
        {
            return Default;
        }
    }

}
