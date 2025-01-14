using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace WutheringWavesTool.Services.Contracts;

public interface IPlayerRecordContext : IDisposable
{
    public IDialogManager DialogManager { get; }

    public ITipShow TipShow { get; }

    public IServiceScope Scope { get; }
    public INavigationService NavigationService { get; }

    public void SetScope(IServiceScope scope);

    public Task<string> ShowInputRecordAsync(object data);
}
