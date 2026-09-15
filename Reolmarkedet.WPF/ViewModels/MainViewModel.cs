using Reolmarkedet.Core.Services;
using Reolmarkedet.WPF.Views;

namespace Reolmarkedet.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly SalesService _salesService;

        public MainViewModel(SalesService salesService)
        {
            _salesService = salesService;

        }
    }
}
