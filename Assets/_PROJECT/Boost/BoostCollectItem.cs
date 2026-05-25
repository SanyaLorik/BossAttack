using UnityEngine;
using Zenject;

public class BoostCollectItem : MonoBehaviour, IPlayer {
    [SerializeField] private DamageVisualizer _damageVisualizer;
    
    private PlayerBoostBoxesSystem _playerBoostBoxesSystem;
    private GameData _gameData;

    public TargetType TargetType => TargetType.Boost;

    public IDamagable Damagable => _damagable;
    private Damagable _damagable;


    public Transform PointToAtack => transform;
    public Transform Transform  => transform;

    public void TeleportToPoint(Vector3 point) {
        Transform.position = point;
    }
    
    
    [Inject]
    private void Initialize(PlayerBoostBoxesSystem playerBoostBoxesSystem, GameData gameData) {
        _playerBoostBoxesSystem = playerBoostBoxesSystem;
        _gameData = gameData;
        
        _damagable = new Damagable(Transform, this);
        _damagable.SetMaxHpGetter(() => _gameData.BoostBoxHp);
        _damagable.Respawn(true);
        _damagable.DamagableDied += OnDamagableDied;
        
        _damageVisualizer.SetDamagable(_damagable);
    }



    private void OnDamagableDied(IDamagable damagable) {
        _damageVisualizer.Unsubscribe();
        Damagable.DamagableDied -= OnDamagableDied;
        
        _playerBoostBoxesSystem.PlusOne();
        SetVisualModelState(false);
    }

    public void SetVisualModelState(bool enable) {
        gameObject.SetActive(enable);
    }
    

    
    
    #region IPlayer
    public void SetPlayStatus(bool goPlay) { }

    public void RotateToTarget(Vector3 point) { }

    public void SetMovingStatus(bool enable) { }

    
    public IPusher Pusher { get; }
    public IBonusUser BonusUser { get; }
    public bool IsPlaying { get; }
    #endregion
    
}