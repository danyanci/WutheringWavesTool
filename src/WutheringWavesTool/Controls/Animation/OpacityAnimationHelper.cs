using System;
using CommunityToolkit.WinUI.Animations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;

namespace WutheringWavesTool.Controls.Animation;

public static class OpacityAnimationHelper
{
    public static void StartAnimationHelper(FrameworkElement control, double to)
    {
        var fadeAnimation = new DoubleAnimation { To = to, Duration = TimeSpan.FromSeconds(0.3) };
        var storyboard = new Storyboard();
        Storyboard.SetTarget(fadeAnimation, control);
        Storyboard.SetTargetProperty(fadeAnimation, "(UIElement.Opacity)");
        storyboard.Children.Add(fadeAnimation);
        storyboard.Begin();
    }
}
