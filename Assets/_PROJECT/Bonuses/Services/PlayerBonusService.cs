using System;
using System.Collections.Generic;
using Zenject;

public class PlayerBonusService : ITickable  {
    private Dictionary<BonusType, ActiveBonus> _bonuses = new();

    private readonly IPlayer _mainPlayer;
    private readonly ActiveBonusCreator _bonusCreator;
    private readonly BonusSpawner _bonusSpawner;
    
    
    public PlayerBonusService(
        IPlayer mainPlayer, 
        ActiveBonusCreator bonusCreator, 
        BonusSpawner bonusSpawner) 
    {
        _mainPlayer = mainPlayer;
        _bonusCreator = bonusCreator;
        _bonusSpawner = bonusSpawner;
    }
    
    
    public event Action<ActiveBonus> BonusActivated;
    public event Action<ActiveBonus> BonusDisactivated;
    
    
    public void TryAddBonus(BonusCollectItem bonusItem) {
        // Перезарядка
        _bonusSpawner.BonusDestroy(bonusItem);
        if (_bonuses.ContainsKey(bonusItem.Bonus.Type)) {
            _bonuses[bonusItem.Bonus.Type].Reload();
            return;
        }
        
        // Бонуса нет, добавим
        bonusItem.Bonus.Use(_mainPlayer.BonusUser);
        ActiveBonus newActiveBonus = _bonusCreator.InitNewBonus(bonusItem.Bonus);
        _bonuses[bonusItem.Bonus.Type] = newActiveBonus;
        BonusActivated?.Invoke(newActiveBonus);
    }
    

    public void Tick() {
        if (_bonuses.Count == 0) return;
        List<BonusType> onDelete = new ();
        
        foreach (ActiveBonus runtimeBonus in _bonuses.Values) {
            if (runtimeBonus.Progress == 1) {
                
                BonusDisactivated?.Invoke(runtimeBonus);
                runtimeBonus.Bonus.StopWork(_mainPlayer.BonusUser);
                onDelete.Add(runtimeBonus.Bonus.Type);
                
            }
        }

        foreach (var bonus in onDelete) {
            _bonuses.Remove(bonus);
        }
    }
}