using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[Serializable]
public class BulletsSpawner : ITickBehaviour {
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Bullet _bulletInstance;

    private ObjectPoolManager _poolManager;
    
    [Inject]
    public void Init(ObjectPoolManager poolManager) {
        _poolManager = poolManager;
    }


    public void OnTick(Vector3 origin, IPlayer damagable) {
        ShootInTarget(damagable.Transform.position);
    }

    private void ShootInTarget(Vector3 target) {
        Bullet bulletInstance = _poolManager.Spawn<Bullet>(_bulletInstance.gameObject, target, PoolType.Bullets);
        bulletInstance.SetPosition(_spawnPoint.position);
        BulletFlightAsync(bulletInstance, target).Forget();
    }

    private async UniTaskVoid BulletFlightAsync(Bullet bulletInstance, Vector3 target) {
        // Полёт
        
        float elapsedTime = 0;
        float distance = Vector3.Distance(bulletInstance.gameObject.transform.position, target);
        float duration = distance / _bulletSpeed;
        Vector3 bulletStartPos = bulletInstance.transform.position;
        
        while (elapsedTime <  duration) {
            float progress = elapsedTime / duration;
            bulletInstance.SetPosition(Vector3.Lerp(bulletStartPos, target, progress)); 
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();            
        }
        _poolManager.ReturnObjectToPool(bulletInstance.gameObject, PoolType.Bullets);
    }
}