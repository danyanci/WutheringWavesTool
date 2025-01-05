using System.Threading.Tasks;
using Waves.Api.Models.Communitys;
using WutheringWavesTool.Common.Bases;
using WutheringWavesTool.WindowModels;

namespace WutheringWavesTool.Services.Contracts;

public interface IViewFactorys
{
    public IAppContext<App> AppContext { get; }
    public GetGeetWindow CreateGeetWindow();

    public WindowModelBase ShowSignWindow(GameRoilDataItem role);

    public WindowModelBase ShowRoleDataWindow(GamerRoilDetily detily);
}
