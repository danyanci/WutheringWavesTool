namespace WutheringWavesTool.Services.Contracts;

public interface IAppContext<T>
    where T : ClientApplication
{
    public T App { get; }

    public Controls.TitleBar MainTitle { get; }

    public Task LauncherAsync(T app);

    public Task TryInvokeAsync(Func<Task> action);

    public void TryInvoke(Action action);

    public ElementTheme CurrentElement { get; set; }

    void SetTitleControl(Controls.TitleBar titleBar);

    public void SetElementTheme(ElementTheme theme);
}
