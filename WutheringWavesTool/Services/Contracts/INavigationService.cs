using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace WutheringWavesTool.Services.Contracts;

public interface INavigationService
{
    public void RegisterView(Frame frame);

    public void UnRegisterView();

    public bool CanGoBack { get; }

    public bool CanGoForward { get; }

    public bool GoBack();

    public bool GoForward();

    public bool NavigationTo(string key, object args, NavigationTransitionInfo transition);

    public bool NavigationTo<ViewModel>(object args, NavigationTransitionInfo transition)
        where ViewModel : ObservableObject;

    public event NavigatedEventHandler Navigated;

    public event NavigationFailedEventHandler NavigationFailed;
}
