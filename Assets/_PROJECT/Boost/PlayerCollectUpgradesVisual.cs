using UnityEngine;
using Zenject;

public class PlayerCollectUpgradesVisual : MonoBehaviour {
    [SerializeField] private ParticleSystem _onBoostedPs;
    
    [Inject] PlayerBoostBoxesSystem _playerBoostBoxesSystem;
    [Inject] PlayerBonusService _playerBonusService;

    private void OnDisable() {
        _playerBoostBoxesSystem.PlayerBoosted -= OnPlayerBoosted;
        _playerBonusService.BonusActivated -= OnBonusActivated; 
    }
    
    
    private void OnEnable() {
        _playerBoostBoxesSystem.PlayerBoosted += OnPlayerBoosted;
        _playerBonusService.BonusActivated += OnBonusActivated; 
    }

    
    private void OnBonusActivated(ActiveBonus bonus) {
        _onBoostedPs.Play(true);
        GameEvents.BonusUseInvoke(bonus.Bonus);
    }


    private void OnPlayerBoosted() {
        _onBoostedPs.Play(true);
        GameEvents.BonusUseInvoke(null);
    }
}