using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Waves.Api.Helper;
using Waves.Api.Models.Enums;
using Waves.Api.Models.Record;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;

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

    [RelayCommand]
    async Task ShowInputRecordAsync()
    {
        this.FiveGroup = await RecordHelper.GetFiveGroupAsync();
        var allRole = await RecordHelper.GetAllRoleAsync();
        var link = await PlayerRecordContext.ShowInputRecordAsync(null);
        var request = RecordHelper.GetRecorRequest(link);
        if (request == null)
        {
            this.PlayerRecordContext.TipShow.ShowMessage(
                "抽卡链接无效",
                Microsoft.UI.Xaml.Controls.Symbol.Clear
            );
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
            return;
        }
        var five = await RecordHelper.FormatStartFiveAsync(items, FiveGroup);
    }

    private bool disposedValue;

    [RelayCommand]
    async Task Loaded() { }

    public IServiceScopeFactory ServiceScopeFactory { get; }
    public IServiceScope Scope { get; }
    public RecordRequest Request { get; private set; }
    public FiveGroupModel? FiveGroup { get; private set; }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
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
