using Jeedom;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.VoiceCommands;

namespace VoiceCommandService
{
    public sealed class DomojeeVoiceCommandService : IBackgroundTask
    {
        private VoiceCommandServiceConnection voiceServiceConnection;
        private BackgroundTaskDeferral serviceDeferral;

        private ResourceMap cortanaResourceMap;
        private ResourceContext cortanaContext;
        private DateTimeFormatInfo dateFormatInfo;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            serviceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCanceled;

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            cortanaResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");

            cortanaContext = ResourceContext.GetForViewIndependentUse();

            dateFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;

            if (triggerDetails != null && triggerDetails.Name == "DomojeeVoiceCommandService")
            {
                try
                {
                    voiceServiceConnection =
                        VoiceCommandServiceConnection.FromAppServiceTriggerDetails(
                            triggerDetails);

                    voiceServiceConnection.VoiceCommandCompleted += OnVoiceCommandCompleted;

                    VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();
                    var userMessage = new VoiceCommandUserMessage();

                    await RequestViewModel.Instance.DownloadObjects();
                    //await RequestViewModel.GetInstance().DownloadEqLogics();
                    //await RequestViewModel.GetInstance().DownloadCommands();
                    var JeedomCommande = voiceCommand.Properties["Commande"][0];
                    var JeedomObject = voiceCommand.Properties["Object"][0];
                    foreach (var Commande in RequestViewModel.CommandList.Where(w => w.name.Equals(voiceCommand.Properties["Commande"][0])))
                    {
                        foreach (var Equipement in RequestViewModel.EqLogicList.Where(w => w.id.Equals(Commande.eqLogic_id)))
                        {
                            foreach (var Object in RequestViewModel.ObjectList.Where(w => w.name.Equals(voiceCommand.Properties["Object"][0])))
                            {
                                if (Equipement.object_id == Object.id)
                                {
                                    await RequestViewModel.Instance.ExecuteCommand(Commande);
                                    userMessage.SpokenMessage = "La valeur de " + Commande.name + " de " + Object.name + " est de " + Commande.Value + Commande.unite;
                                }
                            }
                        }
                    }
                    // Ajout d'une requet jeedom pour retrouver la commande
                    switch (voiceCommand.CommandName)
                    {
                        case "cmdInObjectValue":
                            break;

                        case "cmdInObject":
                            break;

                        default:
                            LaunchAppInForeground();
                            break;
                    }

                    var response = VoiceCommandResponse.CreateResponse(userMessage);

                    response.AppLaunchArgument = "";

                    await voiceServiceConnection.RequestAppLaunchAsync(response);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Handling Voice Command failed " + ex.ToString());
                }
            }
        }

        private async Task ShowProgressScreen(string message)
        {
            var userProgressMessage = new VoiceCommandUserMessage();
            userProgressMessage.DisplayMessage = userProgressMessage.SpokenMessage = message;

            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userProgressMessage);
            await voiceServiceConnection.ReportProgressAsync(response);
        }

        private async void LaunchAppInForeground()
        {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = cortanaResourceMap.GetValue("LaunchingDomojee", cortanaContext).ValueAsString;

            var response = VoiceCommandResponse.CreateResponse(userMessage);

            response.AppLaunchArgument = "";

            await voiceServiceConnection.RequestAppLaunchAsync(response);
        }

        private void OnVoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            if (this.serviceDeferral != null)
            {
                this.serviceDeferral.Complete();
            }
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");
            if (this.serviceDeferral != null)
            {
                //Complete the service deferral
                this.serviceDeferral.Complete();
            }
        }
    }
}