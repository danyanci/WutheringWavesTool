using WutheringWavesTool.Services.Contracts;
using WutheringWavesTool.Services.Navigations.Base;

namespace WutheringWavesTool.Services.Navigations;

public class CommunityNavigationService : NavigationServiceBase
{
    public CommunityNavigationService(IPageService pageService)
        : base(pageService) { }
}
