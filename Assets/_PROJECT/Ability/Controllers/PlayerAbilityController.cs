using Zenject;


public class PlayerAbilityController : AbilityCotrollerBase {

    [Inject] PlayerStatsCalculator _playerStatsCalculator;
    [Inject] IPlayer _player;
    
    public AbilitySystem AbilitySystem => Abilitys[0];

    
    protected override void InitStartGetters() {
        InitPlayerDamage();
        InitPlayerCapacity();
        InitPlayerRateOfFire();
    }

    private void InitPlayerDamage() {
        var abilityValue = AbilitySystem.Effect as IValueGetter;
        if (abilityValue != null) abilityValue.SetValueGetter(() => _playerStatsCalculator.PlayerDamage);
    }
    
    private void InitPlayerCapacity() {
        var abilityCapacity = AbilitySystem.AtackCapacity as IValueGetter;
        if (abilityCapacity != null) abilityCapacity.SetValueGetter(() => _playerStatsCalculator.PlayerCapacity);
        
    }
    
    private void InitPlayerRateOfFire() {
        AbilitySystem.SetValueGetter(() => _playerStatsCalculator.PlayerRateOfFire);
    }


}