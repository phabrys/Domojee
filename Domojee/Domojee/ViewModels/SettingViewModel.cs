using Jeedom;

namespace Domojee.ViewModels
{
    internal class SettingViewModel
    {
        public SettingViewModel()
        {
            Instance = this;
        }

        public static SettingViewModel Instance { get; private set; }

        public bool ConnexionAuto
        {
            get => RequestViewModel.config.ConnexionAuto; set => RequestViewModel.config.ConnexionAuto = value;
        }

        public string Host
        {
            get => RequestViewModel.config.Host; set => RequestViewModel.config.Host = value;
        }

        public string Login
        {
            get => RequestViewModel.config.Login; set => RequestViewModel.config.Login = value;
        }

        public string Password
        {
            get => RequestViewModel.config.Password; set => RequestViewModel.config.Password = value;
        }
    }
}