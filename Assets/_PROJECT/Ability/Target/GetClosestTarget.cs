using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[Serializable]
public enum TargetType {
    Enemy,
    Player,
}


[Serializable]
public class GetClosestTarget : ITargetProvider, IGizmosDrawable {
    [SerializeField] private TargetType TargetType;
    [SerializeField] private float _distance;
    
    
    public IPlayer Same { get; private set; }
    
    private IPlayer _closestUnit;
    private float _bestSqr;
    private float _sqrRange;
    
    private IEnumerable<IPlayer> TargetList
        => TargetType == TargetType.Enemy ? 
            _battleInfo.Enemys 
            : 
            _battleInfo.Players;
    
    
    [Inject] IBattleInfo _battleInfo;

    public void SetSame(IPlayer player) {
        Same = player;
    }

   
    public List<IPlayer> GetTargets(Vector3 origin) {
        List<IPlayer> result = new();
        _closestUnit = null;
        _bestSqr = float.MaxValue;
        _sqrRange = _distance * _distance;
        
        
        IEnumerable<IPlayer> targets = TargetList;
        foreach (var target in targets) {
            if(target == Same || target.Damagable == null) continue;
            if (target.Damagable.CurrentHp == 0) {
                continue;
            }
            CheckTarget(target, origin);
        }
        
        if (_closestUnit != null) {
            result.Add(_closestUnit);
        }
        return result;
    }

    private bool CheckTarget(IPlayer target, Vector3 origin) {
        Vector3 direction = target.Transform.position - origin;
        float sqrDistance = Vector3.SqrMagnitude(direction);
            
        // Скип если далеко
        if (sqrDistance > _sqrRange) 
            return false;
 
        // Проверка что между нами стенка
        if(!HasLineOfSight(origin, direction, target)) 
            return false;

        if (sqrDistance <= _bestSqr) {
            _bestSqr = sqrDistance;
            _closestUnit = target;
        }
        return true;
    }




    private bool HasLineOfSight(Vector3 origin, Vector3 direction, IPlayer target) {
        // if (Physics.Raycast(origin, direction.normalized, out RaycastHit hitInfo, _distance)) {
        //     if (hitInfo.transform == target.Transform) {
        //         return true;
        //     }
        // }
        // return false;
        return true;
    }

    public void DrawGizmos(Vector3 origin) {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, _distance);
    }
}