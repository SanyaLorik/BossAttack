using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;


public class BonusCollectItem : MonoBehaviour {
    [SerializeReference, SubclassSelector] private IBonus _bonus;

    private IBonus Bonus => _bonus;
    
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
        _bonusService.AddNewBonus(Bonus);
        // Destroy(gameObject)
    }
    
    
}
