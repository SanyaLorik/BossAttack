public class ActiveBonusCreator {
    
    private readonly GameData _gameData;
    
    public ActiveBonusCreator(GameData gameData) {
        _gameData = gameData;
    }
    
    
    public ActiveBonus InitNewBonus(IBonus bonus) {
        switch (bonus.Type) {
            case BonusType.Speed:
                return new (bonus, _gameData.SpeedBonusDuration);
            case BonusType.Jump:
                return new (bonus, _gameData.JumpBonusDuration);
            case BonusType.Invincible:
                return new (bonus, _gameData.InvincibleBonusDuration);
            case BonusType.Reload:
                return new (bonus, _gameData.ReloadBonusDuration);
            default:
                return null;
        }
    }
}