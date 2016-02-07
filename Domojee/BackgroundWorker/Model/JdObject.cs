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

        [DataMember]
        public string name;

        [DataMember]
        public string isVisible;

        [DataMember]
        public string father_id;

        [DataMember]
        public ObservableCollection<EqLogic> eqLogics;

        [DataMember]
        public DisplayInfo display;

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