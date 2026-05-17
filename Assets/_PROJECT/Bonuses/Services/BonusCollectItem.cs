using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class BonusCollectItem : MonoBehaviour {
    [SerializeReference, SubclassSelector] private IBonus _bonus;
    
    public IBonus Bonus => _bonus;
    
    
    [Inject] PlayerMovement _mainPlayer;
    [Inject] GameData _gameData;
    [Inject] PlayerBonusService _bonusService;
    

    private void OnTriggerEnter(Collider collider) {
        if (!collider.TryGetComponent(out IPlayer player)) return;
        if (player == _mainPlayer) {
            UseBonus();
        }
    }

    
    private void UseBonus() {
        GameEvents.BonusUseInvoke(Bonus);
        _bonusService.TryAddBonus(this);
    }
    
    
}
