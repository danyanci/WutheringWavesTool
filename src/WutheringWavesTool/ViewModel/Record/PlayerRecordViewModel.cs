using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Waves.Api.Helper;
using Waves.Api.Models.Enums;
using Waves.Api.Models.Record;
using Waves.Api.Models.Wrappers;
using WutheringWavesTool.Common;
using WutheringWavesTool.Models.Args;
using WutheringWavesTool.Services.Contracts;
using WutheringWavesTool.ViewModel.Record;

namespace WutheringWavesTool.ViewModel;

public sealed partial class PlayerRecordViewModel : ViewModelBase, IDisposable
{
    public IPlayerRecordContext PlayerRecordContext { get; }

    public IServiceScopeFactory ServiceScopeFactory { get; }
    public IServiceScope Scope { get; }
    public RecordRequest Request { get; private set; }
    public FiveGroupModel? FiveGroup { get; private set; }
    public List<CommunityRoleData>? AllRole { get; private set; }
    public List<CommunityWeaponData>? AllWeapon { get; private set; }
    public List<int> StartRole { get; private set; }
    public List<int> StartWeapons { get; private set; }
    public List<RecordCardItemWrapper>? RoleActivity { get; private set; }
    public List<RecordCardItemWrapper>? WeaponsActivity { get; private set; }
    public List<RecordCardItemWrapper>? RoleResident { get; private set; }
    public List<RecordCardItemWrapper>? WeaponsResident { get; private set; }
    public List<RecordCardItemWrapper>? Beginner { get; private set; }
    public List<RecordCardItemWrapper>? BeginnerChoice { get; private set; }
    public List<RecordCardItemWrapper>? GratitudeOrientation { get; private set; }

    [ObservableProperty]
    public partial bool IsLoadRecord { get; set; } = false;

    [ObservableProperty]
    public partial CardPoolType? SelectType { get; set; } = null;

    #region 抽卡数据
    [ObservableProperty]
    public partial int AllCount { get; set; }

    [ObservableProperty]
    public partial double ActivityAvg { get; set; }

    [ObservableProperty]
    public partial double ResidentAvg { get; set; }

    [ObservableProperty]
    public partial double Guaranteed { get; set; }

    [ObservableProperty]
    public partial double ScoreValue { get; set; }

    [ObservableProperty]
    public partial double StarAvg { get; set; }
    #endregion

