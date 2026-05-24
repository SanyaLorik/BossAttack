using System.Collections.Generic;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;

public class BossCreateManager : MonoBehaviour {
    // Пусть пока пару штук 
    [SerializeField] private List<BossRoot> _bossRoot;
    [SerializeField] private int _bossCount = 2;

    
    [Inject] PlayerLevel _playerLevel;
    [Inject] MainGameStarter _mainGameStarter;
    [Inject] private DiContainer _diContainer;
    [Inject] private PlayerRegister _playerRegister;
    [Inject] private MapsToBattleChanger _maps;
    
    

    private void OnEnable() {
        _mainGameStarter.GameStarted += OnGameStarted;
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
        
        for (int i = 0; i < _bossCount; i++) {
            BossRoot newBoss = Instantiate(_bossRoot.GetRandomElement());
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