using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows.Interactivity;

namespace UIH.Mcsf.Filming
{

    public class ScaleTextBoxBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty MinValueProperty;

        public double MinValue
        {
            get { return (double)(GetValue(MinValueProperty)); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty;

        public double MaxValue
        {
            get { return (double)(GetValue(MaxValueProperty)); }
            set { SetValue(MaxValueProperty, value); }
        }


        static ScaleTextBoxBehavior()
        {
            MinValueProperty = DependencyProperty.Register(
                "MinValue",
                typeof(double),
                typeof(ScaleTextBoxBehavior),
                new FrameworkPropertyMetadata(0.1));

            MaxValueProperty = DependencyProperty.Register(
                "MaxValue",
                typeof(double),
                typeof(ScaleTextBoxBehavior),
                new FrameworkPropertyMetadata(15.00));

        }


        protected override void OnAttached()
        {
            base.OnAttached();

            // 插入要在将 Behavior 附加到对象时运行的代码。

            this.AssociatedObject.PreviewTextInput += new TextCompositionEventHandler(OnPreviewTextInput);
            //this.AssociatedObject.TextChanged += new TextChangedEventHandler(OnTextChanged);
            System.Windows.Input.InputMethod.SetIsInputMethodEnabled(this.AssociatedObject, false);

            DataObject.AddPastingHandler(this.AssociatedObject, OnClipboardPaste);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            // 插入要在从对象中删除 Behavior 时运行的代码。
        }


        /// <summary>
        /// This checks if the resulting string will match the regex expression
        /// </summary>
        void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Validate(e.Text))
            {
                double caretIndex = this.AssociatedObject.CaretIndex/* - e.Text.Length*/;
                caretIndex = Math.Max(0, (int)Math.Ceiling(caretIndex));
                if (!String.IsNullOrEmpty(this.AssociatedObject.Text)
                    && !String.IsNullOrEmpty(e.Text)
                    && this.AssociatedObject.Text.Length > caretIndex
                    && e.Text.Length > (int)caretIndex
                    && e.Text.IndexOf(this.AssociatedObject.Text[(int)caretIndex]) != -1)
                {
                    this.AssociatedObject.Text = this.AssociatedObject.Text.Remove((int)caretIndex, e.Text.Length);
                    this.AssociatedObject.CaretIndex = (int)caretIndex;
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// This method handles paste and drag/drop events onto the TextBox.  It restricts the character
        /// set and ensures we have consistent behavior.
        /// </summary>
        /// <param name="sender">TextBox sender</param>
        /// <param name="e">EventArgs</param>
        private void OnClipboardPaste(object sender, DataObjectPastingEventArgs e)
        {
            string text = e.SourceDataObject.GetData(e.FormatToApply) as string;

            if (!string.IsNullOrEmpty(text) && !Validate(text))
                e.CancelCommand();
        }


        /// <summary>
        /// This checks if the resulting string will match the regex expression
        /// </summary>
        private bool Validate(string newContent)
        {
            string testString = string.Empty;

            // replace selection with new text.
            int preLength;
            int afterStartIndex;
            int afterLength;
            if (String.IsNullOrWhiteSpace(this.AssociatedObject.Text))
            {
                testString = newContent;
            }
            else
            {
                if (!String.IsNullOrEmpty(this.AssociatedObject.SelectedText))
                {
                    preLength = this.AssociatedObject.SelectionStart;
                    afterStartIndex = this.AssociatedObject.SelectionStart + this.AssociatedObject.SelectionLength;
                    afterLength = this.AssociatedObject.Text.Length - afterStartIndex;
                }
                else
                {
                    preLength = this.AssociatedObject.CaretIndex;
                    afterStartIndex = this.AssociatedObject.CaretIndex;
                    afterLength = this.AssociatedObject.Text.Length - this.AssociatedObject.CaretIndex;
                }
                try
                {
                    string pre;
                    string after;

                    if (this.AssociatedObject.Text.Length > preLength)
                    {
                        pre = this.AssociatedObject.Text.Substring(0, preLength);
                    }
                    else
                    {
                        pre = this.AssociatedObject.Text;
                    }
                    if (AssociatedObject.Text.Length > afterStartIndex && AssociatedObject.Text.Length >= afterLength)
                    {
                        after = this.AssociatedObject.Text.Substring(afterStartIndex, afterLength);
                    }
                    else
                    {
                        after = "";
                    } 
                    testString = pre + newContent + after;
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogDevError(ex.StackTrace);
                }

            }

            Regex regExpr = null;


            regExpr = new Regex(@"^(0|[1-9]\d*)?(\.((\d)(\d)?)?)?$", RegexOptions.Compiled);

            if (regExpr.IsMatch(testString))
            {
                return true;
            }
            return false;
        }
    }
}