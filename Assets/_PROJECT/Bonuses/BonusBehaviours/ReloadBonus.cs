using System;
using UnityEngine;

[Serializable]
public class ReloadBonus : IBonus {
    public BonusType Type => BonusType.Reload;

    
    public void Use(IBonusUser player) {
        player.ReloadClip();
    }

    public void StopWork(IBonusUser player) {}
}