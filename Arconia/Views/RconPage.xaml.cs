using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Arconia.Rcon;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Arconia.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RconPage : Page
    {
        string hostString;
        RconSession session;
        CancellationTokenSource source;
        CancellationToken packetLoopToken;
        Task packetLoopTask;

        ObservableCollection<RconPacket> data = new();

        public RconPage()
        {
            this.InitializeComponent();
        }

        async protected override void OnNavigatedTo(NavigationEventArgs navArgs)
        {
            RconArgs args = (RconArgs)navArgs.Parameter;
            session = new();
            hostString = $"{args.hostname}:{args.port}";
            await session.Connect(args.hostname, args.port, args.password);

            packetLoopTask = DoPacketLoop();
            base.OnNavigatedTo(navArgs);
        }

        private async Task DoPacketLoop()
        {
            session.PacketReceived += OnPacketReceived;
            source = new();
            packetLoopToken = source.Token;

            try
            {
                await session.GetNextPacketAsync(packetLoopToken);
            }
            catch (TaskCanceledException e)
            {

            }
            finally
            {
                source.Dispose();
                session.Dispose();
            }
        }

        private void OnPacketReceived(object sender, RconPacket args)
        {
            data.Add(args);
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            source.Cancel();
            await packetLoopTask;
            Frame.Navigate(typeof(ConnectPage));
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (commandBox.Text == null) return;

            await session.SendPacketAsync(new Random().Next(255), commandBox.Text);
            commandBox.Text = null;
        }
    }

    public readonly record struct RconArgs(string hostname, int port, string password);
}
