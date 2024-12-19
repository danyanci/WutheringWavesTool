using System;
using System.Threading.Tasks;
using Waves.Api.Models.Communitys;
using WutheringWavesTool.Common;

namespace WutheringWavesTool.ViewModel.Communitys;

public sealed partial class GameCountViewModel : ViewModelBase, IDisposable
{
    private bool disposedValue;

    public GameCountViewModel() { }

    internal async Task SetRoilAsync(GameRoilDataItem item)
    {
        await RefreshDataAsync(item);
    }

    private async Task RefreshDataAsync(GameRoilDataItem item) { }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)
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
