namespace WutheringWavesTool.Services.Contracts;

public interface IViewFactorys
{
    public IAppContext<App> AppContext { get; }
    public GetGeetWindow CreateGeetWindow();

    public WindowModelBase ShowSignWindow(GameRoilDataItem role);

    public WindowModelBase ShowRoleDataWindow(GamerRoilDetily detily);

    public WindowModelBase ShowPlayerRecordWindow();
}
