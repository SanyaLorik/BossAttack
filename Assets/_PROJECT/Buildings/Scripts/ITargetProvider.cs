using System.Collections.Generic;
using UnityEngine;

public interface ITargetProvider {
    public IEnumerable<IDamagable> GetTargets(Vector3 origin);
}