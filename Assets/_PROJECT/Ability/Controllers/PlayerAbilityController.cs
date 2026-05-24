using Zenject;


public class PlayerAbilityController : AbilityControllerBase {

    private PlayerStaticStatsCalculator _playerStaticStatsCalculator;


    [Inject]
    public void Initialize(PlayerStaticStatsCalculator playerStaticStatsCalculator) {
        _playerStaticStatsCalculator = playerStaticStatsCalculator;
        InitStartParams();
    }
   

    private void InitStartParams() {
        InitPlayerDamage();
        InitPlayerCapacity();
        InitPlayerRateOfFire();
    }
    

    private void InitPlayerDamage() {
        var abilityValue = Ability.Effect as IValueGetter;
        if (abilityValue != null) abilityValue.SetValueGetter(() => _playerStaticStatsCalculator.PlayerDamage);
    }
    
    private void InitPlayerCapacity() {
        var abilityCapacity = Ability.AtackCapacity as IValueGetter;
        if (abilityCapacity != null) abilityCapacity.SetValueGetter(() => _playerStaticStatsCalculator.PlayerCapacity);
        
    }
    
    private void InitPlayerRateOfFire() {
        Ability.SetValueGetter(() => _playerStaticStatsCalculator.PlayerRateOfFire);
    }


}