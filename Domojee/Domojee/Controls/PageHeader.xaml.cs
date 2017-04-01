using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Domojee.Controls
{
    public sealed partial class PageHeader : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(PageHeader), new PropertyMetadata(""));
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Symbol, typeof(PageHeader), new PropertyMetadata(""));

        public PageHeader()
        {
            this.InitializeComponent();
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public Symbol Icon
        {
            get { return (Symbol)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
    }
}
