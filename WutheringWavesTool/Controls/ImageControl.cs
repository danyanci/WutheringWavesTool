using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace WutheringWavesTool.Controls;

[TemplatePart(Name = MainImageName, Type = typeof(Image))]
[TemplatePart(Name = BackImageName, Type = typeof(Image))]
[TemplateVisualState(GroupName = "ImageGroupStatus", Name = "Loaded")]
[TemplateVisualState(GroupName = "ImageGroupStatus", Name = "Loading")]
[TemplateVisualState(GroupName = "ImageGroupStatus", Name = "Error")]
public partial class ImageControl : Control
{
    public ImageControl()
    {
        DefaultStyleKey = typeof(ImageControl);
    }

    public const string MainImageName = "PARA_MainImage";
    public const string BackImageName = "PARA_BackImage";

    public Image MainImage { get; private set; }
    public Image BackImage { get; private set; }

    protected override void OnApplyTemplate()
    {
        MainImage = (Image)GetTemplateChild(MainImageName);
        BackImage = (Image)GetTemplateChild(BackImageName);
        MainImage.ImageOpened += MainImage_ImageOpened;
        MainImage.ImageFailed += MainImage_ImageFailed;
        base.OnApplyTemplate();
    }

    private void MainImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "Error", true);
    }

    private void MainImage_ImageOpened(object sender, RoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "Loaded", true);
    }

    public ImageSource Source
    {
        get { return (ImageSource)GetValue(SourceProperty); }
        set { SetValue(SourceProperty, value); }
    }

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        "Source",
        typeof(ImageSource),
        typeof(ImageControl),
        new PropertyMetadata(null, OnSourceChanged)
    );

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ImageControl control)
        {
            VisualStateManager.GoToState(control, "Loading", true);
        }
    }

    public Stretch Stretch
    {
        get { return (Stretch)GetValue(StretchProperty); }
        set { SetValue(StretchProperty, value); }
    }

    public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
        "Stretch",
        typeof(Stretch),
        typeof(ImageControl),
        new PropertyMetadata(null)
    );
}
