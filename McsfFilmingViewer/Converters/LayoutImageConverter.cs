using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
namespace UIH.Mcsf.Filming
{
    public class LayoutImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string layoutMark = value.ToString();
            if (string.IsNullOrEmpty(layoutMark))
            {
                return null;
            }
            string layoutIconKey = string.Empty;
            if (layoutMark.Length > 2)  //cell layout: 1x1, 2x2...
            {
                layoutIconKey = GetCellLayoutIconKey(layoutMark);
            }
            else //viewport layout: 1\2\3\4....
            {
                layoutIconKey = GetViewportLayoutIconName(layoutMark);
            }

            var filmmingCard = FilmingViewerContainee.FilmingViewerWindow as FilmingCard;
            if (filmmingCard != null)
            {
                try
                {
                    var img = filmmingCard.FindResource(layoutIconKey) as ImageSource;
                    return img;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.StackTrace);
                }
            }

            return null;

        }

        public static string GetCellLayoutIconKey(string cellLeyouMark)
        {
            //1*1,2*2,2*3，3*3,3*4，4*4, 4*5，4*6,5*6
            if (cellLeyouMark.Equals("1x1"))
            {
                return "layout1x1_white";
            }

            if (cellLeyouMark.Equals("2x2"))
            {
                return "layout2x2_white";
            }

            if (cellLeyouMark.Equals("2x3"))
            {
                return "layout2x3_white";
            }

            if (cellLeyouMark.Equals("3x3"))
            {
                return "layout3x3_white";
            }

            if (cellLeyouMark.Equals("3x4"))
            {
                return "layout3x4_white";
            }

            if (cellLeyouMark.Equals("4x4"))
            {
                return "layout4x4_white";
            }

            if (cellLeyouMark.Equals("4x6"))
            {
                return "4x6_white";
            }

            if (cellLeyouMark.Equals("5x6"))
            {
                return "5x6_white";
            }

            if (cellLeyouMark.Equals("Custom"))
            {
                return "cumstom_white";
            }

            return "cumstom_white";
        }

        public static string GetViewportLayoutIconName(string viewportLayoutIndex)
        {
            try
            {
                string viewportLayoutIconKey = "layout1x1_white";
                int viewportIndex = System.Convert.ToInt32(viewportLayoutIndex);

                switch (viewportIndex)
                {
                    case 2:
                        viewportLayoutIconKey = "layout2x1_white";
                        break;
                    case 3:
                        viewportLayoutIconKey = "layout2x2_white";
                        break;
                    case 4:
                        viewportLayoutIconKey = "layout1x2_white";
                        break;
                    case 5:
                        viewportLayoutIconKey = "layout1x3_white";
                        break;
                    case 6:
                        viewportLayoutIconKey = "layout3x1_white";
                        break;
                    case 7:
                        viewportLayoutIconKey = "left_two_one_white";
                        break;
                    case 8:
                        viewportLayoutIconKey = "up_one_two_white";
                        break;
                    case 9:
                        viewportLayoutIconKey = "up_two_one_white";
                        break;
                    case 10:
                        viewportLayoutIconKey = "left_one_two_white";
                        break;
                    case 11:
                        viewportLayoutIconKey = "up_big_small_white";
                        break;
                    case 12:
                        viewportLayoutIconKey = "up_small_big_white";
                        break;
                    case 13:
                        viewportLayoutIconKey = "left_small_small_big_white";
                        break;
                    case 14:
                        viewportLayoutIconKey = "layout1x4_white";
                        break;
                    case 15:
                        viewportLayoutIconKey = "layout4x1_white";
                        break;
                    case 16:
                        viewportLayoutIconKey = "Viewport_Layout1+1_white4";//"up_small_big_white";
                        break;
                    case 17:
                        viewportLayoutIconKey = "Viewport_Layout1+2+1_white";
                        break;
                    case 18:
                        viewportLayoutIconKey = "layout2x3_white";
                        break;
                    case 19:
                        viewportLayoutIconKey = "left_small_big_white";
                        break;
                    case 20:
                        viewportLayoutIconKey = "left_two_three_white";
                        break;
                    case 21:
                        viewportLayoutIconKey = "layout3x3_white";
                        break;
                    default:
                        viewportLayoutIconKey = "layout1x1_white";
                        break;
                }

                return viewportLayoutIconKey;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.StackTrace);
                return "layout1x1";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
