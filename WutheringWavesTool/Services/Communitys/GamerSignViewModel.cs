using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Waves.Api.Models.Communitys;
using WavesLauncher.Core.Contracts;
using WutheringWavesTool.Common;

namespace WutheringWavesTool.Services.Communitys;

public sealed partial class GamerSignViewModel : ViewModelBase
{
    public GamerSignViewModel(IWavesClient wavesClient)
    {
        WavesClient = wavesClient;
    }

    public IWavesClient WavesClient { get; }
    public GameRoilDataItem SignRoil { get; internal set; }

    [ObservableProperty]
    public partial string UserName { get; set; }

    [ObservableProperty]
    public partial bool SignBthEnable { get; set; }

    [ObservableProperty]
    public partial bool SignBthCheck { get; set; }

    [ObservableProperty]
    public partial int SignCount { get; set; }

    [ObservableProperty]
    public partial int UnSignCount { get; set; }

    [ObservableProperty]
    public partial string SignStatus { get; set; }

    [RelayCommand]
    async Task Loaded()
    {
        UserName = this.SignRoil.RoleName;
        await RefreshSignHistoryAsync();
    }

    async Task RefreshSignHistoryAsync()
    {
        var game = await WavesClient.GetWavesGamerAsync();
        var games = game.Data.Where(p => p.GameId == 3);
        if (games.Count() != 0)
        {
            var result = await WavesClient.GetSignInDataAsync(
                SignRoil.UserId,
                long.Parse(SignRoil.RoleId)
            );
            var signCount = result!.Data.SigInNum;
            var signs = result.Data.SignInGoodsConfigs.Take(signCount);
            foreach (var item in signs)
            {
                item.SignResult = "已签到";
                item.IsSign = true;
            }
            SignCount = signs.Where(x => x.IsSign).Count();
            UnSignCount = result.Data.SignInGoodsConfigs.Count - SignCount;
            if (result.Data.IsSigIn)
            {
                SignBthEnable = false;
                SignBthCheck = true;
                SignStatus = "今日已签到";
            }
            else
            {
                SignBthEnable = true;
                SignBthCheck = false;
                SignStatus = "点击签到";
            }
        }
    }

    [RelayCommand]
    async Task SignAsync()
    {
        var result = await WavesClient.SignInAsync(SignRoil.UserId.ToString(), SignRoil.RoleId);
        if (result.Code == 1511)
        {
            Debug.WriteLine("已经签到！");
        }
        if (result.Code == 220)
        {
            Debug.WriteLine("Token过期，重新登陆");
        }
        if (result.Code == 1505)
        {
            Debug.WriteLine("活动过期");
        }
        if (result.Code == 200)
        {
            await RefreshSignHistoryAsync();
        }
    }
}
