using BackgroundWorker;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Domojee.Controls
{
    public sealed partial class ServerState : UserControl
    {
        public int MessageCount = RequestViewModel.MessageList.Count;

        public ServerState()
        {
            this.InitializeComponent();
        }
    }
}