using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

[Serializable]
public class BulletsSpawner : ITickBehaviour, ISoundPlayer {
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Bullet _bulletInstance;
    [field: SerializeField] public SoundType SoundType { get; private set; }


    private ObjectPoolManager _poolManager;
    private GameData _gameData;
    
    [Inject]
    public void Init(ObjectPoolManager poolManager, GameData gameData) {
        _poolManager = poolManager;
        _gameData = gameData;
    }


    public void OnTick(Vector3 origin, IPlayer damagable) {
        ShootInTarget(damagable.PointToAtack);
    }

    private void ShootInTarget(Transform target) {
        Bullet bulletInstance = _poolManager.Spawn<Bullet>(_bulletInstance.gameObject, _spawnPoint.position, PoolType.Bullets);
        bulletInstance.SetPosition(_spawnPoint.position);
        BulletFlightAsync(bulletInstance, target).Forget();
    }

    private async UniTaskVoid BulletFlightAsync(Bullet bullet, Transform target) {
        // Полёт
        
        float elapsedTime = 0;
        float distance = Vector3.Distance(bullet.gameObject.transform.position, target.position);
        float duration = distance / _bulletSpeed;
        Vector3 bulletStartPos = bullet.transform.position;
        float sign = Random.value > 0.5f ? -1 : 1;
        float yOffset = Random.Range(0, _gameData.YBulletOffset);
        Vector3 targetPosition = target.position + new Vector3(0f, sign * yOffset, 0);
        bullet.InitShoot();
        
        while (elapsedTime <  duration) {
            float progress = elapsedTime / duration;
            if (progress > _gameData.ProgressToShowPaintVisual) {
                break;
            }
            bullet.SetPosition(Vector3.Lerp(bulletStartPos, targetPosition, progress)); 
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();            
        }

        bullet.transform.SetParent(target.transform, true);
        bullet.PlayToEnd();

        await UniTask.WaitForSeconds(_gameData.PaintTimeToWaitAfterDestroyBullet);
        _poolManager.ReturnObjectToPool(bullet.gameObject, PoolType.Bullets);
    }

}