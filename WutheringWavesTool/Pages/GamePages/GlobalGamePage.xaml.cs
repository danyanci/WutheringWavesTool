using System;
using Microsoft.UI.Xaml.Controls;
using WutheringWavesTool.Common;
using WutheringWavesTool.Pages.Bases;

namespace WutheringWavesTool.Pages.GamePages
{
    public sealed partial class GlobalGamePage : GamePageBase, IPage
    {
        public GlobalGamePage()
        {
            this.InitializeComponent();
        }

        public Type PageType => typeof(GlobalGamePage);
    }
}
