using Domojee.Services.SettingsServices;
using Jeedom;
using System;
using Template10.Mvvm;

namespace Domojee.ViewModels
{
    internal class ConnectViewModel : ViewModelBase
    {
        public static ConnectViewModel Instance { get; private set; }

        private ConfigurationViewModel _config;

        public ConnectViewModel()
        {
            Instance = this;
            _config = new ConfigurationViewModel();
        }

        public string Host
        {
            get { return _config.Host; }
            set { _config.Host = value; }
        }

        public string Path
        {
            get { return _config.Path; }
            set { _config.Path = value; }
        }

        public string DnsUri
        {
            get { return _config.DnsUri; }
            set { _config.DnsUri = value; }
        }

        public string ApiKey
        {
            get { return _config.ApiKey; }
            set { _config.ApiKey = value; }
        }

        public bool? IsSelfSigned
        {
            get { return _config.IsSelfSigned; }
            set { _config.IsSelfSigned = value; }
        }

        public bool? UseSSL
        {
            get { return _config.UseSSL; }
            set { _config.UseSSL = value; }
        }
    }
}