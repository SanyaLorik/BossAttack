using UnityEngine;
using UnityEngine.AI;

public class SpawnerInNavMesh {

    private readonly RespawnManager _respawnManager;
    private readonly GameData _gameData;
    
    public SpawnerInNavMesh(RespawnManager respawnManager, GameData gameData) {
        _respawnManager = respawnManager;
        _gameData = gameData;
    }
    
    
    public GameObject SpawnObject(GameObject prefab, Vector3 position) {
        GameObject newObject;
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, _gameData.DistanceToFindNavMeshToBuild, NavMesh.AllAreas)) {
            newObject = Object.Instantiate(prefab, hit.position, Quaternion.identity);
            // Debug.Log($"Могила поставлена на NavMesh: {hit.position}");
        }
        else {
            // Фолбэк — ищем на спавне
            if (NavMesh.SamplePosition(_respawnManager.SpawnPoint.position, out NavMeshHit fallbackHit, _gameData.DistanceToFindNavMeshToBuild, NavMesh.AllAreas)) {
                newObject = Object.Instantiate(prefab, fallbackHit.position, Quaternion.identity);
                Debug.LogWarning($"SpawnerInNavMesh: не нашли NavMesh, ставим на спавн: {fallbackHit.position}");
                return newObject;
            }
            Debug.LogError($"SpawnerInNavMesh: " +
                           $"NavMesh не найден нигде. Позиция спавна: {position}, спавн: {_respawnManager.SpawnPoint.position}");
            newObject = Object.Instantiate(prefab, position, Quaternion.identity);
        }
        return newObject;
    }
    
    
    private GameObject SpawnObjectByRaycast(GameObject prefab, Vector3 position) {
        if (Physics.Raycast(position, Vector3.down * _gameData.DistanceToFindNavMeshToBuild, out RaycastHit hit)) {
            GameObject grave = Object.Instantiate(prefab, hit.point, Quaternion.identity);
            Debug.Log("Спавн успешен");
            return grave;
        }
        else {
            GameObject grave = Object.Instantiate(prefab, position, Quaternion.identity);
            Debug.Log("Земля не найдена, спавн в точке");
            return grave;
        }
    }
}