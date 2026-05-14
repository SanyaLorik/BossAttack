using System;

public enum BonusStatus {
    Default,
    SuperSpeed,
    SuperJump,
    Invincible,
}


public interface IBonusUser {
    public void SetDefaultSpeed();
    public void ReloadClip();
    public void SetBonusSpeed();
    public void SetBigJump(bool state);
    public void SetInvincible(bool invincible);
    public void SetDefault();
    public event Action<BonusStatus, bool> BonusStatusChanged;
    public event Action<bool> InvinsibleStatusChanged;
    public bool IsInvincibleAfterBonus { get; }

}