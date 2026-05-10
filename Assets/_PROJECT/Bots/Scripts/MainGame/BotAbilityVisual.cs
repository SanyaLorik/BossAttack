using SanyaBeerExtension;
using UnityEngine;

public class BotAbilityVisual : MonoBehaviour {
    [SerializeField] private BossAbilityController _abilityController;
    [SerializeField] private GameObject[] _shootingVisual;
    
    
    
    private void OnDisable() {
        _abilityController.NewAbilitySystemEnabled -= OnNewAbilitySystemEnabled;
    }
    
    
    private void OnEnable() {
        _abilityController.NewAbilitySystemEnabled += OnNewAbilitySystemEnabled;
    }
    
    private void OnNewAbilitySystemEnabled(AbilitySystem abilitySystem) {
        switch (abilitySystem.Type) {
            
            case AbilityType.Shooting:
                _shootingVisual.ActiveSelf();
                break;
            
            case AbilityType.Melee:
                _shootingVisual.DisactiveSelf();
                break;
            
        }
    }
    
}