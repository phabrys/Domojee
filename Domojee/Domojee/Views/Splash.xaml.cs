using Domojee.ViewModels;
using Jeedom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Domojee.Views
{
    public sealed partial class Splash : UserControl
    {
        public RequestViewModel ViewModel => RequestViewModel.Instance;

        public Splash(SplashScreen splashScreen)
        {
            this.InitializeComponent();
            this.DataContext = RequestViewModel.Instance;
            //TODO: Modifier le splashscreen et ne plus afficher de progression
        }
    }
}