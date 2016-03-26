using Jeedom.Mvvm;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Windows.UI.Xaml.Media;
using Windows.UI;
namespace Jeedom.Model
{
    [DataContract]

    public class Command : INotifyPropertyChanged
    {
        #region Propriétés sans notification de changement

        private string _name;

        [DataMember]
        public string logicalId;

        [DataMember]
        public string eqLogic_id;

        [DataMember]
        public string id;

        [DataMember]
        public string type;

        /*  [DataMember(Name = "configuration")]
          private CommandConfiguration _configuration;

          public CommandConfiguration configuration

          {
              get
              {
                  if (_configuration == null)
                      return new CommandConfiguration();
                  else
                      return _configuration;
              }

              set
              {
                  _configuration = value;
              }
          }*/

        [DataMember(Name = "display")]
        private CommandDisplay _display;

        public CommandDisplay display

        {
            get
            {
                if (_display == null)
                    return new CommandDisplay();
                else
                    return _display;
            }

            set
            {
                _display = value;
            }
        }

        [DataMember]
        public string subType;

        private string _unite;

        [DataMember(Name = "isVisible")]
        [JsonConverter(typeof(JsonConverters.BooleanJsonConverter))]
        private bool _isVisible;

        public bool isVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                _isVisible = Convert.ToBoolean(value);
            }
        }

        [DataMember(Name = "value")]
        private string _value = "";

        private bool _updating = false;

        public EqLogic Parent;

        #endregion Propriétés sans notification de changement

        #region Propriétés avec notification de changement

        public double datetime;

        public string Value
        {
            get
            {
                if (_value != "" && _value != null)
                {
                    switch (this.subType)
                    {
                        case "numeric":
                            return _value.Replace('.', ',');
                    }
                }
                    return _value;
            }

            set
            {
                _value = value;
                if (_value != "" && _value != null)
                {
                    switch (this.subType)
                    {
                        case "slider":
                            this.WidgetValue.slider = Convert.ToDouble(RequestViewModel.Instance.CommandList.Where(cmd => cmd.id.Equals(_value.Replace('#', ' ').Trim())).First().Value);
                            break;
                        case "message":
                            this.WidgetValue.message = RequestViewModel.Instance.CommandList.Where(cmd => cmd.id.Equals(_value.Replace('#', ' ').Trim())).First().Value;
                            break;
                        case "color":
                            var hexaColor = RequestViewModel.Instance.CommandList.Where(cmd => cmd.id.Equals(_value.Replace('#', ' ').Trim())).First().Value;
                            this.WidgetValue.color = new SolidColorBrush(Color.FromArgb(
                                255,
                                System.Convert.ToByte(hexaColor.Substring(1, 2), 16),
                                System.Convert.ToByte(hexaColor.Substring(3, 2), 16),
                                System.Convert.ToByte(hexaColor.Substring(5, 2), 16)));
                            break;
                    }
                }

                NotifyPropertyChanged();
                if (Parent != null)
                {
                    switch (name)
                    {
                        case "Consommation":
                            Parent.Consommation = value + " ";// + unite;
                            break;

                        case "Puissance":
                            Parent.Puissance = value + " ";// + unite;
                            break;

                        case "Etat":
                            Parent.State = value;
                            break;
                    }
                }
            }
        }

        [DataMember]
        public String name
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

        private ParametersOption _WidgetValue = new ParametersOption();

        public ParametersOption WidgetValue
        {
            get
            {
                return _WidgetValue;
            }

            set
            {
                _WidgetValue = value;
                NotifyPropertyChanged();
            }
        }

        [DataMember]
        public string unite
        {
            get
            {
                return _unite;
            }

            set
            {
                _unite = value;
                NotifyPropertyChanged();
            }
        }

        #endregion Propriétés avec notification de changement

        #region Private Methods

        private RelayCommand<object> _ExecCommand;

        public RelayCommand<object> ExecCommand
        {
            get
            {
                this._ExecCommand = this._ExecCommand ?? new RelayCommand<object>(async parameters =>
                {
                    try
                    {
                        this.Updating = true;
                        Parameters CmdParameters = new Parameters();
                        CmdParameters.id = this.id;
                        CmdParameters.name = this.name;
                        CmdParameters.options = this.WidgetValue;
                        await RequestViewModel.Instance.ExecuteCommand(this, CmdParameters);
                        await RequestViewModel.Instance.UpdateEqLogic(this.Parent);

                        this.Updating = false;
                    }
                    catch (Exception) { }
                });
                return this._ExecCommand;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Private Methods
    }
}