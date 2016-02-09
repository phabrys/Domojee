using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Jeedom.Model
{
    [DataContract]
    public class Command : INotifyPropertyChanged
    {
        [DataMember]
        public string name;

        [DataMember]
        public string logicalId;

        [DataMember]
        public string eqLogic_id;

        [DataMember]
        public string id;

        [DataMember]
        public string type;

        [DataMember]
        public string subType;

        [DataMember]
        private string unite;

        [DataMember]
        public string isVisible;

      /*  [DataMember]
        public GenericTypeEnum generic_type;*/

        private string __value;

        [DataMember(Name = "value")]
        public string _value
        {
            get
            {
                return __value;
            }
            set
            {
                __value = value;
                NotifyPropertyChanged();
                if (Parent != null)
                {
                    switch (name)
                    {
                        case "Consommation":
                            Parent.Consommation = value + " " + unite;
                            break;

                        case "Puissance":
                            Parent.Puissance = value + " " + unite;
                            break;

                        case "Etat":
                            Parent.State = value;
                            break;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public EqLogic Parent { get; set; }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
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

        public string Unite
        {
            get
            {
                return unite;
            }

            set
            {
                unite = value;
            }
        }
    }
}