using System;using Zenject;


public class BossStatsCalculator : IInitializable, IDisposable {
    private readonly GameData _gameData;
    private readonly PlayerLevel _playerLevel;
    private readonly ModifierShopManager _shopManager;

    // Damage
    public int MeleeDamage { get; private set; }
    public int ShootDamage { get; private set; }
    // HP
    public int BossHp { get; private set; }
    // Interval
    public float BossIntervalToAtackInMelee => _gameData.BossIntervalToAtackInMelee;
    public float BossIntervalToAtackInShoot => _gameData.BossIntervalToAtackInShooting;


    public BossStatsCalculator(GameData gameData, PlayerLevel playerLevel) {
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
    }


    private void CalculateMeleeDamage() {
        MeleeDamage = (int)
            (_gameData.BossMeleeDamageBase + _gameData.BossMeleeLevelAddDamage * (_playerLevel.CurrentLevel - 1));
    }


    private void CalculateShootDamage() {
        ShootDamage = (int)
            (_gameData.BossShootDamageBase + _gameData.BossShootLevelAddDamage * (_playerLevel.CurrentLevel - 1));
    }


    private void CalculateBossHp() {
        BossHp = (int)
            (_gameData.BossHpBase + _gameData.BossLevelAddHp * (_playerLevel.CurrentLevel - 1));
    }
    
    
    
    



}