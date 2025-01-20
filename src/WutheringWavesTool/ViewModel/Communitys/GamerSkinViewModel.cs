namespace WutheringWavesTool.ViewModel.Communitys;

public sealed partial class GamerSkinViewModel : ObservableObject, IDisposable
{
    private GameRoilDataItem roil;
    public IWavesClient WavesClient { get; }
    public ITipShow TipShow { get; }

    [ObservableProperty]
    public partial ObservableCollection<RoleSkinList> RoleSkins { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<WeaponSkinList> WeaponSkins { get; set; }

    public GamerSkinViewModel(IWavesClient wavesClient, ITipShow tipShow)
    {
        WavesClient = wavesClient;
        TipShow = tipShow;
    }

    public void SetData(GameRoilDataItem item)
    {
        this.roil = item;
    }

    [RelayCommand]
    async Task Loaded()
    {
        var skin = await this.WavesClient.GetGamerSkinAsync(roil);
        if (skin == null)
        {
            TipShow.ShowMessage("数据拉取失败！", Microsoft.UI.Xaml.Controls.Symbol.Clear);
            return;
        }
        this.RoleSkins = skin.RoleSkinList.ToObservableCollection();
        this.WeaponSkins = skin.WeaponSkinList.ToObservableCollection();
    }

    public void Dispose()
    {
        this.RoleSkins.RemoveAll();
        this.WeaponSkins.RemoveAll();
    }
}
