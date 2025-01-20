namespace WutheringWavesTool.Common.Bases;

public partial class DialogViewModelBase : ViewModelBase
{
    public DialogViewModelBase(IAppContext<App> appContext)
    {
        AppContext = appContext;
    }

    public IAppContext<App> AppContext { get; }

    [RelayCommand]
    protected void Close()
    {
        AppContext.CloseDialog();
    }
}
