using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Arconia.Views
{
    /// <summary>
    /// Page for RCON connection
    /// </summary>
    public sealed partial class ConnectPage : Page
    {
        string connectHost = "localhost";
        int connectPort = 25575;
        string connectPassword;
        public ConnectPage()
        {
            this.InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (connectHost == null || connectPort == 0 || connectPassword == null) return;

            this.Frame.Navigate(typeof(RconPage), new RconArgs(connectHost, connectPort, connectPassword));
        }
    }
}
