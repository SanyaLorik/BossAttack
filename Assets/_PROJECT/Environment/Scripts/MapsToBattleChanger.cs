using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SanyaBeerExtension;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class MapsToBattleChanger : MonoBehaviour {
    [field: SerializeField] public Transform CentralTeleport { get; private set; }
    [SerializeField] private List<MapItem> _mapitems;
    [Header("Ставить 0 и карту Tutorial первой!")]
    [SerializeField] private int _tutorialMapIndex;

    private int MapIndex { get; set; }

    public Transform[] CurrentMapSpawnPoints => _mapitems[MapIndex].SpawnPoints;
    public Transform[] GetCurrentEnemySpawns => _mapitems[MapIndex].CurrentEnemySpawns;
    public Transform[] GetBonusSpawnPoints => _mapitems[MapIndex].BonusSpawnPoints;
    public Transform[] GetBoostSpawnPoints => _mapitems[MapIndex].BoostSpawnPoints;
    public Transform GetCurrentMapFloor => _mapitems[MapIndex].Floor;
    public float CurrentMapYToFind => _mapitems[MapIndex].YToFind;
    public float FallBotFindSamplePosition => _mapitems[MapIndex].FallBotFindSamplePosition;

    public event Action NewMapChanged;
    
    [Inject] private MainGameStarter _mainGameStarter;
    [Inject] private TutorialManager _tutorialManager;
    [Inject] private PlayerMovement _playerMovement;

    
    
    private void OnEnable() {
        _mainGameStarter.GameStarted += OnGameStarted;
        _tutorialManager.TutorialStarted += OnTutorialStarted;
    }

    private void OnTutorialStarted(bool started) {
        if (!started) {
            RemoveTutorMapAsync().Forget();
        }
    }


    private void Start() {
        TryToRemoveTutorialMap();
        MapIndex = Random.Range(0, _mapitems.Count);
    }
    
    
    private async UniTask RemoveTutorMapAsync() {
        await UniTask.WaitUntil(() => _playerMovement.PlayerInSpawn);
        TryToRemoveTutorialMap();
    }

    
    private void TryToRemoveTutorialMap() {
        if (_tutorialManager.TutorialPassed) {
            _mapitems[_tutorialMapIndex].DisactiveSelf();
            _mapitems.RemoveAt(_tutorialMapIndex);
            // Сразу некст показываем
            ChooseNextMap();
        }
    }


    private void OnGameStarted(bool started) {
        if (started) {
            ChooseNextMap();
            NewMapChanged?.Invoke();
        }
    }

    
    private void ChooseNextMap() {
        if (_tutorialManager.TutorialPassed) {
            MapIndex = (MapIndex + 1) % _mapitems.Count;
        }
        else {
            MapIndex = _tutorialMapIndex;
        }
        
        _mapitems.ForEach(m => m.DisactiveSelf());
        _mapitems[MapIndex].gameObject.ActiveSelf();
    }

}