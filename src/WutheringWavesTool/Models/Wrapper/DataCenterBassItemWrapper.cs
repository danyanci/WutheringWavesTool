using System;
using System.IO;
using System.IO.Pipes;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Waves.Api.Models.Communitys.DataCenter;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using WutheringWavesTool.Common;
using WutheringWavesTool.Controls;
using WutheringWavesTool.Helpers;

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
    public partial int StarLevel { get; set; }

    [ObservableProperty]
    public partial BitmapImage RoleIconUrl { get; set; }

    [ObservableProperty]
    public partial string AttributeName { get; set; }

    [ObservableProperty]
    public partial int AttibuteId { get; set; }

    [ObservableProperty]
    public partial BitmapImage TypeImage { get; set; }

    [ObservableProperty]
    public partial string RoleName { get; set; }

    public DataCenterRoilItemWrapper(RoleList roleData)
    {
        this.StarLevel = roleData.StarLevel;
        this.AttributeName = roleData.AttributeName;
        this.RoleName = roleData.RoleName;
        this.AttibuteId = roleData.AttributeId;
        TypeImage = new BitmapImage(new(SwitchType(roleData)));
        RoleIconUrl = new BitmapImage(new(roleData.RoleIconUrl));
    }

    private string SwitchType(RoleList RoleData)
    {
        var TypeImage = "";
        switch (RoleData.AttributeId)
        {
            case 1:
                TypeImage = GameIcon.Icon1;
                break;
            case 2:
                TypeImage = GameIcon.Icon2;
                break;
            case 3:
                TypeImage = GameIcon.Icon3;
                break;
            case 4:
                TypeImage = GameIcon.Icon4;
                break;
            case 5:
                TypeImage = GameIcon.Icon5;
                break;
            case 6:
                TypeImage = GameIcon.Icon6;
                break;
        }
        return TypeImage;
    }

    [RelayCommand]
    void ClickShow()
    {
        //WeakReferenceMessenger.Default.Send<ShowRoleData>(new ShowRoleData(RoleData.RoleId));
    }
}
