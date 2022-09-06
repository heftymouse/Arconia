using Arconia.Core.Rcon;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Arconia.Core.ViewModels
{
    [ObservableObject]
    public partial class RconViewModel
    {
        public string HostName;
        public int Port;
        public ObservableCollection<RconPacket> Responses = new();
        [ObservableProperty]
        private string? currentCommand = null;

        private IRconProvider provider;
        private Random random = new Random();
        private Task packetLoopTask;
        private CancellationTokenSource tokenSource = new();
        private CancellationToken token;

        public RconViewModel(IRconProvider provider, string hostname, int port, string password)
        {
            HostName = hostname;
            Port = port;
            this.provider = provider;

            token = tokenSource.Token;

            packetLoopTask = Task.Run(async () =>
            {
                await provider.Connect(hostname, port, password);
                while (!token.IsCancellationRequested)
                {
                    RconPacket packet = await provider.GetNextPacketAsync(token);
                    Responses.Add(packet);
                }
                token.ThrowIfCancellationRequested();
            }, token);
        }

        [RelayCommand]
        public async Task OnSend()
        {
            if (currentCommand == null) return;

            await provider.SendPacketAsync(random.Next(), currentCommand);
            currentCommand = null;
        }

        [RelayCommand]
        public async void OnDisconnect()
        {
            tokenSource.Cancel();
            await packetLoopTask;
            provider.Dispose();
            tokenSource.Dispose();
        }
    }
}
