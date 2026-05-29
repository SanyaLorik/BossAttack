using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;


[Serializable]
public class GetRadiusTargets : ITargetProvider, IGizmosDrawable {
    [SerializeField] private float _radius;
    
    
    private readonly Collider[] _buffer = new Collider[8];
    public IPlayer Same { get; private set; }
    
    public void SetSame(IPlayer player) {
        Same = player;
    }
    
    
    
    
    
    public List<IPlayer> GetTargets(Transform origin, List<IPlayer> targetList, TargetType targetType) {
        List<IPlayer> result = new ();
        foreach (var target in targetList) {
            if (target.Damagable.CurrentHp == 0 || (target.TargetType & targetType) == 0) continue;
            float distance = Vector3.SqrMagnitude(origin.position - target.Transform.position);
            if (distance <= _radius * _radius && target != Same) {
                result.Add(target);
            }
        }
        return result;
    }


    public void DrawGizmos(Vector3 origin) {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(origin, _radius);
    }
}