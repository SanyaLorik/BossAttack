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
    [SerializeField] private LayerMask _layer;
    
    
    private readonly Collider[] _buffer = new Collider[8];
   
    private IEnumerable<IPlayer> TargetList
        => TargetType == TargetType.Enemy ? 
            _battleInfo.Enemys 
            : 
            _battleInfo.Players;
    
    
    [Inject] IBattleInfo _battleInfo;


    
    public IEnumerable<IPlayer> GetTargets(Vector3 origin) {
        IPlayer closestUnit = null;
        float bestSqr = float.MaxValue;
        float sqrRange = _distance * _distance;
        
        
        IEnumerable<IPlayer> targets = TargetList;
        foreach (var target in targets) {
            
            Vector3 direction = target.Transform.position - origin;
            float sqrDistance = Vector3.SqrMagnitude(direction);
            
            // Скип если далеко
            if (sqrDistance > sqrRange) 
                continue;
 
            // Проверка что между нами стенка
            if(!HasLineOfSight(origin, direction, target)) 
                continue;

            if (sqrDistance <= bestSqr) {
                bestSqr = sqrDistance;
                closestUnit = target;
            }
        }
        if (closestUnit != null)
            yield return closestUnit;
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