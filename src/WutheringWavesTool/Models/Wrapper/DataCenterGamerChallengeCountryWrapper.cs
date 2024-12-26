using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using Waves.Api.Models.Communitys.DataCenter;
using WinUICommunity;

namespace WutheringWavesTool.Models.Wrapper;

public sealed partial class DataCenterGamerChallengeIndexListWrapper : ObservableObject
{
    [ObservableProperty]
    public partial BitmapImage Cover { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial int Value { get; set; }

    public int BossId { get; }

    public DataCenterGamerChallengeIndexListWrapper(IndexList value)
    {
        Cover = new BitmapImage(new(value.BossHeadIcon));
        Value = value.Difficulty;
        Name = value.BossName;
        BossId = value.BossId;
    }
}

public sealed partial class DataCenterGamerChallengeCountryWrapper : ObservableObject
{
    [ObservableProperty]
    public partial string CountryName { get; set; }

    [ObservableProperty]
    public partial int CountryId { get; set; }

    [ObservableProperty]
    public partial BitmapImage Cover { get; set; }

    public DataCenterGamerChallengeCountryWrapper(ChallengeList country)
    {
        this.CountryName = country.Country.CountryName;
        this.CountryId = country.Country.CountryId;
        this.Cover = new BitmapImage(new(country.Country.HomePageIcon));
    }
}
