using UnityEngine;
using Zenject;

public abstract class AbilityEnablerBase: MonoBehaviour {
    [field: SerializeField] public AbilitySystem Ability { get; private set; }

    
    private IPlayer _player;

    private IPlayer Player {
        get {
            _player ??= GetComponentInParent<IPlayer>();
            return _player;
        }
    }
    
    
    [Inject] private BattleManager _battleManager;
    [Inject] private PlayerLevel _playerLevel;
    
    
    [Inject]
    public void Initialize(BattleManager battleManager, PlayerLevel playerLevel) {
        _battleManager = battleManager;
        _playerLevel = playerLevel;
        _battleManager.GameReadyToPlay += OnGameReadyToPlay;
        _battleManager.MainPlayerWin += OnMainPlayerWin;
        InitAbility();
    }
    
    
    private void OnGameReadyToPlay() {
        StartAbility();
    }

    
    private void OnMainPlayerWin(bool _) {
        StopAbility();
    }


    
    private void InitAbility() {
        Debug.Log("_currentPlayer = " + Player);
        Ability.SetSame(Player);
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