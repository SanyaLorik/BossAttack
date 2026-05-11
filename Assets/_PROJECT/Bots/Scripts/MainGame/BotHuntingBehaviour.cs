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
    [SerializeField] private BossAbilityController _abilityController;
    
    private CancellationTokenSource _tokenSource;
    
    private List<AbilitySystem> _abilitySystems;
    
    private IPlayer _targetToHunt;
    private BotWalkManager WalkManager => _manager.BotWalkManager;
    
    [Inject] BotManager _botManager;
    [Inject] GameData _gameData;
    [Inject] BotManager _manager;
    [Inject] IBattleInfo _battleInfo;

    
    private void Awake() {
        _abilitySystems = _abilityController.GetAbilitys();
    }

    private void OnDisable() {
        _abilitySystems.ForEachs(a => a.NewTargetFinded -= TrySetNewTargetToHunt);
        _abilityController.NewAbilitySystemEnabled -= OnNewAbilitySystemEnabled;
        StopHunting();
    }
    
    
    private void OnEnable() {
        _abilitySystems.ForEachs(a => a.NewTargetFinded += TrySetNewTargetToHunt);
        _abilityController.NewAbilitySystemEnabled += OnNewAbilitySystemEnabled;
        _targetToHunt = GetRandomPlayerToFollow();
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



    private void OnNewAbilitySystemEnabled(AbilitySystem abilitySystem) {
        switch (abilitySystem.Type) {
            
            case AbilityType.Shooting:
                _botManager.Agent.speed = _gameData.BossSpeedInShooting;
                _botManager.Agent.stoppingDistance = _gameData.BossStoppingDistanceInShooting;
                break;
            
            case AbilityType.Melee:
                _botManager.Agent.speed = _gameData.BossSpeedInMelee;
                _botManager.Agent.stoppingDistance = _gameData.BossStoppingDistanceInMelee;
                break;
        }
        
    }

    
    private void TrySetNewTargetToHunt(IPlayer target) {
        // Если передал null то чтоб босс не стоял пусть идет за рандом землекопом
        if (target == null) {
            var newTarget = GetRandomPlayerToFollow();
            // Ставим новую если не равна null, иначе остается предыдущая
            if (newTarget != null) {
                _targetToHunt = newTarget;
            }
        }
        else {
            _targetToHunt = target;
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
