namespace WutheringWavesTool.ViewModel.GameViewModels;

partial class GameViewModelBase
{
    private void RegisterMessanger()
    {
        this.Messenger.Register<DeleteGameResource>(this, DeleteGameResourceMethod);
    }

    public abstract Task RemoveGameResource(DeleteGameResource resourceMessage);

    private async void DeleteGameResourceMethod(object recipient, DeleteGameResource message)
    {
        await RemoveGameResource(message);
    }
}
