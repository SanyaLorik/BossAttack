using UnityEngine;
using Zenject;

public class PlayerAbilityController : MonoBehaviour {
    [SerializeField] private AbilitySystem _ability;

    [Inject] PlayerStatsCalculator _playerStatsCalculator;
    
    
    public void Awake() {
        InitPlayerDamage();
        InitPlayerCapacity();
        InitPlayerRateOfFire();
    }

    private void InitPlayerDamage() {
        var abilityValue = _ability.Effect as IEffectValue;
        if (abilityValue != null) abilityValue.SetValueGetter(() => _playerStatsCalculator.PlayerDamage);
    }
    
    private void InitPlayerCapacity() {
        // IN Dev
    }
    
    private void InitPlayerRateOfFire() {
        _ability.SetValueGetter(() => _playerStatsCalculator.PlayerRateOfFire);

    }
}