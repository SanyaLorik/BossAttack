using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class HuntingBehaviour : MonoBehaviour {
    [SerializeField] private BotWalkManager _botWalkManager;
    private List<IPlayer> _otherPlayers = new();
    
    
    
    private IPlayer _targetToHunt;
    
    [Inject] IPlayer _mainPlayer;
    [Inject] GameData _gameData;
    [Inject] BattleManager _battleManager;
    
    
    private async UniTask StartHunting(CancellationToken token) {
        GetNextPlayerVictim();
        // Запускаем таймер каждый раз в фоне просто чекать ближайшего
        GetNextVictimByTimerAsync(token).Forget();
        while (!token.IsCancellationRequested) {
            // За типом бегаем постоянно выбранным
            _botWalkManager.SetAgentGoToPoint(GetNavMeshPosition(_targetToHunt.Transform.position));
            
            await UniTask.WaitForSeconds(_gameData.DurationToGoInPoint ,cancellationToken: token);
            if (_targetToHunt.BonusUser.IsInvincibleAfterBonus) {
                GetNextPlayerVictim();
            }
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
            && _battleManager.MainPlayerPlay 
            && !_mainPlayer.BonusUser.IsInvincibleAfterBonus
           ) {
            _targetToHunt = _mainPlayer;
            return;
        }
        
        // Жертва не обязательно ГГ
        IPlayer closest = _otherPlayers[0];
        float minSqrDistance = float.MaxValue;
    
        foreach (var victim in _otherPlayers) {
            if(victim.BonusUser.IsInvincibleAfterBonus) continue;
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