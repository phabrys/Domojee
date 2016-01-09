using System;
using Windows.Storage;

namespace Domojee.ViewModels
{
    public class ConfigurationViewModel
    {
        string _address;
        string _apikey;
        public string Address
        {
            set
            {
                _address = value;
                RoamingSettings.Values[settingAddress] = value;
            }

            get
            {
                return _address;
            }
        }

        public string ApiKey
        {
            set
            {
                if (value != null)
                {
                    _apikey = value;
                    RoamingSettings.Values[settingAPIKey] = value;
                }
            }
            get
            {
                return _apikey;
            }
        }

        public string ServerName
        {
            get
            {
                try
                {
                    var debut = _address.IndexOf(':') + 3;
                    var fin = _address.IndexOf('/', debut);
                    if (fin == -1)
                    {
                        fin = _address.Length;
                    }
                    return _address.Substring(debut, fin - debut);
                }
                catch (Exception)
                {
                    return "Déconnecté";
                }
            }
        }
        public bool Populated = false;

        ApplicationDataContainer RoamingSettings = ApplicationData.Current.RoamingSettings;

        const string settingAddress = "addressSetting";
        const string settingAPIKey = "apikeySetting";

        public ConfigurationViewModel()
        {
            //_address = "http://192.168.2.6/jeedom";
            //_apikey = "HXCRXXVgJPsi6DTJ9E0c";
            if (RoamingSettings.Values[settingAddress] != null)
            {
                _address = RoamingSettings.Values[settingAddress] as string;
                if (RoamingSettings.Values[settingAPIKey] != null)
                {
                    _apikey = RoamingSettings.Values[settingAPIKey] as string;
                    Populated = true;
                }
            }
        }
    }
}
