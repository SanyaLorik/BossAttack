using System;
using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using ArrayExtension = SanyaBeerExtension.ArrayExtension;
using Random = UnityEngine.Random;


public class BattleManager : MonoBehaviour {
    public bool GameIsOver { get; private set; }
    public bool PlayerReturnToSpawn => _playerMovement.PlayerInSpawn;

    private Vector3 RandomBossPoint => ArrayExtension.GetRandomElement(EnemySpawnPoints).position;
    public Transform[] PlayersSpawnPoints => _mapsToBattleChanger.CurrentMapSpawnPoints;
    public Transform[] EnemySpawnPoints => _mapsToBattleChanger.GetCurrentEnemySpawns;
    public int CountBotsToBattle => _gameData.CountBotsToGame;
    

    private CancellationTokenSource _tokenSource;

    public event Action<int> PlayersCountChanged;
    public event Action GameReadyToPlay;
    public event Action<bool> MainPlayerWin;

    
    // Views
    [Inject] private GameOver _gameOver;
    [Inject] private BattleStartVisualizer _battleStartVisualizer;
    
    
    // Managers
    [Inject] private PlayerMovement _playerMovement;
    [Inject] private BotsMainManager _botsMainManager;
    [Inject] private MainGameStarter _gameStarter;
    [Inject] private GameData _gameData;
    [Inject] private MapsToBattleChanger _mapsToBattleChanger;
    [Inject] private LocalizationData _localization;
    [Inject] private PlayerRegister _playerRegister;
    [Inject] private PlayersDiesObserver _diesObserver;
    
    private void Awake() {
        _diesObserver.PlayerDied += OnPlayerDied;
        _diesObserver.PlayerSpawned += OnPlayerSpawned;
    }

    
    private void OnPlayerSpawned(IPlayer player) {
        // IN DEV...
    }


    private void OnPlayerDied(IPlayer player) {
        // IN DEV...
    }

    
    public void InitForNewGame() {
        GameIsOver = false;
        RegisterAllPlayers();
        InitPlayersInRandomPoints(_playerRegister.Players, PlayersSpawnPoints);

        _tokenSource = new CancellationTokenSource();
        StartBattleAsync(_tokenSource.Token).Forget();
    }
    
    
    private void RegisterAllPlayers() {
        _playerRegister.RegisterUnit(_playerMovement, TargetType.Player);
        IEnumerable<IPlayer> bots = _botsMainManager.GetPlayBotsToGame(CountBotsToBattle);
        foreach (IPlayer bot in bots) {
            _playerRegister.RegisterUnit(bot, TargetType.Player);
        }
        _diesObserver.InitPlayersInBattle(_playerRegister.Players);
    }

    
    
    private void InitPlayersInRandomPoints(List<IPlayer> players, Transform[] points) {
        if (players.Count > points.Length) {
            Debug.LogWarning("Кол-во игроков > кол-ва точек спавна");
            return;
        } 
        int randomStartIndex = Random.Range(0, players.Count);
        for (int i = 0; i < players.Count; i++) {
            int index = (i + randomStartIndex) % players.Count;
            players[i].TeleportToPoint(points[index].position);
            players[i].RotateToTarget(RandomBossPoint);
            players[i].SetPlayStatus(true);
        }
    }
    
    
    private async UniTask StartBattleAsync(CancellationToken token) {
        RotatePlayersToBoss();
        await ShowStartAnimation(true, token);
        GameReadyToPlay?.Invoke();
    }

    
    private async UniTask ShowStartAnimation(bool forbidMove, CancellationToken token) {
        await UniTask.Yield();
        
        if(forbidMove) EnablePlayersMove(false);
        _battleStartVisualizer.ShowAnimation(forbidMove);
        await UniTask.WaitWhile(() => _battleStartVisualizer.AnimationPlay, cancellationToken: token);
        if(forbidMove) EnablePlayersMove(true);
    }

    
    private async UniTask WaitPlayerPressGameOverAsync(bool playerWin) {
        _playerMovement.SetMovingStatus(false);

        if (!playerWin) {
            _playerMovement.SetVisualModelState(true);
        }
        
        _playerMovement.SetPlayStatusSilent(false);
        await UniTask.WaitWhile(() => _gameOver.ResultWindowShowing);
        
        if (!playerWin) {
            _playerMovement.SetVisualModelState(false);
        }
        _playerMovement.SetPlayStatus(false);
        _playerMovement.SetMovingStatus(true);
    }


    
    public void PlayerFalled(IPlayer player) {
        if(GameIsOver) return;
        _diesObserver.RemovePlayers(_playerRegister.Players);
        
        // IN Dev...
    }
    
    
    
    private void EnablePlayersMove(bool enable) {
        _playerRegister.Players.ForEach(p => p.SetMovingStatus(enable));
    }

    private void RotatePlayersToBoss() {
        _playerRegister.Players.ForEach(p => p.RotateToTarget(RandomBossPoint));
    }
    
    
    
    private void GameEnded(bool setGameOver = true) {
        Debug.Log("Игра кончилась");
        GameIsOver = true;
        foreach (IPlayer player in _playerRegister.Players) {
            player.SetPlayStatus(false);
            _playerRegister.UnregisterUnit(player, TargetType.Player);
            
        }
        
        _playerRegister.Players.Clear();
        
        if (setGameOver) {
            _gameStarter.GameOver();
        }
        
    }

}