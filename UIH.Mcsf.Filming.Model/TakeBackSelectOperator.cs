using System.Collections.Generic;
using UIH.Mcsf.Filming.Abstracts;

namespace UIH.Mcsf.Filming.Model
{
    internal class TakeBackSelectOperator<T> : SelectOperator<T> where T : class, ISelect
    {
        public TakeBackSelectOperator(T item, List<T> items) : base(item, items)
        {
        }

        #region Overrides of SelectOperator<T>

        public override void Operate()
        {
            Element.IsSelected = !Element.IsSelected;
            SetFocus(Element);
        }

        #endregion
    }
}