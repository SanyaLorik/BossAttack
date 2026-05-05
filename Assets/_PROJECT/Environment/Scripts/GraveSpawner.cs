using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Zenject;


public class GraveSpawner : MonoBehaviour {
    [SerializeField] private GameObject _gravePrefab;

    [Inject] private BattleManager _battleManager;  
    [Inject] private MainGameStarter _gameStarter;
    [Inject] private RespawnManager _respawnManager;
    [Inject] private SpawnerInNavMesh _spawnerInNavMesh;

    private readonly List<GameObject> _gravesInstances = new();
    
    
    private void OnEnable() {
        _battleManager.PlayerDied += OnPlayerDied;
        _gameStarter.GameStarted += OnGameStarted;
    }

    private void OnGameStarted(bool started) {
        RemoveAllGraves();
    }

    
    private void OnPlayerDied(string _, Vector3 position) {
        SpawnGraveAsync(position).Forget();
    }

    private async UniTask SpawnGraveAsync(Vector3 position) {
        await UniTask.DelayFrame(5);
        GameObject newGrave = _spawnerInNavMesh.SpawnObject(_gravePrefab, position);
        _gravesInstances.Add(newGrave);
        
    }
    
    
    
    private void RemoveAllGraves() {
        _gravesInstances.ForEach(Destroy);
        _gravesInstances.Clear();
    } 
    
}