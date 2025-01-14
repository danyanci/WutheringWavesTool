using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Waves.Api.Models.Communitys;
using WutheringWavesTool.Common;
using WutheringWavesTool.Models.Args;

namespace WutheringWavesTool.Pages.Record;

public sealed partial class RecordItemPage : Page, IPage
{
    public RecordItemPage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is RecordArgs item) { }
        base.OnNavigatedTo(e);
    }

    public Type PageType => typeof(RecordItemPage);
}
