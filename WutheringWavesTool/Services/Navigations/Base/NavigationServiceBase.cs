using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using WutheringWavesTool.Common;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.Services.Navigations.Base;

public class NavigationServiceBase : INavigationService
{
    public NavigationServiceBase(IPageService pageService)
    {
        PageService = pageService;
    }

    private object _parameter;

    public IPageService PageService { get; }

    public Frame RootFrame { get; private set; }

    public bool CanGoBack => RootFrame != null ? RootFrame.CanGoBack : false;

    public bool CanGoForward => RootFrame != null ? RootFrame.CanGoForward : false;

    public event NavigatedEventHandler Navigated;
    public event NavigationFailedEventHandler NavigationFailed;

    public bool GoBack()
    {
        if (RootFrame.CanGoBack)
        {
            ElementSoundPlayer.Play(ElementSoundKind.GoBack);
            RootFrame.GoBack();
            return true;
        }
        return false;
    }

    public bool GoForward()
    {
        if (RootFrame.CanGoForward)
        {
            RootFrame.GoForward();
            return true;
        }
        return false;
    }

    public bool NavigationTo(string key, object args, NavigationTransitionInfo transitionInfo)
    {
        var pageType = PageService.GetPage(key);
        if (pageType == null)
            return false;
        if (RootFrame.Content is IPage OrginpageType)
        {
            if (
                RootFrame != null
                && (OrginpageType.PageType != pageType || args != null && !args.Equals(_parameter))
            )
            {
                _parameter = args;
                return RootFrame.Navigate(pageType, _parameter, transitionInfo);
            }
        }
        else if (RootFrame.Content == null)
        {
            _parameter = args;
            return RootFrame.Navigate(pageType, _parameter, new DrillInNavigationTransitionInfo());
        }
        return false;
    }

    public void RegisterView(Frame frame)
    {
        RootFrame = frame;
        RootFrame.Navigated += Navigated;
        RootFrame.NavigationFailed += NavigationFailed;
    }

    public void UnRegisterView()
    {
        RootFrame.Navigated -= Navigated;
        RootFrame.NavigationFailed -= NavigationFailed;
        RootFrame = null;
    }

    public bool NavigationTo<ViewModel>(object args, NavigationTransitionInfo transitionInfo)
        where ViewModel : ObservableObject =>
        NavigationTo(typeof(ViewModel).FullName, args, transitionInfo);

    public void ClearHistory()
    {
        RootFrame.BackStack.Clear();
        RootFrame.ForwardStack.Clear();
    }
}
