using System.Collections.Generic;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;


public class GraveSpawner : MonoBehaviour {
    [SerializeField] private GameObject _gravePrefab;

    [Inject] private BattleManager _battleManager;  
    [Inject] private MainGameStarter _gameStarter;
    [Inject] private RespawnManager _respawnManager;
    [Inject] private SpawnerInFloor _spawnerInFloor;
    [Inject] private PlayersDiesObserver _diesObserver;

    private readonly Dictionary<IPlayer, GameObject> _playerToGrave = new();
    
    
    private void OnEnable() {
        _diesObserver.PlayerDied += OnPlayerDied;
        _diesObserver.PlayerSpawned += DestroyGraveAfterPlayerSpawn;
        _gameStarter.GameStarted += OnGameStarted;
    }

    private void OnDisable() {
        _diesObserver.PlayerDied -= OnPlayerDied;
        _diesObserver.PlayerSpawned -= DestroyGraveAfterPlayerSpawn;
        _gameStarter.GameStarted -= OnGameStarted;
    }


    private void OnGameStarted(bool started) {
        RemoveAllGraves();
    }

    
    private void OnPlayerDied(IPlayer player) {
        SpawnGrave(player);
    }

    
    private void SpawnGrave(IPlayer player) {
        GameObject newGrave = _spawnerInFloor.SpawnObject(_gravePrefab, player.Transform.position);
        if (_playerToGrave.ContainsKey(player) == false) {
            _playerToGrave[player] = newGrave;
        }
        else {
            Debug.LogError("Grave already spawned");            
        }
    }
    
    
    private void DestroyGraveAfterPlayerSpawn(IPlayer player) {
        if (_playerToGrave.TryGetValue(player, out GameObject grave)) {
            Destroy(grave);
            _playerToGrave.Remove(player);
        }
    }

    
    
    private void RemoveAllGraves() {
        _playerToGrave.ForEach(kvp => Destroy(kvp.Value));
        _playerToGrave.Clear();
    } 
    
}