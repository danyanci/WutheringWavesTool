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
    public long RoilId { get; }

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
    public GameRoilDataItem User { get; }

    public DataCenterRoilItemWrapper(RoleList roleData, GameRoilDataItem user)
    {
        this.StarLevel = roleData.StarLevel;
        this.RoilId = roleData.RoleId;
        this.AttributeName = roleData.AttributeName;
        this.RoleName = roleData.RoleName;
        this.AttibuteId = roleData.AttributeId;
        TypeImage = new BitmapImage(new(SwitchType(roleData)));
        RoleIconUrl = new BitmapImage(new(roleData.RoleIconUrl));
        User = user;
    }

    public DataCenterRoilItemWrapper(RoleList roleData)
    {
        this.StarLevel = roleData.StarLevel;
        this.RoilId = roleData.RoleId;
        this.AttributeName = roleData.AttributeName;
        this.RoleName = roleData.RoleName;
        this.AttibuteId = roleData.AttributeId;
        TypeImage = new BitmapImage(new(SwitchType(roleData)));
        RoleIconUrl = new BitmapImage(new(roleData.RoleIconUrl));
    }

    public static string SwitchType(RoleList RoleData)
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
        if (this.User == null)
            return;
        WeakReferenceMessenger.Default.Send<ShowRoleData>(new ShowRoleData(RoilId, this.User));
    }
}
