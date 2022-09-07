using Arconia.Core.Services;
using Arconia.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arconia.Services
{
    internal class NavigationService : INavigationService
    {
        private Frame frame;

        public NavigationService(Frame frame)
        {
            this.frame = frame;
        }

        public void NavigateTo<T>(object args) where T : IViewModel
        {
            frame.Navigate(App.Views.GetValueOrDefault(typeof(T)), args);
        }

        public void NavigateTo<T>() where T : IViewModel
        {
            frame.Navigate(App.Views.GetValueOrDefault(typeof(T)));
        }
    }
}
