using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitInfo {
    public IDamagable Target;
    public Transform Transform;
}

public interface IBattleInfo {
    public List<UnitInfo> EnemysDamagable { get; }
    public List<UnitInfo> PlayersDamagable { get; }
    public List<UnitInfo> BuildingsDamagable { get; }
}