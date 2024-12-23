using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Waves.Api.Models.Communitys.DataCenter;
using WutheringWavesTool.Common;

namespace WutheringWavesTool.Models.Wrapper;

public partial class DataCenterBassItemWrapper : ObservableObject
{
    [ObservableProperty]
    public partial string DisplayName { get; set; }

    [ObservableProperty]
    public partial string Value { get; set; }
}

public partial class DataCenterRoilItemWrapper : ObservableObject
{
    [ObservableProperty]
    public partial RoleList RoleData { get; set; }

    public DataCenterRoilItemWrapper(RoleList roleData)
    {
        RoleData = roleData;
    }

    [ObservableProperty]
    public partial BitmapImage TypeImage { get; set; }

    [RelayCommand]
    void Loaded()
    {
        SwitchType();
    }

    [RelayCommand]
    void ClickShow()
    {
        //WeakReferenceMessenger.Default.Send<ShowRoleData>(new ShowRoleData(RoleData.RoleId));
    }

    private void SwitchType()
    {
        switch (RoleData.AttributeId)
        {
            case 1:
                TypeImage = new BitmapImage(new(GameIcon.Icon1));
                break;
            case 2:
                TypeImage = new BitmapImage(new(GameIcon.Icon2));
                break;
            case 3:
                TypeImage = new BitmapImage(new(GameIcon.Icon3));
                break;
            case 4:
                TypeImage = new BitmapImage(new(GameIcon.Icon4));
                break;
            case 5:
                TypeImage = new BitmapImage(new(GameIcon.Icon5));
                break;
            case 6:
                TypeImage = new BitmapImage(new(GameIcon.Icon6));
                break;
        }
    }
}
