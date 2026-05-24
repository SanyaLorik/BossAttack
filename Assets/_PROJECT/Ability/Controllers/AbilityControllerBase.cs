using UnityEngine;
using Zenject;

public abstract class AbilityControllerBase: MonoBehaviour {
    [field: SerializeField] public AbilitySystem Ability { get; private set; }

    
    private IPlayer _player;

    private IPlayer Player {
        get {
            _player ??= GetComponentInParent<IPlayer>();
            return _player;
        }
    }
    
    
    [Inject] private BattleManager _battleManager;
    [Inject] private MainGameStarter _mainGameStarter;
    [Inject] private PlayerLevel _playerLevel;
    
    
    [Inject]
    public void Initialize(MainGameStarter mainGameStarter, BattleManager battleManager, PlayerLevel playerLevel) {
        _battleManager = battleManager;
        _playerLevel = playerLevel;
        _mainGameStarter = mainGameStarter;
        _battleManager.GameReadyToPlay += OnGameReadyToPlay;
        _battleManager.MainPlayerWin += OnMainPlayerWin;
        InitAbility();
        CheckNeedToUseAbility();
    }

    public void Unsubsribe() {
        _battleManager.GameReadyToPlay -= OnGameReadyToPlay;
        _battleManager.MainPlayerWin -= OnMainPlayerWin;
    }

    
    private void CheckNeedToUseAbility() {
        if (_mainGameStarter.GameIsStarted) {
            OnGameReadyToPlay();
        }
    }


    private void OnGameReadyToPlay() {
        StartAbility();
    }

    
    private void OnMainPlayerWin(bool _) {
        StopAbility();
    }


    
    private void InitAbility() {
        Ability.SetSame(Player);
    }
    
    public void ReloadAbility() {
        Ability.ReloadClip();
    }

    public void StopAbility() {
        Ability.Stop();
    }

    public void StartAbility() {
        if(_battleManager.GameIsOver) return;
        Ability.StartSystem();
    }

}