using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

[Serializable]
public class ParabolicProjectileDelivery : IHitDelivery, ISoundPlayer {
    [Header("Параметры полёта")]
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _flyHeight;
    [SerializeField, Range(0,2)] private float _arcMultiplier = 1.3f;
    [SerializeField] private float _offsetToFloor = -1f;
    [SerializeField] private AnimationCurve[] _flightCurves;
    [Header("Откуда и что вылетает")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private BulletBase _bulletInstance;
    [Header("Радиус поражения")]
    [SerializeField] private float _hitRadius;
    
    [SerializeField, Range(0f, 1f)] private float _progressToShowPaintVisual = 1f;
    [field: SerializeField] public SoundType SoundType { get; private set; }
    
    
    private ObjectPoolManager _poolManager;
    private GameData _gameData;
    
    
    [Inject]
    public void Init(ObjectPoolManager poolManager, GameData gameData) {
        _poolManager = poolManager;
        _gameData = gameData;
    }
    
    public void Deliver(Vector3 origin, IPlayer target, List<IPlayer> allTargets, IEffect effect) {
        BulletBase paintBulletInstance = _poolManager.Spawn<BulletBase>(_bulletInstance.gameObject, _spawnPoint.position, PoolType.Bullets);
        if (paintBulletInstance == null) {
            Debug.LogError("Пуля = null");
            return;
        }
        paintBulletInstance.SetPosition(_spawnPoint.position);
        ParabolicBulletFlightAsync(paintBulletInstance, target.Transform, allTargets, effect).Forget();
    }
    

    private async UniTaskVoid ParabolicBulletFlightAsync(BulletBase paintBullet, Transform target, List<IPlayer> targetList, IEffect effect) {
        // Полёт
        
        float elapsedTime = 0;
        if (target == null) {
            Debug.LogError("target = " + target);
            return;
        }
        float distance = Vector3.Distance(paintBullet.gameObject.transform.position, target.position);
        float duration = distance * _arcMultiplier / _bulletSpeed;
        Vector3 bulletStartPos = paintBullet.transform.position;
        AnimationCurve flightCurve = _flightCurves.GetRandomElement();
        
        Vector3 targetPosition = target.position + new Vector3(0, _offsetToFloor, 0);
        
        
        paintBullet.InitShoot();
        while (elapsedTime <  duration) {
            float progress = elapsedTime / duration;
            if (progress > _progressToShowPaintVisual) {
                break;
            }
            
            float height = flightCurve.Evaluate(progress) * _flyHeight;
            Vector3 newPos = Vector3.Lerp(bulletStartPos, targetPosition, progress);
            newPos += new Vector3(0, height, 0);
            
            paintBullet.SetPosition(newPos); 
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();            
        }

        paintBullet.PlayToEnd();
        
        // Нанесение урона
        ApplyEffectToTargets(targetList, targetPosition, effect);

        await UniTask.WaitForSeconds(_gameData.PaintTimeToWaitAfterDestroyBullet);
        _poolManager.ReturnObjectToPool(paintBullet.gameObject, PoolType.Bullets);
    }
    
    
    private void ApplyEffectToTargets(List<IPlayer> targetList, Vector3 targetPosition, IEffect effect) {

        foreach (IPlayer player in targetList) {

            if(player == null || player.Damagable.CurrentHp == 0)
                continue;

            float sqrDistance =
                (player.Transform.position - targetPosition).sqrMagnitude; 

            if(sqrDistance <= _hitRadius * _hitRadius) {
                effect.ApplyEffect(player);
            }
        }
    }


}