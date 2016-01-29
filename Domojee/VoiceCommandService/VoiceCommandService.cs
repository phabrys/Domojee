using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Domojee.VoiceCommands
{
    public sealed class DomojeeVoiceCommandService : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();

            //
            // TODO: Insert code 
            //
            //

            _deferral.Complete();
        }
    }
}
