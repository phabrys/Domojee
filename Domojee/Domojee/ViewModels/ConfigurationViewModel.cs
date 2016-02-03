using System;
using Windows.Storage;

namespace Domojee.ViewModels
{
    public class ConfigurationViewModel
    {
        private string _address;
        private string _apikey;

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
        private bool _GeolocActivation;

        public bool GeolocActivation
        {
            set
            {
                _GeolocActivation = value;
                LocalSettings.Values["GeolocActivation"] = value;
            }

            get
            {
                return _GeolocActivation;
            }
        }

        private string _GeolocObjectId;

        public string GeolocObjectId
        {
            set
            {
                _GeolocObjectId = value;
                LocalSettings.Values["GeolocObjectId"] = value;
            }

            get
            {
                return _GeolocObjectId;
            }
        }

        private ApplicationDataContainer RoamingSettings = ApplicationData.Current.RoamingSettings;
        private ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        private const string settingAddress = "addressSetting";
        private const string settingAPIKey = "apikeySetting";

        public ConfigurationViewModel()
        {
            if (RoamingSettings.Values[settingAddress] != null)
            {
                _address = RoamingSettings.Values[settingAddress] as string;
                if (RoamingSettings.Values[settingAPIKey] != null)
                {
                    _apikey = RoamingSettings.Values[settingAPIKey] as string;
                    Populated = true;
                }
            }
            GeolocActivation = (LocalSettings.Values["GeolocActivation"] == null) ? false : Convert.ToBoolean(LocalSettings.Values["GeolocActivation"]);
            _GeolocObjectId = (LocalSettings.Values["GeolocObjectId"] == null) ? "" : LocalSettings.Values["GeolocObjectId"].ToString();
        }
    }
}