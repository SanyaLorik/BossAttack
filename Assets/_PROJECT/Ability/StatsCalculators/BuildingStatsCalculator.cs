using System;
using UnityEngine;
using Zenject;

public class BuildingStatsCalculator : IInitializable, IDisposable {
    private readonly GameData _gameData;
    private readonly PlayerLevel _playerLevel;
    
    // HP
    public int TurretHp { get; private set; }
    public int HealBuildingHp { get; private set; }
    // Damage / Heal
    public int HealValue { get; private set; }
    public int TurretValue { get; private set; }
    public int MineValue { get; private set; }
    
    // Atack Interval 
    public float MineIntervalAtack { get; private set; }
    public float TurretIntervalAtack { get; private set; }
    public float HealIntervalAtack { get; private set; }
    
    
    public BuildingStatsCalculator(
        GameData gameData, 
        PlayerLevel playerLevel
    ) {
        _gameData = gameData;
        _playerLevel = playerLevel;
    }
    
    
    public void Initialize() {
        Subscribe();
        RecalculateStatsDependByPlayer(_playerLevel.CurrentLevel);
        CalculateIntervalToAtack();
    }

    
    
    private void Subscribe() {
        _playerLevel.LevelUp += RecalculateStatsDependByPlayer;
    }


    public void Dispose() {
        _playerLevel.LevelUp -= RecalculateStatsDependByPlayer;
    }
    
    
    private void RecalculateStatsDependByPlayer(int level) {
        CalculateTurretHp(level);
        CalculateHealStationHp(level);
        
        CalculateHealValue(level);
        CalculateTurretValue(level);
        CalculateMineValue(level);
    }
    
    
    private void CalculateTurretHp(int level) {
        TurretHp = (int)
            (_gameData.TurretHpBase + _gameData.BuildingAddLevelHp * (level-1));
        Debug.Log("CalculateTurretHp: " + TurretHp);
    }
    
    private void CalculateHealStationHp(int level) {
        HealBuildingHp = (int)
            (_gameData.HealBuildingHpBase + _gameData.BuildingAddLevelHp * (level-1));
        Debug.Log("CalculateHealStationHp: " + HealBuildingHp);
    }
    
    private void CalculateHealValue(int level) {
        HealValue = (int)
            (_gameData.HealBuildingValueBase + _gameData.HealBuildingAddLevelValue * (level-1));
        Debug.Log("CalculateHealValue: " + HealValue);
    }
    
    private void CalculateTurretValue(int level) {
        TurretValue = (int)
            (_gameData.TurretValueBase + _gameData.TurretAddLevelValue * (level-1));
        Debug.Log("CalculateTurretValue: " + TurretHp);
    }
    
    private void CalculateMineValue(int level) {
        MineValue = (int)
            (_gameData.MineBuildingValueBase + _gameData.MineBuildingAddLevelValue * (level-1));
        Debug.Log("CalculateMineValue: " + MineValue);
    }
    
    private void CalculateIntervalToAtack() {
        MineIntervalAtack = _gameData.MineIntervalAtack;
        HealIntervalAtack = _gameData.HealIntervalAtack;
        TurretIntervalAtack = _gameData.TurretIntervalAtack;
    }
    
}