using System.Collections.Generic;
using UIH.Mcsf.Filming.Abstracts;

namespace UIH.Mcsf.Filming.Utilities
{
    class DummySelectOperator<T> : SelectOperator<T> where T : class, ISelect
    {
        public DummySelectOperator(T item, List<T> items) : base(item, items)
        {
        }

        #region Overrides of SelectOperator<T>

        public override void Operate()
        {
            
        }

        #endregion
    }
}