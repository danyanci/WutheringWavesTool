using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using WutheringWavesTool.Common.PInvoke;

namespace WutheringWavesTool.Controls.Behaviors;

public class ButtonSoundBehavior : Behavior<Button>
{
    protected override void OnAttached()
    {
        AssociatedObject.Click += AssociatedObject_Click;
    }

    private void AssociatedObject_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Sound.PlayClick();
    }

    protected override void OnDetaching()
    {
        AssociatedObject.Click -= AssociatedObject_Click;
    }
}
