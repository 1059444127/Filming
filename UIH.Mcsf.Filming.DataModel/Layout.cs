using System;
using Environment = UIH.Mcsf.Filming.Configure.Environment;

namespace UIH.Mcsf.Filming.DataModel
{
    public abstract class Layout
    {
        public abstract int Capacity { get; }

        public static Layout CreateDefaultLayout()
        {
            var layout = Environment.Instance.GetDefaultLayoutConfigure().Layout;
            return CreateLayout(layout.Columns, layout.Rows);
        }

        public static Layout CreateLayout(int columns, int rows)
        {
            return new SimpleLayout(columns, rows);
        }
    }

    public class XmlLayout : Layout
    {
        public string Xml { get; private set; }
        private readonly int _capacity;

        public XmlLayout(string xml)
        {
            Xml = xml;
            _capacity = CalulateCapacity();
        }

        #region Overrides of Layout

        public override int Capacity
        {
            get { return _capacity; }
        }

        #endregion

        private int CalulateCapacity()
        {
            throw new NotImplementedException();
        }
    }

    public class SimpleLayout : Layout
    {
        public int Col { get; private set; }
        public int Row { get; private set; }

        #region Overrides of Layout

        public SimpleLayout(int col, int row)
        {
            Col = col;
            Row = row;
        }

        public override int Capacity
        {
            get { return Col*Row; }
        }

        #endregion
    }
}