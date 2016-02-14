using Jeedom;
using Template10.Mvvm;

namespace Domojee.ViewModels
{
    public class SplashViewModel : ViewModelBase
    {
        public bool Updating => RequestViewModel.Instance.Updating;
        public string LoadingMessage => RequestViewModel.Instance.LoadingMessage;
    }
}