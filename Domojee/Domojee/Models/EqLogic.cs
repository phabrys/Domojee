using Domojee.Models;
using Domojee.Mvvm;
using Domojee.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Domojee.Models
{
    [DataContract]
    public class EqLogic : INotifyPropertyChanged
    {
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

        private EqLogicDisplay _display;

        [DataMember]
        public EqLogicDisplay display
        {
            get
            {
                return _display;
            }
            set
            {
                _display = value;
                if (_display != null)
                {
                    if (_display.customParameters != null)
                    {
                        if (_display.customParameters.DomojeeColSpan != 0)
                            ColSpan = _display.customParameters.DomojeeColSpan;
                        if (_display.customParameters.DomojeeRowSpan != 0)
                            RowSpan = _display.customParameters.DomojeeRowSpan;
                    }
                }
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<Command> _cmds;

        [DataMember]
        public ObservableCollection<Command> cmds
        {
            get
            {
                return _cmds;
            }

            set
            {
                _cmds = value;
                NotifyPropertyChanged();
            }
        }

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

        private bool _onVisibility = false;

        public bool OnVisibility
        {
            get
            {
                return _onVisibility;
            }
            set
            {
                _onVisibility = value;
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

        private RelayCommand<object> _execCommand;

        public RelayCommand<object> ExecCommand
        {
            get
            {
                this._execCommand = this._execCommand ?? new RelayCommand<object>(async parameters =>
                {
                    // Cherche la commande
                    var cmd = cmds.Where(c => c.name.ToLower() == parameters.ToString().ToLower()).First();
                    cmd.Updating = true;
                    await RequestViewModel.GetInstance().ExecuteCommand(cmd);
                    cmd.Updating = false;
                    Debug.WriteLine(parameters);
                });
                return this._execCommand;
            }
        }

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