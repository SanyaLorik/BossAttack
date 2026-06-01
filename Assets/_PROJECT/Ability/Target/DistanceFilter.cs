using System;
using UnityEngine;

[Serializable]
public class DistanceFilter : ITargetFilter {
    [SerializeField] private float _allowedDistance;

    public void SetDistanceToAtack(float distance) {
        _allowedDistance = distance;
    }
    
    public bool CanApply(Transform origin, IPlayer target) {
        Vector3 toPlayer = target.Transform.position - origin.position;
        return toPlayer.sqrMagnitude <= _allowedDistance * _allowedDistance;
    }
}