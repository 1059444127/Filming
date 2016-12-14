using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UIH.Mcsf.Filming.Interfaces;

namespace UIH.Mcsf.Filming.Model
{
    internal class SelectableList<T> : IList<T> where T : class, ISelect
    {
        private readonly List<T> _elements = new List<T>();

        #region [--Event Handler--]

        private void ItemOnClicked(object sender, ClickStatusEventArgs clickStatusEventArgs)
        {
            var operationElement = sender as T;
            Debug.Assert(operationElement != null);
            var clickStatus = clickStatusEventArgs.ClickStatus;

            var selectOperator = SelectOperatorFactroy<T>.CreateSelectOperator(operationElement, _elements, clickStatus);
            selectOperator.Operate();
        }

        #endregion [--Event Handler--]

        #region [--Private Methods--]

        private void RegisterElementEvent(T item)
        {
            item.Clicked -= ItemOnClicked;
            item.Clicked += ItemOnClicked;
        }

        private void UnRegisterElementEvent(T item)
        {
            item.Clicked -= ItemOnClicked;
        }

        #endregion [--Private Methods--]

        #region Implementation of IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<T>

        public void Add(T item)
        {
            RegisterElementEvent(item);

            _elements.Add(item);
        }

        public void Clear()
        {
            _elements.ForEach(UnRegisterElementEvent);
            _elements.Clear();
        }

        public bool Contains(T item)
        {
            return _elements.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            UnRegisterElementEvent(item);
            return _elements.Remove(item);
        }

        public int Count
        {
            get { return _elements.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Implementation of IList<T>

        public int IndexOf(T item)
        {
            return _elements.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            RegisterElementEvent(item);
            _elements.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            UnRegisterElementEvent(this[index]);
            _elements.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return _elements[index]; }
            set { _elements[index] = value; }
        }

        #endregion
    }
}