using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GreenPlayer.Controls;

public partial class GreenItem : Control
{
    public GreenItem()
    {
        this.DefaultStyleKey = nameof(GreenItem);
    }

    public int Value
    {
        get { return (int)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        "Value",
        typeof(int),
        typeof(GreenItem),
        new PropertyMetadata(null)
    );
}
