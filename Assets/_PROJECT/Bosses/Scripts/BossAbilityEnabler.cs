using UnityEngine;
using Zenject;

public class BossAbilityEnabler : AbilityEnablerBase {
    [SerializeField] private BossConfig _config;
    [Inject] BattleManager _battleManager;
    

    private void OnDisable() {
        _battleManager.MainPlayerWin -= OnMainPlayerWin;
        _battleManager.GameReadyToPlay -= OnGameReadyToPlay;
    }
    
    
    private void OnEnable() {
        // Покачто сразу прям, потом врубать после отсчета
        StartAbility();
        
        _battleManager.MainPlayerWin += OnMainPlayerWin;
        _battleManager.GameReadyToPlay += OnGameReadyToPlay;
    }

    
    private void OnGameReadyToPlay() {
        StartAbility();
    }

    
    private void OnMainPlayerWin(bool _) {
        StopAbility();
    }
    
    protected override void InitStartParams() { }

}