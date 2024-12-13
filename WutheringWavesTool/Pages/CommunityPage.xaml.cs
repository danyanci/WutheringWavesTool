using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WutheringWavesTool.Common;
using WutheringWavesTool.ViewModel;

namespace WutheringWavesTool.Pages;

public sealed partial class CommunityPage : Page, IPage
{
    public CommunityPage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<CommunityViewModel>();
    }

    public Type PageType => typeof(CommunityPage);

    public CommunityViewModel ViewModel { get; }
}
