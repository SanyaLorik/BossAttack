using UnityEngine;
using Zenject;

public class PlayerBoostBoxesSystem : MonoBehaviour {
    public int AccumulatedDamage { get; private set; }
    public int AccumulatedHp { get; private set; }
    
    [Inject] BattleManager _battleManager;
    [Inject] MainGameStarter _mainGameStarter;

    private void OnDisable() {
        _mainGameStarter.GameStarted += OnGameStarted;
    }
    
    private void OnEnable() {
        _mainGameStarter.GameStarted += OnGameStarted;
    }

    public void PlusOne() {
        AccumulatedDamage++;
        AccumulatedHp++;
    }

    private void OnGameStarted(bool obj) {
        AccumulatedDamage = 0;
        AccumulatedHp = 0;
    }
}