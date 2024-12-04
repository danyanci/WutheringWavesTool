using Microsoft.UI.Xaml;
using WinUIEx;

namespace WutheringWavesTool.Common;

public abstract class ClientApplication : Application
{
    public Window MainWindow { get; set; }
}
