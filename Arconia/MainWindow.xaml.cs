using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI.ViewManagement;

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

            AppTitle = new Random().Next(1000000) != 0 ? "Arconia" : "Hraður Viðskiptavinur";
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            MicaHelper helper = new(this);
            helper.TrySetMicaBackdrop();

            RootFrame.Navigate(typeof(ConnectPage));

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
    }
}