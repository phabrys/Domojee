using Jeedom.Api.Json;
using Jeedom.Api.Json.Response;
using Jeedom.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace Domojee.ViewModels
{
    internal class AppViewModel : ViewModelBase
    {
        public static AppViewModel Instance { get; private set; }

        static AppViewModel()
        {
            Instance = Instance ?? new AppViewModel();
        }

        public static async Task<Error> PingJeedom()
        {
            var jsonrpc = new JsonRpcClient();
            if (await jsonrpc.SendRequest("ping"))
            {
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseString>();
                if (response.result == "pong")
                    return null;
            }

            return jsonrpc.Error;
        }
    }
}