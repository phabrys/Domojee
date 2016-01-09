using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Domojee.Models;
using System.Runtime.CompilerServices;

namespace Domojee.Models
{
    [DataContract]
    public class EqLogic : INotifyPropertyChanged
    {
        private string _id;
        [DataMember]
        public string id { get; set; }

        private string _name;

        [DataMember]
        public string name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        private string _eqtype_name;
        [DataMember]
        public String eqType_name
        {
            get
            {
                return _eqtype_name;
            }
            set
            {
                _eqtype_name = value;
                switch (value)
                {
                    case "openzwave":
                        ColSpan = 2;
                        break;
                    case "energy":
                        ColSpan = 1;
                        break;
                    default:
                        break;
                }
                NotifyPropertyChanged();
            }
        }

        [DataMember]
        public ObservableCollection<Command> cmds;

        [DataMember]
        public string isVisible;

        [DataMember]
        public string object_id;

        [DataMember]
        public string logicalId;

        [DataMember]
        public string isEnable;

        private string _state;
        public string State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                NotifyPropertyChanged();
            }
        }

        private string _consommation;
        public string Consommation
        {
            get
            {
                return _consommation;
            }
            set
            {
                _consommation = value;
                NotifyPropertyChanged();
            }
        }

        private string _puissance;
        public string Puissance
        {
            get
            {
                return _puissance;
            }
            set
            {
                _puissance = value;
                NotifyPropertyChanged();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public JdObject Parent;

        private bool _updating;
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

        public int ColSpan = 1;
        public int RowSpan = 1;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Command> GetActionsCmds()
        {
            IEnumerable<Command> results = cmds.Where(c => c.type == "action");
            return new ObservableCollection<Command>(results);
        }

        public ObservableCollection<Command> GetInformationsCmds()
        {
            IEnumerable<Command> results = cmds.Where(c => c.type == "info");
            return new ObservableCollection<Command>(results);
        }
    }
}
