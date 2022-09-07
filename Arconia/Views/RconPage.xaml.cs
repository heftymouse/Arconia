using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Arconia.Core.Rcon;
using Arconia.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Arconia.Core.Services;
using Arconia.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Arconia.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RconPage : Page
    {
        RconViewModel vm;

        public RconPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs navArgs)
        {
            RconArgs args = (RconArgs)navArgs.Parameter;
            vm = new RconViewModel(new NavigationService(this.Frame), new WinRTRconProvider(), args.hostname, args.port, args.password);
        }
    }
}