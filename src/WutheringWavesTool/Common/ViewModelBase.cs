namespace WutheringWavesTool.Common;

public partial class ViewModelBase : ObservableRecipient
{
    public CancellationTokenSource CTS { get; set; }

    public ViewModelBase()
    {
        CTS = new CancellationTokenSource();
    }
}
