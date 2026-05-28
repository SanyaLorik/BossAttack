using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

[Serializable]
public class ParabolicProjectileDelivery : IHitDelivery, ISoundPlayer {
    [Header("Параметры полёта")] [SerializeField]
    private float _bulletSpeed;

    [SerializeField] private float _flyHeight;
    [SerializeField, Range(0, 2)] private float _arcMultiplier = 1.3f;
    [SerializeField] private AnimationCurve[] _flightCurves;

    [Header("Откуда и что вылетает")] [SerializeField]
    private Transform _spawnPoint;

    [SerializeField] private BulletBase _bulletInstance;

    [Header("Радиус поражения")] [SerializeField]
    private float _hitRadius;

    [Header("Визуал попадания")] [SerializeField]
    private GameObject _warningVisual;

    [SerializeField] private float _bulletLifeSecAfterHit = 1f;
    [SerializeField] private float _offsetToFloor = 1f;

    [field: SerializeField] public SoundType SoundType { get; private set; }


    private ObjectPoolManager _poolManager;
    private GameData _gameData;
    private SpawnerInFloor _spawner;


    [Inject]
    public void Init(ObjectPoolManager poolManager, GameData gameData, SpawnerInFloor spawner) {
        _poolManager = poolManager;
        _gameData = gameData;
        _spawner = spawner;
    }


    public void Deliver(Vector3 origin, IPlayer target, TargetType typeToAtack, List<IPlayer> allTargets, IEffect effect) {
        BulletBase paintBulletInstance =
            _poolManager.Spawn<BulletBase>(_bulletInstance.gameObject, _spawnPoint.position, PoolType.Bullets);
        paintBulletInstance.SetPosition(_spawnPoint.position);
        GameObject warningVisual = _spawner.SpawnObjectByRaycast(_warningVisual, target.Transform.position, .05f);
        ParabolicBulletFlightAsync(paintBulletInstance, target.Transform, typeToAtack, allTargets, effect, warningVisual)
            .Forget();
        
    }



    private async UniTaskVoid ParabolicBulletFlightAsync(
        BulletBase paintBullet, 
        Transform target, 
        TargetType typeToAtack, 
        List<IPlayer> targetList, 
        IEffect effect,
        GameObject warningVisual
    ) {
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

        Vector3 targetPosition = target.position;
        targetPosition.y = warningVisual.transform.position.y + _offsetToFloor;
        
        
        paintBullet.InitShoot();
        while (elapsedTime <  duration) {
            float progress = elapsedTime / duration;
            float height = flightCurve.Evaluate(progress) * _flyHeight;
            Vector3 newPos = Vector3.Lerp(bulletStartPos, targetPosition, progress);
            newPos += new Vector3(0, height, 0);
            
            paintBullet.SetPosition(newPos); 
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();            
        }

        paintBullet.PlayToEnd();
        
        // Нанесение урона
        ApplyEffectToTargets(targetList, typeToAtack, targetPosition, effect);
        Object.Destroy(warningVisual);
        
        await UniTask.WaitForSeconds(_bulletLifeSecAfterHit);
        _poolManager.ReturnObjectToPool(paintBullet.gameObject, PoolType.Bullets);
    }
    
    
    private void ApplyEffectToTargets(List<IPlayer> targetList, TargetType typeToAtack, Vector3 targetPosition, IEffect effect) {

        foreach (IPlayer player in targetList) {

            if(player == null || player.Damagable.CurrentHp == 0)
                continue;

            float sqrDistance =
                (player.Transform.position - targetPosition).sqrMagnitude; 

            if((player.TargetType & typeToAtack) != 0 && sqrDistance <= _hitRadius * _hitRadius) {
                effect.ApplyEffect(player);
            }
        }
    }


}