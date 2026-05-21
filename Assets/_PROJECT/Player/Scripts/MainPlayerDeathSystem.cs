using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class MainPlayerDeathSystem: ProgressVisualizer {
    [SerializeField] private AbilityCotrollerBase _abilityCotroller;
    
    [Inject] private PlayersDiesObserver _playersDiesObserver;
    [Inject] private PlayerMovement _mainPlayer;
    [Inject] private PlayerRegister _playerRegister;
    [Inject] private CameraOrbitalController _camera;
    [Inject] private GameData _gameData;
    
    private CancellationTokenSource _respawnTokenSource;

    private void Start() {
        FastHide();
    }

    public void OnDestroy() {
        _playersDiesObserver.PlayerSpawned -= OnPlayersSpawned;   
        _playersDiesObserver.PlayerDied -= OnPlayersDied;   
    }
    
    
    public void OnEnable() {
        _playersDiesObserver.PlayerSpawned += OnPlayersSpawned;   
        _playersDiesObserver.PlayerDied += OnPlayersDied;  
    }

    
    public void StartRespawnTimer() {
        UniTaskHelper.DisposeTask(ref _respawnTokenSource);
        _respawnTokenSource = new CancellationTokenSource();
        StartTimerToRespawnPlayer(_respawnTokenSource.Token).Forget();
        ShowBarAnimation(true);
    }
    

    public void StopTimer() {
        UniTaskHelper.DisposeTask(ref _respawnTokenSource);
        ShowBarAnimation(false);
    }

    
    private void OnPlayersSpawned(IPlayer player) {
        if (player != _mainPlayer) return;
        _abilityCotroller.StartAbility();
        _mainPlayer.SetMovingStatus(true);
        _mainPlayer.SetVisualModelState(true);
        _camera.FollowPlayer();
        ShowBarAnimation(false);
    }

 
    private void OnPlayersDied(IPlayer player) {
       if (player != _mainPlayer) return;
        _abilityCotroller.StopAbility();
        _mainPlayer.SetMovingStatus(false);
        _mainPlayer.SetVisualModelState(false);
        SetCameraFollowToRandomPlayer();
        StartRespawnTimer();
        ShowBarAnimation(true);
    }

    private void SetCameraFollowToRandomPlayer() {
        IPlayer randomBot = EnumerableHelper.GetRandomElementInListWhere(
            _playerRegister.Players,
            bot => bot.Damagable.CurrentHp != 0
        );
        _camera.SetFollowToBot(randomBot.Transform);
    }


    private async UniTask StartTimerToRespawnPlayer(CancellationToken token) {
        int duration = _gameData.TimeToRespawnPlayer;
        float elapsedTime = duration;
        while (elapsedTime > 0 && !token.IsCancellationRequested) {
            float progress =  elapsedTime / duration;
            SetProgressPercentage(progress, (int)elapsedTime);

            elapsedTime -= 1f;
            await UniTask.WaitForSeconds(1f, cancellationToken: token);
        }

        RespawnMainPlayer();
    }

    private void RespawnMainPlayer() {
        _mainPlayer.Damagable.Respawn(false);
    }

}