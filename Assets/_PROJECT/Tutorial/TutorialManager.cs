using System;
using Architecture_M;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public enum TutorialStep {
}


public class TutorialManager : MonoBehaviour {
    [SerializeField] private Narrator _narrator;
    [SerializeField] private float _timeToWaitForBombExplode;
    [SerializeField] private LineToObjects _lineToObjects;

    
    public bool InitBombToMainPlayer { get; private set; } = true;
    public bool TutorialPassed => Saves.TutorialPassed;
    public event Action NewTutorialStep;  
    public event Action<bool> TutorialStarted;  


    private GameSave Saves => _saver.GetSave<GameSave>();
    
    [Inject] private IGameSave _saver; 
    [Inject] private MainGameStarter _gameStarter; 
    [Inject] private BattleManager _battleManager; 
    [Inject] private MainGameStarter _mainGameStarter; 
    [Inject] private PlayerMovement _mainPlayer; 
    [Inject] private PlayerBonusManager _playerBonusManager;
    
    //
    //
    // public void OnEnable() {
    //     if (!TutorialPassed) {
    //         _battleManager.GameReadyToPlay += StartTutorial;
    //         GameEvents.BonusUsed += OnBonusUsed;
    //     }
    // }
    //
    //
    // private void Start() {
    //     _narrator.Disactive();
    // }
    //
    //
    // private void StartTutorial() {
    //     TutorialStartAsync().Forget();
    // }
    //
    //
    // private async UniTask TutorialStartAsync() {
    //     TutorialStarted?.Invoke(true);
    //     
    //     // Догони врага и передай бомбу
    //     NewTutorialStep?.Invoke();
    //     await PassBombToEnemyStep();
    //     
    //     // Убегай от врага с бомбой
    //     NewTutorialStep?.Invoke();
    //     await RunAwayFromEnemyStep();
    //     
    //     // Догони врага, передай бомбу и выиграй!  
    //     NewTutorialStep?.Invoke();
    //     await CatchUpEnemyWithSpeedBonusStep();
    //     OnTutorialEnd();
    //     
    //     TutorialStarted?.Invoke(false);
    // }
    //
    //
    // // Взрыв бота 1
    // private async UniTask PassBombToEnemyStep() {
    //     _playerBonusManager.SetAvailableToUseBonuses(false);
    //     
    //     _narrator.Active();
    //     _narrator.SetTutorialText(TutorialStep.PassBombToEnemy);
    //     
    //     _lineToObjects.SetTarget(_battleManager.RandomEnemy.RoleBehaviour.transform);
    //     await UniTask.WaitWhile(() => _mainPlayer.RoleBehaviour.CurrentRole == PlayerRoleInGame.Hunter);
    //     await UniTask.WaitForSeconds(_timeToWaitForBombExplode);
    //     _lineToObjects.HideArrow();
    //     
    //     InitBombToMainPlayer = false;
    //     _bomb.ExplodeBombLater();
    // }   
    //
    //
    // // Бомба не взрывается а передается просто игроку 
    // private async UniTask RunAwayFromEnemyStep() {
    //     _playerBonusManager.SetAvailableToUseBonuses(true);
    //     _narrator.ShowScreenFinger();
    //     
    //     _narrator.SetTutorialText(TutorialStep.RunAwayFromEnemy);
    //     
    //     await UniTask.WaitWhile(() => _mainPlayer.RoleBehaviour.CurrentRole != PlayerRoleInGame.Hunter);
    // }  
    //
    //
    // private async UniTask CatchUpEnemyWithSpeedBonusStep() {
    //     _narrator.SetTutorialText(TutorialStep.CatchUpEnemyWithSpeedBonus);
    //     _narrator.HideScreenFinger();
    //    
    //     _lineToObjects.SetTarget(_battleManager.RandomEnemy.RoleBehaviour.transform);
    //     
    //     await UniTask.WaitWhile(() => _mainPlayer.RoleBehaviour.CurrentRole == PlayerRoleInGame.Hunter);
    //     await UniTask.WaitForSeconds(_timeToWaitForBombExplode);
    //     
    //     _lineToObjects.HideArrow();
    //     
    //     _bomb.ExplodeBombLater();
    // }  
    //
    //
    // private void OnBonusUsed(IBonus bonus) {
    //     _narrator.HideScreenFinger();
    // }
    //
    //
    // private void OnTutorialEnd() {
    //     Saves.TutorialPassed = true;
    //     _saver.Save();
    //     _playerBonusManager.SetAvailableToUseBonuses(true);
    //     _narrator.DisableNarrator();
    //     _battleManager.GameReadyToPlay -= StartTutorial;
    //     GameEvents.BonusUsed -= OnBonusUsed;
    // }

}
