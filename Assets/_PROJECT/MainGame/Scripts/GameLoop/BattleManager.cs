using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _PROJECT.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Zenject;
using ArrayExtension = SanyaBeerExtension.ArrayExtension;
using Random = UnityEngine.Random;


public class BattleManager : MonoBehaviour, IBattleInfo {
    public bool MainPlayerPlay { get; private set; }
    public bool GameIsOver { get; private set; }
    public bool PlayerReturnToSpawn => _mainPlayer.PlayerInSpawn;

    public int CountPlayersToNewBattle => _mapsToBattleChanger.CurrentMapSpawnPoints.Length;
    private Vector3 RandomBossPoint => ArrayExtension.GetRandomElement(EnemySpawnPoints).position;
    
    public int AllRoundsCount { get; private set; }

    public int RoundNumber { get; private set; }

    public Transform[] PlayersSpawnPoints => _mapsToBattleChanger.CurrentMapSpawnPoints;
    public Transform[] EnemySpawnPoints => _mapsToBattleChanger.GetCurrentEnemySpawns;


    public List<UnitInfo> EnemysDamagable { get; } = new(8);
    public List<UnitInfo> BuildingsDamagable { get; } = new(8);
    public List<UnitInfo> PlayersDamagable  { get; } = new(8);
    
    
    private readonly List<IPlayer> _players = new(8);
    
    
    public event Action<string, Vector3> PlayerDied;
    public event Action<int> PlayersCountChanged;
    public event Action<int> NewRoundStarted;
    public event Action GameReadyToPlay;
    public event Action<bool> MainPlayerWin;
    public event Action ForceStartedNewGame;

    private CancellationTokenSource _tokenSource;
    private int PlayersCount => _players.Count;
    
    // Views
    [Inject] private GameOver _gameOver;
    [Inject] private BattleStartVisualizer _battleStartVisualizer;
    
    
    // Managers
    [Inject] private PlayerMovement _mainPlayer;
    [Inject] private BotsMainManager _botsMainManager;
    [Inject] private MainGameStarter _gameStarter;
    [Inject] private GameData _gameData;
    [Inject] private MapsToBattleChanger _mapsToBattleChanger;
    [Inject] private LocalizationData _localization;


