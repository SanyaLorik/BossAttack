using TMPro;
using UnityEngine;
using Zenject;

public class BossesCountView : MonoBehaviour {
    [Header("Данные по канвасу")]
    [SerializeField] private TextMeshProUGUI _countPlayersText;

    private int _currentBossCount;
    private int _allBossCount;
    
    [Inject] MainGameStarter _gameStarter;
    [Inject] BattleManager _battleManager;
    [Inject] PlayerMovement _mainPlayer;
    [Inject] BossCreateManager _bossCreateManager;
    [Inject] BossesDiesObserver _bossesDiesObserver;


    
    private void OnEnable() {
        _bossCreateManager.BossesCreated += OnBossesCreate;
        _bossesDiesObserver.BossDied += OnBossDie;
    }

    private void OnBossDie(IPlayer _) {
        _currentBossCount--;
        UpdateBossCountInfo();
    }

    
    private void OnBossesCreate(int count) {
        _allBossCount = count;
        _currentBossCount = count;
        UpdateBossCountInfo();
    }

    private void UpdateBossCountInfo() {
        _countPlayersText.text = $"{_currentBossCount}/{_allBossCount}";
    }
}