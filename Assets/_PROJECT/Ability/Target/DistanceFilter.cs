using System;
using UnityEngine;

[Serializable]
public class DistanceFilter : ITargetFilter {
    [SerializeField] private float _allowedDistance;

    public bool CanApply(Transform origin, IPlayer target) {
        Vector3 toPlayer = target.Transform.position - origin.position;
        return toPlayer.sqrMagnitude <= _allowedDistance * _allowedDistance;
    }
}