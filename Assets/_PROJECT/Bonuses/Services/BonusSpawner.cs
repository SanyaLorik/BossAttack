using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;


public class BonusSpawner : MonoBehaviour {
    [SerializeField] private List<WeightedPrefabItem<BonusCollectItem>> _bonusCollectItems;
    private Dictionary<Transform, GameObject> _pointToBonus = new();

    [Header("Параметры спавна")] 
    [SerializeField] private int _startIndex;
    [SerializeField] private int _selectionSize;
    
    
    private bool _allowToSpawn;
    private WeightedPool<BonusCollectItem> _weightedPool;
    
    
    private float _nextSpawnTime;
    private int _spawnCount => _pointToBonus.Count;
    
    [Inject] private MapsToBattleChanger _mapsChanger;
    [Inject] private GameData _gameData;
    [Inject] private DiContainer _container;
    [Inject] private SpawnerInFloor _spawner;
    [Inject] private PlayerMovement _player;

    private void Awake() {
        _weightedPool = new WeightedPool<BonusCollectItem>(_bonusCollectItems);
    }

    
    private void Start() {
        StartSpawning();
    }


    private void Update() {
        if(!_allowToSpawn) return;
        if(Time.time < _nextSpawnTime) return;
        if (_spawnCount == _gameData.MaxCountBonusesInMap) return;
        _nextSpawnTime = Time.time + _gameData.DurationToSpawnNewBonus;
        SpawnBonus();
        if (_spawnCount != _gameData.MaxCountBonusesInMap) {
            _nextSpawnTime = Time.time + _gameData.DurationToSpawnNewBonus;
        }
    }

    public void BonusDestroy(BonusCollectItem bonus) {
        // Если уже фулл, то обновим т.к 1 удалим
        if (_spawnCount == _gameData.MaxCountBonusesInMap) {
            _nextSpawnTime = Time.time + _gameData.DurationToSpawnNewBonus;
        }
        var keyValuePair = _pointToBonus.First(p => p.Value == bonus.gameObject);
        _pointToBonus.Remove(keyValuePair.Key);
        Destroy(bonus.gameObject);
    }

    
    public void StopSpawning() {
        _allowToSpawn = false;
        foreach (var spawnObj in _pointToBonus) {
            if (spawnObj.Value != null) {
                Destroy(spawnObj.Value);
            }
        }
        _pointToBonus.Clear();
    }

    
    public void StartSpawning() {
        _nextSpawnTime = Time.time + _gameData.DurationToSpawnNewBonus;
        _allowToSpawn = true;
    }
    
    
    private BonusCollectItem GetNewBonusToSpawn() {
        return _weightedPool.GetRandom();
    }

    
    private Transform GetPointToSpawn() {
        Transform[] points = _mapsChanger.GetBonusSpawnPoints; 
        
        var data = GetDataToSort(points);

        Array.Sort(data, (a,b) 
            => b.dist.CompareTo(a.dist));

        
        var findPoint = FindRandomPoint(data);

        // PrintData(data);
        
        return findPoint;
    }

    private Transform FindRandomPoint((Transform t, float dist)[] data) {
        int selectionSizeCuted = Math.Min(_selectionSize, data.Length);
        

        int randomIndex = Random.Range(_startIndex, _startIndex + selectionSizeCuted);
        bool choosed = false;

        for (var i = 0; i < data.Length; i++) {
            int index = (i + randomIndex) % data.Length;
            Transform currentPoint = data[index].t;

            // Debug.Log("Проверка индекса " + index);
            if (_pointToBonus.ContainsKey(currentPoint) == false) {
                randomIndex = index;
                choosed = true;
                break;
            }
        }

        if (choosed) {
            // Debug.Log("Успешно выбрана точка " + randomIndex);
            return data[randomIndex].t;
        }
        Debug.LogError("Указан маленький диапазон SelectionSize = " + _selectionSize);
        return data[0].t;
    }
    


    private void PrintData((Transform t, float dist)[] data) {
        for (var index = 0; index < data.Length; index++) {
            var point = data[index];
            Debug.Log($"Точка {index}, Расстояние до игрока (^2) = {point.dist}");
        }
    }
    
    
    private (Transform t, float dist)[] GetDataToSort(Transform[] points) {
        var data = new (Transform t, float dist)[points.Length];

        for (int i = 0; i < points.Length; i++) {
            data[i] = (
                points[i], 
                (points[i].position - _player.Transform.position).sqrMagnitude
            );
        }

        return data;
    }



    private void SpawnBonus() {
        BonusCollectItem bonusPrefab = GetNewBonusToSpawn();
        Transform spawnPoint = GetPointToSpawn();
        
        GameObject newBonus = _spawner.SpawnObject(bonusPrefab.gameObject, spawnPoint.position);
        // Debug.Log($"Спавн бонуса в {spawnPoint.position}");
        _container.InjectGameObject(newBonus);
        _pointToBonus[spawnPoint] = newBonus;
    }
}