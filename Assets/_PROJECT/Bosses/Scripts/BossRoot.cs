using UnityEngine;
using Zenject;

public class BossRoot : MonoBehaviour {
    [SerializeField] private BossAbilityController _bossAbilityController;
    [field: SerializeField] public BotManager BotManager { get; private set; }
    [SerializeField] private BossHuntingBehaviour _hunting;
    [SerializeField] private BossConfig _config;
    
    
    [Inject] PlayerLevel _playerLevel;
    [Inject] BattleManager _battleManager;
    
    

    private AbilitySystem Ability => _bossAbilityController.Ability;
    private IPlayer Player => BotManager;


    public void InitStats() {
        Debug.Log("Init stats boss");
        if (Ability.Effect is not IValueGetter abilityEffectValue) return;
        
        BossRuntimeStats stats = BossStatsCalculator.GetStatsByLevel(_config, _playerLevel.CurrentLevel);
        
        abilityEffectValue.SetValueGetter(() => stats.Damage);
        Ability.SetValueGetter(() => stats.RateOfFire);
        
        Player.Damagable.SetMaxHpGetter(() => stats.Hp);
        Player.Damagable.Respawn(false);
        BotManager.BotWalkManager.SetSpeed(stats.MoveSpeed);
        BotManager.BotWalkManager.SetStoppingDistance(stats.StopingDistance);
    }


    public void DisposeLogic() {
        _hunting.DisposeLogic();
        _bossAbilityController.Unsubsribe();
        BotManager.BotWalkManager.ResetLogic();
    }
    

}