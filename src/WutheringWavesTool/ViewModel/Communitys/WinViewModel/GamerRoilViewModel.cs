using Windows.Web.AtomPub;
using WutheringWavesTool.Helpers;
using WutheringWavesTool.Models.Wrapper.WindowRoils;

namespace WutheringWavesTool.ViewModel.Communitys.WinViewModel;

public sealed partial class GamerRoilViewModel : ViewModelBase
{
    public NavigationRoilsDetilyItem ItemData { get; private set; }
    public IWavesClient WavesClient { get; }

    public GamerRoilViewModel(IWavesClient wavesClient)
    {
        WavesClient = wavesClient;
    }

    internal void SetData(NavigationRoilsDetilyItem? navigationRoilsDetilyItem)
    {
        if (navigationRoilsDetilyItem != null)
            this.ItemData = navigationRoilsDetilyItem;
    }

    #region RoleData
    [ObservableProperty]
    public partial string RolePic { get; set; }

    [ObservableProperty]
    public partial string RoleName { get; set; }

    [ObservableProperty]
    public partial int RoleLevel { get; set; }

    [ObservableProperty]
    public partial int RoleStar { get; set; }

    [ObservableProperty]
    public partial BitmapImage AttImage { get; set; }
    #endregion

    #region Weapon
    [ObservableProperty]
    public partial BitmapImage WeaponImage { get; set; }

    [ObservableProperty]
    public partial string WeaponName { get; set; }

    [ObservableProperty]
    public partial string WeaponTypeName { get; set; }

    [ObservableProperty]
    public partial int WeaponLevel { get; set; }

    [ObservableProperty]
    public partial int WeaponStarLevel { get; set; }

    [ObservableProperty]
    public partial string WeaponSession { get; set; }

    [ObservableProperty]
    public partial int WeaponReason { get; set; }

    #endregion

    #region Skill
    [ObservableProperty]
    public partial ObservableCollection<SkillList> Skills { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<EquipPhantomList> PhantomData { get; private set; }

    [ObservableProperty]
    public partial ObservableCollection<ChainList> Chains { get; set; }
    #endregion

    #region Visibility
    [ObservableProperty]
    public partial Visibility WeaponVisibility { get; set; }

    [ObservableProperty]
    public partial Visibility ChainVisibility { get; set; }

    [ObservableProperty]
    public partial Visibility PhantomDataVisibility { get; set; }
    #endregion

    [RelayCommand]
    async Task Loaded()
    {
        var result = await WavesClient.GetGamerRoilDetily(this.ItemData.Item, this.ItemData.RoilId);
        if (result == null)
        {
            return;
        }
        this.RolePic = result.Role.RolePicUrl;
        this.RoleName = result.Role.RoleName;
        this.RoleLevel = result.Role.Level;
        this.RoleStar = result.Role.StarLevel;
        this.AttImage = new BitmapImage(new(RoleHelper.SwitchType(result.Role.AttributeId)));
        //武器与技能
        this.WeaponImage = new(new(result.WeaponData.Weapon.WeaponIcon));
        this.WeaponName = result.WeaponData.Weapon.WeaponName;
        this.WeaponLevel = result.WeaponData.Level;
        this.WeaponStarLevel = result.WeaponData.Weapon.WeaponStarLevel;
        this.WeaponTypeName = result.WeaponData.Weapon.WeaponEffectName.ToString();
        this.WeaponSession = result.WeaponData.Weapon.EffectDescription;
        this.WeaponReason = result.WeaponData.ResonLevel;
        this.Skills = result.SkillList.ToObservableCollection();
        //声骸
        this.PhantomData = result.PhantomData.EquipPhantomList.ToObservableCollection();
        //共鸣链
        this.Chains = result.ChainList.ToObservableCollection();
    }

    internal void SetPage(string? v)
    {
        if (v == "Weapon")
        {
            WeaponVisibility = Visibility.Visible;
            ChainVisibility = Visibility.Collapsed;
            PhantomDataVisibility = Visibility.Collapsed;
        }
        else if (v == "Chain")
        {
            WeaponVisibility = Visibility.Collapsed;
            ChainVisibility = Visibility.Visible;
            PhantomDataVisibility = Visibility.Collapsed;
        }
        else if (v == "PhantomData")
        {
            WeaponVisibility = Visibility.Collapsed;
            ChainVisibility = Visibility.Collapsed;
            PhantomDataVisibility = Visibility.Visible;
        }
    }
}
