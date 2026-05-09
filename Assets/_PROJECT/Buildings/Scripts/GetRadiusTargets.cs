using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;


[Serializable]
public class GetRadiusTargets : ITargetProvider, IGizmosDrawable {
    [SerializeField] private TargetType TargetType;
    [SerializeField] private float _radius;
    [SerializeField] private LayerMask _layer;
    
    
    private readonly Collider[] _buffer = new Collider[8];
   
    private IEnumerable<IPlayer> TargetList
        => TargetType == TargetType.Enemy ? 
            _battleInfo.Enemys 
            : 
            _battleInfo.Players;
    
    [Inject] IBattleInfo _battleInfo;
    
    
    
    public IEnumerable<IDamagable> GetTargets(Vector3 origin) {
        IEnumerable<IPlayer> targets = TargetList;
        foreach (var target in targets) {
            float distance = Vector3.SqrMagnitude(origin - target.Transform.position);
            if (distance <= _radius * _radius) {
                yield return target.Damagable;
            }
        }
    }

    public void DrawGizmos(Vector3 origin) {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(origin, _radius);
    }
}