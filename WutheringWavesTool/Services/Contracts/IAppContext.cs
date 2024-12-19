using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WutheringWavesTool.Common;

namespace WutheringWavesTool.Services.Contracts;

public interface IAppContext<T>
    where T : ClientApplication
{
    public T App { get; }

    public Task LauncherAsync(T app);

    public Task TryInvokeAsync(Action action);

    public XamlRoot Root { get; }

    public void RegisterRoot(XamlRoot root);

    public Task ShowLoginDialogAsync();
    public Task<ContentDialogResult> ShowBindGameDataAsync(string name);
    public void CloseDialog();
}
