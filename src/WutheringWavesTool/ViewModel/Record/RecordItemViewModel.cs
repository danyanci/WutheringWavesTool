using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Waves.Api.Helper;
using Waves.Api.Models.Enums;
using Waves.Api.Models.Record;
using Waves.Api.Models.Wrappers;
using WutheringWavesTool.Common;
using WutheringWavesTool.Models.Args;
using WutheringWavesTool.Models.Wrapper;

namespace WutheringWavesTool.ViewModel.Record;

public sealed partial class RecordItemViewModel : ViewModelBase, IDisposable
{
    private bool disposedValue;

    public CardPoolType Type { get; private set; }
    public RecordRequest Request { get; private set; }
    public IList<RecordCardItemWrapper> Items { get; set; }

    public RecordArgs DataItem { get; private set; }

    [ObservableProperty]
    public partial CardItemObservableCollection<RecordActivityFiveStarItemWrapper> StarItems { get; set; }

    internal void SetData(RecordArgs item)
    {
        this.DataItem = item;
        switch (item.Type)
        {
            case Waves.Api.Models.Enums.CardPoolType.RoleActivity:
                this.Items = item.RoleActivity.ToList();
                break;
            case Waves.Api.Models.Enums.CardPoolType.WeaponsActivity:
                this.Items = item.WeaponsActivity.ToList();
                break;
            case Waves.Api.Models.Enums.CardPoolType.RoleResident:
                this.Items = item.RoleActivity.ToList();
                break;
            case Waves.Api.Models.Enums.CardPoolType.WeaponsResident:
                this.Items = item.WeaponsResident.ToList();
                break;
            case Waves.Api.Models.Enums.CardPoolType.Beginner:
                this.Items = item.Beginner.ToList();
                break;
            case Waves.Api.Models.Enums.CardPoolType.BeginnerChoice:
                this.Items = item.BeginnerChoice.ToList();
                break;
            case Waves.Api.Models.Enums.CardPoolType.GratitudeOrientation:
                this.Items = item.GratitudeOrientation.ToList();
                break;
        }
    }

    [RelayCommand]
    void Loaded()
    {
        if (DataItem.Type == CardPoolType.RoleActivity)
        {
            StarItems = RecordHelper
                .FormatStartFive(
                    this.Items,
                    RecordHelper.FormatFiveRoleStar(this.DataItem.FiveGroup!)
                )!
                .Format(this.DataItem.AllRole)
                .Reverse()
                .ToCardItemObservableCollection();
        }
        if (DataItem.Type == CardPoolType.WeaponsActivity)
        {
            StarItems = RecordHelper
                .FormatStartFive(
                    this.Items,
                    RecordHelper.FormatFiveWeaponeRoleStar(this.DataItem.FiveGroup!)
                )!
                .Format(this.DataItem.AllWeapon)
                .Reverse()
                .ToCardItemObservableCollection();
        }
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                this.Items.Clear();
                this.StarItems.RemoveAllItem();
                Request = null;
                DataItem = null;
                this.StarItems = null;
                this.Items = null;
            }

            disposedValue = true;
        }
    }

    // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    // ~RecordItemViewModel()
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
