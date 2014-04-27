﻿using Chibre_Server.Common;
using Chibre_Server.Game;
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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément du menu volant des paramètres, consultez la page http://go.microsoft.com/fwlink/?LinkId=273769

namespace Chibre_Server
{
    public sealed partial class SettingsFlyout : Windows.UI.Xaml.Controls.SettingsFlyout
    {
        private Settings settings;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public SettingsFlyout()
        {
            this.InitializeComponent();

            settings = Settings.GetInstance();
            defaultViewModel["Settings"] = settings;
        }
    }
}
