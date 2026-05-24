using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BossCreateManager : MonoBehaviour {
    // Пусть пока пару штук 
    [SerializeField] private List<BossRoot> _bossRoot;
    [field: SerializeField] public int BossCount { get; private set; }  = 2;

    private List<BossRoot> _bossInstances = new List<BossRoot>();

    [Inject] private PlayerLevel _playerLevel;
    [Inject] private MainGameStarter _mainGameStarter;
    [Inject] private DiContainer _diContainer;
    [Inject] private PlayerRegister _playerRegister;
    [Inject] private MapsToBattleChanger _maps;
    [Inject] private BattleManager _battleManager;
    [Inject] private BossesDiesObserver _bossesDiesObserver;
    

    private void OnEnable() {
        _mainGameStarter.GameStarted += OnGameStarted;
        _battleManager.MainPlayerWin += DisposeBotsLogic;
        _bossesDiesObserver.BossDied += OnBossDie;
    }

    
    private void OnBossDie(IPlayer boss) {
        BossRoot bossRoot = _bossInstances.Find(b => b.BotManager == boss);
        if (bossRoot == null) {
            Debug.LogError("Босс умер но не найден в списке");
            return;
        }
        bossRoot.DisposeLogic();
        _bossInstances.Remove(bossRoot);
    }


    private void DisposeBotsLogic(bool win) {
        _bossInstances.ForEach(b => b.DisposeLogic());
        _bossInstances.Clear();
    }

    
    private void OnGameStarted(bool started) {
        if (started) {
            InstanceNewBosses();
        }
        else {
            DestroyBosses();
        }
    }


    private void InstanceNewBosses() {
        for (int i = 0; i < BossCount; i++) {
            BossRoot newBoss = Instantiate(_bossRoot[i]);
            _bossInstances.Add(newBoss);
            InitBoss(newBoss);
            newBoss.InitStats();
            _playerRegister.RegisterUnit(newBoss.BotManager);
            newBoss.BotManager.TeleportToPoint(_maps.GetCurrentEnemySpawns[i].position);
        }
    }

    
    private void InitBoss(BossRoot  boss) {
        _diContainer.InjectGameObject(boss.gameObject);
    }

    
    private void DestroyBosses() {
        foreach (IPlayer boss in _playerRegister.PlayUnits) {
            if (boss != null && (boss.TargetType & TargetType.Boss) != 0) {
                Destroy(boss.Transform.gameObject);
            }
        }
    }
}