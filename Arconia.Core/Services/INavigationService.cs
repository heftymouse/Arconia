using Arconia.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arconia.Core.Services
{
    public interface INavigationService
    {
        void NavigateTo<T>() where T : IViewModel;
        void NavigateTo<T>(object args) where T : IViewModel;
    }
}
