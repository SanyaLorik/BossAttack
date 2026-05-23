using UnityEngine;

public class MineController : MonoBehaviour {
    [SerializeField] private AbilitySystem _abilitySystem;
    [SerializeField] private BuildZoneItem _buildZoneItem;

    private void OnEnable() {
        _abilitySystem.NewTargetAttacked += MineExplode;
    }
    
    private void OnDisable() {
        _abilitySystem.NewTargetAttacked -= MineExplode;
    }
    
    
    private void MineExplode(IPlayer player) {
        if (player == null) return;
        _abilitySystem.Stop();
        _buildZoneItem.Destroy();
    }
}