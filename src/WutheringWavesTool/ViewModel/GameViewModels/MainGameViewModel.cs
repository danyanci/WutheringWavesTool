using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Waves.Api.Models;
using Waves.Api.Models.Communitys;
using Waves.Core.GameContext;
using Waves.Core.GameContext.Contexts;
using WavesLauncher.Core.Contracts;
using WutheringWavesTool.Common;
using WutheringWavesTool.Models;
using WutheringWavesTool.Models.Messanger;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.ViewModel.GameViewModels;

public sealed partial class MainGameViewModel : GameViewModelBase
{
    public MainGameViewModel(
        [FromKeyedServices(nameof(MainGameContext))] IGameContext gameContext,
        IPickersService pickersService,
        IAppContext<App> appContext,
        ITipShow tipShow,
        IWavesClient wavesClient
    )
        : base(gameContext, pickersService, appContext, tipShow)
    {
        WavesClient = wavesClient;
        this.RegisterMessanger();
    }

    private ObservableCollection<Content> activity;
    private ObservableCollection<Content> news;
    private ObservableCollection<Content> notice;

    [ObservableProperty]
    public partial ObservableCollection<Content> NowNews { get; set; }

    [ObservableProperty]
    public partial GamerDataModel GamerData { get; set; }

    [ObservableProperty]
    public partial Visibility GamerDataVisibility { get; set; } = Visibility.Collapsed;

    [ObservableProperty]
    public partial bool SelectBarLoad { get; set; } = false;
    public IWavesClient WavesClient { get; }

    public override async Task LoadedAfter()
    {
        var result =
            await this.GameContext.GetGameLauncherStarterAsync(
                await this.GameContext.GetGameLauncherSourceAsync(),
                true
            ) ?? null;
        if (result != null && result.Guidance != null)
        {
            this.news = result.Guidance.News.Contents.ToObservableCollection();
            this.notice = result.Guidance.Notice.Contents.ToObservableCollection();
            this.activity = result.Guidance.Activity.Contents.ToObservableCollection();
        }
        await RefreshBindAsync();
        this.SelectBarLoad = true;
    }

    private void RegisterMessanger()
    {
        this.Messenger.Register<RefreshBindUser>(this, RefreshBindUserMethod);
    }

    private async void RefreshBindUserMethod(object recipient, RefreshBindUser message)
    {
        await RefreshBindAsync();
    }

    private async Task RefreshBindAsync()
    {
        if (!(await WavesClient.IsLoginAsync()))
            return;
        var bindUser = await GameContext.GameLocalConfig.GetConfigAsync(
            GameContextExtension.BindUser
        );
        if (string.IsNullOrWhiteSpace(bindUser))
        {
            GamerDataVisibility = Visibility.Collapsed;
            return;
        }
        else
        {
            var gamers = await WavesClient.GetWavesGamerAsync();
            var first = gamers!.Data.Where(x => x.RoleId.ToString() == bindUser).First();
            var rr = await WavesClient.GetGamerDataAsync(first);
            this.GamerData = rr!;
            GamerDataVisibility = Visibility.Visible;
            //刷新每日体力
        }
    }

    [RelayCommand]
    async Task RefershGamerData()
    {
        await this.RefreshBindAsync();
    }

    [RelayCommand]
    async Task ShowBind()
    {
        if (await WavesClient.IsLoginAsync())
        {
            await this.AppContext.ShowBindGameDataAsync("Main");
        }
        else
        {
            TipShow.ShowMessage("尚未登录或登录失效", Microsoft.UI.Xaml.Controls.Symbol.Clear);
        }
    }

    internal void SelectNews(string? v)
    {
        if (this.IsLoading)
            return;
        switch (v)
        {
            case "Dynamic":
                this.NowNews = this.activity;
                break;
            case "Subtitle":
                this.NowNews = this.notice;
                break;
            case "News":
                this.NowNews = this.news;
                break;
            default:
                break;
        }
    }
}
