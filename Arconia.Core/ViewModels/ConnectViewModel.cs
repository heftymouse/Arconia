using Arconia.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arconia.Core.ViewModels
{
    [ObservableObject]
    public partial class ConnectViewModel : IViewModel
    {
        [ObservableProperty]
        private string hostname = "localhost";

        [ObservableProperty]
        private int port = 25575;

        [ObservableProperty]
        private string password = "";

        private INavigationService navigationService;

        public ConnectViewModel(INavigationService navService)
        {
            navigationService = navService;
        }

        [RelayCommand]
        private void OnConnect()
        {
            if (string.IsNullOrEmpty(Hostname) || Port < 1 || string.IsNullOrEmpty(Password)) return;
            navigationService.NavigateTo<RconViewModel>(new RconArgs(Hostname, Port, Password));
        }
    }
}
