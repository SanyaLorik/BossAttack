using System;
using UnityEngine;
using Zenject;

public class PlayerBonusUser : MonoBehaviour, IBonusUser {
    
    public event Action<BonusStatus, bool> BonusStatusChanged;
    public event Action<bool> InvinsibleStatusChanged;
    public bool IsInvincibleAfterBonus { get; private set; }

    [Inject] private PlayerPetsManager _petsManager;
    [Inject] private PlayerMovement _playerMovement;
    [Inject] private GameData _gameData;
    
    
    private void Start() {
        SetDefault();
    }
    

    public void SetDefault() {
        SetDefaultSpeed();
        SetInvincible(false);
        SetBigJump(false);
    }
    
    public void SetDefaultSpeed() {
        float walkSpeed  = _gameData.WalkSpeed + _petsManager.PetsRatioSum;
        _playerMovement.UpdateWalkSpeed(walkSpeed);
        BonusStatusChanged?.Invoke(BonusStatus.SuperSpeed, false);
    }

    public void ReloadClip() {
        _playerMovement.AbilityCotroller.ReloadAbility();
    }


    public void SetBonusSpeed() {
        float walkSpeed = _gameData.VelocityBonusSpeed + _petsManager.PetsRatioSum;
        _playerMovement.UpdateWalkSpeed(walkSpeed);
        BonusStatusChanged?.Invoke(BonusStatus.SuperSpeed, true);
    }
    
    
    public void SetInvincible(bool invincible) {
        IsInvincibleAfterBonus = invincible;
        BonusStatusChanged?.Invoke(BonusStatus.Invincible, invincible);
        InvinsibleStatusChanged?.Invoke(invincible);
    }

    
    public void SetBigJump(bool bigJump) {
        float firstJumpForce = bigJump ? _gameData.JumpBonusHeight : _gameData.JumpForce; 
        float secondJumpForce = bigJump ? _gameData.DoubleJumpBonusHeight : _gameData.SecondJumpForce;
        _playerMovement.UpdateFirstJumpForce(firstJumpForce);
        _playerMovement.UpdateSecondJumpForce(secondJumpForce);
        
        
        BonusStatusChanged?.Invoke(BonusStatus.SuperJump, bigJump);
    }
}