using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Jeedom.Model
{
    [DataContract]
    public class JdObject : INotifyPropertyChanged
    {
        private string _id;

        [DataMember]
        public string id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

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

        private string _isVisible;

        [DataMember]
        public string isVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                _isVisible = value;
                NotifyPropertyChanged();
            }
        }

        private string _father_id;

        [DataMember]
        public string father_id
        {
            get
            {
                return _father_id;
            }

            set
            {
                _father_id = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<EqLogic> _eqLogics;

        [DataMember]
        public ObservableCollection<EqLogic> eqLogics
        {
            get
            {
                return _eqLogics;
            }

            set
            {
                _eqLogics = value;
                NotifyPropertyChanged();
            }
        }

        private DisplayInfo _display;

        [DataMember]
        public DisplayInfo display
        {
            get
            {
                return _display;
            }

            set
            {
                _display = value;
                NotifyPropertyChanged();
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

        private string _image;

        public string Image
        {
            get
            {
                return _image;
            }

            set
            {
                _image = value;
                NotifyPropertyChanged();
            }
        }

        public string Count
        {
            get
            {
                if (eqLogics != null)
                {
                    string s;
                    switch (eqLogics.Count)
                    {
                        case 0:
                            s = "aucun équipement";
                            break;

                        case 1:
                            s = "1 équipement";
                            break;

                        default:
                            s = eqLogics.Count + " équipements";
                            break;
                    }
                    return s;
                }
                else
                    return "aucun équipement";
            }
        }
    }
}