using System;
using Windows.Storage;

namespace Jeedom
{
    public class ConfigurationViewModel
    {
        private string _host;
        private string _path;
        private string _apikey;
        private bool? _selfSigned = false;
        private bool? _useSsl = false;
        private string _dnsuri;

        public Uri Uri
        {
            get
            {
                var uri = new UriBuilder(_useSsl == true ? "https" : "http", _host, _useSsl == true ? 443 : 80, _path);
                return uri.Uri;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }

            set
            {
                _path = value;
                RoamingSettings.Values[settingPath] = value;
            }
        }

        public bool? IsSelfSigned
        {
            get
            {
                return _selfSigned;
            }

            set
            {
                _selfSigned = value;
                RoamingSettings.Values[settingSelfSigned] = value;
            }
        }

        public bool? UseSSL
        {
            get
            {
                return _useSsl;
            }
            set
            {
                _useSsl = value;
                RoamingSettings.Values[settingUseSsl] = value;
            }
        }

        public string Host
        {
            set
            {
                _host = value;
                RoamingSettings.Values[settingHost] = value;
            }

            get
            {
                return _host;
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

        public string DnsUri
        {
            set
            {
                if (value != null)
                {
                    _dnsuri = value;
                    RoamingSettings.Values[settingDNS] = value;
                }
            }
            get
            {
                return _dnsuri;
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

        private bool _GeoFenceActivation;

        public bool GeoFenceActivation
        {
            set
            {
                _GeoFenceActivation = value;
                LocalSettings.Values["GeoFenceActivation"] = value;
            }

            get
            {
                return _GeoFenceActivation;
            }
        }

        private string _GeoFenceActivationDistance;

        public string GeoFenceActivationDistance
        {
            set
            {
                _GeoFenceActivationDistance = value;
                LocalSettings.Values["GeoFenceActivationDistance"] = value;
            }

            get
            {
                return _GeoFenceActivationDistance;
            }
        }

        private bool _NotificationActivation;

        public bool NotificationActivation
        {
            set
            {
                _NotificationActivation = value;
                LocalSettings.Values["NotificationActivation"] = value;
            }

            get
            {
                return _NotificationActivation;
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

        private string _NotificationObjectId;

        public string NotificationObjectId
        {
            set
            {
                _NotificationObjectId = value;
                LocalSettings.Values["NotificationObjectId"] = value;
            }

            get
            {
                return _NotificationObjectId;
            }
        }

        private ApplicationDataContainer RoamingSettings = ApplicationData.Current.RoamingSettings;
        private ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        private const string settingHost = "addressSetting";
        private const string settingPath = "pathSetting";
        private const string settingAPIKey = "apikeySetting";
        private const string settingUseSsl = "useSslSetting";
        private const string settingSelfSigned = "selfSignedSetting";
        private const string settingDNS = "DnsSetting";

        public ConfigurationViewModel()
        {
            Populated = true;

            _host = RoamingSettings.Values[settingHost] as string;
            if (_host == null)
                Populated = false;

            _apikey = RoamingSettings.Values[settingAPIKey] as string;
            if (_apikey == null)
                Populated = false;

            _path = RoamingSettings.Values[settingPath] as string;
            if (_path == null)
                Populated = false;

            if (RoamingSettings.Values[settingUseSsl] != null)
                _useSsl = Convert.ToBoolean(RoamingSettings.Values[settingUseSsl]);

            if (RoamingSettings.Values[settingSelfSigned] != null)
                _selfSigned = Convert.ToBoolean(RoamingSettings.Values[settingSelfSigned]);

            _GeolocActivation = (LocalSettings.Values["GeolocActivation"] == null) ? false : Convert.ToBoolean(LocalSettings.Values["GeolocActivation"]);
            _GeoFenceActivation = (LocalSettings.Values["GeoFenceActivation"] == null) ? false : Convert.ToBoolean(LocalSettings.Values["GeoFenceActivation"]);
            _NotificationActivation = (LocalSettings.Values["NotificationActivation"] == null) ? false : Convert.ToBoolean(LocalSettings.Values["NotificationActivation"]);

            _GeolocObjectId = (LocalSettings.Values["GeolocObjectId"] == null) ? "" : LocalSettings.Values["GeolocObjectId"].ToString();
            _NotificationObjectId = (LocalSettings.Values["NotificationObjectId"] == null) ? "" : LocalSettings.Values["NotificationObjectId"].ToString();
            _GeoFenceActivationDistance = (LocalSettings.Values["GeoFenceActivationDistance"] == null) ? "" : LocalSettings.Values["GeoFenceActivationDistance"].ToString();
        }
    }
}