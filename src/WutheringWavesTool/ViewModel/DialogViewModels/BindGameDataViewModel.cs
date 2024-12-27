using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Waves.Api.Models.Communitys;
using Waves.Core.GameContext;
using Waves.Core.GameContext.Contexts;
using WavesLauncher.Core.Contracts;
using WutheringWavesTool.Common;
using WutheringWavesTool.Common.Bases;
using WutheringWavesTool.Models;
using WutheringWavesTool.Models.Messanger;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.ViewModel.DialogViewModels;

public sealed partial class BindGameDataViewModel : DialogViewModelBase, IDisposable
{
    public BindGameDataViewModel(IAppContext<App> appContext, IWavesClient wavesClient)
        : base(appContext)
    {
        WavesClient = wavesClient;
    }

    public IGameContext? GameContext { get; private set; }
    public IWavesClient WavesClient { get; }

    [ObservableProperty]
    public partial ObservableCollection<GameRoilDataItem> Roils { get; set; }

    [ObservableProperty]
    public partial GameRoilDataItem SelectRoil { get; set; }

    internal void InitCore(string str)
    {
        switch (str)
        {
            case "Main":
                this.GameContext = Instance.Service!.GetRequiredKeyedService<IGameContext>(
                    nameof(MainGameContext)
                );
                break;
            case "Global":
                this.GameContext = Instance.Service!.GetRequiredKeyedService<IGameContext>(
                    nameof(GlobalGameContext)
                );
                break;
            case "Bilibili":
                this.GameContext = Instance.Service!.GetRequiredKeyedService<IGameContext>(
                    nameof(BilibiliGameContext)
                );
                break;
        }
    }

    [RelayCommand]
    async Task LoadedAsync()
    {
        if (GameContext == null)
            return;
        var gamers = await WavesClient.GetWavesGamerAsync(this.CTS.Token);
        if (gamers == null)
        {
            return;
        }
        this.Roils = gamers.Data.ToObservableCollection();
        var bindUser = await GameContext.GameLocalConfig.GetConfigAsync(
            GameContextExtension.BindUser
        );
        if (string.IsNullOrWhiteSpace(bindUser))
            return;
        foreach (var item in Roils)
        {
            if (item.RoleId.ToString() == bindUser)
            {
                SelectRoil = item;
                break;
            }
        }
    }

    [RelayCommand]
    async Task Primary()
    {
        if (SelectRoil == null)
        {
            this.Close();
            return;
        }
        await GameContext!.GameLocalConfig.SaveConfigAsync(
            GameContextExtension.BindUser,
            this.SelectRoil.RoleId.ToString()
        );
        WeakReferenceMessenger.Default.Send<RefreshBindUser>(new(this.SelectRoil.RoleId));
        this.Close();
    }

    [RelayCommand]
    async Task DialogClose()
    {
        await GameContext!.GameLocalConfig.SaveConfigAsync(GameContextExtension.BindUser, "");
        WeakReferenceMessenger.Default.Send<RefreshBindUser>(new(null));
        this.Close();
    }

    public void Dispose()
    {
        this.Roils.Clear();
        this.SelectRoil = null;
    }
}
