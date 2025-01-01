using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Waves.Api.Models.Communitys;
using Waves.Api.Models.Communitys.DataCenter;
using WavesLauncher.Core.Contracts;
using WutheringWavesTool.Common;
using WutheringWavesTool.Models.Wrapper;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.ViewModel.Communitys;

public sealed partial class GamerChallengeViewModel : ViewModelBase, IDisposable
{
    private List<ChallengeList> orginCountrys;

    public GamerChallengeViewModel(IWavesClient wavesClient, ITipShow tipShow)
    {
        Countrys = new();
        WavesClient = wavesClient;
        TipShow = tipShow;
    }

    public IWavesClient WavesClient { get; }
    public ITipShow TipShow { get; }

    [ObservableProperty]
    public partial ObservableCollection<DataCenterGamerChallengeCountryWrapper> Countrys { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<DataCenterChallengeBossItemWrapper> Items { get; set; }

    [ObservableProperty]
    public partial DataCenterGamerChallengeCountryWrapper SelectCountry { get; set; }

    public GameRoilDataItem RoilItem { get; private set; }

    async partial void OnSelectCountryChanged(DataCenterGamerChallengeCountryWrapper value)
    {
        if (value == null)
            return;
        if (Items != null && Items.Count > 0)
            Items.Clear();
        else
            Items = new();
        var result = await WavesClient.GetGamerChallengeDetails(
            this.RoilItem,
            value.CountryId,
            this.CTS.Token
        );
        if (result == null)
        {
            TipShow.ShowMessage("数据拉取失败！", Microsoft.UI.Xaml.Controls.Symbol.Clear);
            return;
        }
        var value2 = result!.ChallengeInfo.Detilys.GroupBy(x => x.BossId);
        foreach (var item in value2)
        {
            List<DataCenterGamerChallengeIndexListWrapper> indexList =
                new List<DataCenterGamerChallengeIndexListWrapper>();
            foreach (var index in item)
            {
                indexList.Add(new(index));
            }
            DataCenterChallengeBossItemWrapper listItem = new DataCenterChallengeBossItemWrapper(
                indexList,
                item
            );
            this.Items.Add(listItem);
        }
    }

    [RelayCommand]
    async Task Loaded()
    {
        var challData = await WavesClient.GetGamerChallengeIndexDataAsync(RoilItem, this.CTS.Token);
        this.orginCountrys = challData.ChallengeList;
        foreach (var item in orginCountrys)
        {
            Countrys.Add(new(item));
        }
        if (Countrys.Count > 0)
        {
            SelectCountry = Countrys[0];
        }
    }

    public void Dispose()
    {
        this.Countrys.RemoveAll();
        this.orginCountrys.Clear();
        foreach (var item in Items)
        {
            item.IndexWrapper?.RemoveAll();
            item.IndexWrapper = null;
            item.BossCover = null;
        }
        this.Items.RemoveAll();
        this.Countrys = null;
        this.orginCountrys = null;
        this.Items = null;
    }

    internal async Task SetDataAsync(GameRoilDataItem Roilitem)
    {
        this.RoilItem = Roilitem;
    }
}
