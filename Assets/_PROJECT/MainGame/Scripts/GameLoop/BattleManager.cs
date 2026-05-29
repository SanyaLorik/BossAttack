using System;
using System.Collections.Generic;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
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
    public event Action MainPlayerReturnedToSpawn;

    
    // Views
    [Inject] private GameOver _gameOver;
    [Inject] private BattleStartVisualizer _battleStartVisualizer;
    [Inject] private GameTimerToEnd _gameTimerToEnd;
    
    
    // Managers
    [Inject] private PlayerMovement _playerMovement;
    [Inject] private BotsMainManager _botsMainManager;
    [Inject] private MainGameStarter _gameStarter;
    [Inject] private RespawnManager _respawn;
    [Inject] private GameData _gameData;
    [Inject] private MapsToBattleChanger _mapsToBattleChanger;
    [Inject] private LocalizationData _localization;
    [Inject] private PlayerRegister _playerRegister;
    [Inject] private PlayersDiesObserver _playersDiesObserver;
    [Inject] private MainPlayerDeathSystem _mainPlayerDeathSystem;
    [Inject] private BossesDiesObserver _bossDiesObserver;

    
    private void Awake() {
        _bossDiesObserver.BossDied += OnBossDied;
        _gameTimerToEnd.GameEnded += TimeOver;
    }

    
    private void OnBossDied(IPlayer boss) {
        if (_playerRegister.AllBossesDied() && !GameIsOver) {
            Debug.Log("Боссы все погибли, игрок победил");
            _gameTimerToEnd.StopTimer();
            SetPlayerWin(true);
        }
    }


    private void TimeOver() {
        Debug.Log("Время кончилось");
        SetPlayerWin(false);
    }

    private void SetPlayerWin(bool win) {
        if(GameIsOver) return;
        GameIsOver = true;
        _mainPlayerDeathSystem.StopSystem();
        MainPlayerWin?.Invoke(win);
        WaitPlayerPressGameOverAsync().Forget();
    }


    
    public void InitForNewGame() {
        GameIsOver = false;
        RegisterAllPlayers();
        InitPlayersInRandomPoints(_playerRegister.GetPlayers(), PlayersSpawnPoints);

        _tokenSource = new CancellationTokenSource();
        StartBattleAsync(_tokenSource.Token).Forget();
    }
    
    
    private void RegisterAllPlayers() {
        _playerRegister.RegisterUnit(_playerMovement);
        IEnumerable<IPlayer> bots = _botsMainManager.GetPlayBotsToGame(CountBotsToBattle);
        foreach (IPlayer bot in bots) {
            _playerRegister.RegisterUnit(bot);
            Debug.Log("В бой идет бот " + bot.Transform.gameObject.name);
        }
        _playersDiesObserver.RemovePlayers();
        _bossDiesObserver.RemoveBosses();
        
        _playersDiesObserver.InitPlayersInBattle(_playerRegister.GetPlayers());
        _bossDiesObserver.InitBossesInBattle(_playerRegister.GetBosses());
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
        _gameTimerToEnd.StartGameTimerToEnd();
    }

    
    private async UniTask ShowStartAnimation(bool forbidMove, CancellationToken token) {
        await UniTask.Yield();
        
        if(forbidMove) EnablePlayersMove(false);
        _battleStartVisualizer.ShowAnimation(forbidMove);
        await UniTask.WaitWhile(() => _battleStartVisualizer.AnimationPlay, cancellationToken: token);
        if(forbidMove) EnablePlayersMove(true);
    }

    
    private async UniTask WaitPlayerPressGameOverAsync() {
        _playerMovement.SetMovingStatus(false);
        
        _playerMovement.SetPlayStatusSilent(false);
        await UniTask.WaitWhile(() => _gameOver.ResultWindowShowing);
        MainPlayerReturnedToSpawn?.Invoke();
        
        _playerMovement.SetPlayStatus(false);
        _playerMovement.SetMovingStatus(true);

        GameEnded();
    }


    
    public void PlayerFalled(IPlayer player) {
        player.TeleportToPoint(GameIsOver || !player.IsPlaying
            ? _respawn.SpawnPoint.position
            : _mapsToBattleChanger.CurrentMapSpawnPoints.GetRandomElement().position);
    }
    
    
    
    private void EnablePlayersMove(bool enable) {
        _playerRegister.GetPlayers().ForEach(p => p.SetMovingStatus(enable));
    }

    private void RotatePlayersToBoss() {
        _playerRegister.GetPlayers().ForEach(p => p.RotateToTarget(RandomBossPoint));
    }
    
    
    
    private void GameEnded() {
        Debug.Log("Игра кончилась");
        foreach (IPlayer player in _playerRegister.GetPlayers()) {
            player.SetPlayStatus(false);
        }
        _playerRegister.UnregisterAllUnits();
        _gameStarter.GameOver();
    }

}