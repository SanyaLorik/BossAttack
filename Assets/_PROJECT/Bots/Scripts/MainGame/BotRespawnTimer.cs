using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BotRespawnTimer : MonoBehaviour {
    [SerializeField] BotManager _bot;
    [SerializeField] private AbilityControllerBase _abilityController;
    

    private CancellationTokenSource _respawnTokenSource;
    
    [Inject] private PlayersDiesObserver _playersDiesObserver;
    [Inject] private GameData _gameData;
    [Inject] private BattleManager _battleManager;
    
    
    public void OnDisable() {
        _playersDiesObserver.PlayerSpawned -= OnPlayersSpawned;   
        _playersDiesObserver.PlayerDied -= OnPlayersDied;   
        _battleManager.MainPlayerWin -= OnBattleEnd;
    }
    
    
    public void OnEnable() {
        _playersDiesObserver.PlayerSpawned += OnPlayersSpawned;   
        _playersDiesObserver.PlayerDied += OnPlayersDied;  
        _battleManager.MainPlayerWin += OnBattleEnd;
    }

    private void OnBattleEnd(bool _) {
        UniTaskHelper.DisposeTask(ref _respawnTokenSource);
        _bot.Damagable.Respawn(false);
    }

    
    private void OnPlayersSpawned(IPlayer player) {
        if (player != _bot) return;
        _abilityController.StartAbility();
        _bot.SetMovingStatus(true);
        _bot.SetVisualModelState(true);
    }

 
    private void OnPlayersDied(IPlayer player) {
        if (player != _bot) return;
        _abilityController.StopAbility();
        _bot.SetMovingStatus(false);
        _bot.SetVisualModelState(false);
        StartRespawnTimer();
    }
        
    
    private void StartRespawnTimer() {
        UniTaskHelper.DisposeTask(ref _respawnTokenSource);
        _respawnTokenSource = new CancellationTokenSource();
        StartTimerToRespawnPlayer(_respawnTokenSource.Token).Forget();
    }
    
    private async UniTask StartTimerToRespawnPlayer(CancellationToken token) {
        int duration = _gameData.TimeToRespawnPlayer;
        float elapsedTime = 0f;
        while (elapsedTime < duration && !token.IsCancellationRequested) {
            // float progress =  elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();
        }
        RespawnPlayer();
    }
    
    
    private void RespawnPlayer() {
        _bot.Damagable.Respawn(false);
    }
   
    
}