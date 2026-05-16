public enum BonusType {
    Speed,
    Jump,
    Invincible,
    Reload,
}

public interface IBonus {
    public BonusType Type { get; }
    public void Use(IBonusUser player);
    public void StopWork(IBonusUser player);
}