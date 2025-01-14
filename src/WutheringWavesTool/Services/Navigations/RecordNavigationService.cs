using WutheringWavesTool.Services.Contracts;
using WutheringWavesTool.Services.Navigations.Base;

namespace WutheringWavesTool.Services.Navigations;

public class RecordNavigationService : NavigationServiceBase
{
    public RecordNavigationService(IPageService pageService)
        : base(pageService) { }
}
