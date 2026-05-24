using UnityEngine;

public interface ICombatTarget {
    TargetType TargetType { get; }
    IDamagable Damagable { get; }
    Transform PointToAtack { get; }
    Transform Transform { get; }
    void TeleportToPoint(Vector3 point);
    
}