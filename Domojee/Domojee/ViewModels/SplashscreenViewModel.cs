using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace Domojee.ViewModels
{
    internal class SplashscreenViewModel : ViewModelBase
    {
        private bool Updating => Jeedom.RequestViewModel.GetInstance().Updating;
    }
}