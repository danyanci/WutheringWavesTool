using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Waves.Api.Models.Communitys;
using WutheringWavesTool.Common;

namespace WutheringWavesTool.ViewModel.Communitys.WinViewModel;

public sealed partial class GamerRoilDetilyViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial Role RoleData { get; set; }

    [ObservableProperty]
    public partial BitmapImage TypeImage { get; private set; }

    [ObservableProperty]
    public partial WeaponData WeaponData { get; set; }

    internal void SetData(GamerRoilDetily value)
    {
        this.RoleData = value.Role;
        this.WeaponData = value.WeaponData;
    }

    [RelayCommand]
    void Loaded()
    {
        SwitchType();
    }

    private void SwitchType()
    {
        switch (RoleData.AttributeId)
        {
            case 1:
                this.TypeImage = new BitmapImage(new(GameIcon.Icon1));
                break;
            case 2:
                this.TypeImage = new BitmapImage(new(GameIcon.Icon2));
                break;
            case 3:
                this.TypeImage = new BitmapImage(new(GameIcon.Icon3));
                break;
            case 4:
                this.TypeImage = new BitmapImage(new(GameIcon.Icon4));
                break;
            case 5:
                this.TypeImage = new BitmapImage(new(GameIcon.Icon5));
                break;
            case 6:
                this.TypeImage = new BitmapImage(new(GameIcon.Icon6));
                break;
        }
    }
}
