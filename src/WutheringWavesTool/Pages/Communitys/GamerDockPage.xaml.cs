using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Waves.Api.Models.Communitys;
using WutheringWavesTool.Common;
using WutheringWavesTool.ViewModel.Communitys;

namespace WutheringWavesTool.Pages.Communitys;

public sealed partial class GamerDockPage : Page, IPage, IDisposable
{
    private bool disposedValue;

    public GamerDockPage()
    {
        this.InitializeComponent();

        this.ViewModel = Instance.Service.GetRequiredService<GamerDockViewModel>();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is GameRoilDataItem item)
        {
            await this.ViewModel.SetDataAsync(item);
        }
        base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        this.Dispose();
        GC.Collect();
        base.OnNavigatedFrom(e);
    }

    public GamerDockViewModel ViewModel { get; }

    public Type PageType => typeof(GamerDockViewModel);

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                this.Bindings.StopTracking();
                this.ViewModel.Dispose();
            }

            // TODO: 释放未托管的资源(未托管的对象)并重写终结器
            // TODO: 将大型字段设置为 null
            disposedValue = true;
        }
    }

    // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    // ~GamerDockPage()
    // {
    //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
