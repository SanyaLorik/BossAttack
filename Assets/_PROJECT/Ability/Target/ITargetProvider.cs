using System.Collections.Generic;
using UnityEngine;

public interface ITargetProvider {
    public List<IPlayer> GetTargets(Transform origin, List<IPlayer> targetList, TargetType targetType);
    public IPlayer Same { get; }
    public void SetSame(IPlayer player);

}