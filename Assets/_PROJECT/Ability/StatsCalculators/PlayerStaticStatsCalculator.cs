using System;
using Zenject;
using UnityEngine;


public class PlayerStaticStatsCalculator : IInitializable, IDisposable {
    private readonly GameData _gameData;
    private readonly PlayerLevel _playerLevel;
    private readonly ModifierShopManager _modifierShopManager;
    private readonly PlayerBoostBoxesSystem _playerBoostBoxesSystem;
    
    // Hp
    public int PlayerHp => _playerHpByLevel + _playerBoostBoxesSystem.AccumulatedHp;
    private int _playerHpByLevel;
    
    // Damage
    private int _playerDamageByLevel;
    public float PlayerDamage => _playerDamageByLevel + _playerBoostBoxesSystem.AccumulatedDamage;
    
    public float PlayerRateOfFire { get; private set; }
    public int PlayerCapacity { get; private set; }
    
    
    public PlayerStaticStatsCalculator(
        GameData gameData, 
        PlayerLevel playerLevel, 
        ModifierShopManager modifierShopManager,
        PlayerBoostBoxesSystem playerBoostBoxesSystem
    ) {
        _gameData = gameData;
        _playerLevel = playerLevel;
        _modifierShopManager = modifierShopManager;
        _playerBoostBoxesSystem = playerBoostBoxesSystem;
        RecalculateLevelStats(_playerLevel.CurrentLevel);
    }
    
    
    public void Initialize() {
        Subscribe();
        RecalculateLevelStats(_playerLevel.CurrentLevel);
        RecalculatePlayerModifierStats();
    }

    
    
    private void Subscribe() {
        _playerLevel.LevelUp += RecalculateLevelStats;
        _modifierShopManager.ModifierUpdated += RecalculatePlayerModifierStats;
    }


    public void Dispose() {
        _playerLevel.LevelUp -= RecalculateLevelStats;
        _modifierShopManager.ModifierUpdated -= RecalculatePlayerModifierStats;
    }
    
    
    private void RecalculateLevelStats(int level) {
        CalculatePlayerHp(level);
    }

    
    private void RecalculatePlayerModifierStats() {
        // Damage
        int damageLevel = _modifierShopManager.GetModifierLevelWithType(ModifierType.Damage);
        CalculatePlayerDamage(damageLevel);
        
        // atackCapacity
        int capacity = _modifierShopManager.GetModifierLevelWithType(ModifierType.Capacity);
        CalculatePlayerCapacity(capacity);
        
        // Rate of fire
        int rateOfFire = _modifierShopManager.GetModifierLevelWithType(ModifierType.RateOfFire);
        CalculatePlayerRateOfFire(rateOfFire);
    }
    
    
    
    private void CalculatePlayerHp(int level) {
        _playerHpByLevel = (int)
            (_gameData.PlayerHpBase + _gameData.PlayerLevelAddHp * (level-1));
    }
    
    
    
    private void CalculatePlayerDamage(int level) {
        _playerDamageByLevel = (int)
            (_gameData.PlayerDamageBase + _gameData.PlayerLevelAddDamage * (level-1));
        Debug.Log($"_playerDamageByLevel: {_playerDamageByLevel}");
            
    }
    
    private void CalculatePlayerCapacity(int level) {
        PlayerCapacity = (int)
            (_gameData.PlayerCapacityBase + _gameData.PlayerLevelAddCapacity * (level-1));
        Debug.Log($"PlayerCapacity: {PlayerCapacity}");
            
    }
    
    
    private void CalculatePlayerRateOfFire(int level) {
        // Значение скорости уменьшается т.к я еблан
        // и обозвал это скорость стрельбы но по сути это просто задержка между стрельбой
        PlayerRateOfFire =
            (_gameData.PlayerRateOfFireBase - _gameData.PlayerLevelAddRateOfFire * (level-1));
        Debug.Log($"PlayerRateOfFire: {PlayerRateOfFire}");
            
    }
}