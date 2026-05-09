using System.Collections.Generic;
using UnityEngine;

public interface ITargetProvider {
    public IEnumerable<IPlayer> GetTargets(Vector3 origin);
}