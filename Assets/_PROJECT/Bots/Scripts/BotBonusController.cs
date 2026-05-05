using System;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class BotBonusController : MonoBehaviour, IBonusUser {
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private BotJumpController botJumpController;



    public event Action<BonusStatus, bool> BonusStatusChanged;
    public event Action<bool> InvinsibleStatusChanged;
    public bool IsInvincibleAfterBonus { get; private set; }

    
    [Inject] GameData _gameData;
    
    private void Start() {
        SetDefault();
    }
    

    public void SetDefault() {
        SetDefaultSpeed();
        SetInvincible(false);
        SetBigJump(false);
    }
    
    
    public void SetDefaultSpeed() {
        _agent.speed = _gameData.BotSpeed;
        BonusStatusChanged?.Invoke(BonusStatus.SuperSpeed, false);
        // Debug.Log($"SetDefaultSpeed {_botMonolog.NickName}");
    }
    
    
    public void SetHunterSpeed() {
        _agent.speed = _gameData.HunterSpeed;
        BonusStatusChanged?.Invoke(BonusStatus.SuperSpeed, true);
    }

    
    public void SetBonusSpeed() {
        _agent.speed = _gameData.VelocityBonusSpeed;
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