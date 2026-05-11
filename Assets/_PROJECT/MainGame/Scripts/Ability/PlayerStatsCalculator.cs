using System;
using Zenject;
using UnityEngine;


public class PlayerStatsCalculator : IInitializable, IDisposable {
    private readonly GameData _gameData;
    private readonly PlayerLevel _playerLevel;
    private readonly ModifierShopManager _modifierShopManager;
    
    public int PlayerHp { get; private set; }
    public int PlayerDamage { get; private set; }
    public int PlayerRateOfFire { get; private set; }
    public int PlayerCapacity { get; private set; }
    public int BuildHp { get; private set; }
    
    
    public PlayerStatsCalculator(
        GameData gameData, 
        PlayerLevel playerLevel, 
        ModifierShopManager modifierShopManager
    ) {
        _gameData = gameData;
        _playerLevel = playerLevel;
        _modifierShopManager = modifierShopManager;
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
        CalculateBuildHp(level);
    }

    
    private void RecalculatePlayerModifierStats() {
        // Damage
        int damageLevel = _modifierShopManager.GetModifierLevelWithType(ModifierType.Damage);
        CalculatePlayerDamage(damageLevel);
        
        // Capacity
        int capacity = _modifierShopManager.GetModifierLevelWithType(ModifierType.Capacity);
        CalculatePlayerCapacity(capacity);
        
        // Rate of fire
        int rateOfFire = _modifierShopManager.GetModifierLevelWithType(ModifierType.RateOfFire);
        CalculatePlayerRateOfFire(rateOfFire);
    }
    
    
    
    private void CalculatePlayerHp(int level) {
        PlayerHp = (int)
            (_gameData.PlayerHpBase + _gameData.PlayerHpMultiplier * (level-1));
    }
    
    
    private void CalculateBuildHp(int level) {
        BuildHp = (int)
            (_gameData.BuildHpBase + _gameData.BuildHpMultiplier * (level-1));
    }

    
    private void CalculatePlayerDamage(int level) {
        PlayerDamage = (int)
            (_gameData.PlayerDamageBase + _gameData.PlayerLevelAddDamage * (level-1));
        Debug.Log($"PlayerDamage: {PlayerDamage}");
            
    }
    
    private void CalculatePlayerCapacity(int level) {
        PlayerCapacity = (int)
            (_gameData.PlayerCapacityBase + _gameData.PlayerLevelAddCapacity * (level-1));
        Debug.Log($"PlayerCapacity: {PlayerCapacity}");
            
    }
    
    
    private void CalculatePlayerRateOfFire(int level) {
        // Значение скорости уменьшается т.к я еблан
        // и обозвал это скорость стрельбы но по сути это просто задержка между стрельбой
        PlayerRateOfFire = (int)
            (_gameData.PlayerRateOfFireBase - _gameData.PlayerLevelAddRateOfFire * (level-1));
        Debug.Log($"PlayerRateOfFire: {PlayerRateOfFire}");
            
    }
}