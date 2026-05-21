using System.Collections.Generic;
using UnityEngine;

public interface ITargetProvider {
    public List<IPlayer> GetTargets(Vector3 origin);
    public IPlayer Same { get; }
    public void SetSame(IPlayer player);

}