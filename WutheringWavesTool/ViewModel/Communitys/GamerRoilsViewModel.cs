using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Waves.Api.Models.Communitys;
using Waves.Api.Models.Communitys.DataCenter;
using WavesLauncher.Core.Contracts;
using WutheringWavesTool.Common;
using WutheringWavesTool.Models;
using WutheringWavesTool.Models.Wrapper;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.ViewModel.Communitys;

public sealed partial class GameRoilsViewModel : ViewModelBase, IDisposable
{
    private bool disposedValue;
    private ObservableCollection<DataCenterRoilItemWrapper> cacheRoils;

    public IWavesClient WavesClient { get; }
    public ITipShow TipShow { get; }
    public GameRoilDataItem User { get; private set; }

    public GameRoilsViewModel(IWavesClient wavesClient, ITipShow tipShow)
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
        this.User = item;
        var data = await WavesClient.RefreshGamerDataAsync(item, this.CTS.Token);
        if (!data.Success)
        {
            TipShow.ShowMessage(data.Msg, Symbol.Clear);
            return;
        }
        var GameRoil = await WavesClient.GetGamerRoleDataAsync(User, this.CTS.Token);
        this.cacheRoils = FormatRole(GameRoil);
        if (this.SelectFilter == null)
        {
            this.SelectFilter = GamerFilter[0];
        }
    }

    partial void OnSelectFilterChanged(GamerRoleFilter value)
    {
        if (value.Id == 0)
        {
            this.RoleDatas = this.cacheRoils.ToObservableCollection();
        }
        else
        {
            this.RoleDatas = this
                .cacheRoils.Where(x => x.RoleData.AttributeId == value.Id)
                .ToObservableCollection();
        }
    }

    private ObservableCollection<DataCenterRoilItemWrapper> FormatRole(GamerRoleData gameRoil)
    {
        ObservableCollection<DataCenterRoilItemWrapper> items = new();
        if (gameRoil == null)
        {
            TipShow.ShowMessage("网络错误！", Symbol.Clear);
            return items;
        }
        foreach (var item in gameRoil.RoleList)
        {
            items.Add(new(item));
        }
        return items;
    }

    [ObservableProperty]
    public partial ObservableCollection<DataCenterRoilItemWrapper> RoleDatas { get; set; }

    [ObservableProperty]
    public partial GamerRoleFilter SelectFilter { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<GamerRoleFilter> GamerFilter { get; set; } =
        new ObservableCollection<GamerRoleFilter>()
        {
            new() { DisplayName = "全部", Id = 0 },
            new() { DisplayName = "冷凝", Id = 1 },
            new() { DisplayName = "热熔", Id = 2 },
            new() { DisplayName = "导电", Id = 3 },
            new() { DisplayName = "气动", Id = 4 },
            new() { DisplayName = "衍射", Id = 5 },
            new() { DisplayName = "湮灭", Id = 6 },
        };

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
