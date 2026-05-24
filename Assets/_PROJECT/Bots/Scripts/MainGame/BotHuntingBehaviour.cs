using System;
using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;


[Serializable]
public class BotHuntingBehaviour : MonoBehaviour {
    [SerializeField] private BossAbilityController abilityController;
    [SerializeField] private BotManager _manager;
    
    private CancellationTokenSource _tokenSource;
    
    private AbilitySystem Ability =>  abilityController.Ability;
    
    private IPlayer _targetToHunt;
    private BotWalkManager WalkManager => _manager.BotWalkManager;
    

    private GameData _gameData;
    private IBattleInfo _battleInfo;

    
    
    [Inject]
    public void Initialize(GameData gameData, IBattleInfo battleInfo) {
        _gameData = gameData;
        _battleInfo = battleInfo;
        Ability.NewTargetFinded += TrySetNewTargetToHunt;
        TrySetNewTargetToHunt(_targetToHunt);
        StartHunting();
    }
    



    private IPlayer GetRandomPlayerToFollow() {
        IPlayer player = EnumerableHelper.GetRandomElementInListWhere(
            _battleInfo.Players,
            player => player.Damagable.CurrentHp != 0
        );
        return player;
    }


    private bool _findInAllPlace;
    private void TrySetNewTargetToHunt(IPlayer target) {
        // Если передал null то чтоб босс не стоял пусть идет за рандом землекопом
        if (target == null && !_findInAllPlace) {
            _findInAllPlace =  true;
            var newTarget = GetRandomPlayerToFollow();
            // Ставим новую если не равна null, иначе остается предыдущая
            if (newTarget != null) {
                _targetToHunt = newTarget;
            }
        }
        else {
            _targetToHunt = target;
            _findInAllPlace = false;
        }
    }
    
    
    private void StartHunting() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        _tokenSource = new CancellationTokenSource();
        StartHuntingAsync(_tokenSource.Token).Forget();
    }

    public void StopHunting() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }
    

    private async UniTask StartHuntingAsync(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            // За типом бегаем постоянно выбранным
            await UniTask.WaitWhile(() => _targetToHunt == null, cancellationToken: token);
            WalkManager.SetAgentGoToPoint(GetNavMeshPosition(_targetToHunt.Transform.position));
            await UniTask.WaitForSeconds(_gameData.DurationToGoInPoint ,cancellationToken: token);
        }
    }
    
    private Vector3 GetNavMeshPosition(Vector3 target) {
        if (NavMesh.SamplePosition(target, out NavMeshHit hit, _gameData.DistanceToFloor, NavMesh.AllAreas)) {
            return hit.position;
        }
        return target;
    }
    
    
    
    private async UniTask StartDefaultHuntingAsync(CancellationToken token) {
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
        //
        // // Выбор жертвой ГГ
        // if (Random.value < _gameData.ChanceToGoPlayerInHunt 
        //     && _battleInfo.MainPlayerPlay 
        //     && !_mainPlayer.BonusUser.IsInvincibleAfterBonus
        // ) {
        //     _targetToHunt = _battleInfo.MainPlayer;
        //     return;
        // }
        //
        // // Жертва не обязательно ГГ
        // _units = _battleInfo.Players;
        // IPlayer closest = _units[0];
        // float minSqrDistance = float.MaxValue;
        //
        // foreach (var victim in _units) {
        //     Vector3 offset = victim.Transform.position - transform.position;
        //     float sqrDist = offset.sqrMagnitude; // БЕЗ КОРНЯ!
        //
        //     if (sqrDist < minSqrDistance) {
        //         minSqrDistance = sqrDist;
        //         closest = victim;
        //     }
        // }
        // // Debug.Log("Найдена жертва: " +  closest.);
        // _targetToHunt = closest;
    }

}
