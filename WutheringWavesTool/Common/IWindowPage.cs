using Microsoft.UI.Xaml;

namespace WutheringWavesTool.Common;

public interface IWindowPage
{
    public void SetWindow(Window window);

    public void SetData(object value);
}
