using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;


[Serializable]
public class GuaranteedHitBulletsSpawner : IHitDelivery, ISoundPlayer {
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private BulletBase _bulletInstance;
    [SerializeField, Range(0f, 1f)] private float _progressToShowPaintVisual = 0.9f;
    [SerializeField] private float _bulletLifeSecAfterHit = 1f;

    [field: SerializeField] public SoundType SoundType { get; private set; }


    private ObjectPoolManager _poolManager;
    private GameData _gameData;
    
    
    [Inject]
    public void Init(ObjectPoolManager poolManager, GameData gameData) {
        _poolManager = poolManager;
        _gameData = gameData;
    }

    
    public void Deliver(Vector3 origin, IPlayer target, List<IPlayer> targetList, IEffect effect) {
        BulletBase paintBulletInstance = _poolManager.Spawn<BulletBase>(_bulletInstance.gameObject, _spawnPoint.position, PoolType.Bullets);
        if (paintBulletInstance == null) {
            Debug.LogError("Пуля = null");
            return;
        }
        paintBulletInstance.SetPosition(_spawnPoint.position);
        BulletFlightAsync(paintBulletInstance, target, effect).Forget();
    }


    private async UniTaskVoid BulletFlightAsync(BulletBase paintBullet, IPlayer target, IEffect effect) {
        // Полёт
        Transform targetTransform = target.PointToAtack;
        
        float elapsedTime = 0;
        float distance = Vector3.Distance(paintBullet.gameObject.transform.position, targetTransform.position);
        float duration = distance / _bulletSpeed;
        Vector3 bulletStartPos = paintBullet.transform.position;

        
        paintBullet.InitShoot();
        
        while (elapsedTime <  duration) {
            float progress = elapsedTime / duration;
            if (progress > _progressToShowPaintVisual) {
                break;
            }
            paintBullet.SetPosition(Vector3.Lerp(bulletStartPos, targetTransform.position + Offset(), progress)); 
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();            
        }

        paintBullet.transform.SetParent(targetTransform, true);
        paintBullet.PlayToEnd();
        
        // Нанесение урона
        effect.ApplyEffect(target);

        await UniTask.WaitForSeconds(_bulletLifeSecAfterHit);
        _poolManager.ReturnObjectToPool(paintBullet.gameObject, PoolType.Bullets);
    }

    private Vector3 Offset() {
        float signY = Random.value > 0.5f ? -1 : 1;
        float yOffset = Random.Range(0, _gameData.YBulletOffset);
        
        return new Vector3(0f, signY * yOffset, 0);
    }


}