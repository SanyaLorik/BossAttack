using System;using Zenject;


public class StatsCalculator : IInitializable, IDisposable {
    private readonly GameData _gameData;
    private readonly PlayerLevel _playerLevel;

    // Damage
    public int MeleeDamage { get; private set; }
    public int ShootDamage { get; private set; }
    // HP
    public int BossHp { get; private set; }
    public int PlayerHp { get; private set; }
    public int BuildHp { get; private set; }


    public StatsCalculator(GameData gameData, PlayerLevel playerLevel) {
        _gameData = gameData;
        _playerLevel = playerLevel;
        RecalculateLevelStats();
    }
    
    
    public void Initialize() {
        _playerLevel.LevelUp += OnLevelUp;
        RecalculateLevelStats();
    }
    
    public void Dispose() {
        _playerLevel.LevelUp -= OnLevelUp;
    }

    private void OnLevelUp(int level) {
        RecalculateLevelStats();
    }

    private void RecalculateLevelStats() {
        CalculateMeleeDamage();
        CalculateShootDamage();
        CalculateBossHp();
        CalculatePlayerHp();
        CalculateBuildHp();
    }


    private void CalculateMeleeDamage() {
        MeleeDamage = (int)(_gameData.BossMeleeDamageBase 
                          * 
                          Math.Pow(_gameData.BossMeleeDamageMultiplier, _playerLevel.CurrentLevel));
    }


    private void CalculateShootDamage() {
        ShootDamage = (int)(_gameData.BossShootDamageBase 
                          * 
                          Math.Pow(_gameData.BossShootDamageMultiplier, _playerLevel.CurrentLevel));
    }


    private void CalculateBossHp() {
        BossHp = (int)(_gameData.BossHpBase
                     * 
                     Math.Pow(_gameData.BossHpMultiplier, _playerLevel.CurrentLevel));
    }
    
    private void CalculatePlayerHp() {
        PlayerHp = (int)(_gameData.PlayerHpBase
                       * 
                       Math.Pow(_gameData.PlayerHpMultiplier, _playerLevel.CurrentLevel));
    }
    
    private void CalculateBuildHp() {
        BuildHp = (int)(_gameData.BuildHpBase
                         *
                         Math.Pow(_gameData.BuildHpMultiplier, _playerLevel.CurrentLevel));
    }



}