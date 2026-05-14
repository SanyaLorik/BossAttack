using System;

[Serializable]
public class ReloadBonus : IBonus {
    public void Use(IBonusUser player) {
        player.ReloadClip();
    }

    public void StopWork(IBonusUser player) {}
}