using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UIH.Mcsf.Filming.Abstracts;

namespace UIH.Mcsf.Filming.Utilities
{
    internal abstract class SelectOperator<T> where T : class, ISelect
    {
        protected readonly T Element;
        protected readonly List<T> Elements;

        protected SelectOperator(T item, List<T> elements)
        {
            Element = item;
            Elements = elements;
        }

        protected void SelectRange(int index1, int index2, bool isSelected)
        {
            var first = Math.Min(index1, index2);
            var last = Math.Max(index1, index2);

            Debug.Assert(first >= 0);
            Debug.Assert(last < Elements.Count);

            for (var i = first; i <= last; i++)
            {
                Elements[i].IsSelected = isSelected;
            }
        }

        protected void SetFocus(T value)
        {
            if (value != null) value.IsFocused = true;
            Elements.Except(new[] {value}).ToList().ForEach(e => e.IsFocused = false);
        }

        protected T GetFocus()
        {
            return Elements.FirstOrDefault(e => e.IsFocused);
        }

        public abstract void Operate();
    }
}