using UnityEngine;
using Zenject;

public class PlayerAbilityController : MonoBehaviour {
    [SerializeField] private AbilitySystem _ability;

    [Inject] PlayerStatsCalculator _playerStatsCalculator;
    [Inject] IPlayer _player;
    
    
    public void Awake() {
        InitAbility();
        InitPlayerDamage();
        InitPlayerCapacity();
        InitPlayerRateOfFire();
    }
    
    private void InitAbility() {
        _ability.SetSame(_player);
    }

    private void InitPlayerDamage() {
        var abilityValue = _ability.Effect as IValueGetter;
        if (abilityValue != null) abilityValue.SetValueGetter(() => _playerStatsCalculator.PlayerDamage);
    }
    
    private void InitPlayerCapacity() {
        var abilityCapacity = _ability.AtackCapacity as IValueGetter;
        if (abilityCapacity != null) abilityCapacity.SetValueGetter(() => _playerStatsCalculator.PlayerCapacity);
        
    }
    
    private void InitPlayerRateOfFire() {
        _ability.SetValueGetter(() => _playerStatsCalculator.PlayerRateOfFire);

    }
}