    public PlayerRecordViewModel(IServiceScopeFactory serviceScopeFactory)
    {
        ServiceScopeFactory = serviceScopeFactory;
        this.Scope = ServiceScopeFactory.CreateScope();
        this.PlayerRecordContext =
            Scope.ServiceProvider.GetRequiredKeyedService<IPlayerRecordContext>("PlayerRecord");
        this.PlayerRecordContext.SetScope(this.Scope);
    }

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
        if (string.IsNullOrWhiteSpace(link.Item1) && link.Item2 == null)
        {
            this.SelectType = null;
            this.IsLoadRecord = false;
            return;
        }
        if (!string.IsNullOrWhiteSpace(link.Item1))
        {
            var request = RecordHelper.GetRecorRequest(link.Item1);
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
            if (await WriteCacheAsync())
            {
                this.FiveGroup = await RecordHelper.GetFiveGroupAsync();
                this.AllRole = await RecordHelper.GetAllRoleAsync();
                this.AllWeapon = await RecordHelper.GetAllWeaponAsync();
                this.StartRole = RecordHelper.FormatFiveRoleStar(FiveGroup);
                this.StartWeapons = RecordHelper.FormatFiveWeaponeRoleStar(FiveGroup);
                CalculateRange();
                this.IsLoadRecord = true;
                SelectType = CardPoolType.RoleActivity;
            }
            else
            {
                this.PlayerRecordContext.TipShow.ShowMessage(
                    "写入抽卡缓存失败",
                    Microsoft.UI.Xaml.Controls.Symbol.Clear
                );
                this.SelectType = null;
                this.IsLoadRecord = false;
            }
        }
        else
        {
            this.FiveGroup = await RecordHelper.GetFiveGroupAsync();
            this.AllRole = await RecordHelper.GetAllRoleAsync();
            this.AllWeapon = await RecordHelper.GetAllWeaponAsync();
            this.StartRole = RecordHelper.FormatFiveRoleStar(FiveGroup);
            this.StartWeapons = RecordHelper.FormatFiveWeaponeRoleStar(FiveGroup);
            RoleActivity = link.Item2.RoleActivityItems.ToList();
            WeaponsActivity = link.Item2.WeaponsActivityItems.ToList();
            WeaponsResident = link.Item2.WeaponsResidentItems.ToList();
            RoleResident = link.Item2.RoleResidentItems.ToList();
            Beginner = link.Item2.RoleActivityItems.ToList();
            BeginnerChoice = link.Item2.BeginnerChoiceItems.ToList();
            GratitudeOrientation = link.Item2.GratitudeOrientationItems.ToList();
            CalculateRange();
            this.IsLoadRecord = true;
            SelectType = CardPoolType.RoleActivity;
        }
    }

    private void CalculateRange()
    {
        if (RoleActivity == null || FiveGroup == null)
            return;
        this.ActivityAvg = Math.Round(
            RecordHelper
                .FormatRecordFive(this.RoleActivity)
                .Concat(RecordHelper.FormatRecordFive(this.WeaponsActivity))
                .CalculateAvg(),
            2
        );
        this.ResidentAvg = Math.Round(
            RecordHelper
                .FormatRecordFive(this.RoleResident)
                .Concat(RecordHelper.FormatRecordFive(this.WeaponsResident))
                .CalculateAvg(),
            2
        );
        var roleAAvg = RecordHelper.FormatRecordFive(this.RoleActivity).CalculateAvg();
        var weaponAAvg = RecordHelper.FormatRecordFive(this.WeaponsActivity!).CalculateAvg();
        var resident = RecordHelper
            .FormatRecordFive(this.RoleResident!.Concat(this.WeaponsResident!))
            .CalculateAvg();
        var range = RecordHelper
            .FormatStartFive(RoleActivity, RecordHelper.FormatFiveRoleStar(FiveGroup!))!
            .GetGuaranteedRange();
        var value = Math.Round(RecordHelper.Score(range, roleAAvg, weaponAAvg, resident), 2);
        this.Guaranteed = range;
        this.ScoreValue = value;
        this.StarAvg = Math.Round(
            RecordHelper
                .FormatRecordFive(this.RoleActivity)
                .Concat(RecordHelper.FormatRecordFive(this.RoleResident))
                .Concat(RecordHelper.FormatRecordFive(this.WeaponsActivity))
                .Concat(RecordHelper.FormatRecordFive(this.WeaponsResident))
                .Concat(RecordHelper.FormatRecordFive(this.WeaponsActivity))
                .CalculateAvg(),
            2
        );
        this.AllCount =
            RoleActivity.Count
            + WeaponsActivity.Count
            + RoleResident.Count
            + WeaponsResident.Count
            + Beginner.Count
            + BeginnerChoice.Count
            + GratitudeOrientation.Count;
    }

    private async Task<bool> WriteCacheAsync()
    {
        RoleActivity = await RecordHelper.GetRecordAsync(this.Request, CardPoolType.RoleActivity);
        WeaponsActivity = await RecordHelper.GetRecordAsync(
            this.Request,
            CardPoolType.WeaponsActivity
        );
        RoleResident = await RecordHelper.GetRecordAsync(this.Request, CardPoolType.RoleResident);
        WeaponsResident = await RecordHelper.GetRecordAsync(
            this.Request,
            CardPoolType.WeaponsResident
        );
        Beginner = await RecordHelper.GetRecordAsync(this.Request, CardPoolType.Beginner);
        BeginnerChoice = await RecordHelper.GetRecordAsync(
            this.Request,
            CardPoolType.BeginnerChoice
        );
        GratitudeOrientation = await RecordHelper.GetRecordAsync(
            this.Request,
            CardPoolType.GratitudeOrientation
        );
        if (
            RoleActivity == null
            || WeaponsActivity == null
            || RoleResident == null
            || WeaponsResident == null
            || Beginner == null
            || BeginnerChoice == null
            || GratitudeOrientation == null
        )
        {
            this.PlayerRecordContext.TipShow.ShowMessage("数据拉取失败!", Symbol.Clear);
            return false;
        }
        var guid = Guid.NewGuid();
        await this.PlayerRecordContext.RecordCacheService.CreateRecordAsync(
            new RecordCacheDetily()
            {
                Guid = guid.ToString(),
                Name = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:FFFF")}",
                RoleActivityItems = RoleActivity,
                RoleResidentItems = RoleResident,
                WeaponsActivityItems = WeaponsActivity,
                WeaponsResidentItems = WeaponsResident,
                BeginnerItems = Beginner,
                BeginnerChoiceItems = BeginnerChoice,
                GratitudeOrientationItems = GratitudeOrientation,
            }
        );
        return true;
    }

    private bool disposedValue;

    partial void OnSelectTypeChanged(CardPoolType? value)
    {
        if (value == null)
            return;
        var arg = new RecordArgs(
            RoleActivity,
            WeaponsActivity,
            RoleResident,
            WeaponsResident,
            Beginner,
            BeginnerChoice,
            GratitudeOrientation
        )
        {
            Request = this.Request,
            Roles = this.StartRole,
            Weapons = this.StartWeapons,
            FiveGroup = this.FiveGroup,
            AllRole = this.AllRole,
            AllWeapon = this.AllWeapon,
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
