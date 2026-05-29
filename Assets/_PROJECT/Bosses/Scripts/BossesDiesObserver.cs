using System;
using System.Collections.Generic;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class BossesDiesObserver {
    public event Action<IPlayer> BossDied;
    public event Action<IPlayer> BossSpawned;

    public int CountLifeBosses => _currentBosses.Count;
    
    private List<IPlayer> _currentBosses = new();
    
    
    
    
    public void InitBossesInBattle(List<IPlayer> bosses) {
        foreach (var b in bosses) {
            b.Damagable.DamagableDied += OnBossDie;
            b.Damagable.DamagableSpawned += OnBossSpawn;
        }
        _currentBosses = bosses;
    }



    
    public void RemoveBosses() {
        foreach (var p in _currentBosses) {
            p.Damagable.DamagableDied -= OnBossDie;
            p.Damagable.DamagableSpawned -= OnBossSpawn;
        }
    }
    

    private void OnBossDie(IDamagable damagable) {
        IPlayer boss = _currentBosses.Find(p => p.Damagable == damagable);
        if (boss == null) {
            Debug.LogError("Проблема с поиском игрока");
        }
        else {
            BossDied?.Invoke(boss);
        }
    }
    
    
    private void OnBossSpawn(IDamagable damagable) {
        IPlayer boss = _currentBosses.Find(p => p.Damagable == damagable);
        if (boss == null) {
            Debug.LogError("Проблема с поиском босса");
        }
        BossSpawned?.Invoke(boss);
    }
}