using Zenject;


public class PlayerAbilityController : AbilityEnablerBase {

    [Inject] PlayerStaticStatsCalculator _playerStaticStatsCalculator;
    [Inject] IPlayer _player;
    
    
    private void OnDisable() {
        _battleManager.MainPlayerWin -= OnMainPlayerWin;
        _battleManager.GameReadyToPlay -= OnGameReadyToPlay;
    }
    
    private void OnEnable() {
        _battleManager.MainPlayerWin += OnMainPlayerWin;
        _battleManager.GameReadyToPlay += OnGameReadyToPlay;
    }

    protected override void InitStartParams() {
        InitPlayerDamage();
        InitPlayerCapacity();
        InitPlayerRateOfFire();
    }
    
    private void OnGameReadyToPlay() {
        StartAbility();
    }

    private void OnMainPlayerWin(bool _) {
        StopAbility();
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