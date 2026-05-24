using UnityEngine;
using Zenject;

public class BoostCollectItem : MonoBehaviour, ICombatTarget {
    
    [Inject] PlayerBoostBoxesSystem _playerBoostBoxesSystem;
    [Inject] PlayerMovement _mainPlayer;

    public TargetType TargetType => TargetType.Boost;

    public IDamagable Damagable { get; }
    public Transform PointToAtack { get; }
    public Transform Transform { get; }
    public void TeleportToPoint(Vector3 point) { }

    private void OnEnable() {
        Damagable.DamagableDied += OnDamagableDied;
    }

    private void OnDamagableDied(IDamagable obj) {
        _playerBoostBoxesSystem.PlusOne();
    }

    
}