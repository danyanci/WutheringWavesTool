using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Waves.Api.Models.Communitys;
using Waves.Api.Models.Communitys.DataCenter;
using WavesLauncher.Core.Contracts;
using WutheringWavesTool.Common;
using WutheringWavesTool.Models.Wrapper;

namespace WutheringWavesTool.ViewModel.Communitys;

public sealed partial class GamerChallengeViewModel : ViewModelBase, IDisposable
{
    private List<ChallengeList> orginCountrys;

    public GamerChallengeViewModel(IWavesClient wavesClient)
    {
        Countrys = new();
        WavesClient = wavesClient;
    }

    public IWavesClient WavesClient { get; }

    [ObservableProperty]
    public partial ObservableCollection<DataCenterGamerChallengeCountryWrapper> Countrys { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<DataCenterGamerChallengeIndexListWrapper> Items { get; set; }

    [ObservableProperty]
    public partial DataCenterGamerChallengeCountryWrapper SelectCountry { get; set; }

    public GameRoilDataItem RoilItem { get; private set; }

    async partial void OnSelectCountryChanged(DataCenterGamerChallengeCountryWrapper value)
    {
        if (value == null)
            return;
        var first = orginCountrys
            .Where(x => x.Country.CountryId == value.CountryId)
            .FirstOrDefault();
        if (first == null)
            return;
        if (Items != null && Items.Count > 0)
            Items.Clear();
        else
            Items = new();
        foreach (var item in first.IndexList)
        {
            Items.Add(new(item));
        }
        var result = await WavesClient.GetGamerChallengeDetails(
            this.RoilItem,
            value.CountryId,
            this.CTS.Token
        );
    }

    [RelayCommand]
    async Task Loaded()
    {
        var challData = await WavesClient.GetGamerChallengeIndexDataAsync(RoilItem);
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
