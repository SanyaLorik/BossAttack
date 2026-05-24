using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;
using UnityEngine;
using Zenject;

[Serializable]
public class NotGuaranteedHitBulletsSpawner : IHitDelivery, ISoundPlayer {
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private BulletBase _bulletInstance;
    [SerializeField] private float _hitRadius = 1f;
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
        BulletFlightAsync(paintBulletInstance, target, targetList,  effect).Forget();
    }


    private async UniTaskVoid BulletFlightAsync(BulletBase paintBullet, IPlayer target, List<IPlayer> targetList, IEffect effect) {
        // Полёт
        Transform targetTransform = target.PointToAtack;
        
        Vector3 direction = (targetTransform.position - paintBullet.gameObject.transform.position);
        Vector3 directionNormalized = direction.normalized;
        
        paintBullet.InitShoot();
        
        float traveledDistance = 0f;
        bool isHited = false;
        
        // Берем с запасом ну шоб летело шо называется
        float maxDistance = direction.magnitude * 2f;
        
        while (traveledDistance < maxDistance) {
            
            Vector3 prevPosition = paintBullet.transform.position;
            Vector3 nextPosition = prevPosition + directionNormalized * (_bulletSpeed * Time.deltaTime);
            
            paintBullet.SetPosition(nextPosition); 
            traveledDistance += _bulletSpeed * Time.deltaTime;
            
            // Проверка попадания
            if (TryApplyEffect(targetList, paintBullet.transform, effect)) {
                isHited = true;
                paintBullet.PlayToEnd();
                await UniTask.WaitForSeconds(_bulletLifeSecAfterHit);

                if(paintBullet != null) _poolManager.ReturnObjectToPool(paintBullet.gameObject, PoolType.Bullets);
                break;
            }
            
            await UniTask.Yield();            
        }
        if(!isHited) _poolManager.ReturnObjectToPool(paintBullet.gameObject, PoolType.Bullets);
        
    }

    
    private bool TryApplyEffect(List<IPlayer> targetList, Transform bullet, IEffect effect) {

        foreach (IPlayer player in targetList) {

            if(player == null || player.Damagable.CurrentHp == 0)
                continue;

            float sqrDistance =
                (player.PointToAtack.position - bullet.transform.position).sqrMagnitude; 

            if(sqrDistance <= _hitRadius * _hitRadius) {
                effect.ApplyEffect(player);
                // bullet.transform.SetParent(player.Transform, true);
                return true;    
            }
        }
        return false;
    }
    
    
    
    
    private Vector3 Offset() {
        float signY = Random.value > 0.5f ? -1 : 1;
        float yOffset = Random.Range(0, _gameData.YBulletOffset);
        
        return new Vector3(0f, signY * yOffset, 0);
    }


}