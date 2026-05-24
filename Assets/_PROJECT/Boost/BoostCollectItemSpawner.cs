using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BoostCollectItemSpawner : MonoBehaviour {
    [SerializeField] private BoostCollectItem _boostPrefab;
    
    private List<BoostCollectItem> _boostCollectItems = new();
    private CancellationTokenSource _tokenSource;
    
    [Inject] BattleManager _battleManager;
    [Inject] MainGameStarter _mainGameStarter;
    [Inject] MapsToBattleChanger _mapsChanger;
    [Inject] GameData _gameData;
    [Inject] PlayerRegister _playerRegister;

    
    private void OnDisable() {
        _mainGameStarter.GameStarted -= OnGameStarted;
        UniTaskHelper.DisposeTask(ref _tokenSource);
    }
    
    
    private void OnEnable() {
        _mainGameStarter.GameStarted += OnGameStarted;
    }
    
    private void OnGameStarted(bool started) {
        if (started) {
            SpawnBoosts();
        }
        else {
            DestroyBoosts();
        }
    }

    private void SpawnBoosts() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        DestroyBoosts();
        SpawnBoostsAsync(_tokenSource.Token).Forget();
    }
    

    private async UniTask SpawnBoostsAsync(CancellationToken token) {
        foreach (var point in _mapsChanger.GetBoostSpawnPoints) {
            SpawnNewBoost(point.position);
            await UniTask.Yield(token);
        }
    }

    private void SpawnNewBoost(Vector3 position) {
        BoostCollectItem newBoost = Instantiate(_boostPrefab);
        newBoost.TeleportToPoint(position);
        // _playerRegister.RegisterUnit();
    }

    private void DestroyBoosts() {
        foreach (var boost in _boostCollectItems) {
            if (boost != null) {
                Destroy(boost.gameObject);
            }
        }
        _boostCollectItems.Clear();
    }
}