using System;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using Zenject;

public class GameTimerToEnd : ProgressVisualizer {
    [Inject] GameData _gameData;
    
    
    public event Action GameEnded;

    private CancellationTokenSource _tokenSource;

    private void Start() {
        FastHide();
    }

    public void StartGameTimerToEnd(int bossCount) {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        WaitWhileTimerAsync(bossCount, _tokenSource.Token).Forget();
        ShowBarAnimation(true);
    }

    public void StopTimer() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        ShowBarAnimation(false);
    }

    
    
    private async UniTask WaitWhileTimerAsync(int bossCount, CancellationToken token) {
        int duration = _gameData.TimeToOneBoss * bossCount;
        float elapsedTime = duration;
        while (elapsedTime > 0 && !token.IsCancellationRequested) {
            float progress =  elapsedTime / duration;
            SetProgressPercentage(progress, (int)elapsedTime, true);

            elapsedTime -= 1f;
            await UniTask.WaitForSeconds(1f, cancellationToken: token);
        }
        ShowBarAnimation(false);
        GameEnded?.Invoke();
        
    }
}