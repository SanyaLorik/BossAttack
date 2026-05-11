using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;


public class BonusCollectItem : MonoBehaviour {
    [SerializeReference, SubclassSelector] private IBonus _bonus;

    private IBonus Bonus => _bonus;
    
    private CancellationTokenSource _tokenSource;
    
    [Inject] private DiContainer _diContainer;
    [Inject] PlayerMovement _mainPlayer;
    [Inject] GameData _gameData;
    
    
    [Inject]
    private void Init() {
        _diContainer.QueueForInject(Bonus);
    }

    private void OnTriggerEnter(Collider collider) {
        if (!collider.TryGetComponent(out IPlayer player)) return;
        if (player == _mainPlayer) {
            UseBonus();
        }
    }


    private void UseBonus() {
        GameEvents.BonusUseInvoke(Bonus);
        Bonus.Use(_mainPlayer.BonusUser);
        
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new  CancellationTokenSource();
        StartUseTimerAsync(_tokenSource.Token).Forget();
    }


    private async UniTask StartUseTimerAsync(CancellationToken token) {
        float duration = _gameData.BonusDuration;
        await UniTask.WaitForSeconds(duration, cancellationToken: token);
        Bonus.StopWork(_mainPlayer.BonusUser);
    }
    
    
}
