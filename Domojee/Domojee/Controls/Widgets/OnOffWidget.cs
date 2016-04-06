using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Domojee.Widgets
{
    [TemplatePart(Name = "PART_Root", Type = typeof(Border))]
    public sealed class OnOffWidget : Control
    {
        #region Public Fields

        public static readonly DependencyProperty ColorProperty =
         DependencyProperty.Register("Color", typeof(Brush), typeof(OnOffWidget), new PropertyMetadata(null));

        public static readonly DependencyProperty ColSpanProperty =
            DependencyProperty.Register("ColSpan", typeof(int), typeof(OnOffWidget), new PropertyMetadata(1));

        public static readonly DependencyProperty EqLogicProperty =
            DependencyProperty.Register("EqLogic", typeof(Jeedom.Model.EqLogic), typeof(OnOffWidget), new PropertyMetadata(null));

        [DefaultValue("&#xF0EB;")]
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(OnOffWidget), new PropertyMetadata("&#xF0EB;"));

        public static readonly DependencyProperty OffColorProperty =
            DependencyProperty.Register("OffColor", typeof(Brush), typeof(OnOffWidget), new PropertyMetadata(null));

        public static readonly DependencyProperty OnColorProperty =
            DependencyProperty.Register("OnColor", typeof(Brush), typeof(OnOffWidget), new PropertyMetadata(null));

        public static readonly DependencyProperty RowSpanProperty =
            DependencyProperty.Register("RowSpan", typeof(int), typeof(OnOffWidget), new PropertyMetadata(1));

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(Boolean), typeof(OnOffWidget), new PropertyMetadata(true, OnStatePropertyChanged));

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register("Subtitle", typeof(string), typeof(OnOffWidget), new PropertyMetadata("Sous-titre"));

        [DefaultValue("Titre")]
        public static readonly DependencyProperty TitleProperty =
           DependencyProperty.Register("Title", typeof(string), typeof(OnOffWidget), new PropertyMetadata("Titre", OnTitlePropertyChanged));

        #endregion Public Fields

        #region Private Fields

        private Border part_root;

        #endregion Private Fields

        #region Public Constructors

        public OnOffWidget()
        {
            this.DefaultStyleKey = typeof(OnOffWidget);
            EqLogic = new Jeedom.Model.EqLogic();
            //this.Click += MainClick;
        }

        #endregion Public Constructors

        #region Public Properties

        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public int ColSpan
        {
            get { return (int)GetValue(ColSpanProperty); }

            set { SetValue(ColSpanProperty, value); }
        }

        public Jeedom.Model.EqLogic EqLogic
        {
            get { return (Jeedom.Model.EqLogic)GetValue(EqLogicProperty); }
            set { SetValue(EqLogicProperty, value); }
        }

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public Brush OffColor
        {
            get { return (Brush)GetValue(OffColorProperty); }
            set { SetValue(OffColorProperty, value); }
        }

        public Brush OnColor
        {
            get { return (Brush)GetValue(OnColorProperty); }
            set { SetValue(OnColorProperty, value); }
        }

        public int RowSpan
        {
            get { return (int)GetValue(RowSpanProperty); }

            set { SetValue(RowSpanProperty, value); }
        }

        public Boolean State
        {
            get { return (Boolean)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public string Subtitle
        {
            get { return (string)GetValue(SubtitleProperty); }
            set { SetValue(SubtitleProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Convertie une couleur hexa
        /// </summary>
        /// <param name="hexaColor"></param>
        /// <returns></returns>
        public static SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            return new SolidColorBrush(
                Windows.UI.Color.FromArgb(
                    255,
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16)
                )
            );
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            part_root = GetTemplateChild("PART_Root") as Border;

            if (this.OffColor == null)
                this.OffColor = GetColorFromHexa("#434A54");
            if (this.OnColor == null)
                this.OnColor = GetColorFromHexa("#8CC152");

            OnStatePropertyChanged(this, null);
        }

        #endregion Protected Methods

        #region Private Methods

        private static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnOffWidget source = d as OnOffWidget;
            if (source.State == true)
            {
                source.Color = source.OnColor;
            }
            else
            {
                source.Color = source.OffColor;
            }
        }

        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var i = 1;
        }

        private void MainClick(object sender, RoutedEventArgs e)
        {
            if (State == true)
            {
                State = false;
            }
            else
            {
                State = true;
            }
        }

        #endregion Private Methods
    }
}