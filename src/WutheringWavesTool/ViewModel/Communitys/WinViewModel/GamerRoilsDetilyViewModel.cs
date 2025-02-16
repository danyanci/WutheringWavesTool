using WutheringWavesTool.Models.Wrapper.WindowRoils;

namespace WutheringWavesTool.ViewModel.Communitys.WinViewModel;

public sealed partial class GamerRoilsDetilyViewModel : ViewModelBase
{
    public IGamerRoilContext GamerRoilContext { get; }

    public GamerRoilsDetilyViewModel(
        IServiceScopeFactory serviceScopeFactory,
        IWavesClient wavesClient
    )
    {
        ServiceScopeFactory = serviceScopeFactory;
        this.Scope = ServiceScopeFactory.CreateScope();
        this.GamerRoilContext = Scope.ServiceProvider.GetRequiredKeyedService<IGamerRoilContext>(
            nameof(GamerRoilContext)
        );
        GamerRoilContext.SetScope(Scope);
        WavesClient = wavesClient;
    }

    public ShowRoleData Data { get; internal set; }
    public IServiceScopeFactory ServiceScopeFactory { get; }
    public IServiceScope Scope { get; }
    public IWavesClient WavesClient { get; }

    [ObservableProperty]
    public partial ObservableCollection<INavigationRoilsItem> Roles { get; set; }
    public long? SelectCache { get; internal set; }

    [ObservableProperty]
    public partial INavigationRoilsItem SelectItem { get; set; }

    [ObservableProperty]
    public partial string Title { get; set; }

    [RelayCommand]
    async Task Loaded()
    {
        await Task.Delay(200);
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
                Roles.Add(new NavigationRoilsDetilyItem(gr, Data.GameRoilDataItem));
            }
        }
        var first = this.Roles.First(x =>
        {
            if (x is NavigationRoilsDetilyItem)
                return true;
            return false;
        });
        if (this.SelectCache != null)
        {
            var filter = this
                .Roles.Where(x =>
                {
                    if (x is NavigationRoilsDetilyItem item && item.RoilId == this.SelectCache)
                    {
                        return true;
                    }
                    return false;
                })
                .FirstOrDefault();
            if (filter != null)
            {
                this.SelectItem = filter;
            }
        }
        else if (first != null)
        {
            if (first != null)
            {
                this.SelectItem = first;
            }
            else
            {
                throw new Exception("没有角色，你的账号数据为空");
            }
        }
        else
        {
            throw new Exception("没有角色，你的账号数据为空");
        }
    }

    internal void SwitchPage(NavigationRoilsDetilyItem item)
    {
        this.Title = item.RoleName;
        this.GamerRoilContext.NavigationService.NavigationTo<GamerRoilViewModel>(
            item,
            new DrillInNavigationTransitionInfo()
        );
    }

    /// <summary>
    /// 关闭作用域
    /// </summary>
    internal void Close()
    {
        this.Scope.Dispose();
    }
}
