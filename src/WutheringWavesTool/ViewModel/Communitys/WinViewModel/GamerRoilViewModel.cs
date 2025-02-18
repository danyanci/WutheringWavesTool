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

    #region RoleData
    [ObservableProperty]
    public partial string RolePic { get; set; }
    #endregion

    #region Weapon

    #endregion

    [RelayCommand]
    async Task Loaded()
    {
        var result = await WavesClient.GetGamerRoilDetily(this.ItemData.Item, this.ItemData.RoilId);
        if (result == null)
        {
            return;
        }
        this.RolePic = result.Role.RolePicUrl;
    }
}
