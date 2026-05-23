using SanyaBeerExtension;
using TMPro;
using UnityEngine;
using Zenject;

public class BattleCanvasVisualiser : MonoBehaviour {
    [Header("Данные по канвасу")]
    [SerializeField] private TextMeshProUGUI _countPlayersText;

    
    [Inject] MainGameStarter _gameStarter;
    [Inject] BattleManager _battleManager;
    [Inject] PlayerMovement _mainPlayer;


    
    private void OnEnable() {
        _gameStarter.GameStarted += OnGameStarted;
        _battleManager.PlayersCountChanged += OnChangePlayersCount;
    }

    private void OnChangePlayersCount(int count) {
        _countPlayersText.text = $"{count}/{_battleManager.CountBotsToBattle}";
    }

    
    
    private void OnGameStarted(bool started) {
        if (started) {
            Debug.Log("OnGameStarted " + started);
        }
    }


}