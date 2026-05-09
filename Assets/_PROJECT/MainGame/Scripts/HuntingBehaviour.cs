using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;


public class HuntingBehaviour : MonoBehaviour {
    private List<IPlayer> _units = new();

    private CancellationTokenSource _tokenSource;
    
    private IPlayer _targetToHunt;
    
    [Inject] IPlayer _mainPlayer;
    [Inject] GameData _gameData;
    [Inject] BotManager _manager;
    [Inject] IBattleInfo _battleInfo;


    private BotWalkManager WalkManager => _manager.BotWalkManager;
    
    public void StartHunting() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        StartHuntingAsync(_tokenSource.Token).Forget();
    }

    public void StopHunting() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }
    

    private async UniTask StartHuntingAsync(CancellationToken token) {
        await UniTask.WaitWhile(() => _battleInfo.Players.Count == 0, cancellationToken: token);
        GetNextPlayerVictim();
        // Запускаем таймер каждый раз в фоне просто чекать ближайшего
        GetNextVictimByTimerAsync(token).Forget();
        while (!token.IsCancellationRequested) {
            // За типом бегаем постоянно выбранным
            WalkManager.SetAgentGoToPoint(GetNavMeshPosition(_targetToHunt.Transform.position));
            
            await UniTask.WaitForSeconds(_gameData.DurationToGoInPoint ,cancellationToken: token);
            GetNextPlayerVictim();
        }
    }

    private async UniTask GetNextVictimByTimerAsync(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            await UniTask.WaitForSeconds(_gameData.DurationToHuntWithoutCheck, cancellationToken: token);
            GetNextPlayerVictim();
        }
    }
    
    
    private void GetNextPlayerVictim() {
        
        // Выбор жертвой ГГ
        if (Random.value < _gameData.ChanceToGoPlayerInHunt 
            && _battleInfo.MainPlayerPlay 
            && !_mainPlayer.BonusUser.IsInvincibleAfterBonus
        ) {
            _targetToHunt = _battleInfo.MainPlayer;
            return;
        }
        
        // Жертва не обязательно ГГ
        _units = _battleInfo.Players;
        IPlayer closest = _units[0];
        float minSqrDistance = float.MaxValue;
    
        foreach (var victim in _units) {
            Vector3 offset = victim.Transform.position - transform.position;
            float sqrDist = offset.sqrMagnitude; // БЕЗ КОРНЯ!
        
            if (sqrDist < minSqrDistance) {
                minSqrDistance = sqrDist;
                closest = victim;
            }
        }
        // Debug.Log("Найдена жертва: " +  closest.);
        _targetToHunt = closest;
    }
    
    
    private Vector3 GetNavMeshPosition(Vector3 target) {
        if (NavMesh.SamplePosition(target, out NavMeshHit hit, _gameData.DistanceToFloor, NavMesh.AllAreas)) {
            return hit.position;
        }
        return target;
    }


}