public static class BossStatsCalculator {
    
    public static BossRuntimeStats GetStatsByLevel(BossConfig config, int playerLevel) {
        int level = playerLevel - 1;

        return new BossRuntimeStats {
            Damage = config.BaseDamage + config.LevelAddDamage * level,
            Hp = config.BaseHp + config.LevelAddHp * level,
            MoveSpeed = config.MoveSpeed,
            RateOfFire = config.RateOfFire,
            StopingDistance = config.StopingDistance,
            DistanceToAtack = config.DistanceToAtack,
        };
    }
}