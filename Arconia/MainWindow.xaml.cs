using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media;
using Windows.UI.ViewManagement;
using WinRT.Interop;

using Arconia.Views;
using Arconia.Helpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Arconia
{
    /// <summary>
    /// Main App Window
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        string AppTitle
        {
            get => Title;
            set
            {
                Title = value;
                Bindings.Update();
            }
        }
        public MainWindow()
        {
            this.InitializeComponent();

            AppTitle = "Arconia";
            SetCustomTitlebar();
            MicaHelper helper = new(this);
            helper.TrySetMicaBackdrop();

            RootFrame.Navigate(typeof(SettingsPage));

            Activated += (sender, e) =>
            {
                UISettings settings = new UISettings();
                if (e.WindowActivationState == WindowActivationState.Deactivated)
                {
                    AppTitleTextBlock.Foreground =
                        new SolidColorBrush(settings.UIElementColor(UIElementType.GrayText));
                }
                else
                {
                    AppTitleTextBlock.Foreground =
                        new SolidColorBrush(settings.GetColorValue(UIColorType.Foreground));
                }
            };
        }

        void SetCustomTitlebar()
        {
            if (!AppWindowTitleBar.IsCustomizationSupported()) return;

            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(wndId);

            var titlebar = appWindow.TitleBar;
            titlebar.ExtendsContentIntoTitleBar = true;
            titlebar.ButtonBackgroundColor = Colors.Transparent;
            titlebar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titlebar.ButtonHoverBackgroundColor = ((Microsoft.UI.Xaml.Media.SolidColorBrush)App.Current.Resources.ThemeDictionaries["SystemControlBackgroundListLowBrush"]).Color;
            titlebar.ButtonPressedBackgroundColor = ((Microsoft.UI.Xaml.Media.SolidColorBrush)App.Current.Resources.ThemeDictionaries["SystemControlBackgroundListMediumBrush"]).Color;
        }
    }
}