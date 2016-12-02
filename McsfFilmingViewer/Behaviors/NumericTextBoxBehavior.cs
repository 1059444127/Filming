using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interactivity;
using System.Text.RegularExpressions;


namespace UIH.Mcsf.Filming
{
    /// <key>\n 
    /// PRA:Yes \n
    /// Traced from: SSFS_PRA_PR_FaultTolerant \n
    /// Tests: N/A \n
    /// Description: Validate the input \n
    /// Short Description: Behaviors \n
    /// Component:PR \n
    /// </key> \n
    public class NumericTextBoxBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty MinValueProperty;

        public int MinValue
        {
            get { return (int)(GetValue(MinValueProperty)); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty;

        public int MaxValue
        {
            get { return (int)(GetValue(MaxValueProperty)); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty IsFloatProperty;

        public bool IsFloat
        {
            get { return true; }
            set { SetValue(IsFloatProperty, value); }
        }

        static NumericTextBoxBehavior()
        {
            MinValueProperty = DependencyProperty.Register(
                "MinValue",
                typeof(int),
                typeof(NumericTextBoxBehavior),
                new FrameworkPropertyMetadata(0));

            MaxValueProperty = DependencyProperty.Register(
                "MaxValue",
                typeof(int),
                typeof(NumericTextBoxBehavior),
                new FrameworkPropertyMetadata(int.MaxValue));

            IsFloatProperty = DependencyProperty.Register(
                "IsFloat",
                typeof(bool),
                typeof(NumericTextBoxBehavior),
                new FrameworkPropertyMetadata(false));
        }

        public NumericTextBoxBehavior()
        {
            //
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            // 插入要在将 Behavior 附加到对象时运行的代码。

            this.AssociatedObject.PreviewTextInput += new TextCompositionEventHandler(OnPreviewTextInput);
            this.AssociatedObject.TextChanged += new TextChangedEventHandler(OnTextChanged);
            System.Windows.Input.InputMethod.SetIsInputMethodEnabled(this.AssociatedObject, false);

            DataObject.AddPastingHandler(this.AssociatedObject, OnClipboardPaste);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            // 插入要在从对象中删除 Behavior 时运行的代码。
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
             this.AssociatedObject.Text = correctText(this.AssociatedObject.Text);
        }

        private string correctText(string text)
        {
            string result = text;
            try
            {               
                double textValue = Double.Parse(text);
                if (textValue > MaxValue)
                {
                    result = MaxValue.ToString();
                }
                else if (textValue < MinValue)
                {
                    result = MinValue.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
            return result;
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
                    Logger.LogFuncException(ex.Message+ex.StackTrace);

                }

            }

            Regex regExpr = null;
            //bool isMatch = false;

            //if (IsFloat)
            //{
            //    //regExpr = new Regex(@"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$",
            //    //    RegexOptions.Compiled);

            //    regExpr = new Regex(@"^(([0-9]+\.[0-9])|([0-9]*[1-9][0-9]*\.[0-9])|([0-9]*[1-9][0-9]*))$",
            //        RegexOptions.Compiled);
            //    isMatch = regExpr.IsMatch(testString);
            //    if (!isMatch)
            //    {
            //        regExpr = new Regex(@"^[0-9]*[1-9][0-9]*[.]$", RegexOptions.Compiled);
            //    }
            //}
            //else
            //{
            //    regExpr = new Regex(@"^[0-9]*[1-9][0-9]*$", RegexOptions.Compiled);
            //}

            regExpr = new Regex(@"^-?(0|[1-9]\d*)?(\.\d*)?$", RegexOptions.Compiled);

            if (regExpr.IsMatch(testString))
            {
                //try
                //{
                //    double inputValue = Double.Parse(testString);
                //    if (MinValue > inputValue || MaxValue < inputValue)
                //    {
                //        return false;
                //    }
                //    else
                //    {
                //        return true;
                //    }
                //}
                //catch (System.Exception ex)
                //{
                //    System.Console.WriteLine(ex.StackTrace);
                //}
                return true;
            }

            return false;
        }
    }
}