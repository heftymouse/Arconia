using Arconia.Core.Rcon;
using Arconia.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Arconia.Core.ViewModels
{
    [ObservableObject]
    public partial class RconViewModel : IViewModel
    {
        public string HostName;
        public int Port;
        private string password;
        public ObservableCollection<RconPacket> Responses { get; } = new();
        [ObservableProperty]
        private string? currentCommand = null;

        private IRconProvider provider;
        private INavigationService navigationService;
        private Random random = new Random();
        private Task? packetLoopTask;
        private CancellationTokenSource tokenSource;
        private CancellationToken token;

        public RconViewModel(INavigationService navService, IRconProvider provider, string hostname, int port, string password)
        {
            HostName = hostname;
            Port = port;
            this.password = password;
            this.provider = provider;
            navigationService = navService;
            tokenSource = new();
        }

        [RelayCommand]
        public async Task OnLoad()
        {
            token = tokenSource.Token;
            await provider.Connect(HostName, Port, password);
            packetLoopTask = DoPacketLoop();
        }

        private async Task DoPacketLoop()
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    RconPacket packet = await provider.GetNextPacketAsync(token);
                    Responses.Add(packet);
                }
            }
            catch (TaskCanceledException)
            {

            }
            finally
            {
                provider.Dispose();
                tokenSource.Dispose();
            }
        }

        [RelayCommand]
        public async Task OnSend()
        {
            if (CurrentCommand == null) return;

            await provider.SendPacketAsync(random.Next(), CurrentCommand);
            CurrentCommand = null;
        }

        [RelayCommand]
        public async void OnDisconnect()
        {
            tokenSource.Cancel();
            await packetLoopTask;
            navigationService.NavigateTo<ConnectViewModel>();
        }
    }

    public readonly record struct RconArgs(string hostname, int port, string password);
}
