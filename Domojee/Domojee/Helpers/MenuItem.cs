using System;
using Windows.UI.Xaml.Controls;

namespace Domojee.Helpers
{
    public class MenuItem
    {
        public Symbol Icon { get; set; }
        public string Text { get; set; }
        public Type PageType { get; set; }
        public int Pos { get; set; }
    }
}