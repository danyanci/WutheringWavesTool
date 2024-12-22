using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Waves.Api.Models.Communitys;
using Waves.Api.Models.Communitys.DataCenter;
using WavesLauncher.Core.Contracts;
using WutheringWavesTool.Common;
using WutheringWavesTool.Models.Wrapper;

namespace WutheringWavesTool.ViewModel.Communitys;

public partial class GamerDockViewModel : ViewModelBase, ICommunityViewModel
{
    public IWavesClient WavesClient { get; }

    [ObservableProperty]
    public partial ObservableCollection<DataCenterPhantomItemWrapper> GamerPhantoms { get; set; }

    [ObservableProperty]
    public partial GamerCalabashData GamerCalabash { get; set; }
    public GameRoilDataItem GameRoil { get; private set; }

    public GamerDockViewModel(IWavesClient wavesClient)
    {
        WavesClient = wavesClient;
    }

    public void Dispose()
    {
        GamerCalabash = null;
        this.GamerPhantoms.RemoveAll();
        this.GamerPhantoms = null;
    }

    private async Task RefreshDataAsync(GameRoilDataItem item)
    {
        this.GameRoil = item;
        this.GamerCalabash = await WavesClient.GetGamerCalabashDataAsync(GameRoil);
        this.GamerPhantoms = FormatData(GamerCalabash);
    }

    private ObservableCollection<DataCenterPhantomItemWrapper> FormatData(
        GamerCalabashData gamerCalabash
    )
    {
        ObservableCollection<DataCenterPhantomItemWrapper> items = new();
        foreach (var item in gamerCalabash.PhantomList)
        {
            items.Add(new DataCenterPhantomItemWrapper() { BassData = item });
        }
        return items;
    }

    public async Task SetDataAsync(GameRoilDataItem item)
    {
        await RefreshDataAsync(item);
    }
}
