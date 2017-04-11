using Jeedom;
using Jeedom.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domojee.ViewModels
{
    internal class SceneViewModel
    {
        public ObservableCollection<Scene> SceneList => RequestViewModel.Instance.SceneList;
    }
}
