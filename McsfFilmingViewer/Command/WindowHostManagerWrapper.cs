using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UIH.Mcsf.Controls;

namespace UIH.Mcsf.Filming.Command
{
    class WindowHostManagerWrapper
    {
        public static void ShowSecondaryWindow(UIElement uiElement, string title, WindowStartupLocation startupLocation = WindowStartupLocation.CenterScreen)
        {
            try
            {
                Logger.LogFuncUp();

                IsQuittingJob = null;

                if (uiElement == null)
                {
                    Logger.LogWarning("uiElement to be shown is null");
                    return;
                }

                WindowHostManager.HostUIe = Window.GetWindow(FilmingViewerContainee.FilmingViewerWindow);
                WindowHostManager.Content = uiElement;
                WindowHostManager.HostAdorner.Title = title;
                WindowHostManager.HostAdorner.StartupLocation = startupLocation;
                uiElement.KeyDown -= UIElementOnKeyDown;
                uiElement.KeyDown += UIElementOnKeyDown;
                FilmingViewerContainee.Main.OnEnterSecondaryUI();
                WindowHostManager.Show();

                Logger.LogFuncDown();
            }
            catch (Exception ex)
            {
                Logger.LogFuncException(ex.Message+ex.StackTrace);
            }
        }

        private static void UIElementOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Escape)
            {
                CloseSecondaryWindow();
                keyEventArgs.Handled = true;
            }
        }

        public static bool? IsQuittingJob { get; set; }

        public static void CloseSecondaryWindow()
        {
            WindowHostManager.Close();
            FilmingViewerContainee.Main.OnExitSecondaryUI(IsQuittingJob);
        }

        static WindowHostManagerWrapper()
        {
            WindowHostManager.HostAdorner.OnClosing +=
                (sender, args) => FilmingViewerContainee.Main.OnExitSecondaryUI(IsQuittingJob);
        }
    }
}
