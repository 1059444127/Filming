using System.Collections.Generic;
using UIH.Mcsf.Filming.Abstracts;

namespace UIH.Mcsf.Filming.Utilities
{
    internal class SelectOnlyOpertator<T> : SelectOperator<T> where T : class, ISelectable
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