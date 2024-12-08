using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using WutheringWavesTool.Common;
using WutheringWavesTool.Pages.Bases;
using WutheringWavesTool.ViewModel.GameViewModels;

namespace WutheringWavesTool.Pages.GamePages
{
    public sealed partial class GlobalGamePage : GamePageBase, IPage
    {
        public GlobalGamePage()
        {
            this.InitializeComponent();
            this.ViewModel = Instance.Service.GetRequiredService<GlobalGameViewModel>();
        }

        public Type PageType => typeof(GlobalGamePage);

        public GlobalGameViewModel ViewModel { get; }
    }
}
