using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WutheringWavesTool.Common;

public partial class ViewModelBase : ObservableRecipient
{
    public CancellationTokenSource CTS { get; set; }

    public ViewModelBase()
    {
        CTS = new CancellationTokenSource();
    }
}