    public void InitForNewGame(bool mainPlayerPlay) {
        GameIsOver = false;
        MainPlayerPlay = mainPlayerPlay;
        GetNewPlayers(MainPlayerPlay);
        InitPlayers();
    }

    
    public void ForceEndNewGame() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        ForceStartedNewGame?.Invoke();
        GameEnded(false);
    }


    public void PlayerFalled(IPlayer player) {
        if(GameIsOver) return;
       
        
        if (player == _mainPlayer) {
            SetLooseMainPlayer();
        }
        else {
            SetLooseBot(player);
        }
    }
    
    private void SetLooseMainPlayer() {
        if(!MainPlayerPlay) return;
        MainPlayerPlay = false;
        MainPlayerWin?.Invoke(false);
        RemovePlayer(_mainPlayer);
        PlayerDied?.Invoke(_localization.You, _mainPlayer.Transform.position);
        WaitPlayerPressGameOverAsync(false).Forget();
        Debug.Log("Вы выбыли из игры");
    }

    
    private void SetLooseBot(IPlayer player) {
        // Пока ужас
        BotMonolog botMonolog = player.Transform.gameObject.GetComponentInParent<BotMonolog>();
        if (botMonolog != null) {
            PlayerDied?.Invoke(botMonolog.NickName, player.Transform.position);
            Debug.Log($"{botMonolog.NickName} проиграл");
            player.SetPlayStatus(false);
            RemovePlayer(player);
        }
    }
    
    
    private void GetNewPlayers(bool mainPlayerPlay) {
        int countBots = CountPlayersToNewBattle;
        if (mainPlayerPlay) {
            RegisterPlayer(_mainPlayer);
            countBots--;
        }
        IEnumerable<IPlayer> bots = _botsMainManager.GetBotsToGame(countBots);
        foreach (IPlayer bot in bots) RegisterPlayer(bot);
    }

    
    private void RegisterPlayer(IPlayer player) {
        _players.Add(player);
        PlayersDamagable.Add(new UnitInfo {
            Target = player.Damagable,
            Transform = player.Transform,
        });
    }

    
    private void UnregisterPlayer(IPlayer player) {
        _players.Remove(player);
        PlayersDamagable.Remove(PlayersDamagable.Find(info => info.Target ==  player.Damagable));
    }


    private void InitPlayers() {
        foreach (var player in _players) {
            player.SetPlayStatus(true);
        }
        TeleportPlayersToPoints(_players, PlayersSpawnPoints);
        PlayersCountChanged?.Invoke(_players.Count);
        _tokenSource = new CancellationTokenSource();
        GoBattleAsync(_tokenSource.Token).Forget();
    }

    
    private async UniTask GoBattleAsync(CancellationToken token) {
        RotatePlayersToBoss();
        await ShowStartAnimation(true, token);
        GameReadyToPlay?.Invoke();

        RoundNumber = 1;
        while (!token.IsCancellationRequested && PlayersCount > 1) {
            NewRoundStarted?.Invoke(RoundNumber);
            
            await UniTask.WaitUntil(() => PlayersCount == 1, cancellationToken: token);
            
            await UniTask.WaitForSeconds(_gameData.TimeAfterEndRound, cancellationToken: token);
            
            if (PlayersCount != 1) {
                await ShowStartAnimation(false, token);
            }
            RoundNumber++;
        }
        
        if (MainPlayerPlay) {
            MainPlayerWin?.Invoke(true);
            await WaitPlayerPressGameOverAsync(true);
        }
        GameEnded();
    }

    
    
    private async UniTask ShowStartAnimation(bool forbidMove, CancellationToken token) {
        await UniTask.Yield();
        
        if(forbidMove) EnablePlayersMove(false);
        _battleStartVisualizer.ShowAnimation(forbidMove);
        await UniTask.WaitWhile(() => _battleStartVisualizer.AnimationPlay, cancellationToken: token);
        if(forbidMove) EnablePlayersMove(true);
    }

    
    private async UniTask WaitPlayerPressGameOverAsync(bool playerWin) {
        _mainPlayer.SetMovingStatus(false);

        if (!playerWin) {
            _mainPlayer.HideVisualModel(true);
        }
        
        _mainPlayer.SetPlayStatusSilent(false);
        await UniTask.WaitWhile(() => _gameOver.ResultWindowShowing);
        
        if (!playerWin) {
            _mainPlayer.HideVisualModel(false);
        }
        _mainPlayer.SetPlayStatus(false);
        _mainPlayer.SetMovingStatus(true);
    }


    private void GameEnded(bool setGameOver = true) {
        Debug.Log("Игра кончилась");
        GameIsOver = true;
        foreach (IPlayer player in _players) {
            player.SetPlayStatus(false);
        }
        
        _players.Clear();
        
        if (setGameOver) {
            _gameStarter.GameOver();
        }
        
    }

    
    private void CheckPlayers() {
        
    }

    private void RemovePlayer(IPlayer player) {
        player.SetPlayStatus(false);
        UnregisterPlayer(player);
        PlayersCountChanged?.Invoke(PlayersCount);
        Debug.Log("Игроков: " + PlayersCount);
    }


    private void TeleportPlayersToPoints(List<IPlayer> players, Transform[] points) {
        if (players.Count < points.Length) {
            Debug.LogWarning("Кол-во игроков < кол-ва точек спавна");
            return;
        } 
        int randomStartIndex = Random.Range(0, points.Length);
        for (int i = 0; i < points.Length; i++) {
            int index = (i + randomStartIndex) % points.Length;
            players[i].TeleportToPoint(points[index].position);
            players[i].RotateToTarget(RandomBossPoint);
        }
    }
    
    private void EnablePlayersMove(bool enable) {
        _players.ForEach(p => p.SetMovingStatus(enable));
    }

    private void RotatePlayersToBoss() {
        _players.ForEach(p => p.RotateToTarget(RandomBossPoint));
    }
    
    
    public void SetGameOverToBots() {
    }
}