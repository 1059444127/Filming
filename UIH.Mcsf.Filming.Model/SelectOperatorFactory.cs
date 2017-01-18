using System.Collections.Generic;
using UIH.Mcsf.Filming.Abstracts;

namespace UIH.Mcsf.Filming.Model
{
    internal class SelectOperatorFactory<T> where T : class, ISelect
    {
        public static SelectOperator<T> CreateSelectOperator(T item, List<T> items, IClickStatus clickStatus)
        {
            // 1. only right mouse clicked
            if (clickStatus.IsRightMouseButtonClicked && !clickStatus.IsLeftMouseButtonClicked)
            {
                if (clickStatus.IsCtrlPressed && !clickStatus.IsShiftPressed)
                    return new DummySelectOperator<T>(item, items); // modifier key, only ctrl pressed
                if (item.IsSelected) return new DummySelectOperator<T>(item, items); // click on selected element
                return new SelectOnlyOpertator<T>(item, items);
            }

            // 2. left mouse clicked

            // 2.1 ctrl & shift pressed
            if (clickStatus.IsShiftPressed && clickStatus.IsCtrlPressed)
                return new SelectRangeBaseOnFocusOperator<T>(item, items);
            // 2.2 only shift pressed
            if (clickStatus.IsShiftPressed)
                return new SelectRangeOperator<T>(item, items);
            // 2.3 only ctrl pressed
            if (clickStatus.IsCtrlPressed)
                return new TakeBackSelectOperator<T>(item, items);
            // 2.4 no modifier key pressed
            return new SelectOnlyOpertator<T>(item, items);
        }
    }
}