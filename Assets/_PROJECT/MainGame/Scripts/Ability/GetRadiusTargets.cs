using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;


[Serializable]
public class GetRadiusTargets : ITargetProvider, IGizmosDrawable {
    [SerializeField] private TargetType TargetType;
    [SerializeField] private float _radius;
    
    
    private readonly Collider[] _buffer = new Collider[8];
    public IPlayer Same { get; private set; }
    
    public void SetSame(IPlayer player) {
        Same = player;
    }


    private IEnumerable<IPlayer> TargetList
        => TargetType == TargetType.Enemy ? 
            _battleInfo.Enemys 
            : 
            _battleInfo.Players;
    
    [Inject] IBattleInfo _battleInfo;
    
    
    
    public IEnumerable<IPlayer> GetTargets(Vector3 origin) {
        IEnumerable<IPlayer> targets = TargetList;
        foreach (var target in targets) {
            float distance = Vector3.SqrMagnitude(origin - target.Transform.position);
            if (distance <= _radius * _radius && target != Same) {
                yield return target;
            }
        }
    }


    public void DrawGizmos(Vector3 origin) {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(origin, _radius);
    }
}