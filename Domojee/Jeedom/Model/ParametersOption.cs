using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace Jeedom.Model
{
    public class ParametersOption : INotifyPropertyChanged
    {
        private double _slider;
        public double slider
        {
            get
            {
                return _slider;
            }

            set
            {
                _slider = value;
                NotifyPropertyChanged();
            }
        }
        private string _title;
        public string title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
                NotifyPropertyChanged();
            }
        }
        public string _message;
        public string message
        {
            get
            {
                return _message;
            }

            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }
        public string color;
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}