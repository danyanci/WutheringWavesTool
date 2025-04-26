namespace WutheringWavesTool.ViewModel.GameViewModels;

partial class GameContextViewModelBase
{
    [RelayCommand]
    async Task UpdateGameAsync()
    {
        if (_bthType == 3)
        {
            await GameContext.StartGameAsync();
        }
        if (_bthType == 4)
        {
            await GameContext.UpdateGameAsync();
        }
        if (_bthType == 5)
        {
            await GameContext.StopGameAsync();
        }
    }
}
