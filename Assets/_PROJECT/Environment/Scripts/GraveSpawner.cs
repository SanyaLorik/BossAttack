using System.Collections.Generic;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;


public class GraveSpawner : MonoBehaviour {
    [SerializeField] private GameObject _gravePrefabForPlayer;
    [SerializeField] private GameObject _gravePrefabForBoss;

    
    [Inject] private BattleManager _battleManager;  
    [Inject] private MainGameStarter _gameStarter;
    [Inject] private RespawnManager _respawnManager;
    [Inject] private SpawnerInFloor _spawnerInFloor;
    [Inject] private PlayersDiesObserver _playerDies;
    [Inject] private BossesDiesObserver _bossesDies;

    private readonly Dictionary<IPlayer, GameObject> _playerToGrave = new();
    private readonly List<GameObject> _bossesGraves = new(8);
    
    
    private void OnEnable() {
        _playerDies.PlayerDied += OnPlayerDied;
        _bossesDies.BossDied += OnBossDied;
        
        _playerDies.PlayerSpawned += DestroyGraveAfterPlayerSpawn;
        _gameStarter.GameStarted += OnGameStarted;
    }


    private void OnDisable() {
        _playerDies.PlayerDied -= OnPlayerDied;
        _bossesDies.BossDied -= OnBossDied;
        
        
        _playerDies.PlayerSpawned -= DestroyGraveAfterPlayerSpawn;
        _gameStarter.GameStarted -= OnGameStarted;
    }
    
    private void OnBossDied(IPlayer player) {
        SpawnGraveForBoss(player);
    }


    private void OnGameStarted(bool started) {
        RemoveAllGraves();
    }

    
    private void OnPlayerDied(IPlayer player) {
        SpawnGraveForPlayer(player);
    }

    
    private void SpawnGraveForPlayer(IPlayer player) {
        GameObject newGrave = _spawnerInFloor.SpawnObject(_gravePrefabForPlayer, player.Transform.position);
        if (_playerToGrave.ContainsKey(player) == false) {
            _playerToGrave[player] = newGrave;
        }
        else {
            Debug.LogError("Grave already spawned");            
        }
    }
    
    private void SpawnGraveForBoss(IPlayer player) {
        GameObject newGrave = _spawnerInFloor.SpawnObject(_gravePrefabForBoss, player.Transform.position);
       _bossesGraves.Add(newGrave);
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
        
        _bossesGraves.ForEach(Destroy);
        _bossesGraves.Clear();

    } 
    
}