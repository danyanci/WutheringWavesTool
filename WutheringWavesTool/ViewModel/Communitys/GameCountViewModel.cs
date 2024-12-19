using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Waves.Api.Models.Communitys;
using WavesLauncher.Core.Contracts;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.ViewModel.Communitys;

public sealed partial class GameCountViewModel : ViewModelBase, IDisposable
{
    private bool disposedValue;

    public IWavesClient WavesClient { get; }
    public ITipShow TipShow { get; }

    public GameCountViewModel(IWavesClient wavesClient, ITipShow tipShow)
    {
        WavesClient = wavesClient;
        TipShow = tipShow;
    }

    internal async Task SetRoilAsync(GameRoilDataItem item)
    {
        await RefreshDataAsync(item);
    }

    private async Task RefreshDataAsync(GameRoilDataItem item)
    {
        var data = await WavesClient.RefreshGamerDataAsync(item, this.CTS.Token);
        if (!data.Success)
        {
            TipShow.ShowMessage(data.Msg, Symbol.Clear);
            return;
        }
        var gameData = await WavesClient.GetGamerDataAsync(item, this.CTS.Token);
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)
                this.CTS.Cancel();
            }

            // TODO: 释放未托管的资源(未托管的对象)并重写终结器
            // TODO: 将大型字段设置为 null
            disposedValue = true;
        }
    }

    // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    // ~GameCountViewModel()
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
