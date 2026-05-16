using System;
using System.Collections.Generic;
using Zenject;

public class PlayerBonusService : ITickable  {
    private Dictionary<BonusType, ActiveBonus> _bonuses = new();

    private readonly IPlayer _mainPlayer;
    private readonly GameData _gameData;
    private readonly ActiveBonusCreator _bonusCreator;
    
    public PlayerBonusService(IPlayer mainPlayer, GameData gameData, ActiveBonusCreator bonusCreator) {
        _mainPlayer = mainPlayer;
        _gameData = gameData;
        _bonusCreator = bonusCreator;
    }
    
    
    public event Action<ActiveBonus> BonusActive;
    public event Action<ActiveBonus> BonusDisactive;
    
    
    public void TryAddBonus(IBonus bonus) {
        // Перезарядка
        if (_bonuses.ContainsKey(bonus.Type)) {
            _bonuses[bonus.Type].Reload();
            return;
        }
        
        // Бонуса нет, добавим
        bonus.Use(_mainPlayer.BonusUser);
        ActiveBonus newActiveBonus = _bonusCreator.InitNewBonus(bonus);
        _bonuses[bonus.Type] = newActiveBonus;
        BonusActive?.Invoke(newActiveBonus);
    }
    

    public void Tick() {
        if (_bonuses.Count == 0) return;
        List<BonusType> onDelete = new ();
        
        foreach (ActiveBonus runtimeBonus in _bonuses.Values) {
            if (runtimeBonus.Progress == 1) {
                
                BonusDisactive?.Invoke(runtimeBonus);
                runtimeBonus.Bonus.StopWork(_mainPlayer.BonusUser);
                onDelete.Add(runtimeBonus.Bonus.Type);
                
            }
        }

        foreach (var bonus in onDelete) {
            _bonuses.Remove(bonus);
        }
    }
}