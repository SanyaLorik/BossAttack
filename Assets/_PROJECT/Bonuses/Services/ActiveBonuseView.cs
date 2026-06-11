using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class ActiveBonuseView : ProgressVisualizer {
    [field: SerializeField] public BonusType BonusType { get; private set; }
    [SerializeField] private GameObject _visual;

    
    private CancellationTokenSource _tokenSource;

    [Inject] private GameData _gameData;
    
    public void ActiveBonus(ActiveBonus bonus) {
        if(!_visual.activeSelf) _visual.ActiveSelf();
        _tokenSource ??= new CancellationTokenSource();
        BonusTimerStartAsync(bonus, _tokenSource.Token).Forget();
    }
    
    
    public void DisactiveBonus() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        DisactiveVisual();
    }

    public void DisactiveVisual() {
        _visual.DisactiveSelf();
    }

    private async UniTask BonusTimerStartAsync(ActiveBonus bonus, CancellationToken token) {
        while (!token.IsCancellationRequested) {
            SetPercentage(1f - bonus.Progress, false);
            await UniTask.Yield();
        }
    }
    
    
}