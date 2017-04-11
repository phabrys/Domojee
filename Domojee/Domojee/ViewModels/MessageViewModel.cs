using Jeedom;
using Jeedom.Model;
using System.Collections.ObjectModel;

namespace Domojee.ViewModels
{
    internal class MessageViewModel
    {
        public ObservableCollection<Message> MessageList => RequestViewModel.Instance.MessageList;
    }
}