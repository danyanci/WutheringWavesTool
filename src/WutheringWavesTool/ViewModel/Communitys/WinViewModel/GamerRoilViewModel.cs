using WutheringWavesTool.Models.Wrapper.WindowRoils;

namespace WutheringWavesTool.ViewModel.Communitys.WinViewModel;

public sealed partial class GamerRoilViewModel : ViewModelBase
{
    public NavigationRoilsDetilyItem ItemData { get; private set; }
    public IWavesClient WavesClient { get; }

    public GamerRoilViewModel(IWavesClient wavesClient)
    {
        WavesClient = wavesClient;
    }

    internal void SetData(NavigationRoilsDetilyItem? navigationRoilsDetilyItem)
    {
        if (navigationRoilsDetilyItem != null)
            this.ItemData = navigationRoilsDetilyItem;
    }

    [RelayCommand]
    async Task Loaded()
    {
        var result = await WavesClient.GetGamerRoilDetily(this.ItemData.Item, this.ItemData.RoilId);
    }
}
