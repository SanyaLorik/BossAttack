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
    public int CountPlayersToNewBattle => _mapsToBattleChanger.CurrentMapSpawnPoints.Length;
    
    public int AllRoundsCount { get; private set; }

    public int RoundNumber { get; private set; }

    
    private CancellationTokenSource _tokenSource;


    public event Action<string, Vector3> PlayerDied;
    public event Action<int> PlayersCountChanged;
    public event Action<int> NewRoundStarted;
    public event Action GameReadyToPlay;
    public event Action<bool> MainPlayerWin;
    public event Action ForceStartedNewGame;

    
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

    

    public void InitForNewGame(bool mainPlayerPlay) {
        GameIsOver = false;
        _playerRegister.SetMainPlayerPlay(mainPlayerPlay);
        GetNewPlayers(mainPlayerPlay);
        InitPlayers();
    }

    
    public void ForceEndNewGame() {
        UniTaskHelper.DisposeTask(ref _tokenSource);
        ForceStartedNewGame?.Invoke();
        GameEnded(false);
    }


    public void PlayerFalled(IPlayer player) {
        if(GameIsOver) return;
       
        
        if (player == _playerMovement) {
            SetLooseMainPlayer();
        }
        else {
            SetLooseBot(player);
        }
    }
    
    private void SetLooseMainPlayer() {
        if(!_playerRegister.MainPlayerPlay) return;
        _playerRegister.SetMainPlayerPlay(false);
        MainPlayerWin?.Invoke(false);
        RemovePlayer(_playerMovement);
        PlayerDied?.Invoke(_localization.You, _playerMovement.Transform.position);
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
            _playerRegister.RegisterUnit(_playerMovement, TargetType.Player);
            countBots--;
        }
        IEnumerable<IPlayer> bots = _botsMainManager.GetPlayBotsToGame(countBots);
        foreach (IPlayer bot in bots) _playerRegister.RegisterUnit(bot, TargetType.Player);
    }



    private void InitPlayers() {
        foreach (var player in _playerRegister.Players) {
            player.SetPlayStatus(true);
        }
        TeleportPlayersToPoints(_playerRegister.Players, PlayersSpawnPoints);
        PlayersCountChanged?.Invoke(_playerRegister.Players.Count);
        _tokenSource = new CancellationTokenSource();
        GoBattleAsync(_tokenSource.Token).Forget();
    }

    
    private async UniTask GoBattleAsync(CancellationToken token) {
        RotatePlayersToBoss();
        await ShowStartAnimation(true, token);
        GameReadyToPlay?.Invoke();

        RoundNumber = 1;
        while (!token.IsCancellationRequested && _playerRegister.PlayersCount > 1) {
            NewRoundStarted?.Invoke(RoundNumber);
            
            await UniTask.WaitUntil(() => _playerRegister.PlayersCount == 1, cancellationToken: token);
            
            await UniTask.WaitForSeconds(_gameData.TimeAfterEndRound, cancellationToken: token);
            
            if (_playerRegister.PlayersCount != 1) {
                await ShowStartAnimation(false, token);
            }
            RoundNumber++;
        }
        
        if (_playerRegister.MainPlayerPlay) {
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
        _playerMovement.SetMovingStatus(false);

        if (!playerWin) {
            _playerMovement.HideVisualModel(true);
        }
        
        _playerMovement.SetPlayStatusSilent(false);
        await UniTask.WaitWhile(() => _gameOver.ResultWindowShowing);
        
        if (!playerWin) {
            _playerMovement.HideVisualModel(false);
        }
        _playerMovement.SetPlayStatus(false);
        _playerMovement.SetMovingStatus(true);
    }


    private void GameEnded(bool setGameOver = true) {
        Debug.Log("Игра кончилась");
        GameIsOver = true;
        foreach (IPlayer player in _playerRegister.Players) {
            player.SetPlayStatus(false);
        }
        
        _playerRegister.Players.Clear();
        
        if (setGameOver) {
            _gameStarter.GameOver();
        }
        
    }
    

    private void RemovePlayer(IPlayer player) {
        player.SetPlayStatus(false);
        _playerRegister.UnregisterUnit(player, TargetType.Player);
        PlayersCountChanged?.Invoke(_playerRegister.PlayersCount);
        Debug.Log("Игроков: " + _playerRegister.PlayersCount);
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
        _playerRegister.Players.ForEach(p => p.SetMovingStatus(enable));
    }

    private void RotatePlayersToBoss() {
        _playerRegister.Players.ForEach(p => p.RotateToTarget(RandomBossPoint));
    }
    
    
    public void SetGameOverToBots() {
    }
}