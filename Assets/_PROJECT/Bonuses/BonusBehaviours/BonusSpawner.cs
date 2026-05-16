using System;
using System.Collections.Generic;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;




public class BonusSpawner : MonoBehaviour {
    [SerializeField] private List<WeightedPrefabItem<BonusCollectItem>> _bonusCollectItems;
  
    private bool _allowToSpawn;
    private WeightedPool<BonusCollectItem> _weightedPool;
    private List<GameObject> _spawns = new();
    private float _nextSpawnTime;
    private int _spawnCount => _spawns.Count;
    
    [Inject] private MapsToBattleChanger _mapsChanger;
    [Inject] private GameData _gameData;
    [Inject] private DiContainer _container;
    [Inject] private SpawnerInNavMesh _spawner;

    private void Awake() {
        _weightedPool = new WeightedPool<BonusCollectItem>(_bonusCollectItems);
    }

    
    private void Start() {
        StartSpawning();
    }


    private void Update() {
        if(!_allowToSpawn) return;
        if(Time.time < _nextSpawnTime) return;
        if(_spawnCount == _gameData.MaxCountBonusesInMap) return;
        _nextSpawnTime = Time.time + _gameData.DurationToSpawnNewBonus;
        SpawnBonus();
    }

    public void BonusDestroy(BonusCollectItem bonus) {
        _spawns.Remove(bonus.gameObject);
        Destroy(bonus.gameObject);
    }

    
    public void StopSpawning() {
        _allowToSpawn = false;
        foreach (var spawnObj in _spawns) {
            if (spawnObj != null) {
                Destroy(spawnObj);
            }
        }
        _spawns.Clear();
    }

    
    public void StartSpawning() {
        _nextSpawnTime = Time.time + _gameData.DurationToSpawnNewBonus;
        _allowToSpawn = true;
    }
    
    
    private BonusCollectItem GetNewBonusToSpawn() {
        return _weightedPool.GetRandom();
    }

    
    private Vector3 GetPointToSpawn() {
        // Raycast
        return _mapsChanger.GetBonusSpawnPoints.GetRandomElement().position;
    }

    
    private void SpawnBonus() {
        BonusCollectItem bonusPrefab = GetNewBonusToSpawn();
        Vector3 spawnPoint = GetPointToSpawn();
        
        GameObject newBonus = _spawner.SpawnObject(bonusPrefab.gameObject, spawnPoint);
        _container.InjectGameObject(newBonus);
        _spawns.Add(newBonus);
    }
}