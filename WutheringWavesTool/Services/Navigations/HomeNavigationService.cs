using WutheringWavesTool.Services.Contracts;
using WutheringWavesTool.Services.Navigations.Base;

namespace WutheringWavesTool.Services.Navigations;

public class HomeNavigationService : NavigationServiceBase
{
    public HomeNavigationService(IPageService pageService)
        : base(pageService) { }
}
