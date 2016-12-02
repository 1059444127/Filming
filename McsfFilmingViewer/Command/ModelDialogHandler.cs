using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace UIH.Mcsf.Filming.Command
{
    public class ModelDialogHandler
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool EnableWindow(IntPtr hwnd, bool enable);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetParent(IntPtr hwnd);

        /// <summary>
        /// this function used for show model window in main framework window.
        /// </summary>
        /// <param name="wnd">the dialog used in user conctrol</param>
        public static void ShowModelWnd(Window wnd)
        {
            var hwndSource = PresentationSource.FromVisual(FilmingViewerContainee.FilmingViewerWindow) as HwndSource;
            if (hwndSource != null)
            {
                var hwndParent = GetParent(hwndSource.Handle);
                EnableWindow(hwndParent, false);
                try
                {
                    Window childWnd = wnd;
                    //new WindowInteropHelper(childWnd) { Owner = hwndParent };
                    childWnd.ShowDialog();
                }
                catch (Exception ex)
                {
                    Logger.LogError("ModelDialogHandler.ShowModelWnd:"+ex.StackTrace);
                }
                finally
                {
                    EnableWindow(hwndParent, true);
                }
            }
        }
    }
}
