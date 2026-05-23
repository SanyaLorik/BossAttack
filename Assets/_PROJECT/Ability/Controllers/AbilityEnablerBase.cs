using UnityEngine;
using Zenject;

public abstract class AbilityEnablerBase: MonoBehaviour {
    [field: SerializeField] public AbilitySystem Ability { get; private set; }

    
    [Inject] IPlayer _currentPlayer;
    [Inject] protected BattleManager _battleManager;
    [Inject] protected PlayerLevel _playerLevel;
    

    private void Awake() {
        InitAbility();
        InitStartParams();
    }

    protected abstract void InitStartParams();
    
    private void InitAbility() {
        Ability.SetSame(_currentPlayer);
    }
    
    public void ReloadAbility() {
        Ability.ReloadClip();
    }

    public void StopAbility() {
        Ability.Stop();
    }

    public void StartAbility() {
        Ability.StartSystem();
    }

}