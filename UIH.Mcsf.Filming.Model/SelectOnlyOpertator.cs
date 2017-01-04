using System.Collections.Generic;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.Model
{
    internal class SelectOnlyOpertator<T> : SelectOperator<T> where T : class, ISelect
    {
        public SelectOnlyOpertator(T item, List<T> items) : base(item, items)
        {
        }

        private void SelectOnly()
        {
            Elements.ForEach(e => e.IsSelected = false);
            Element.IsSelected = true;
        }

        #region Overrides of SelectOperator<T>

        public override void Operate()
        {
            SelectOnly();
            SetFocus(Element);
        }

        #endregion
    }
}