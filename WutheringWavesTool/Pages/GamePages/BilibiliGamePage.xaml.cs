using System;
using WutheringWavesTool.Common;
using WutheringWavesTool.Pages.Bases;

namespace WutheringWavesTool.Pages.GamePages;

public sealed partial class BilibiliGamePage : GamePageBase, IPage
{
    public BilibiliGamePage()
    {
        this.InitializeComponent();
    }

    public Type PageType => typeof(BilibiliGamePage);
}
