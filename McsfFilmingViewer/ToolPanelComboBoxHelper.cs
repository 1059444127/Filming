using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace UIH.Mcsf.Filming
{
    public class ToolPanelComboBoxHelper
    {
        public const string BoundToRadioButtonPropertyName = "BoundToRadioButton";

        public static void SetBoundToRadioButtonProperty(DependencyObject d, bool rbBound)
        {
            d.SetValue(BoundToRadioButtonProperty, rbBound);
        }

        public static readonly DependencyProperty BoundToRadioButtonProperty = DependencyProperty.RegisterAttached(
            BoundToRadioButtonPropertyName, typeof(bool), typeof(ToolPanelComboBoxHelper),
            new PropertyMetadata(false, OnBoundToRadioBtnPropertyChangedCallback));


        private static void OnBoundToRadioBtnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var comboBox = d as ComboBox;
            if (comboBox != null)
            {
                comboBox.SelectionChanged -= comboBox_SelectionChanged;
                comboBox.SelectionChanged += comboBox_SelectionChanged;
            }
        }


        public static void btn_Click(object sender, RoutedEventArgs e)
        {
            var rb = sender as ButtonBase;
            var cbItem = rb.Tag as ComboBoxItem;
            if (cbItem.IsEnabled)
            {
                cbItem.IsSelected = false;
                cbItem.IsSelected = true;
            }
        }

        private static void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var comboBox = sender as ComboBox;
            if (comboBox == null)
                return;

            var btn = comboBox.Tag as ButtonBase;
            if (btn == null)
                return;

            btn.Click -= btn_Click;
            btn.Click += btn_Click;

            var selItem = comboBox.SelectedItem as ComboBoxItem;
            btn.Tag = selItem;

            var cImg = selItem.Content as Image;
            var rImg = btn.Content as Image;

            //btn.IsEnabled = selItem.IsEnabled;
            SetBinding(selItem, "IsEnabled", btn, ButtonBase.IsEnabledProperty);
            SetBinding(selItem, "ToolTip", btn, ButtonBase.ToolTipProperty);

            if (cImg != null && rImg != null)
            {
                SetBinding(cImg, "Tag", rImg, Image.SourceProperty);
            }

            //if (btn is RadioButton)
            //{
            //    (btn as RadioButton).IsChecked = true;
            //}
        }

        private static void SetBinding(FrameworkElement sourceElt, string path, FrameworkElement targetElt, DependencyProperty dp)
        {
            BindingOperations.ClearBinding(targetElt, dp);
            Binding bind = new Binding(path);
            bind.Source = sourceElt;
            bind.Mode = BindingMode.OneWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            targetElt.SetBinding(dp, bind);
        }
    }
}
