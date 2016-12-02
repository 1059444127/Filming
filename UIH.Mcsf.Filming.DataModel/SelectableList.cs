using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UIH.Mcsf.Filming.DataModel
{
    public class SelectableList<T> : List<T> where T : class, ISelect
    {
        #region [--Event Handlers--]

        private void ElementOnClicked(object sender, ClickStatusEventArgs clickStatusEventArgs)
        {
            ClickOn(sender as T, clickStatusEventArgs.ClickStatus);
        }

        #endregion [--Event Handlers--]

        #region [--Constructor--]

        public SelectableList(IEnumerable<T> elements)
        {
            AddRange(elements);
        }

        protected SelectableList()
        {
        }

        ~SelectableList()
        {
            ForEach(e => e.Clicked -= ElementOnClicked);
        }

        #endregion [--Constructor--]

        #region [--Override--]

        protected void InsertRange(int index, IEnumerable<T> elements, bool isSelected = false)
        {
            var enumerable = elements as List<T> ?? elements.ToList();
            foreach (var element in enumerable)
            {
                element.IsSelected = isSelected;
                RegisterClickEvent(element);
            }
            base.InsertRange(index, enumerable);
        }

        protected new void AddRange(IEnumerable<T> elements)
        {
            var enumerable = elements as List<T> ?? elements.ToList();
            foreach (var element in enumerable)
            {
                RegisterClickEvent(element);
            }
            base.AddRange(enumerable);
        }

        protected new void RemoveRange(int index, int count)
        {
            var elements = GetRange(index, count);
            elements.ForEach(e => e.Clicked -= ElementOnClicked);
            base.RemoveRange(index, count);
        }

        #endregion [--Override--]

        #region [--Private Methods--]

        #region Focus 

        private int _focus = -1;

        private int Focus
        {
            get { return _focus; }
            set
            {
                if (_focus == value) return;
                SetFocusStatusOfElementAt(_focus, false);
                SetFocusStatusOfElementAt(value, true);
                _focus = value;
            }
        }

        #endregion Focus

        private void RegisterClickEvent(T element)
        {
            element.Clicked -= ElementOnClicked;
            element.Clicked += ElementOnClicked;
        }

        private void ClickOn(T operationElement, IClickStatus clickStatus)
        {
            Debug.Assert(operationElement != null);

            //0. 焦点
            var operationElementIndex = IndexOf(operationElement);
            var lastFocus = Focus;
            var lastFocusElement = this.ElementAtOrDefault(lastFocus);

            //1. 仅仅右键按下
            if (clickStatus.IsRightMouseButtonClicked && !clickStatus.IsLeftMouseButtonClicked)
            {
                if (clickStatus.IsCtrlPressed && !clickStatus.IsShiftPressed) return; //仅仅ctrl按下， 不改变选中状态
                if (operationElement.IsSelected) return; //作用在选中的cell上，不改变选中状态
                //operationCell未选中                               //作用在未选中的cell上，仅仅选中该cell
                SelectOnly(operationElement);
                return;
            }

            //2. 有左键按下           


            //2.1 ctrl+shift按下
            if (clickStatus.IsShiftPressed && clickStatus.IsCtrlPressed)
            {
                SelectRange(lastFocus, operationElementIndex, lastFocusElement != null && lastFocusElement.IsSelected);
            }
            //2.2 仅仅 ctrl 按下
            else if (clickStatus.IsShiftPressed)
            {
                ForEach(cell => cell.IsSelected = false);
                SelectRange(lastFocus, operationElementIndex, true);
            }
            //2.3 仅仅 ctrl 按下
            else if (clickStatus.IsCtrlPressed)
            {
                operationElement.IsSelected = !operationElement.IsSelected;
                Focus = operationElementIndex;
            }
            //2.4 没有modifier key 按下
            else
            {
                SelectOnly(operationElement);
            }
        }

        private void SelectOnly(T element)
        {
            ForEach(e => e.IsSelected = false);
            element.IsSelected = true;
            Focus = IndexOf(element);
        }

        //会引起Focus变更

        private void SelectRange(int index1, int index2, bool isSelected)
        {
            var first = Math.Min(index1, index2);
            var last = Math.Max(index1, index2);

            if (first < 0 || last >= Count)
            {
                Logger.Instance.LogDevWarning(string.Format("Select Out of Range[{0}:{1}]", first, last));
                return;
            }

            for (var i = first; i <= last; i++)
            {
                this[i].IsSelected = isSelected;
            }
        }

        private void SetFocusStatusOfElementAt(int index, bool isFocused)
        {
            var element = this.ElementAtOrDefault(index);
            if (element != null) element.IsFocused = isFocused;
        }

        #endregion [--Private Methods--]
    }
}