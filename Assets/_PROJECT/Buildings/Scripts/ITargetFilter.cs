using UnityEngine;

public interface ITargetFilter {
    public bool CanApply(Transform origin, IDamagable target);
}