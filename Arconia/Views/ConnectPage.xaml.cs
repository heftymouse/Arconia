using Arconia.Core.ViewModels;
using Arconia.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics;

namespace Arconia.Views
{
    /// <summary>
    /// Page for RCON connection
    /// </summary>
    public sealed partial class ConnectPage : Page
    {
        ConnectViewModel vm;

        public ConnectPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            vm = new ConnectViewModel(new NavigationService(this.Frame));
        }
    }
}