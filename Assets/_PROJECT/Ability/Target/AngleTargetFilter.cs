using System;
using UnityEngine;

[Serializable]
public class AngleTargetFilter : ITargetFilter {
    [SerializeField] private float _viewAngle;
    
    public bool CanApply(Transform origin, IPlayer target) {
        return EnemyVisible(origin, target.Transform);
    }
    
    private bool EnemyVisible(Transform origin, Transform enemy) {
        Vector3 dir = enemy.position - origin.position;
        dir.y = 0f;
        dir.Normalize();

        Vector3 forward = origin.forward;
        forward.y = 0f;
        forward.Normalize();

        float angle = Vector3.Angle(forward, dir);

        return angle <= _viewAngle * 0.5f;
    }
}

[Serializable]
public class DistanceFilter : ITargetFilter {
    [SerializeField] private float _allowedDistance;

    public bool CanApply(Transform origin, IPlayer target) {
        Vector3 distance = target.Transform.position - origin.position;
        return distance.sqrMagnitude <= _allowedDistance * _allowedDistance;
    }
}