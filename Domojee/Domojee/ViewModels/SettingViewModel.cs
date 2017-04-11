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
            get { return RequestViewModel.config.ConnexionAuto; }
            set { RequestViewModel.config.ConnexionAuto = value; }
        }

        public string Host
        {
            get { return RequestViewModel.config.Host; }
            set { RequestViewModel.config.Host = value; }
        }

        public string Login
        {
            get { return RequestViewModel.config.Login; }
            set { RequestViewModel.config.Login = value; }
        }

        public string Password
        {
            get { return RequestViewModel.config.Password; }
            set { RequestViewModel.config.Password = value; }
        }
    }
}