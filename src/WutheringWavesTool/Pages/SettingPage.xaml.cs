using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using WutheringWavesTool.Common;
using WutheringWavesTool.Models;
using WutheringWavesTool.ViewModel;

namespace WutheringWavesTool.Pages;

public sealed partial class SettingPage : Page, IPage
{
    public SettingPage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<SettingViewModel>();
    }

    public Type PageType => typeof(SettingPage);

    public SettingViewModel ViewModel { get; }

    private void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        this.ViewModel.WallpaperService.SetWrallpaper(
            (args.InvokedItem as WallpaperModel)!.FilePath
        );
    }
}
