using CommunityToolkit.Mvvm.ComponentModel;
using Waves.Api.Models.Communitys.DataCenter;

namespace WutheringWavesTool.Models.Wrapper;

public partial class DataCenterPhantomItemWrapper : ObservableObject
{
    [ObservableProperty]
    public partial PhantomList BassData { get; set; }
}
