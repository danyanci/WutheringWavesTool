using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Waves.Api.Helper;
using Waves.Api.Models.Enums;
using Waves.Api.Models.Record;
using WutheringWavesTool.Common;
using WutheringWavesTool.Models.Args;
using WutheringWavesTool.Services.Contracts;
using WutheringWavesTool.ViewModel.Record;

namespace WutheringWavesTool.ViewModel;

public sealed partial class PlayerRecordViewModel : ViewModelBase, IDisposable
{
    public IPlayerRecordContext PlayerRecordContext { get; }

    public PlayerRecordViewModel(IServiceScopeFactory serviceScopeFactory)
    {
        ServiceScopeFactory = serviceScopeFactory;
        this.Scope = ServiceScopeFactory.CreateScope();
        this.PlayerRecordContext =
            Scope.ServiceProvider.GetRequiredKeyedService<IPlayerRecordContext>("PlayerRecord");
        this.PlayerRecordContext.SetScope(this.Scope);
    }

    [ObservableProperty]
    public partial bool IsLoadRecord { get; set; } = false;

    [ObservableProperty]
    public partial CardPoolType? SelectType { get; set; } = null;

    [ObservableProperty]
    public partial ObservableCollection<CardPoolType> CardPools { get; set; } =
        new ObservableCollection<CardPoolType>()
        {
            CardPoolType.RoleActivity,
            CardPoolType.WeaponsActivity,
            CardPoolType.RoleResident,
            CardPoolType.WeaponsResident,
            CardPoolType.Beginner,
            CardPoolType.BeginnerChoice,
            CardPoolType.GratitudeOrientation,
        };

    [RelayCommand]
    async Task ShowInputRecordAsync()
    {
        var link = await PlayerRecordContext.ShowInputRecordAsync(null);
        var request = RecordHelper.GetRecorRequest(link);
        if (request == null)
        {
            this.PlayerRecordContext.TipShow.ShowMessage(
                "抽卡链接无效",
                Microsoft.UI.Xaml.Controls.Symbol.Clear
            );
            this.SelectType = null;
            this.IsLoadRecord = false;
            return;
        }
        this.Request = request;
        var items = await RecordHelper.GetRecordAsync(Request, CardPoolType.RoleActivity);
        if (items == null)
        {
            this.PlayerRecordContext.TipShow.ShowMessage(
                "抽卡链接过期",
                Microsoft.UI.Xaml.Controls.Symbol.Clear
            );
            this.SelectType = null;
            this.IsLoadRecord = false;
            return;
        }
        this.FiveGroup = await RecordHelper.GetFiveGroupAsync();
        var allRole = await RecordHelper.GetAllRoleAsync();
        this.StartRole = RecordHelper.FormatFiveRoleStar(FiveGroup);
        this.StartWeapons = RecordHelper.FormatFiveWeaponeRoleStar(FiveGroup);
        this.IsLoadRecord = true;
        SelectType = CardPoolType.RoleActivity;
    }

    private bool disposedValue;

    partial void OnSelectTypeChanged(CardPoolType? value)
    {
        if (value == null)
            return;
        var arg = new RecordArgs()
        {
            Request = this.Request,
            Roles = this.StartRole,
            Weapons = this.StartWeapons,
            Type = value.Value,
        };
        this.PlayerRecordContext.NavigationService.NavigationTo<RecordItemViewModel>(
            arg,
            new DrillInNavigationTransitionInfo()
        );
    }

    public async Task Loaded(Frame frame = null)
    {
        await this.ShowInputRecordAsync();
    }

    public IServiceScopeFactory ServiceScopeFactory { get; }
    public IServiceScope Scope { get; }
    public RecordRequest Request { get; private set; }
    public FiveGroupModel? FiveGroup { get; private set; }
    public List<int> StartRole { get; private set; }
    public List<int> StartWeapons { get; private set; }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                this.PlayerRecordContext.NavigationService.UnRegisterView();
                this.PlayerRecordContext.Dispose();
                Scope.Dispose();
            }
            disposedValue = true;
        }
    }

    // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    // ~PlayerRecordViewModel()
    // {
    //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
