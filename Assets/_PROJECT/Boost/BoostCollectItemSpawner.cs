using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BoostCollectItemSpawner : MonoBehaviour {
    [SerializeField] private BoostCollectItem _boostPrefab;
    
    private List<BoostCollectItem> _boostCollectItems = new();
    
    [Inject] BattleManager _battleManager;
    [Inject] MainGameStarter _mainGameStarter;
    [Inject] MapsToBattleChanger _mapsChanger;
    [Inject] GameData _gameData;
    [Inject] PlayerRegister _playerRegister;
    [Inject] DiContainer _diContainer;

    
    
    private void OnDisable() {
        _mainGameStarter.GameStarted -= OnGameStarted;
        _mapsChanger.NewMapChanged -= OnNewMapChanged;
    }
    
    
    private void OnEnable() {
        _mainGameStarter.GameStarted += OnGameStarted;
        _mapsChanger.NewMapChanged += OnNewMapChanged;
    }

    private void OnNewMapChanged() {
        SpawnBoosts();
    }

    private void OnGameStarted(bool started) {
        if (!started) {
            DestroyBoosts();
        }
    }

    private void SpawnBoosts() {
        DestroyBoosts();
        
        foreach (var point in _mapsChanger.GetBoostSpawnPoints) {
            BoostCollectItem newBoost =
                _diContainer.InstantiatePrefabForComponent<BoostCollectItem>(_boostPrefab);

            newBoost.TeleportToPoint(point.position);
            _playerRegister.RegisterUnit(newBoost);;
        }
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