using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BotWanderInBattleBehaviour : MonoBehaviour {
    [SerializeField] private BotManager _manager;
    
    private CancellationTokenSource _tokenSource;
    
    
    [Inject] MapsToBattleChanger _mapsChanger;
    
    public void StartWandering() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new  CancellationTokenSource();
        WanderingInPlace(_tokenSource.Token).Forget();
    }
    
    private async UniTask WanderingInPlace(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            await UniTask.WaitWhile(() => _manager.IsPushed, cancellationToken: token);
            Vector3 target = _manager.BotWalkManager.GetTargetPoint(_mapsChanger.GetCurrentMapFloor, _mapsChanger.CurrentMapYToFind);
            await _manager.BotWalkManager.SetAgentGoToPointAsync(target, token);
        }
    }
}