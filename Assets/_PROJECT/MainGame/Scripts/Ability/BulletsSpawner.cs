using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

[Serializable]
public class BulletsSpawner : ITickBehaviour, ISoundPlayer {
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private BulletBase _bulletInstance;
    [SerializeField, Range(0f, 1f)] private float _progressToShowPaintVisual = 0.9f;
    
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
        BulletBase paintBulletInstance = _poolManager.Spawn<BulletBase>(this._bulletInstance.gameObject, _spawnPoint.position, PoolType.Bullets);
        if (paintBulletInstance == null) {
            Debug.LogError("Пуля = " + paintBulletInstance);
            return;
        }
        paintBulletInstance.SetPosition(_spawnPoint.position);
        BulletFlightAsync(paintBulletInstance, target).Forget();
    }

    private async UniTaskVoid BulletFlightAsync(BulletBase paintBullet, Transform target) {
        // Полёт
        
        float elapsedTime = 0;
        if (target == null) {
            Debug.LogError("target = " + target);
            return;
        }
        float distance = Vector3.Distance(paintBullet.gameObject.transform.position, target.position);
        float duration = distance / _bulletSpeed;
        Vector3 bulletStartPos = paintBullet.transform.position;

        
        paintBullet.InitShoot();
        
        while (elapsedTime <  duration) {
            float progress = elapsedTime / duration;
            if (progress > _progressToShowPaintVisual) {
                break;
            }
            paintBullet.SetPosition(Vector3.Lerp(bulletStartPos, target.position + Offset(), progress)); 
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();            
        }

        paintBullet.transform.SetParent(target.transform, true);
        paintBullet.PlayToEnd();

        await UniTask.WaitForSeconds(_gameData.PaintTimeToWaitAfterDestroyBullet);
        _poolManager.ReturnObjectToPool(paintBullet.gameObject, PoolType.Bullets);
    }

    private Vector3 Offset() {
        float signY = Random.value > 0.5f ? -1 : 1;
        float yOffset = Random.Range(0, _gameData.YBulletOffset);
        
        return new Vector3(0f, signY * yOffset, 0);
    }
}