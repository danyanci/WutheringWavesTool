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
        WeakReferenceMessenger.Default.Register<SwitchRoleMessager>(this, SwitchRoleMethod);
    }

    private async void SwitchRoleMethod(object recipient, SwitchRoleMessager message)
    {
        this.roil = message.Data.Item;
        await Loaded();
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
        WeakReferenceMessenger.Default.UnregisterAll(this);
        this.RoleSkins.RemoveAll();
        this.WeaponSkins.RemoveAll();
    }
}
