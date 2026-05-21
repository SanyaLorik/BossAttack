using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class BotRespawnTimer : MonoBehaviour {
    [SerializeField] BotManager _bot;
    [SerializeField] private AbilityCotrollerBase _abilityCotroller;
    

    private CancellationTokenSource _respawnTokenSource;
    
    [Inject] private PlayersDiesObserver _playersDiesObserver;
    [Inject] private GameData _gameData;
    
    
    public void OnDestroy() {
        _playersDiesObserver.PlayerSpawned -= OnPlayersSpawned;   
        _playersDiesObserver.PlayerDied -= OnPlayersDied;   
    }
    
    
    public void OnEnable() {
        _playersDiesObserver.PlayerSpawned += OnPlayersSpawned;   
        _playersDiesObserver.PlayerDied += OnPlayersDied;  
    }
    
    private void OnPlayersSpawned(IPlayer player) {
        if (player != _bot) return;
        _abilityCotroller.StartAbility();
        _bot.SetMovingStatus(true);
        _bot.SetVisualModelState(true);
    }

 
    private void OnPlayersDied(IPlayer player) {
        if (player != _bot) return;
        _abilityCotroller.StopAbility();
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