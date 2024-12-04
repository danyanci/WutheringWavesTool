using System;
using System.Threading.Tasks;
using WutheringWavesTool.Common;

namespace WutheringWavesTool.Services.Contracts;

public interface IAppContext<T>
    where T : ClientApplication
{
    public T App { get; }

    public Task LauncherAsync(T app);

    public Task TryInvokeAsync(Action action);
}
