using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDiesObserver {
    
    public event Action<IPlayer> PlayerDied;
    public event Action<IPlayer> PlayerSpawned;

    private List<IPlayer> _currentPlayers = new();
    
    public void InitPlayersInBattle(List<IPlayer> players) {
        foreach (var p in players) {
            p.Damagable.DamagableDied += OnPlayerDie;
            p.Damagable.DamagableSpawned += OnPlayerSpawn;
        }
        _currentPlayers = players;
    }

    
    public void RemovePlayers(List<IPlayer> players) {
        foreach (var p in players) {
            p.Damagable.DamagableDied -= OnPlayerDie;
            p.Damagable.DamagableSpawned -= OnPlayerSpawn;
        }

        _currentPlayers = null;
    }
    

    private void OnPlayerDie(IDamagable damagable) {
        IPlayer player = _currentPlayers.Find(p => p.Damagable == damagable);
        if (player == null) {
            Debug.LogError("Проблема с поиском игрока");
        }
        PlayerDied?.Invoke(player);
    }
    
    
    private void OnPlayerSpawn(IDamagable damagable) {
        IPlayer player = _currentPlayers.Find(p => p.Damagable == damagable);
        if (player == null) {
            Debug.LogError("Проблема с поиском игрока");
        }
        PlayerSpawned?.Invoke(player);
    }

}