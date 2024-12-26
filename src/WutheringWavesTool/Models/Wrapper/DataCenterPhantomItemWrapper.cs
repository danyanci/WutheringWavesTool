using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Waves.Api.Models.Communitys.DataCenter;
using WutheringWavesTool.Helpers;

namespace WutheringWavesTool.Models.Wrapper;

public partial class DataCenterPhantomItemWrapper : ObservableObject
{
    public DataCenterPhantomItemWrapper(PhantomList bassData)
    {
        this.Star = bassData.Star;
        this.MaxStar = bassData.MaxStar;
        this.Name = bassData.Phantom.Name;
        this.CoverUrl = bassData.Phantom.IconUrl;
        this.Cover = new BitmapImage(new(CoverUrl));
    }

    [ObservableProperty]
    public partial int Star { get; set; }

    [ObservableProperty]
    public partial int MaxStar { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; }
    public string CoverUrl { get; }

    [ObservableProperty]
    public partial BitmapImage Cover { get; set; }
}
