using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BotWanderingInBattle : MonoBehaviour {
    [SerializeField] private BotWalkManager _botWalkManager;
    
    
    private CancellationTokenSource _tokenSource;
    
    [Inject] GameData _gameData;
    [Inject] MapsToBattleChanger _mapsChanger;
    
    public void StartWandering() {
        _tokenSource = new CancellationTokenSource();
        WanderingInPlace(_tokenSource.Token).Forget();
    }

    public void StopWandering() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _botWalkManager.ResetLogic();
    }
    
    
    private async UniTask WanderingInPlace(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            // await UniTask.WaitWhile(() => _botWalkManager.IsPushed, cancellationToken: token);
            Vector3 target = GetRandomPointInMap();
            _botWalkManager.SetAgentGoToPoint(target);
            await UniTask.WaitForSeconds(GetRandomTimingToStayInPoint(), cancellationToken: token);
        }
    }

    private float GetRandomTimingToStayInPoint() {
        return Random.Range(_gameData.TimeToStayOnPointInBattle.From, _gameData.TimeToStayOnPointInBattle.To);
    }

    private Vector3 GetRandomPointInMap() {
        Vector3 target = _botWalkManager.GetTargetPoint(_mapsChanger.GetCurrentMapFloor, _mapsChanger.CurrentMapYToFind);
        return target;
    }
}