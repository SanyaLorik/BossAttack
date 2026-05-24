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
    

    private void OnEnable() {
        _mainGameStarter.GameStarted += OnGameStarted;
        _battleManager.MainPlayerWin += DisposeBotsLogic;
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
            ClearBosses();
        }
    }


    private void InstanceNewBosses() {
        
        for (int i = 0; i < BossCount; i++) {
            BossRoot newBoss = Instantiate(_bossRoot[i]);
            _bossInstances.Add(newBoss);
            InitBoss(newBoss);
            newBoss.InitStats();
            _playerRegister.RegisterUnit(newBoss.BotManager, TargetType.Enemy);
            newBoss.BotManager.TeleportToPoint(_maps.GetCurrentEnemySpawns[i].position);
        }
    }

    
    private void InitBoss(BossRoot  boss) {
        _diContainer.InjectGameObject(boss.gameObject);
    }

    
    private void ClearBosses() {
        _playerRegister.Bosses.ForEach(b => Destroy(b.Transform.gameObject));
    }
}