using System;
using UnityEngine;

[Serializable]
public class BigJumpBonus : IBonus {
    public BonusType Type => BonusType.Jump;
    
    public void Use(IBonusUser player) {
        player.SetBigJump(true);
    }

    public void StopWork(IBonusUser player) {
        player.SetBigJump(false);
    }
}