using System.Threading.Tasks;
using WutheringWavesTool.WindowModels;

namespace WutheringWavesTool.Services.Contracts;

public interface IViewFactorys
{
    public IAppContext<App> AppContext { get; }
    public GetGeetWindow CreateGeetWindow();
}
