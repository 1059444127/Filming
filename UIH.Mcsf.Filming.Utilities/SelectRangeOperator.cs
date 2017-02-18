using System.Collections.Generic;
using UIH.Mcsf.Filming.Abstracts;

namespace UIH.Mcsf.Filming.Utilities
{
    internal class SelectRangeOperator<T> : SelectOperator<T> where T : class, ISelectable
    {
        public SelectRangeOperator(T item, List<T> items) : base(item, items)
        {
        }

        #region Overrides of SelectOperator<T>

        public override void Operate()
        {
            Elements.ForEach(e => e.IsSelected = false);

            SelectRangeFromFocusToElement();
        }

        #endregion

        private void SelectRangeFromFocusToElement()
        {
            var focus = GetFocus();
            if (focus == null) return;
            var focusIndex = Elements.IndexOf(focus);
            var operationIndex = Elements.IndexOf(Element);

            SelectRange(focusIndex, operationIndex, true);
        }
    }
}