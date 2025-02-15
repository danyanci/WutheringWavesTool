using WutheringWavesTool.Models.Wrapper.WindowRoils;

namespace WutheringWavesTool.ViewModel.Communitys.WinViewModel;

public sealed partial class GamerRoilsDetilyViewModel : ViewModelBase
{
    public GamerRoilsDetilyViewModel(IWavesClient wavesClient)
    {
        WavesClient = wavesClient;
    }

    public ShowRoleData Data { get; internal set; }
    public IWavesClient WavesClient { get; }

    [ObservableProperty]
    public partial ObservableCollection<INavigationRoilsItem> Roles { get; set; }

    [RelayCommand]
    async Task Loaded()
    {
        var GameRoil = await WavesClient.GetGamerRoleDataAsync(
            Data.GameRoilDataItem,
            this.CTS.Token
        );
        if (GameRoil == null)
            return;
        Roles = new();
        var group = GameRoil.RoleList.GroupBy(x => x.AttributeId);
        foreach (var item in group)
        {
            this.Roles.Add(new NavigationRoilsTypeItem(item.First().AttributeName));
            foreach (var gr in item)
            {
                Roles.Add(new NavigationRoilsDetilyItem(gr));
            }
        }
    }
}
