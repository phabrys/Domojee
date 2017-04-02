using Jeedom;

namespace Domojee.ViewModels
{
    internal class SettingViewModel
    {
        public static SettingViewModel Instance { get; private set; }

        public SettingViewModel()
        {
            Instance = this;
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

        public bool ConnexionAuto
        {
            get
            {
                //TODO: changer le type quand libuwpjeedom sera publiée
                if (RequestViewModel.config.ConnexionAuto == null)
                    RequestViewModel.config.ConnexionAuto = true;
                return (bool)RequestViewModel.config.ConnexionAuto;
            }
            set { RequestViewModel.config.ConnexionAuto = value; }
        }
    }
}