using UnityEngine;

public interface ITargetFilter {
    public bool CanApply(Transform origin, IPlayer target);
}