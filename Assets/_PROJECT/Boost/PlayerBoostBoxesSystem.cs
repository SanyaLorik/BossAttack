using UnityEngine;
using Zenject;

public class PlayerBoostBoxesSystem : MonoBehaviour {
    public int AccumulatedDamage { get; private set; }
    public int AccumulatedHp { get; private set; }
    
    
    
    [Inject] BattleManager _battleManager;
    [Inject] MainGameStarter _mainGameStarter;
    [Inject] IPlayer _mainPlayer;
    [Inject] GameData _gameData;

    private IDamagable _playerDamagable => _mainPlayer.Damagable;
    
    
    
    private void OnDisable() {
        _mainGameStarter.GameStarted += OnGameStarted;
    }
    
    private void OnEnable() {
        _mainGameStarter.GameStarted += OnGameStarted;
    }

    public void PlusOne() {
        AccumulatedDamage += _gameData.BoostDamageAdd;
        AccumulatedHp += _gameData.BoostHpAdd;
        _playerDamagable.ApplyHeal(_gameData.BoostHpAdd);
    }

    private void OnGameStarted(bool obj) {
        AccumulatedDamage = 0;
        AccumulatedHp = 0;
    }
}