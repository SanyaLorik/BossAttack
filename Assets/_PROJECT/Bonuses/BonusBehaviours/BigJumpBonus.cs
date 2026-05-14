using System;

[Serializable]
public class BigJumpBonus : IBonus {
    public void Use(IBonusUser player) {
        player.SetBigJump(true);
    }

    public void StopWork(IBonusUser player) {
        player.SetBigJump(false);
    }
}