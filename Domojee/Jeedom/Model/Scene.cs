using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Jeedom.Model
{
    [DataContract]
    public class Scene : INotifyPropertyChanged
    {
        [DataMember]
        public string object_id;

        [DataMember]
        public string isActive;

        [DataMember]
        public string state;

        [DataMember]
        public string timeout;

        [DataMember]
        public string id;

        [DataMember]
        public string isVisible;

        [DataMember]
        public string description;

        [DataMember]
        public string lastLaunch;

        public string LastLaunch
        {
            get
            {
                return lastLaunch;
            }

            set
            {
                lastLaunch = value;
                NotifyPropertyChanged();
            }
        }

        [DataMember]
        public string name;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool _updating = false;

        public bool Updating
        {
            get
            {
                return _updating;
            }

            set
            {
                _updating = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }
    }
}