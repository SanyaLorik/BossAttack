using System;
using UnityEngine;
using Zenject;

public class BotBonusController : MonoBehaviour, IBonusUser {
    [SerializeField] private BotJumpController botJumpController;

    public event Action<BonusStatus, bool> BonusStatusChanged;
    public event Action<bool> InvinsibleStatusChanged;
    public bool IsInvincibleAfterBonus { get; private set; }

    
    [Inject] GameData _gameData;
    [Inject] BotManager _manager;
    
    private void Start() {
        SetDefault();
    }
    

    public void SetDefault() {
        SetDefaultSpeed();
        SetInvincible(false);
        SetBigJump(false);
    }
    
    
    public void SetDefaultSpeed() {
        _manager.BotWalkManager.SetSpeed(_gameData.BotSpeed);
        BonusStatusChanged?.Invoke(BonusStatus.SuperSpeed, false);
        // Debug.Log($"SetDefaultSpeed {BotMonolog.NickName}");
    }

    public void ReloadClip() {
        throw new NotImplementedException();
    }


    public void SetBonusSpeed() {
        _manager.BotWalkManager.SetSpeed(_gameData.VelocityBonusSpeed);
        BonusStatusChanged?.Invoke(BonusStatus.SuperSpeed, true);
    }

    public void SetBigJump(bool bigJump) {
        botJumpController.SetBigJump(bigJump);
        BonusStatusChanged?.Invoke(BonusStatus.SuperJump, bigJump);
    }

    public void SetInvincible(bool invincible) {
        IsInvincibleAfterBonus = invincible;
        BonusStatusChanged?.Invoke(BonusStatus.Invincible, invincible);
        InvinsibleStatusChanged?.Invoke(invincible);
    }

}